using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.ExamSchedule.DTOs;
using SchoolERP.Application.Features.ExamSchedule.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for ExamSchedule records. Calls the repository (via the
/// Unit of Work), applies business rules (FK existence, parent-exam status
/// gating, duplicate subject/date checks), and maps entities to/from DTOs
/// using AutoMapper.
/// </summary>
public class ExamScheduleService : IExamScheduleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExamScheduleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamScheduleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.ExamScheduleRepository.GetAllWithDetailsAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ExamScheduleDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<ExamScheduleDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamScheduleRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<ExamScheduleDto>(entity);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamScheduleDto>> GetSchedulesByExamAsync(int examId, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.ExamRepository.ExamExistsAsync(examId, cancellationToken))
        {
            throw new NotFoundException(nameof(Exam), examId);
        }

        var entities = await _unitOfWork.ExamScheduleRepository.GetSchedulesByExamAsync(examId, cancellationToken);
        return _mapper.Map<IReadOnlyList<ExamScheduleDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamScheduleDto>> GetSchedulesByClassAsync(int classId, int? examId, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.SchoolClassRepository.ExistsAsync(classId, cancellationToken))
        {
            throw new NotFoundException(nameof(SchoolClass), classId);
        }

        var entities = await _unitOfWork.ExamScheduleRepository.GetSchedulesByClassAsync(classId, examId, cancellationToken);
        return _mapper.Map<IReadOnlyList<ExamScheduleDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ExamScheduleDto>> GetSchedulesByTeacherAsync(int teacherId, int? examId, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.TeacherRepository.ExistsAsync(teacherId, cancellationToken))
        {
            throw new NotFoundException(nameof(Teacher), teacherId);
        }

        var entities = await _unitOfWork.ExamScheduleRepository.GetSchedulesByTeacherAsync(teacherId, examId, cancellationToken);
        return _mapper.Map<IReadOnlyList<ExamScheduleDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<ExamScheduleDto> CreateAsync(CreateExamScheduleDto request, CancellationToken cancellationToken = default)
    {
        var exam = await _unitOfWork.ExamRepository.GetByIdAsync(request.ExamId, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), request.ExamId);

        EnsureExamAcceptsScheduleChanges(exam.Status);

        var schoolClass = await _unitOfWork.SchoolClassRepository.GetByIdAsync(request.ClassId, cancellationToken)
            ?? throw new NotFoundException(nameof(SchoolClass), request.ClassId);

        var subject = await _unitOfWork.SubjectRepository.GetByIdAsync(request.SubjectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Subject), request.SubjectId);

        ValidateMarks(request.FullMarks, request.PassMarks);

        await EnsureNoDuplicateSubjectAsync(request.ExamId, request.ClassId, request.SubjectId, excludeId: null, cancellationToken);
        await EnsureDateNotAlreadyScheduledAsync(request.ExamId, request.ClassId, request.ExamDate, excludeId: null, cancellationToken);

        var entity = _mapper.Map<ExamSchedule>(request);

        await _unitOfWork.ExamScheduleRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Avoid a second round-trip: attach the already-fetched navigation
        // entities so the returned DTO is correctly enriched with names.
        entity.Exam = exam;
        entity.SchoolClass = schoolClass;
        entity.Subject = subject;

        return _mapper.Map<ExamScheduleDto>(entity);
    }

    /// <inheritdoc />
    public async Task<ExamScheduleDto> UpdateAsync(int id, UpdateExamScheduleDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamScheduleRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamSchedule), id);

        var exam = await _unitOfWork.ExamRepository.GetByIdAsync(request.ExamId, cancellationToken)
            ?? throw new NotFoundException(nameof(Exam), request.ExamId);

        // The schedule's *current* parent exam must also still accept changes,
        // in case the request is attempting to move it to a different exam.
        var currentExamStatus = await _unitOfWork.ExamRepository.GetExamStatusAsync(entity.ExamId, cancellationToken);
        if (currentExamStatus.HasValue)
        {
            EnsureExamAcceptsScheduleChanges(currentExamStatus.Value);
        }

        EnsureExamAcceptsScheduleChanges(exam.Status);

        var schoolClass = await _unitOfWork.SchoolClassRepository.GetByIdAsync(request.ClassId, cancellationToken)
            ?? throw new NotFoundException(nameof(SchoolClass), request.ClassId);

        var subject = await _unitOfWork.SubjectRepository.GetByIdAsync(request.SubjectId, cancellationToken)
            ?? throw new NotFoundException(nameof(Subject), request.SubjectId);

        ValidateMarks(request.FullMarks, request.PassMarks);

        await EnsureNoDuplicateSubjectAsync(request.ExamId, request.ClassId, request.SubjectId, excludeId: id, cancellationToken);
        await EnsureDateNotAlreadyScheduledAsync(request.ExamId, request.ClassId, request.ExamDate, excludeId: id, cancellationToken);

        _mapper.Map(request, entity);

        _unitOfWork.ExamScheduleRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        entity.Exam = exam;
        entity.SchoolClass = schoolClass;
        entity.Subject = subject;

        return _mapper.Map<ExamScheduleDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ExamScheduleRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamSchedule), id);

        var examStatus = await _unitOfWork.ExamRepository.GetExamStatusAsync(entity.ExamId, cancellationToken);

        if (examStatus.HasValue)
        {
            EnsureExamAcceptsScheduleChanges(examStatus.Value);
        }

        _unitOfWork.ExamScheduleRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Ensures the parent exam is still in a state that allows its schedules to
    /// be created, changed or removed (Draft or Published only). Completed and
    /// Cancelled exams are read-only, including their schedules.
    /// </summary>
    private static void EnsureExamAcceptsScheduleChanges(ExamStatus status)
    {
        if (status is ExamStatus.Completed or ExamStatus.Cancelled)
        {
            throw new BadRequestException($"Schedules cannot be changed for a {status} exam.");
        }
    }

    /// <summary>Ensures FullMarks/PassMarks satisfy FullMarks &gt; 0, PassMarks &gt; 0, and PassMarks &lt; FullMarks.</summary>
    private static void ValidateMarks(int fullMarks, int passMarks)
    {
        if (fullMarks <= 0)
        {
            throw new BadRequestException("Full marks must be greater than 0.");
        }

        if (passMarks <= 0)
        {
            throw new BadRequestException("Pass marks must be greater than 0.");
        }

        if (passMarks >= fullMarks)
        {
            throw new BadRequestException("Pass marks must be less than full marks.");
        }
    }

    /// <summary>Ensures the subject is not already scheduled for the same exam + class.</summary>
    private async Task EnsureNoDuplicateSubjectAsync(int examId, int classId, int subjectId, int? excludeId, CancellationToken cancellationToken)
    {
        var isDuplicate = await _unitOfWork.ExamScheduleRepository.DuplicateScheduleExistsAsync(examId, classId, subjectId, excludeId, cancellationToken);

        if (isDuplicate)
        {
            throw new BadRequestException("This subject is already scheduled for the selected exam and class.");
        }
    }

    /// <summary>Ensures the exam + class does not already have a different subject scheduled on the same date.</summary>
    private async Task EnsureDateNotAlreadyScheduledAsync(int examId, int classId, DateTime examDate, int? excludeId, CancellationToken cancellationToken)
    {
        var isTaken = await _unitOfWork.ExamScheduleRepository.DateAlreadyScheduledAsync(examId, classId, examDate, excludeId, cancellationToken);

        if (isTaken)
        {
            throw new BadRequestException("Another subject is already scheduled for this exam and class on the same date.");
        }
    }
}
