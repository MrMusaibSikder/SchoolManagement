namespace CourseHub.Application.Features.Public;

public interface IPublicInstitutionService
{
    Task<InstitutionProfileResponse?> GetProfileAsync(CancellationToken cancellationToken = default);
}
