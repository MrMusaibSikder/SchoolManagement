using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Helpers;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Application.Features.Result.DTOs;
using SchoolERP.Application.Features.Result.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for Result (mark entry) records. Calls the repository (via
/// the Unit of Work), applies every mark-entry business rule (teacher
/// assignment via SubjectTeacher, exam-status gating, duplicate prevention,
/// mark-range validation, draft/submit workflow, lock/unlock), computes the
/// subject Grade/GPA/Pass-Fail via <see cref="IGradeLookupService"/> (which
/// consults the academic year's configurable grade bands, falling back to
/// <see cref="GradeCalculator"/> when none are configured), and maps
/// entities to/from DTOs using AutoMapper.
/// </summary>
public class ResultService : IResultService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IGradeLookupService _gradeLookupService;
    private readonly IResultAuditService _resultAuditService;

    public ResultService(IUnitOfWork unitOfWork, IMapper mapper, IGradeLookupService gradeLookupService, IResultAuditService resultAuditService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _gradeLookupService = gradeLookupService;
        _resultAuditService = resultAuditService;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ResultDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.ResultRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<ResultDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<ResultDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ResultRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<ResultDto>(entity);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ResultDto>> GetByExamScheduleAsync(int examScheduleId, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.ExamScheduleRepository.ExistsAsync(examScheduleId, cancellationToken))
        {
            throw new NotFoundException(nameof(ExamSchedule), examScheduleId);
        }

        var entities = await _unitOfWork.ResultRepository.GetByExamScheduleAsync(examScheduleId, cancellationToken);
        return _mapper.Map<IReadOnlyList<ResultDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ResultDto>> GetByStudentAndExamAsync(int studentId, int examId, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.StudentRepository.ExistsAsync(studentId, cancellationToken))
        {
            throw new NotFoundException(nameof(Student), studentId);
        }

        var entities = await _unitOfWork.ResultRepository.GetByStudentAndExamAsync(studentId, examId, cancellationToken);
        return _mapper.Map<IReadOnlyList<ResultDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<ResultDto> CreateAsync(CreateResultDto request, CancellationToken cancellationToken = default)
    {
        var schedule = await GetScheduleAndValidateEntryAllowedAsync(request.ExamScheduleId, cancellationToken);

        if (!await _unitOfWork.StudentRepository.ExistsAsync(request.StudentId, cancellationToken))
        {
            throw new NotFoundException(nameof(Student), request.StudentId);
        }

        await EnsureTeacherIsAssignedAsync(request.TeacherId, schedule.SubjectId, cancellationToken);

        var existing = await _unitOfWork.ResultRepository.GetByStudentAndScheduleAsync(request.StudentId, request.ExamScheduleId, cancellationToken);
        if (existing is not null)
        {
            throw new BadRequestException("Marks have already been entered for this student and subject. Use update instead.");
        }

        var entity = _mapper.Map<Result>(request);
        await ApplyGradingAsync(entity, schedule);

        await _unitOfWork.ResultRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(entity.Id, cancellationToken) ?? _mapper.Map<ResultDto>(entity);
    }

    /// <inheritdoc />
    public async Task<ResultDto> UpdateAsync(int id, UpdateResultDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ResultRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Result), id);

        EnsureNotLocked(entity);

        var schedule = await GetScheduleAndValidateEntryAllowedAsync(entity.ExamScheduleId, cancellationToken);

        await EnsureTeacherIsAssignedAsync(request.TeacherId, schedule.SubjectId, cancellationToken);

        _mapper.Map(request, entity);
        await ApplyGradingAsync(entity, schedule);

        _unitOfWork.ResultRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _resultAuditService.LogAsync(nameof(Result), entity.Id, ResultAuditAction.MarkUpdated, request.TeacherId, cancellationToken: cancellationToken);

        return await GetByIdAsync(entity.Id, cancellationToken) ?? _mapper.Map<ResultDto>(entity);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ResultDto>> BulkEntryAsync(BulkMarkEntryDto request, CancellationToken cancellationToken = default)
    {
        var schedule = await GetScheduleAndValidateEntryAllowedAsync(request.ExamScheduleId, cancellationToken);

        await EnsureTeacherIsAssignedAsync(request.TeacherId, schedule.SubjectId, cancellationToken);

        var studentIds = request.Entries.Select(e => e.StudentId).ToList();
        var allStudents = await _unitOfWork.StudentRepository.GetAllAsync(cancellationToken);
        var validStudentIds = allStudents.Select(s => s.Id).ToHashSet();

        var existingEntries = await _unitOfWork.ResultRepository.GetByExamScheduleAsync(request.ExamScheduleId, cancellationToken);
        var existingByStudent = existingEntries.ToDictionary(x => x.StudentId);

        var gradeBands = await _gradeLookupService.GetBandsAsync(schedule.Exam!.AcademicYearId, cancellationToken);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var item in request.Entries)
            {
                if (!validStudentIds.Contains(item.StudentId))
                {
                    throw new NotFoundException(nameof(Student), item.StudentId);
                }

                if (existingByStudent.TryGetValue(item.StudentId, out var existing))
                {
                    EnsureNotLocked(existing);

                    existing.MarksObtained = item.MarksObtained;
                    existing.GraceMarks = item.GraceMarks;
                    existing.AttendanceStatus = item.AttendanceStatus;
                    existing.Remarks = item.Remarks;
                    existing.EnteredByTeacherId = request.TeacherId;
                    ApplyGrading(existing, schedule, gradeBands);

                    _unitOfWork.ResultRepository.Update(existing);
                }
                else
                {
                    var entity = new Result
                    {
                        StudentId = item.StudentId,
                        ExamScheduleId = request.ExamScheduleId,
                        MarksObtained = item.MarksObtained,
                        GraceMarks = item.GraceMarks,
                        AttendanceStatus = item.AttendanceStatus,
                        Remarks = item.Remarks,
                        EnteredByTeacherId = request.TeacherId
                    };
                    ApplyGrading(entity, schedule, gradeBands);

                    await _unitOfWork.ResultRepository.AddAsync(entity, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        await _resultAuditService.LogAsync(
            nameof(ExamSchedule), request.ExamScheduleId, ResultAuditAction.MarkUpdated, request.TeacherId,
            $"Bulk entry: {request.Entries.Count} student(s).", cancellationToken);

        return await GetByExamScheduleAsync(request.ExamScheduleId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ResultDto>> SubmitAsync(int examScheduleId, int teacherId, CancellationToken cancellationToken = default)
    {
        var schedule = await GetScheduleAndValidateEntryAllowedAsync(examScheduleId, cancellationToken);

        await EnsureTeacherIsAssignedAsync(teacherId, schedule.SubjectId, cancellationToken);

        var entries = await _unitOfWork.ResultRepository.GetByExamScheduleAsync(examScheduleId, cancellationToken);

        if (entries.Count == 0)
        {
            throw new BadRequestException("There are no mark entries to submit for this exam schedule.");
        }

        foreach (var entry in entries)
        {
            if (entry.IsLocked)
                continue;

            var tracked = await _unitOfWork.ResultRepository.GetByIdTrackedAsync(entry.Id, cancellationToken);
            if (tracked is null)
                continue;

            tracked.EntryStatus = MarkEntryStatus.Submitted;
            _unitOfWork.ResultRepository.Update(tracked);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByExamScheduleAsync(examScheduleId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task LockByExamScheduleAsync(int examScheduleId, CancellationToken cancellationToken = default)
    {
        var entries = await _unitOfWork.ResultRepository.GetByExamScheduleAsync(examScheduleId, cancellationToken);

        foreach (var entry in entries)
        {
            var tracked = await _unitOfWork.ResultRepository.GetByIdTrackedAsync(entry.Id, cancellationToken);
            if (tracked is null || tracked.IsLocked)
                continue;

            tracked.IsLocked = true;
            tracked.LockedAt = DateTime.UtcNow;
            _unitOfWork.ResultRepository.Update(tracked);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UnlockByExamScheduleAsync(int examScheduleId, CancellationToken cancellationToken = default)
    {
        var entries = await _unitOfWork.ResultRepository.GetByExamScheduleAsync(examScheduleId, cancellationToken);

        foreach (var entry in entries)
        {
            var tracked = await _unitOfWork.ResultRepository.GetByIdTrackedAsync(entry.Id, cancellationToken);
            if (tracked is null || !tracked.IsLocked)
                continue;

            tracked.IsLocked = false;
            tracked.LockedAt = null;
            _unitOfWork.ResultRepository.Update(tracked);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.ResultRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Result), id);

        EnsureNotLocked(entity);

        _unitOfWork.ResultRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Loads the exam schedule and verifies its parent exam is Published (the
    /// only status during which marks entry is permitted: Draft has no active
    /// exam window yet, Completed/Cancelled are read-only).
    /// </summary>
    private async Task<ExamSchedule> GetScheduleAndValidateEntryAllowedAsync(int examScheduleId, CancellationToken cancellationToken)
    {
        var schedule = await _unitOfWork.ExamScheduleRepository.GetByIdWithDetailsAsync(examScheduleId, cancellationToken)
            ?? throw new NotFoundException(nameof(ExamSchedule), examScheduleId);

        var examStatus = schedule.Exam?.Status;

        if (examStatus != ExamStatus.Published)
        {
            throw new BadRequestException($"Marks can only be entered while the exam is Published. Current status: {examStatus}.");
        }

        return schedule;
    }

    /// <summary>Ensures the teacher is assigned to the subject via the existing SubjectTeacher mapping. Rejects entry otherwise.</summary>
    private async Task EnsureTeacherIsAssignedAsync(int teacherId, int subjectId, CancellationToken cancellationToken)
    {
        if (!await _unitOfWork.TeacherRepository.ExistsAsync(teacherId, cancellationToken))
        {
            throw new NotFoundException(nameof(Teacher), teacherId);
        }

        var isAssigned = await _unitOfWork.SubjectTeacherRepository.ExistsAsync(subjectId, teacherId, cancellationToken);

        if (!isAssigned)
        {
            throw new BadRequestException("This teacher is not assigned to the subject and cannot enter marks for it.");
        }
    }

    /// <summary>Throws if the mark entry is locked (e.g. after the exam result was published).</summary>
    private static void EnsureNotLocked(Result entity)
    {
        if (entity.IsLocked)
        {
            throw new BadRequestException("This mark entry is locked and cannot be changed. An administrator must unlock it first.");
        }
    }

    /// <summary>
    /// Computes Percentage/Grade/GPA/IsPassed for a single mark entry,
    /// fetching the academic year's grade bands for this one call. Used by
    /// the single Create/Update paths; <see cref="BulkEntryAsync"/> instead
    /// pre-fetches bands once and calls <see cref="ApplyGrading"/> directly
    /// to avoid N+1 queries across many students.
    /// </summary>
    private async Task ApplyGradingAsync(Result entity, ExamSchedule schedule)
    {
        var bands = await _gradeLookupService.GetBandsAsync(schedule.Exam!.AcademicYearId);
        ApplyGrading(entity, schedule, bands);
    }

    /// <summary>
    /// Computes Percentage/Grade/GPA/IsPassed for a mark entry from its
    /// (MarksObtained + GraceMarks) against the schedule's FullMarks/PassMarks,
    /// via <see cref="_gradeLookupService"/>'s synchronous, pre-fetched-bands
    /// resolver. Non-Present attendance always yields Grade "F" / GPA 0 / not passed.
    /// </summary>
    private void ApplyGrading(Result entity, ExamSchedule schedule, IReadOnlyList<Domain.Entities.GradeSetup> gradeBands)
    {
        if (entity.AttendanceStatus != MarkAttendanceStatus.Present)
        {
            entity.MarksObtained = 0;
            entity.GraceMarks = 0;
            entity.Percentage = 0;
            entity.Grade = "F";
            entity.GPA = 0;
            entity.IsPassed = false;
            return;
        }

        var totalMarks = entity.MarksObtained + entity.GraceMarks;

        if (totalMarks > schedule.FullMarks)
        {
            throw new BadRequestException($"Total marks (obtained + grace = {totalMarks}) cannot exceed full marks ({schedule.FullMarks}).");
        }

        var percentage = schedule.FullMarks == 0 ? 0 : Math.Round(totalMarks / schedule.FullMarks * 100m, 2);
        var passPercentage = schedule.FullMarks == 0 ? 0 : Math.Round((decimal)schedule.PassMarks / schedule.FullMarks * 100m, 2);

        var (grade, gradePoint, _) = _gradeLookupService.ResolveByPercentage(gradeBands, percentage, passPercentage);

        entity.Percentage = percentage;
        entity.Grade = grade;
        entity.GPA = gradePoint;
        entity.IsPassed = totalMarks >= schedule.PassMarks;
    }
}
