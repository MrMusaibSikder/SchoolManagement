using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Common.Options;
using CourseHub.Application.Common.Security;
using CourseHub.Application.Features.Auth.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Enums;
using CourseHub.Domain.Exceptions;
using Microsoft.Extensions.Options;

namespace CourseHub.Application.Features.Auth;

/// <summary>
/// Orchestrates the authentication use cases. CourseHub is single-institute,
/// so there is no institution scoping here — every operation is global.
/// Contains no framework dependencies (EF Core, ASP.NET Core) — only
/// Application abstractions, consistent with Clean Architecture.
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";
    private const string GenericAuthFailureMessage = "Authentication failed.";
    private const string DefaultRegistrationRole = SystemRoleNames.Student;

    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRolePermissionRepository _rolePermissionRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ISecureTokenGenerator _secureTokenGenerator;
    private readonly ITokenHasher _tokenHasher;
    private readonly IEmailSender _emailSender;
    private readonly JwtOptions _jwtOptions;
    private readonly PasswordResetOptions _passwordResetOptions;
    private readonly SeedOptions _seedOptions;

    public AuthenticationService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IRolePermissionRepository rolePermissionRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordResetTokenRepository passwordResetTokenRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        ISecureTokenGenerator secureTokenGenerator,
        ITokenHasher tokenHasher,
        IEmailSender emailSender,
        IOptions<JwtOptions> jwtOptions,
        IOptions<PasswordResetOptions> passwordResetOptions,
        IOptions<SeedOptions> seedOptions)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _rolePermissionRepository = rolePermissionRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordResetTokenRepository = passwordResetTokenRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _secureTokenGenerator = secureTokenGenerator;
        _tokenHasher = tokenHasher;
        _emailSender = emailSender;
        _jwtOptions = jwtOptions.Value;
        _passwordResetOptions = passwordResetOptions.Value;
        _seedOptions = seedOptions.Value;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var emailExists = await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (emailExists)
        {
            // Registration duplicate-email is intentionally NOT genericized
            // the way login is: the caller is actively trying to create an
            // account, so confirming "this email is taken" here does not
            // create the same enumeration risk as a login/forgot-password
            // response would.
            throw new ValidationException("An account with this email already exists.");
        }

        var roleName = ResolveRegistrationRole(request);
        var role = await _roleRepository.GetByNameAsync(roleName, cancellationToken)
            ?? throw new DomainException(
                $"Required role '{roleName}' is not seeded yet. Restart the API once so startup seeding can run, then try again.");

        var passwordHash = _passwordHasher.HashPassword(request.Password);
        var user = User.Create(request.Email, passwordHash, request.FirstName, request.LastName);

        await _userRepository.AddAsync(user, cancellationToken);

        var userRole = UserRole.Create(user.Id, role.Id);
        await _userRoleRepository.AddAsync(userRole, cancellationToken);

        var refreshToken = await CreateRefreshTokenAsync(user.Id, ipAddress: null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var roleNames = new[] { role.Name };
        var permissions = await _rolePermissionRepository.GetPermissionNamesForRolesAsync(roleNames, cancellationToken);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roleNames, permissions);
        return BuildAuthResponse(user, accessToken, refreshToken.RawToken, roleNames);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        // Same exception/message whether the user doesn't exist, the
        // password is wrong, or the account is inactive — this is what
        // prevents account enumeration via the login endpoint.
        if (user is null || !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
        {
            throw new AuthenticationException(InvalidCredentialsMessage);
        }

        if (user.Status != UserStatus.Active)
        {
            throw new AuthenticationException(InvalidCredentialsMessage);
        }

        user.RecordLogin();

        var refreshToken = await CreateRefreshTokenAsync(user.Id, ipAddress, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var roles = await _userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
        var permissions = await _rolePermissionRepository.GetPermissionNamesForRolesAsync(roles, cancellationToken);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles, permissions);
        return BuildAuthResponse(user, accessToken, refreshToken.RawToken, roles);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var tokenHash = _tokenHasher.Hash(request.RefreshToken);
        var existingToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (existingToken is null)
        {
            throw new AuthenticationException(GenericAuthFailureMessage);
        }

        if (existingToken.IsRevoked)
        {
            // Reuse of an already-rotated-out token is a strong signal the
            // token was stolen. Respond by revoking every active session
            // for that user, forcing re-authentication everywhere.
            var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(existingToken.UserId, cancellationToken);
            foreach (var token in activeTokens)
            {
                token.Revoke();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            throw new AuthenticationException("Refresh token has already been used. All sessions have been revoked for security.");
        }

        if (existingToken.IsExpired)
        {
            throw new AuthenticationException(GenericAuthFailureMessage);
        }

        var user = await _userRepository.GetByIdAsync(existingToken.UserId, cancellationToken);
        if (user is null || user.Status != UserStatus.Active)
        {
            throw new AuthenticationException(GenericAuthFailureMessage);
        }

        var newRefreshToken = await CreateRefreshTokenAsync(user.Id, ipAddress, cancellationToken);

        // Rotation: revoke the used token and link it to its replacement.
        existingToken.Revoke(newRefreshToken.Id);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var roles = await _userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
        var permissions = await _rolePermissionRepository.GetPermissionNamesForRolesAsync(roles, cancellationToken);
        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles, permissions);
        return BuildAuthResponse(user, accessToken, newRefreshToken.RawToken, roles);
    }

    public async Task LogoutAsync(Guid currentUserId, LogoutRequest request, CancellationToken cancellationToken = default)
    {
        var tokenHash = _tokenHasher.Hash(request.RefreshToken);
        var existingToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        // Not found, already revoked, or belonging to a different user: in
        // every case logout should look like a harmless success rather
        // than leaking which condition applied.
        if (existingToken is null || existingToken.UserId != currentUserId)
        {
            return;
        }

        existingToken.Revoke();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangePasswordAsync(Guid currentUserId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(currentUserId, cancellationToken)
            ?? throw new NotFoundException("User", currentUserId);

        if (!_passwordHasher.VerifyPassword(user.PasswordHash, request.CurrentPassword))
        {
            throw new ValidationException("Current password is incorrect.");
        }

        if (_passwordHasher.VerifyPassword(user.PasswordHash, request.NewPassword))
        {
            throw new ValidationException("New password must be different from the current password.");
        }

        var newHash = _passwordHasher.HashPassword(request.NewPassword);
        user.ChangePasswordHash(newHash);

        await RevokeAllActiveSessionsAsync(user.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        // Always behave the same way whether or not the account exists —
        // callers must not be able to tell the difference from the response.
        if (user is null || user.Status != UserStatus.Active)
        {
            return;
        }

        var rawToken = _secureTokenGenerator.Generate();
        var tokenHash = _tokenHasher.Hash(rawToken);
        var expiresAt = DateTime.UtcNow.AddMinutes(_passwordResetOptions.ExpirationMinutes);

        var resetToken = PasswordResetToken.Create(user.Id, tokenHash, expiresAt);
        await _passwordResetTokenRepository.AddAsync(resetToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var resetLink = $"{_passwordResetOptions.ResetUrlBase}?token={Uri.EscapeDataString(rawToken)}";
        await _emailSender.SendPasswordResetEmailAsync(user.Email, resetLink, cancellationToken);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var tokenHash = _tokenHasher.Hash(request.Token);
        var resetToken = await _passwordResetTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (resetToken is null || !resetToken.IsValid)
        {
            throw new AuthenticationException("This password reset link is invalid or has expired.");
        }

        var user = await _userRepository.GetByIdAsync(resetToken.UserId, cancellationToken)
            ?? throw new AuthenticationException("This password reset link is invalid or has expired.");

        var newHash = _passwordHasher.HashPassword(request.NewPassword);
        user.ChangePasswordHash(newHash);

        resetToken.MarkAsUsed();

        await RevokeAllActiveSessionsAsync(user.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<CurrentUserResponse> GetCurrentUserAsync(Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(currentUserId, cancellationToken)
            ?? throw new NotFoundException("User", currentUserId);

        var roles = await _userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
        var permissions = await _rolePermissionRepository.GetPermissionNamesForRolesAsync(roles, cancellationToken);

        return new CurrentUserResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Status.ToString(),
            roles,
            permissions,
            user.LastLoginAt);
    }

    /// <summary>
    /// SuperAdminCode (if it matches configuration) wins over everything.
    /// Otherwise: "Teacher" if explicitly requested, else "Student".
    /// "Admin" can never be self-selected here — see RegisterRequest doc.
    /// </summary>
    private string ResolveRegistrationRole(RegisterRequest request)
    {
        if (!string.IsNullOrEmpty(_seedOptions.SuperAdminInviteCode) &&
            !string.IsNullOrEmpty(request.SuperAdminCode) &&
            string.Equals(request.SuperAdminCode, _seedOptions.SuperAdminInviteCode, StringComparison.Ordinal))
        {
            return SystemRoleNames.SuperAdmin;
        }

        if (!string.IsNullOrWhiteSpace(request.RequestedRole) &&
            string.Equals(request.RequestedRole, SystemRoleNames.Teacher, StringComparison.OrdinalIgnoreCase))
        {
            return SystemRoleNames.Teacher;
        }

        return DefaultRegistrationRole;
    }

    private async Task<(Guid Id, string RawToken)> CreateRefreshTokenAsync(Guid userId, string? ipAddress, CancellationToken cancellationToken)
    {
        var rawToken = _secureTokenGenerator.Generate();
        var tokenHash = _tokenHasher.Hash(rawToken);
        var expiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays);

        var refreshToken = RefreshToken.Create(userId, tokenHash, expiresAt, ipAddress);
        await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);

        return (refreshToken.Id, rawToken);
    }

    private async Task RevokeAllActiveSessionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(userId, cancellationToken);
        foreach (var token in activeTokens)
        {
            token.Revoke();
        }
    }

    private static AuthResponse BuildAuthResponse(User user, AccessTokenResult accessToken, string rawRefreshToken, IReadOnlyList<string> roles)
    {
        var userSummary = new UserSummaryResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.Status.ToString(),
            roles);

        return new AuthResponse(accessToken.Token, rawRefreshToken, accessToken.ExpiresAtUtc, userSummary);
    }
}
