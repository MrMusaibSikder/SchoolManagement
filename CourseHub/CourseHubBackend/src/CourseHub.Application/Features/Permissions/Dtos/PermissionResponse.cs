namespace CourseHub.Application.Features.Permissions.Dtos;

public record PermissionResponse(
    Guid Id,
    string Name,
    string Resource,
    string Action,
    string? Description);
