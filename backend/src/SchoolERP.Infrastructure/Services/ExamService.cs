using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Exam.DTOs;
using SchoolERP.Application.Features.Exam.Interfaces;
using SchoolERP.Application.Features.ExamSchedule.DTOs;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for Exam records. Calls the repository (via the Unit of
/// Work), applies business rules (duplicate validation, the Draft/Published/
/// Completed/Cancelled lifecycle, dashboard/routine/calendar composition),
/// and maps entities to/from DTOs using AutoMapper. Deliberately excludes any
/// marks/grade/result concerns — those belong to the future Result module.
/// </summary>
public class ExamService : IExamService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExamService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.ExamRepository.GetAllWithDetailsAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ExamDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<ExamDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<ExamDto>(entity);
    }

    /// <inheritdoc />
    public async Task<ExamDetailsDto> GetExamDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamRepository.GetExamWithSchedulesAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), id);

        var schedules = _mapper.Map<IReadOnlyList<ExamScheduleDto>>(entity.ExamSchedules);

        return new ExamDetailsDto
        {
            Id = entity.Id,
            Name = entity.Name,
            ExamTypeId = entity.ExamTypeId,
            ExamTypeName = entity.ExamType?.Name ?? string.Empty,
            AcademicYearId = entity.AcademicYearId,
            AcademicYearName = entity.AcademicYear?.Name ?? string.Empty,
            Status = entity.Status,
            TotalSchedules = schedules.Count,
            Schedules = schedules
        };
    }

    /// <inheritdoc />
    public async Task<ExamSummaryDto> GetExamSummaryAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamRepository.GetExamWithSchedulesAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), id);

        return BuildSummary(entity);
    }

    /// <inheritdoc />
    public async Task<ExamStatisticsDto> GetExamStatisticsAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamRepository.GetExamWithStatisticsAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), id);

        var schedules = entity.ExamSchedules.ToList();
        var startDate = schedules.Count > 0 ? schedules.Min(s => s.ExamDate) : (DateTime?)null;
        var endDate = schedules.Count > 0 ? schedules.Max(s => s.ExamDate) : (DateTime?)null;

        return new ExamStatisticsDto
        {
            ExamId = entity.Id,
            ExamName = entity.Name,
            Status = entity.Status,
            TotalSchedules = schedules.Count,
            TotalSubjects = schedules.Select(s => s.SubjectId).Distinct().Count(),
            TotalClasses = schedules.Select(s => s.ClassId).Distinct().Count(),
            StartDate = startDate,
            EndDate = endDate,
            DurationInDays = startDate.HasValue && endDate.HasValue
                ? (endDate.Value.Date - startDate.Value.Date).Days
                : 0
        };
    }

    /// <inheritdoc />
    public async Task<ExamDto> CreateAsync(CreateExamDto request, CancellationToken cancellationToken = default)
    {
        var examType = await _unitOfWork.ExamTypeRepository.GetByIdAsync(request.ExamTypeId, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamType), request.ExamTypeId);

        var academicYear = await _unitOfWork.AcademicYearRepository.GetByIdAsync(request.AcademicYearId, cancellationToken)
            ?? throw new NotFoundException(nameof(AcademicYear), request.AcademicYearId);

        await EnsureNotDuplicateAsync(request.Name, request.AcademicYearId, request.ExamTypeId, excludeId: null, cancellationToken);

        var entity = _mapper.Map<Exam>(request);
        entity.Status = ExamStatus.Draft;

        await _unitOfWork.ExamRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Avoid a second round-trip: attach the already-fetched navigation
        // entities so the returned DTO is correctly enriched with names.
        entity.ExamType = examType;
        entity.AcademicYear = academicYear;

        return _mapper.Map<ExamDto>(entity);
    }

    /// <inheritdoc />
    public async Task<ExamDto> UpdateAsync(int id, UpdateExamDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), id);

        EnsureEditable(entity);

        var examType = await _unitOfWork.ExamTypeRepository.GetByIdAsync(request.ExamTypeId, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamType), request.ExamTypeId);

        var academicYear = await _unitOfWork.AcademicYearRepository.GetByIdAsync(request.AcademicYearId, cancellationToken)
            ?? throw new NotFoundException(nameof(AcademicYear), request.AcademicYearId);

        await EnsureNotDuplicateAsync(request.Name, request.AcademicYearId, request.ExamTypeId, excludeId: id, cancellationToken);

        _mapper.Map(request, entity);

        _unitOfWork.ExamRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        entity.ExamType = examType;
        entity.AcademicYear = academicYear;

        return _mapper.Map<ExamDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), id);

        if (entity.Status is ExamStatus.Completed or ExamStatus.Cancelled)
        {
            throw new BadRequestException($"A {entity.Status} exam cannot be deleted.");
        }

        _unitOfWork.ExamRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ExamDto> PublishExamAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), id);

        if (entity.Status != ExamStatus.Draft)
        {
            throw new BadRequestException($"Only a Draft exam can be published. Current status: {entity.Status}.");
        }

        entity.Status = ExamStatus.Published;

        return await SaveStatusAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ExamDto> CompleteExamAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), id);

        if (entity.Status != ExamStatus.Published)
        {
            throw new BadRequestException($"Only a Published exam can be marked Completed. Current status: {entity.Status}.");
        }

        entity.Status = ExamStatus.Completed;

        return await SaveStatusAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ExamDto> CancelExamAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), id);

        if (entity.Status is not (ExamStatus.Draft or ExamStatus.Published))
        {
            throw new BadRequestException($"A {entity.Status} exam cannot be cancelled.");
        }

        entity.Status = ExamStatus.Cancelled;

        return await SaveStatusAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ExamDto> ReopenExamAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), id);

        if (entity.Status != ExamStatus.Cancelled)
        {
            throw new BadRequestException("Only a Cancelled exam can be reopened.");
        }

        // Reopen goes back to Draft (not straight to Published), so the normal
        // Draft -> Published flow — and its validation — still applies.
        entity.Status = ExamStatus.Draft;

        return await SaveStatusAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<ExamDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;

        var total = await _unitOfWork.ExamRepository.CountByStatusAsync(null, cancellationToken);
        var draft = await _unitOfWork.ExamRepository.CountByStatusAsync(ExamStatus.Draft, cancellationToken);
        var published = await _unitOfWork.ExamRepository.CountByStatusAsync(ExamStatus.Published, cancellationToken);
        var completed = await _unitOfWork.ExamRepository.CountByStatusAsync(ExamStatus.Completed, cancellationToken);
        var cancelled = await _unitOfWork.ExamRepository.CountByStatusAsync(ExamStatus.Cancelled, cancellationToken);

        var upcoming = await GetUpcomingExamsAsync(5, cancellationToken);

        var recentEntities = await _unitOfWork.ExamRepository.GetRecentExamsAsync(5, cancellationToken);
        var recent = recentEntities.Select(BuildSummary).ToList();

        return new ExamDashboardDto
        {
            TotalExams = total,
            DraftExams = draft,
            PublishedExams = published,
            CompletedExams = completed,
            CancelledExams = cancelled,
            UpcomingExamsCount = upcoming.Count,
            UpcomingExams = upcoming,
            RecentExams = recent
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<UpcomingExamDto>> GetUpcomingExamsAsync(int count, CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var entities = await _unitOfWork.ExamRepository.GetUpcomingExamsAsync(today, count, cancellationToken);

        return entities.Select(exam =>
        {
            var nextDate = exam.ExamSchedules
                .Where(s => !s.IsDeleted && s.ExamDate.Date >= today)
                .Min(s => s.ExamDate);

            return new UpcomingExamDto
            {
                ExamId = exam.Id,
                ExamName = exam.Name,
                ExamTypeName = exam.ExamType?.Name ?? string.Empty,
                NextExamDate = nextDate,
                DaysRemaining = (nextDate.Date - today).Days,
                TotalSchedules = exam.ExamSchedules.Count(s => !s.IsDeleted)
            };
        }).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamCalendarDto>> GetExamCalendarAsync(DateTime fromDate, DateTime toDate, int? classId, CancellationToken cancellationToken = default)
    {
        if (toDate.Date < fromDate.Date)
        {
            throw new BadRequestException("'toDate' cannot be earlier than 'fromDate'.");
        }

        var schedules = await _unitOfWork.ExamScheduleRepository.GetExamCalendarAsync(fromDate, toDate, classId, cancellationToken);

        return schedules.Select(s => new ExamCalendarDto
        {
            ScheduleId = s.Id,
            ExamId = s.ExamId,
            ExamName = s.Exam?.Name ?? string.Empty,
            SubjectId = s.SubjectId,
            SubjectName = s.Subject?.Name ?? string.Empty,
            ClassId = s.ClassId,
            ClassName = s.SchoolClass?.Name ?? string.Empty,
            ExamDate = s.ExamDate,
            FullMarks = s.FullMarks,
            PassMarks = s.PassMarks
        }).ToList();
    }

    /// <inheritdoc />
    public async Task<ExamRoutineDto> GetExamRoutineAsync(int examId, CancellationToken cancellationToken = default)
    {
        var exam = await _unitOfWork.ExamRepository.GetByIdWithDetailsAsync(examId, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), examId);

        var schedules = await _unitOfWork.ExamScheduleRepository.GetSchedulesByExamAsync(examId, cancellationToken);

        return new ExamRoutineDto
        {
            ExamId = exam.Id,
            ExamName = exam.Name,
            ExamTypeName = exam.ExamType?.Name ?? string.Empty,
            AcademicYearName = exam.AcademicYear?.Name ?? string.Empty,
            Status = exam.Status,
            Schedules = _mapper.Map<IReadOnlyList<ExamScheduleDto>>(schedules)
        };
    }

    /// <inheritdoc />
    public async Task<ClassRoutineDto> GetClassRoutineAsync(int examId, int classId, CancellationToken cancellationToken = default)
    {
        var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), examId);

        var schoolClass = await _unitOfWork.SchoolClassRepository.GetByIdAsync(classId, cancellationToken)
            ?? throw new NotFoundException(nameof(SchoolClass), classId);

        var schedules = await _unitOfWork.ExamScheduleRepository.GetSchedulesByClassAsync(classId, examId, cancellationToken);

        return new ClassRoutineDto
        {
            ExamId = exam.Id,
            ExamName = exam.Name,
            ClassId = schoolClass.Id,
            ClassName = schoolClass.Name,
            Schedules = _mapper.Map<IReadOnlyList<ExamScheduleDto>>(schedules)
        };
    }

    /// <inheritdoc />
    public async Task<StudentRoutineDto> GetStudentRoutineAsync(int studentId, int examId, CancellationToken cancellationToken = default)
    {
        var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Student), studentId);

        var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), examId);

        var schoolClass = await _unitOfWork.SchoolClassRepository.GetByIdAsync(student.ClassId, cancellationToken);

        var schedules = await _unitOfWork.ExamScheduleRepository.GetSchedulesByClassAsync(student.ClassId, examId, cancellationToken);

        return new StudentRoutineDto
        {
            StudentId = student.Id,
            StudentName = student.FullName,
            ClassId = student.ClassId,
            ClassName = schoolClass?.Name ?? string.Empty,
            ExamId = exam.Id,
            ExamName = exam.Name,
            Schedules = _mapper.Map<IReadOnlyList<ExamScheduleDto>>(schedules)
        };
    }

    /// <inheritdoc />
    public async Task<TeacherRoutineDto> GetTeacherRoutineAsync(int teacherId, int? examId, CancellationToken cancellationToken = default)
    {
        var teacher = await _unitOfWork.TeacherRepository.GetByIdAsync(teacherId, cancellationToken)
            ?? throw new NotFoundException(nameof(Teacher), teacherId);

        var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(teacher.EmployeeId, cancellationToken);

        string? examName = null;

        if (examId.HasValue)
        {
            var exam = await _unitOfWork.ExamRepository.GetByIdAsync(examId.Value, cancellationToken)
                ?? throw new NotFoundException(nameof(Exam), examId.Value);

            examName = exam.Name;
        }

        var schedules = await _unitOfWork.ExamScheduleRepository.GetSchedulesByTeacherAsync(teacherId, examId, cancellationToken);

        return new TeacherRoutineDto
        {
            TeacherId = teacher.Id,
            TeacherName = employee?.FullName ?? string.Empty,
            ExamId = examId,
            ExamName = examName,
            Schedules = _mapper.Map<IReadOnlyList<ExamScheduleDto>>(schedules)
        };
    }

    /// <summary>Builds a lightweight summary DTO from an Exam entity whose ExamSchedules (and their navigations) are already loaded.</summary>
    private static ExamSummaryDto BuildSummary(Exam exam)
    {
        var schedules = exam.ExamSchedules.Where(s => !s.IsDeleted).ToList();

        return new ExamSummaryDto
        {
            Id = exam.Id,
            Name = exam.Name,
            ExamTypeName = exam.ExamType?.Name ?? string.Empty,
            AcademicYearName = exam.AcademicYear?.Name ?? string.Empty,
            Status = exam.Status,
            TotalSchedules = schedules.Count,
            FirstExamDate = schedules.Count > 0 ? schedules.Min(s => s.ExamDate) : null,
            LastExamDate = schedules.Count > 0 ? schedules.Max(s => s.ExamDate) : null
        };
    }

    /// <summary>Ensures the exam is still in Draft status; throws otherwise. Only Draft exams may be edited.</summary>
    private static void EnsureEditable(Exam exam)
    {
        if (exam.Status != ExamStatus.Draft)
        {
            throw new BadRequestException($"A {exam.Status} exam cannot be edited. Only Draft exams can be updated.");
        }
    }

    /// <summary>Ensures no other (non-deleted) exam already exists with the same AcademicYear + ExamType + Name.</summary>
    private async Task EnsureNotDuplicateAsync(string name, int academicYearId, int examTypeId, int? excludeId, CancellationToken cancellationToken)
    {
        var isDuplicate = await _unitOfWork.ExamRepository.DuplicateExamExistsAsync(name, academicYearId, examTypeId, excludeId, cancellationToken);

        if (isDuplicate)
        {
            throw new BadRequestException($"An exam named '{name}' already exists for this academic year and exam type.");
        }
    }

    /// <summary>Persists a lifecycle status change and returns the enriched, updated DTO.</summary>
    private async Task<ExamDto> SaveStatusAsync(Exam entity, CancellationToken cancellationToken)
    {
        _unitOfWork.ExamRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var refreshed = await _unitOfWork.ExamRepository.GetByIdWithDetailsAsync(entity.Id, cancellationToken);
        return _mapper.Map<ExamDto>(refreshed);
    }
}
