namespace CourseHub.Application.Features.Permissions.Dtos;

public record RolePermissionsResponse(
    Guid RoleId,
    string RoleName,
    IReadOnlyList<string> Permissions);
