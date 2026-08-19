namespace CourseHub.Application.Features.Auth.Dtos;

public record ResetPasswordRequest(
    string Token,
    string NewPassword,
    string ConfirmNewPassword);
