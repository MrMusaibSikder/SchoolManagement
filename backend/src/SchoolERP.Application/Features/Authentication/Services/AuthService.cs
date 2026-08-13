using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.Authentication.DTOs;
using SchoolERP.Application.Features.Authentication.Interfaces;
using SchoolERP.Application.Features.User.Interfaces;
using SchoolERP.Application.Features.UserRole.Interfaces;
using Microsoft.Extensions.Options;

namespace SchoolERP.Application.Features.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly JwtSettings _jwtSettings;
    private readonly PasswordResetSettings _passwordResetSettings;

    public AuthService(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IJwtService jwtService,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        IEmailService emailService,
        IOptions<JwtSettings> jwtSettings,
        IOptions<PasswordResetSettings> passwordResetSettings)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _jwtSettings = jwtSettings.Value;
        _passwordResetSettings = passwordResetSettings.Value;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        // Find User by Username or Email
        var users = await _userRepository.GetAllAsync();

        var user = users.FirstOrDefault(x =>
            x.Username == request.UsernameOrEmail ||
            x.Email == request.UsernameOrEmail);

        if (user == null)
            return null;

        // Verify Password
        var validPassword = _passwordHasher.Verify(
            request.Password,
            user.PasswordHash);

        if (!validPassword)
            return null;

        // Get Roles
        var userRoles = await _userRoleRepository.GetAllAsync();

        var roles = userRoles
            .Where(x => x.UserId == user.Id)
            .Select(x => x.Role.Name)
            .ToList();

        // Generate JWT
        var token = _jwtService.GenerateToken(user, roles);

        // Generate + persist a refresh token so the client can silently obtain
        // new access tokens without forcing the user to log in again.
        var refreshToken = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            Token = _jwtService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new LoginResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Roles = roles,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiresAt = refreshToken.ExpiresAt
        };
    }


    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        // Check Username
        if (await _userRepository.UsernameExistsAsync(request.Username))
            throw new Exception("Username already exists.");

        // Check Email
        if (await _userRepository.EmailExistsAsync(request.Email))
            throw new Exception("Email already exists.");

        // Get Role
        var role = (await _unitOfWork.RoleRepository.GetAllAsync())
            .FirstOrDefault(x => x.Name == request.RoleName);

        if (role is null)
            throw new Exception("Role not found.");

        // Create User
        var user = new Domain.Entities.User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            IsActive = true
        };

        await _userRepository.AddAsync(user);

        // Save to generate User.Id
        await _unitOfWork.SaveChangesAsync();

        // Assign Role
        await _userRoleRepository.AddAsync(new Domain.Entities.UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        await _unitOfWork.SaveChangesAsync();

        return new RegisterResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = role.Name
        };
    }

    /// <inheritdoc />
    public async Task<RefreshTokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var existingToken = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(request.RefreshToken);

        if (existingToken is null || !existingToken.IsActive)
            return null;

        var user = await _userRepository.GetByIdAsync(existingToken.UserId);
        if (user is null || !user.IsActive)
            return null;

        var userRoles = await _userRoleRepository.GetAllAsync();
        var roles = userRoles
            .Where(x => x.UserId == user.Id)
            .Select(x => x.Role.Name)
            .ToList();

        // Rotate: issue a brand new refresh token and revoke the one that was
        // just used, pointing at its replacement for audit purposes.
        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            UserId = user.Id,
            Token = _jwtService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            CreatedAt = DateTime.UtcNow
        };

        existingToken.RevokedAt = DateTime.UtcNow;
        existingToken.ReplacedByToken = newRefreshToken.Token;

        await _unitOfWork.RefreshTokenRepository.AddAsync(newRefreshToken);
        _unitOfWork.RefreshTokenRepository.Update(existingToken);
        await _unitOfWork.SaveChangesAsync();

        var newAccessToken = _jwtService.GenerateToken(user, roles);

        return new RefreshTokenResponseDto
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Roles = roles,
            Token = newAccessToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiresAt = newRefreshToken.ExpiresAt
        };
    }

    /// <inheritdoc />
    public async Task LogoutAsync(int userId, LogoutRequestDto request)
    {
        if (!string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            var token = await _unitOfWork.RefreshTokenRepository.GetByTokenAsync(request.RefreshToken);

            // Only revoke if it belongs to the caller and is still active; otherwise
            // treat logout as a no-op so it stays idempotent and doesn't leak
            // information about other users' tokens.
            if (token is not null && token.UserId == userId && token.IsActive)
            {
                token.RevokedAt = DateTime.UtcNow;
                _unitOfWork.RefreshTokenRepository.Update(token);
                await _unitOfWork.SaveChangesAsync();
            }

            return;
        }

        // No specific token supplied: revoke every active session for this user
        // ("logout from all devices").
        var activeTokens = await _unitOfWork.RefreshTokenRepository.GetActiveByUserIdAsync(userId);
        foreach (var activeToken in activeTokens)
        {
            activeToken.RevokedAt = DateTime.UtcNow;
            _unitOfWork.RefreshTokenRepository.Update(activeToken);
        }

        if (activeTokens.Count > 0)
            await _unitOfWork.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task ChangePasswordAsync(int userId, ChangePasswordDto request)
    {
        var user = await _userRepository.GetByIdTrackedAsync(userId)
            ?? throw new Common.Exceptions.NotFoundException(nameof(Domain.Entities.User), userId);

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new Common.Exceptions.BadRequestException("Current password is incorrect.");

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task ForgotPasswordAsync(ForgotPasswordDto request)
    {
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(x => x.Email == request.Email);

        // Intentionally do not reveal whether the email exists: always "succeed".
        if (user is null)
            return;

        var resetToken = new Domain.Entities.PasswordResetToken
        {
            UserId = user.Id,
            Token = _jwtService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(_passwordResetSettings.TokenExpiryMinutes),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.PasswordResetTokenRepository.AddAsync(resetToken);
        await _unitOfWork.SaveChangesAsync();

        var body = $"Hello {user.Username},\n\n" +
                   $"Use the following token to reset your password: {resetToken.Token}\n" +
                   $"This token expires in {_passwordResetSettings.TokenExpiryMinutes} minutes.\n\n" +
                   "If you did not request a password reset, you can safely ignore this email.";

        await _emailService.SendAsync(user.Email, "SchoolERP Password Reset", body);
    }

    /// <inheritdoc />
    public async Task ResetPasswordAsync(ResetPasswordDto request)
    {
        var token = await _unitOfWork.PasswordResetTokenRepository.GetByTokenAsync(request.Token);

        if (token is null || !token.IsActive)
            throw new Common.Exceptions.BadRequestException("The password reset token is invalid or has expired.");

        var user = await _userRepository.GetByIdTrackedAsync(token.UserId)
            ?? throw new Common.Exceptions.NotFoundException(nameof(Domain.Entities.User), token.UserId);

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);
        _userRepository.Update(user);

        token.IsUsed = true;
        token.UsedAt = DateTime.UtcNow;
        _unitOfWork.PasswordResetTokenRepository.Update(token);

        await _unitOfWork.SaveChangesAsync();
    }
}