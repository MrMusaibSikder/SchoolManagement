using CourseHub.Application.Features.Auth.Dtos;

namespace CourseHub.Application.Features.Auth;

public interface IAuthenticationService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponse> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken = default);

    Task<AuthResponse> RefreshAsync(RefreshTokenRequest request, string? ipAddress, CancellationToken cancellationToken = default);

    Task LogoutAsync(Guid currentUserId, LogoutRequest request, CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(Guid currentUserId, ChangePasswordRequest request, CancellationToken cancellationToken = default);

    Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);

    Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);

    Task<CurrentUserResponse> GetCurrentUserAsync(Guid currentUserId, CancellationToken cancellationToken = default);
}
