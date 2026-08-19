using CourseHub.Application.Common.Interfaces;

namespace CourseHub.Application.Features.Public;

public class PublicInstitutionService : IPublicInstitutionService
{
    private readonly IInstitutionRepository _institutionRepository;

    public PublicInstitutionService(IInstitutionRepository institutionRepository)
    {
        _institutionRepository = institutionRepository;
    }

    public async Task<InstitutionProfileResponse?> GetProfileAsync(CancellationToken cancellationToken = default)
    {
        var institution = await _institutionRepository.GetAsync(cancellationToken);

        // Not public/not active: behave the same as "no data" rather than
        // leaking that a private institution profile exists.
        if (institution is null || !institution.IsPublic || !institution.IsActive)
        {
            return null;
        }

        return new InstitutionProfileResponse(
            institution.Name,
            institution.Slug,
            institution.LogoUrl,
            institution.CoverImageUrl,
            institution.Description,
            institution.Address,
            institution.Phone,
            institution.Email,
            institution.Website);
    }
}
