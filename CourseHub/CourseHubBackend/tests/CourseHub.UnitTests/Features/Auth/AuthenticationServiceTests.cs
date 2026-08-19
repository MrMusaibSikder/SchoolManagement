using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Common.Options;
using CourseHub.Application.Features.Auth;
using CourseHub.Application.Features.Auth.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Exceptions;
using Microsoft.Extensions.Options;
using Moq;

namespace CourseHub.UnitTests.Features.Auth;

public class AuthenticationServiceTests
{
    private readonly Mock<IUserRepository> _userRepository = new();
    private readonly Mock<IRoleRepository> _roleRepository = new();
    private readonly Mock<IUserRoleRepository> _userRoleRepository = new();
    private readonly Mock<IRolePermissionRepository> _rolePermissionRepository = new();
    private readonly Mock<IRefreshTokenRepository> _refreshTokenRepository = new();
    private readonly Mock<IPasswordResetTokenRepository> _passwordResetTokenRepository = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<IJwtTokenService> _jwtTokenService = new();
    private readonly Mock<ISecureTokenGenerator> _secureTokenGenerator = new();
    private readonly Mock<ITokenHasher> _tokenHasher = new();
    private readonly Mock<IEmailSender> _emailSender = new();

    private readonly AuthenticationService _sut;

    public AuthenticationServiceTests()
    {
        var jwtOptions = Options.Create(new JwtOptions
        {
            Issuer = "Test",
            Audience = "Test",
            SecretKey = "test-secret",
            AccessTokenExpirationMinutes = 15,
            RefreshTokenExpirationDays = 7,
        });

        var passwordResetOptions = Options.Create(new PasswordResetOptions
        {
            ExpirationMinutes = 60,
            ResetUrlBase = "https://localhost/reset-password",
        });

        var seedOptions = Options.Create(new SeedOptions
        {
            SuperAdminInviteCode = "the-real-invite-code",
        });

        _rolePermissionRepository
            .Setup(x => x.GetPermissionNamesForRolesAsync(It.IsAny<IReadOnlyList<string>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<string>());

        _jwtTokenService
            .Setup(x => x.GenerateAccessToken(It.IsAny<User>(), It.IsAny<IReadOnlyList<string>>(), It.IsAny<IReadOnlyList<string>>()))
            .Returns(new AccessTokenResult("access-token", DateTime.UtcNow.AddMinutes(15)));

        _sut = new AuthenticationService(
            _userRepository.Object,
            _roleRepository.Object,
            _userRoleRepository.Object,
            _rolePermissionRepository.Object,
            _refreshTokenRepository.Object,
            _passwordResetTokenRepository.Object,
            _unitOfWork.Object,
            _passwordHasher.Object,
            _jwtTokenService.Object,
            _secureTokenGenerator.Object,
            _tokenHasher.Object,
            _emailSender.Object,
            jwtOptions,
            passwordResetOptions,
            seedOptions);
    }

    private static User CreateActiveUser(string email = "user@example.com", string passwordHash = "stored-hash")
    {
        return User.Create(email, passwordHash, "Jane", "Doe");
    }

    private void SetupRole(string name)
    {
        var role = Role.CreateSystemRole(name);
        _roleRepository.Setup(x => x.GetByNameAsync(name, It.IsAny<CancellationToken>())).ReturnsAsync(role);
    }

    // ---------- Register ----------

    [Fact]
    public async Task RegisterAsync_Success_DefaultsToStudentRole()
    {
        _userRepository.Setup(x => x.ExistsByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        SetupRole("Student");
        _passwordHasher.Setup(x => x.HashPassword("Password123")).Returns("hashed-password");
        _secureTokenGenerator.Setup(x => x.Generate()).Returns("raw-refresh-token");
        _tokenHasher.Setup(x => x.Hash("raw-refresh-token")).Returns("hashed-refresh-token");

        var request = new RegisterRequest("user@example.com", "Password123", "Password123", "Jane", "Doe");

        var result = await _sut.RegisterAsync(request);

        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("raw-refresh-token", result.RefreshToken);
        Assert.Equal("user@example.com", result.User.Email);
        Assert.Equal(new[] { "Student" }, result.User.Roles);
        _userRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _userRoleRepository.Verify(x => x.AddAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_RequestedRoleTeacher_AssignsTeacherRole()
    {
        _userRepository.Setup(x => x.ExistsByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        SetupRole("Teacher");
        _passwordHasher.Setup(x => x.HashPassword("Password123")).Returns("hashed-password");
        _secureTokenGenerator.Setup(x => x.Generate()).Returns("raw-refresh-token");
        _tokenHasher.Setup(x => x.Hash("raw-refresh-token")).Returns("hashed-refresh-token");

        var request = new RegisterRequest("user@example.com", "Password123", "Password123", "Jane", "Doe", RequestedRole: "Teacher");

        var result = await _sut.RegisterAsync(request);

        Assert.Equal(new[] { "Teacher" }, result.User.Roles);
    }

    [Fact]
    public async Task RegisterAsync_CorrectSuperAdminCode_AssignsSuperAdminRegardlessOfRequestedRole()
    {
        _userRepository.Setup(x => x.ExistsByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        SetupRole("SuperAdmin");
        _passwordHasher.Setup(x => x.HashPassword("Password123")).Returns("hashed-password");
        _secureTokenGenerator.Setup(x => x.Generate()).Returns("raw-refresh-token");
        _tokenHasher.Setup(x => x.Hash("raw-refresh-token")).Returns("hashed-refresh-token");

        var request = new RegisterRequest(
            "user@example.com", "Password123", "Password123", "Jane", "Doe",
            RequestedRole: "Teacher", SuperAdminCode: "the-real-invite-code");

        var result = await _sut.RegisterAsync(request);

        Assert.Equal(new[] { "SuperAdmin" }, result.User.Roles);
    }

    [Fact]
    public async Task RegisterAsync_WrongSuperAdminCode_FallsBackToStudent()
    {
        _userRepository.Setup(x => x.ExistsByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        SetupRole("Student");
        _passwordHasher.Setup(x => x.HashPassword("Password123")).Returns("hashed-password");
        _secureTokenGenerator.Setup(x => x.Generate()).Returns("raw-refresh-token");
        _tokenHasher.Setup(x => x.Hash("raw-refresh-token")).Returns("hashed-refresh-token");

        var request = new RegisterRequest(
            "user@example.com", "Password123", "Password123", "Jane", "Doe",
            SuperAdminCode: "wrong-code");

        var result = await _sut.RegisterAsync(request);

        Assert.Equal(new[] { "Student" }, result.User.Roles);
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ThrowsValidationException()
    {
        _userRepository.Setup(x => x.ExistsByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var request = new RegisterRequest("user@example.com", "Password123", "Password123", "Jane", "Doe");

        await Assert.ThrowsAsync<ValidationException>(() => _sut.RegisterAsync(request));
        _userRepository.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_RoleNotSeeded_ThrowsDomainException()
    {
        _userRepository.Setup(x => x.ExistsByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _roleRepository.Setup(x => x.GetByNameAsync("Student", It.IsAny<CancellationToken>())).ReturnsAsync((Role?)null);

        var request = new RegisterRequest("user@example.com", "Password123", "Password123", "Jane", "Doe");

        await Assert.ThrowsAsync<DomainException>(() => _sut.RegisterAsync(request));
    }

    // ---------- Login ----------

    [Fact]
    public async Task LoginAsync_Success_RecordsLoginAndReturnsTokens()
    {
        var user = CreateActiveUser();
        _userRepository.Setup(x => x.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(x => x.VerifyPassword(user.PasswordHash, "Password123")).Returns(true);
        _userRoleRepository.Setup(x => x.GetRoleNamesForUserAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Student" });
        _secureTokenGenerator.Setup(x => x.Generate()).Returns("raw-refresh-token");
        _tokenHasher.Setup(x => x.Hash("raw-refresh-token")).Returns("hashed-refresh-token");

        var request = new LoginRequest("user@example.com", "Password123");

        var result = await _sut.LoginAsync(request, ipAddress: "127.0.0.1");

        Assert.Equal("access-token", result.AccessToken);
        Assert.Equal("raw-refresh-token", result.RefreshToken);
        Assert.Equal(new[] { "Student" }, result.User.Roles);
        Assert.NotNull(user.LastLoginAt);
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ThrowsAuthenticationException()
    {
        var user = CreateActiveUser();
        _userRepository.Setup(x => x.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(x => x.VerifyPassword(user.PasswordHash, "WrongPassword")).Returns(false);

        var request = new LoginRequest("user@example.com", "WrongPassword");

        await Assert.ThrowsAsync<AuthenticationException>(() => _sut.LoginAsync(request, null));
    }

    [Fact]
    public async Task LoginAsync_UnknownEmail_ThrowsAuthenticationException()
    {
        _userRepository.Setup(x => x.GetByEmailAsync("nobody@example.com", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var request = new LoginRequest("nobody@example.com", "Password123");

        await Assert.ThrowsAsync<AuthenticationException>(() => _sut.LoginAsync(request, null));
    }

    [Fact]
    public async Task LoginAsync_InactiveUser_ThrowsAuthenticationException()
    {
        var user = CreateActiveUser();
        user.Deactivate();
        _userRepository.Setup(x => x.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(x => x.VerifyPassword(user.PasswordHash, "Password123")).Returns(true);

        var request = new LoginRequest("user@example.com", "Password123");

        await Assert.ThrowsAsync<AuthenticationException>(() => _sut.LoginAsync(request, null));
    }

    // ---------- Refresh ----------

    [Fact]
    public async Task RefreshAsync_ValidToken_RotatesAndReturnsNewTokens()
    {
        var user = CreateActiveUser();
        var existingToken = RefreshToken.Create(user.Id, "old-hash", DateTime.UtcNow.AddDays(1));

        _tokenHasher.Setup(x => x.Hash("raw-old-token")).Returns("old-hash");
        _tokenHasher.Setup(x => x.Hash("raw-new-token")).Returns("new-hash");
        _refreshTokenRepository.Setup(x => x.GetByTokenHashAsync("old-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingToken);
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userRoleRepository.Setup(x => x.GetRoleNamesForUserAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Student" });
        _secureTokenGenerator.Setup(x => x.Generate()).Returns("raw-new-token");

        var request = new RefreshTokenRequest("raw-old-token");

        var result = await _sut.RefreshAsync(request, ipAddress: "127.0.0.1");

        Assert.Equal("raw-new-token", result.RefreshToken);
        Assert.True(existingToken.IsRevoked);
        Assert.NotNull(existingToken.ReplacedByTokenId);
    }

    [Fact]
    public async Task RefreshAsync_ExpiredToken_ThrowsAndDoesNotRotate()
    {
        var user = CreateActiveUser();
        var expiredToken = RefreshToken.Create(user.Id, "old-hash", DateTime.UtcNow.AddMinutes(-5));

        _tokenHasher.Setup(x => x.Hash("raw-old-token")).Returns("old-hash");
        _refreshTokenRepository.Setup(x => x.GetByTokenHashAsync("old-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expiredToken);

        var request = new RefreshTokenRequest("raw-old-token");

        await Assert.ThrowsAsync<AuthenticationException>(() => _sut.RefreshAsync(request, null));
        Assert.False(expiredToken.IsRevoked);
    }

    [Fact]
    public async Task RefreshAsync_RevokedTokenReuse_RevokesAllActiveSessionsAndThrows()
    {
        var user = CreateActiveUser();
        var reusedToken = RefreshToken.Create(user.Id, "old-hash", DateTime.UtcNow.AddDays(1));
        reusedToken.Revoke(); // simulate: already rotated out once before

        var otherActiveToken1 = RefreshToken.Create(user.Id, "hash-a", DateTime.UtcNow.AddDays(1));
        var otherActiveToken2 = RefreshToken.Create(user.Id, "hash-b", DateTime.UtcNow.AddDays(1));

        _tokenHasher.Setup(x => x.Hash("raw-old-token")).Returns("old-hash");
        _refreshTokenRepository.Setup(x => x.GetByTokenHashAsync("old-hash", It.IsAny<CancellationToken>()))
            .ReturnsAsync(reusedToken);
        _refreshTokenRepository.Setup(x => x.GetActiveByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { otherActiveToken1, otherActiveToken2 });

        var request = new RefreshTokenRequest("raw-old-token");

        await Assert.ThrowsAsync<AuthenticationException>(() => _sut.RefreshAsync(request, null));
        Assert.True(otherActiveToken1.IsRevoked);
        Assert.True(otherActiveToken2.IsRevoked);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // ---------- Logout ----------

    [Fact]
    public async Task LogoutAsync_RevokesTheSuppliedToken()
    {
        var userId = Guid.NewGuid();
        var token = RefreshToken.Create(userId, "hash", DateTime.UtcNow.AddDays(1));

        _tokenHasher.Setup(x => x.Hash("raw-token")).Returns("hash");
        _refreshTokenRepository.Setup(x => x.GetByTokenHashAsync("hash", It.IsAny<CancellationToken>())).ReturnsAsync(token);

        await _sut.LogoutAsync(userId, new LogoutRequest("raw-token"));

        Assert.True(token.IsRevoked);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_TokenBelongsToDifferentUser_DoesNotRevokeIt()
    {
        var tokenOwnerId = Guid.NewGuid();
        var callerUserId = Guid.NewGuid();
        var token = RefreshToken.Create(tokenOwnerId, "hash", DateTime.UtcNow.AddDays(1));

        _tokenHasher.Setup(x => x.Hash("raw-token")).Returns("hash");
        _refreshTokenRepository.Setup(x => x.GetByTokenHashAsync("hash", It.IsAny<CancellationToken>())).ReturnsAsync(token);

        await _sut.LogoutAsync(callerUserId, new LogoutRequest("raw-token"));

        Assert.False(token.IsRevoked);
        _unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    // ---------- Change password ----------

    [Fact]
    public async Task ChangePasswordAsync_Success_UpdatesHashAndRevokesActiveSessions()
    {
        var user = CreateActiveUser(passwordHash: "old-hash");
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(x => x.VerifyPassword("old-hash", "CurrentPass1")).Returns(true);
        _passwordHasher.Setup(x => x.VerifyPassword("old-hash", "NewPass123")).Returns(false);
        _passwordHasher.Setup(x => x.HashPassword("NewPass123")).Returns("new-hash");

        var activeToken = RefreshToken.Create(user.Id, "hash", DateTime.UtcNow.AddDays(1));
        _refreshTokenRepository.Setup(x => x.GetActiveByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { activeToken });

        await _sut.ChangePasswordAsync(user.Id, new ChangePasswordRequest("CurrentPass1", "NewPass123", "NewPass123"));

        Assert.Equal("new-hash", user.PasswordHash);
        Assert.True(activeToken.IsRevoked);
    }

    [Fact]
    public async Task ChangePasswordAsync_WrongCurrentPassword_ThrowsValidationException()
    {
        var user = CreateActiveUser(passwordHash: "old-hash");
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(x => x.VerifyPassword("old-hash", "WrongCurrent")).Returns(false);

        await Assert.ThrowsAsync<ValidationException>(() =>
            _sut.ChangePasswordAsync(user.Id, new ChangePasswordRequest("WrongCurrent", "NewPass123", "NewPass123")));
    }

    [Fact]
    public async Task ChangePasswordAsync_SameAsCurrentPassword_ThrowsValidationException()
    {
        var user = CreateActiveUser(passwordHash: "old-hash");
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(x => x.VerifyPassword("old-hash", It.IsAny<string>())).Returns(true);

        await Assert.ThrowsAsync<ValidationException>(() =>
            _sut.ChangePasswordAsync(user.Id, new ChangePasswordRequest("CurrentPass1", "CurrentPass1", "CurrentPass1")));
    }

    // ---------- Forgot / reset password ----------

    [Fact]
    public async Task ForgotPasswordAsync_UnknownEmail_DoesNothingAndDoesNotThrow()
    {
        _userRepository.Setup(x => x.GetByEmailAsync("nobody@example.com", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        await _sut.ForgotPasswordAsync(new ForgotPasswordRequest("nobody@example.com"));

        _emailSender.Verify(x => x.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _passwordResetTokenRepository.Verify(x => x.AddAsync(It.IsAny<PasswordResetToken>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ForgotPasswordAsync_KnownActiveUser_CreatesTokenAndSendsEmail()
    {
        var user = CreateActiveUser("user@example.com");
        _userRepository.Setup(x => x.GetByEmailAsync("user@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _secureTokenGenerator.Setup(x => x.Generate()).Returns("raw-reset-token");
        _tokenHasher.Setup(x => x.Hash("raw-reset-token")).Returns("hashed-reset-token");

        await _sut.ForgotPasswordAsync(new ForgotPasswordRequest("user@example.com"));

        _passwordResetTokenRepository.Verify(x => x.AddAsync(It.IsAny<PasswordResetToken>(), It.IsAny<CancellationToken>()), Times.Once);
        _emailSender.Verify(x => x.SendPasswordResetEmailAsync("user@example.com", It.Is<string>(link => link.Contains("raw-reset-token")), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ResetPasswordAsync_ValidToken_UpdatesPasswordAndRevokesSessions()
    {
        var user = CreateActiveUser(passwordHash: "old-hash");
        var resetToken = PasswordResetToken.Create(user.Id, "hash", DateTime.UtcNow.AddMinutes(30));

        _tokenHasher.Setup(x => x.Hash("raw-reset-token")).Returns("hash");
        _passwordResetTokenRepository.Setup(x => x.GetByTokenHashAsync("hash", It.IsAny<CancellationToken>())).ReturnsAsync(resetToken);
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _passwordHasher.Setup(x => x.HashPassword("NewPass123")).Returns("new-hash");

        var activeToken = RefreshToken.Create(user.Id, "rt-hash", DateTime.UtcNow.AddDays(1));
        _refreshTokenRepository.Setup(x => x.GetActiveByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { activeToken });

        await _sut.ResetPasswordAsync(new ResetPasswordRequest("raw-reset-token", "NewPass123", "NewPass123"));

        Assert.Equal("new-hash", user.PasswordHash);
        Assert.True(resetToken.IsUsed);
        Assert.True(activeToken.IsRevoked);
    }

    [Fact]
    public async Task ResetPasswordAsync_ExpiredToken_ThrowsAuthenticationException()
    {
        var user = CreateActiveUser();
        var expiredToken = PasswordResetToken.Create(user.Id, "hash", DateTime.UtcNow.AddMinutes(-5));

        _tokenHasher.Setup(x => x.Hash("raw-reset-token")).Returns("hash");
        _passwordResetTokenRepository.Setup(x => x.GetByTokenHashAsync("hash", It.IsAny<CancellationToken>())).ReturnsAsync(expiredToken);

        await Assert.ThrowsAsync<AuthenticationException>(() =>
            _sut.ResetPasswordAsync(new ResetPasswordRequest("raw-reset-token", "NewPass123", "NewPass123")));
    }

    [Fact]
    public async Task ResetPasswordAsync_AlreadyUsedToken_ThrowsAuthenticationException()
    {
        var user = CreateActiveUser();
        var usedToken = PasswordResetToken.Create(user.Id, "hash", DateTime.UtcNow.AddMinutes(30));
        usedToken.MarkAsUsed();

        _tokenHasher.Setup(x => x.Hash("raw-reset-token")).Returns("hash");
        _passwordResetTokenRepository.Setup(x => x.GetByTokenHashAsync("hash", It.IsAny<CancellationToken>())).ReturnsAsync(usedToken);

        await Assert.ThrowsAsync<AuthenticationException>(() =>
            _sut.ResetPasswordAsync(new ResetPasswordRequest("raw-reset-token", "NewPass123", "NewPass123")));
    }

    // ---------- Current user ----------

    [Fact]
    public async Task GetCurrentUserAsync_ReturnsUserWithRoles()
    {
        var user = CreateActiveUser();
        _userRepository.Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _userRoleRepository.Setup(x => x.GetRoleNamesForUserAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { "Teacher" });

        var result = await _sut.GetCurrentUserAsync(user.Id);

        Assert.Equal(user.Email, result.Email);
        Assert.Equal(new[] { "Teacher" }, result.Roles);
    }

    [Fact]
    public async Task GetCurrentUserAsync_UnknownUser_ThrowsNotFoundException()
    {
        var userId = Guid.NewGuid();
        _userRepository.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.GetCurrentUserAsync(userId));
    }
}
