using CourseHub.Application.Common.Dtos;
using CourseHub.Application.Common.Interfaces;
using CourseHub.Application.Common.Security;
using CourseHub.Application.Features.Students.Dtos;
using CourseHub.Domain.Entities;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Application.Features.Students;

public class StudentService : IStudentService
{
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private readonly IStudentRepository _studentRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StudentService(
        IStudentRepository studentRepository,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _studentRepository = studentRepository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<StudentResponse>> SearchAsync(string? search, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var normalizedPage = Math.Max(page, 1);
        var normalizedPageSize = Math.Clamp(pageSize <= 0 ? DefaultPageSize : pageSize, 1, MaxPageSize);

        var (items, totalCount) = await _studentRepository.SearchAsync(search, normalizedPage, normalizedPageSize, cancellationToken);

        var responses = items.Select(ToResponse).ToList();

        return new PagedResult<StudentResponse>(responses, totalCount, normalizedPage, normalizedPageSize);
    }

    public async Task<StudentResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await RequireStudentAsync(id, cancellationToken);
        return ToResponse(student);
    }

    public async Task<StudentResponse> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken = default)
    {
        // Same "promotion of an existing User" pattern as
        // TeacherService.CreateAsync — see that class for the full
        // reasoning. Every check here throws a friendly 400/404 instead
        // of letting the DB's unique indexes (UserId, StudentId — see
        // StudentConfiguration) reject it as a raw constraint violation.
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        var userRoles = await _userRoleRepository.GetRoleNamesForUserAsync(user.Id, cancellationToken);
        if (!userRoles.Contains(SystemRoleNames.Student, StringComparer.OrdinalIgnoreCase))
        {
            throw new ValidationException(
                $"User '{user.Email}' does not have the {SystemRoleNames.Student} role. Assign the role before creating a student profile.");
        }

        if (await _studentRepository.ExistsByUserIdAsync(user.Id, cancellationToken))
        {
            throw new ValidationException("This user already has a student profile.");
        }

        await EnsureStudentIdIsAvailableAsync(request.StudentId, excludingId: null, cancellationToken);

        var student = Student.Create(user.Id, request.StudentId, request.FirstName, request.LastName);

        await _studentRepository.AddAsync(student, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(student);
    }

    public async Task<StudentResponse> UpdateProfileAsync(Guid id, UpdateStudentProfileRequest request, CancellationToken cancellationToken = default)
    {
        var student = await RequireStudentAsync(id, cancellationToken);

        student.UpdateProfile(request.FirstName, request.LastName, request.DateOfBirth);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(student);
    }

    public async Task<StudentResponse> UpdateContactAsync(Guid id, UpdateStudentContactRequest request, CancellationToken cancellationToken = default)
    {
        var student = await RequireStudentAsync(id, cancellationToken);

        student.UpdateContact(request.Phone, request.Email, request.Address);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(student);
    }

    public async Task<StudentResponse> UpdateGuardianAsync(Guid id, UpdateStudentGuardianRequest request, CancellationToken cancellationToken = default)
    {
        var student = await RequireStudentAsync(id, cancellationToken);

        student.UpdateGuardian(request.GuardianName, request.GuardianPhone);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(student);
    }

    public async Task<StudentResponse> UpdateProfileImageAsync(Guid id, UpdateStudentProfileImageRequest request, CancellationToken cancellationToken = default)
    {
        var student = await RequireStudentAsync(id, cancellationToken);

        student.UpdateProfileImage(request.ProfileImageUrl);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ToResponse(student);
    }

    public async Task<StudentResponse> ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await RequireStudentAsync(id, cancellationToken);
        student.Activate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(student);
    }

    public async Task<StudentResponse> DeactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await RequireStudentAsync(id, cancellationToken);
        student.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(student);
    }

    public async Task<StudentResponse> MakeProfilePublicAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await RequireStudentAsync(id, cancellationToken);
        student.MakeProfilePublic();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(student);
    }

    public async Task<StudentResponse> MakeProfilePrivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await RequireStudentAsync(id, cancellationToken);
        student.MakeProfilePrivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ToResponse(student);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var student = await RequireStudentAsync(id, cancellationToken);

        // Soft delete — and unlike Teacher, this one IS backed by a real
        // FK today: Enrollment.StudentId has DeleteBehavior.Restrict
        // against Student (see EnrollmentConfiguration), specifically so
        // enrollment history survives. A hard delete on a student with
        // any enrollment would fail with a raw FK-constraint
        // DbUpdateException; Deactivate keeps the row and every
        // enrollment record intact.
        student.Deactivate();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureStudentIdIsAvailableAsync(string studentId, Guid? excludingId, CancellationToken cancellationToken)
    {
        var taken = await _studentRepository.ExistsByStudentIdAsync(studentId, excludingId, cancellationToken);

        if (taken)
        {
            throw new ValidationException($"A student with id '{studentId}' already exists.");
        }
    }

    private async Task<Student> RequireStudentAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _studentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("Student", id);
    }

    private static StudentResponse ToResponse(Student student) => new(
        student.Id,
        student.UserId,
        student.StudentId,
        student.FirstName,
        student.LastName,
        student.ProfileImageUrl,
        student.DateOfBirth,
        student.Phone,
        student.Email,
        student.Address,
        student.GuardianName,
        student.GuardianPhone,
        student.IsActive,
        student.IsProfilePublic,
        student.CreatedAt,
        student.UpdatedAt);
}
