using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Common.Security;
using CourseHub.Application.Features.Teachers.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Application.Features.Teachers;

public class TeacherService : ITeacherService
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly ITeacherRepository _teacherRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TeacherService(
        ITeacherRepository teacherRepository,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _teacherRepository = teacherRepository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<TeacherResponse>> SearchAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var normalizedPage = Math.Max(page, 1);
        var normalizedPageSize = Math.Clamp(pageSize <= 0 ? DefaultPageSize : pageSize, 1, MaxPageSize);

        var (items, totalCount) = await _teacherRepository.SearchAsync(search, normalizedPage, normalizedPageSize, cancellationToken);

        var responses = items.Select(ToResponse).ToList();

        return new PagedResult<TeacherResponse>(responses, totalCount, normalizedPage, normalizedPageSize);
    }

    public async Task<TeacherResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var teacher = await RequireTeacherAsync(id, cancellationToken);
        return ToResponse(teacher);
    }

    public async Task<TeacherResponse> CreateAsync(CreateTeacherRequest request, CancellationToken cancellationToken = default)
    {
        // A Teacher profile always belongs to an existing User account —
        // this is a promotion, not a standalone record. Every check below
        // throws a friendly 400/404 instead of letting the DB's unique
        // indexes (UserId, EmployeeId — see TeacherConfiguration) reject
        // it as a raw constraint violation.
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        var userRoles = await _userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
        if (!userRoles.Contains(SystemRoleNames.Teacher, StringComparer.OrdinalIgnoreCase))
        {
            throw new ValidationException(
                $"User '{user.Email}' does not have the {SystemRoleNames.Teacher} role. Assign the role before creating a teacher profile.");
        }

        if (await _teacherRepository.ExistsByUserIdAsync(user.Id, cancellationToken))
        {
            throw new ValidationException("This user already has a teacher profile.");
        }

        await EnsureEmployeeIdIsAvailableAsync(request.EmployeeId, excludingId: null, cancellationToken);

        var teacher = Teacher.Create(user.Id, request.EmployeeId, request.FirstName, request.LastName);

        await _teacherRepository.AddAsync(teacher, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(teacher);
    }

    public async Task<TeacherResponse> UpdateProfileAsync(Guid id, UpdateTeacherProfileRequest request, CancellationToken cancellationToken = default)
    {
        var teacher = await RequireTeacherAsync(id, cancellationToken);

        teacher.UpdateProfile(request.FirstName, request.LastName, request.Bio);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(teacher);
    }

    public async Task<TeacherResponse> UpdateContactAsync(Guid id, UpdateTeacherContactRequest request, CancellationToken cancellationToken = default)
    {
        var teacher = await RequireTeacherAsync(id, cancellationToken);

        teacher.UpdateContact(request.Phone, request.Email);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(teacher);
    }

    public async Task<TeacherResponse> UpdateProfileImageAsync(Guid id, UpdateTeacherProfileImageRequest request, CancellationToken cancellationToken = default)
    {
        var teacher = await RequireTeacherAsync(id, cancellationToken);

        teacher.UpdateProfileImage(request.ProfileImageUrl);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(teacher);
    }

    public async Task<TeacherResponse> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var teacher = await RequireTeacherAsync(id, cancellationToken);
        teacher.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(teacher);
    }

    public async Task<TeacherResponse> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var teacher = await RequireTeacherAsync(id, cancellationToken);
        teacher.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(teacher);
    }

    public async Task<TeacherResponse> MakeProfilePublicAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var teacher = await RequireTeacherAsync(id, cancellationToken);
        teacher.MakeProfilePublic();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(teacher);
    }

    public async Task<TeacherResponse> MakeProfilePrivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var teacher = await RequireTeacherAsync(id, cancellationToken);
        teacher.MakeProfilePrivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(teacher);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var teacher = await RequireTeacherAsync(id, cancellationToken);

        // Soft delete, same reasoning as CourseService.DeleteAsync: no FK
        // currently references Teacher.Id (Batch-to-teacher assignment is
        // explicitly deferred — see the comment on Batch), but Employee
        // history and public-profile audit trail matter regardless, and a
        // predictable "DELETE always deactivates, never removes the row"
        // contract keeps the admin API consistent across every resource.
        teacher.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureEmployeeIdIsAvailableAsync(string employeeId, Guid? excludingId, CancellationToken cancellationToken)
    {
        var taken = await _teacherRepository.ExistsByEmployeeIdAsync(employeeId, excludingId, cancellationToken);

        if (taken)
        {
            throw new ValidationException($"A teacher with employee id '{employeeId}' already exists.");
        }
    }

    private async Task<Teacher> RequireTeacherAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _teacherRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Teacher", id);
    }

    private static TeacherResponse ToResponse(Teacher teacher) => new(
        teacher.Id,
        teacher.UserId,
        teacher.EmployeeId,
        teacher.FirstName,
        teacher.LastName,
        teacher.ProfileImageUrl,
        teacher.Phone,
        teacher.Email,
        teacher.Bio,
        teacher.IsActive,
        teacher.IsProfilePublic,
        teacher.CreatedAt,
        teacher.UpdatedAt);
}
