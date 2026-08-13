using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Helpers;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.FinalResult.DTOs;
using SchoolERP.Application.Features.FinalResult.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for weighted final result calculation, ranking and
/// publishing. Combines every <see cref="ExamResult"/> (via the underlying
/// subject <see cref="Result"/> rows) across the exams in an academic year's
/// active <see cref="ExamWeightSetup"/> into one <see cref="FinalResult"/>
/// per student. A flat 33% pass threshold is applied to each subject's
/// already-weighted percentage for the final grade (individual exams may use
/// stricter/looser PassMarks; the final result standardizes on one policy).
/// </summary>
public class FinalResultService : IFinalResultService
{
    private const decimal FinalPassPercentage = 33m;

    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly IGradeLookupService _gradeLookupService;
    private readonly IResultAuditService _resultAuditService;

    public FinalResultService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserService currentUserService,
        IGradeLookupService gradeLookupService,
        IResultAuditService resultAuditService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _gradeLookupService = gradeLookupService;
        _resultAuditService = resultAuditService;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<FinalResultDto>> CalculateAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        var activeSetup = await _unitOfWork.ExamWeightSetupRepository.GetActiveByAcademicYearAsync(academicYearId, cancellationToken)
            ?? throw new BadRequestException("No active exam weight setup exists for this academic year. Activate one before calculating final results.");

        var examWeights = activeSetup.Items
            .Where(i => !i.IsDeleted)
            .ToDictionary(i => i.ExamId, i => i.WeightPercentage);

        if (examWeights.Count == 0)
        {
            throw new BadRequestException("The active weight setup has no exam weight items.");
        }

        var gradeBands = await _gradeLookupService.GetBandsAsync(academicYearId, cancellationToken);

        // Collect (StudentId, SubjectId) -> weighted percentage contributions across every weighted exam.
        var contributions = new Dictionary<(int StudentId, int SubjectId), List<(decimal Weight, decimal Percentage)>>();

        foreach (var (examId, weight) in examWeights)
        {
            var marks = await _unitOfWork.ResultRepository.GetByExamAsync(examId, cancellationToken);

            foreach (var mark in marks)
            {
                // Blocked entries are excluded entirely from calculation.
                if (mark.AttendanceStatus == Domain.Enums.MarkAttendanceStatus.Blocked)
                    continue;

                var subjectId = mark.ExamSchedule!.SubjectId;
                var key = (mark.StudentId, subjectId);

                if (!contributions.TryGetValue(key, out var list))
                {
                    list = new List<(decimal, decimal)>();
                    contributions[key] = list;
                }

                list.Add((weight, mark.Percentage ?? 0));
            }
        }

        if (contributions.Count == 0)
        {
            throw new BadRequestException("No mark entries exist for any exam in the active weight setup yet.");
        }

        var existingResults = await _unitOfWork.FinalResultRepository.GetByAcademicYearAsync(academicYearId, cancellationToken);
        var existingByStudent = existingResults.ToDictionary(x => x.StudentId);

        // Cache of ClassId -> optional SubjectIds, populated lazily per class
        // encountered below, so each class's ClassSubject rows are fetched
        // once regardless of how many students share that class (avoids N+1).
        var optionalSubjectsByClass = new Dictionary<int, IReadOnlyList<int>>();

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            foreach (var studentId in contributions.Keys.Select(k => k.StudentId).Distinct())
            {
                var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId, cancellationToken);

                if (student is not null && !optionalSubjectsByClass.TryGetValue(student.ClassId, out _))
                {
                    optionalSubjectsByClass[student.ClassId] = await _unitOfWork.ClassSubjectRepository.GetOptionalSubjectIdsAsync(student.ClassId, cancellationToken);
                }

                var optionalSubjectIds = student is not null && optionalSubjectsByClass.TryGetValue(student.ClassId, out var ids)
                    ? ids
                    : Array.Empty<int>();

                var subjectDetails = contributions
                    .Where(kv => kv.Key.StudentId == studentId)
                    .Select(kv =>
                    {
                        var weightedPercentage = Math.Round(kv.Value.Sum(c => c.Weight / 100m * c.Percentage), 2);
                        var (grade, gradePoint, _) = _gradeLookupService.ResolveByPercentage(gradeBands, weightedPercentage, FinalPassPercentage);

                        return new FinalResultDetail
                        {
                            SubjectId = kv.Key.SubjectId,
                            FinalMarks = weightedPercentage,
                            FinalGradeLabel = grade,
                            FinalGradePoint = gradePoint,
                            IsOptional = optionalSubjectIds.Contains(kv.Key.SubjectId)
                        };
                    })
                    .ToList();

                var mandatoryDetails = subjectDetails.Where(d => !d.IsOptional).ToList();
                var optionalDetails = subjectDetails.Where(d => d.IsOptional).ToList();

                // Optional subjects never count as failed and only contribute a
                // capped bonus grade point (standard Bangladesh "4th subject" rule).
                var finalMarks = Math.Round(subjectDetails.Average(d => d.FinalMarks), 2);
                var finalGpa = ResultGradingRules.CalculateGpaWithOptionalBonus(
                    mandatoryDetails.Select(d => d.FinalGradePoint),
                    optionalDetails.Select(d => d.FinalGradePoint));

                var (finalGrade, _) = _gradeLookupService.ResolveByGradePoint(gradeBands, finalGpa);
                var isPassed = mandatoryDetails.Count == 0
                    ? subjectDetails.All(d => d.FinalGradeLabel != "F")
                    : mandatoryDetails.All(d => d.FinalGradeLabel != "F");

                if (existingByStudent.TryGetValue(studentId, out var existing))
                {
                    var tracked = await _unitOfWork.FinalResultRepository.GetByIdTrackedWithDetailsAsync(existing.Id, cancellationToken);
                    if (tracked is null)
                        continue;

                    if (tracked.IsPublished)
                    {
                        throw new BadRequestException("This academic year's final results are already published and cannot be recalculated. Unpublish first.");
                    }

                    tracked.ExamWeightSetupId = activeSetup.Id;
                    tracked.FinalMarks = finalMarks;
                    tracked.FinalGPA = finalGpa;
                    tracked.FinalGrade = finalGrade;
                    tracked.IsPassed = isPassed;
                    tracked.PromotionStatus = isPassed ? PromotionStatus.Promoted : PromotionStatus.NotPromoted;

                    // Replacing the tracked Details collection lets EF Core's
                    // "delete orphans" behavior (the default for required FKs)
                    // remove the old rows and insert the new ones on SaveChanges.
                    tracked.Details.Clear();
                    foreach (var detail in subjectDetails)
                    {
                        tracked.Details.Add(detail);
                    }

                    _unitOfWork.FinalResultRepository.Update(tracked);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    var newResult = new FinalResult
                    {
                        StudentId = studentId,
                        AcademicYearId = academicYearId,
                        ExamWeightSetupId = activeSetup.Id,
                        FinalMarks = finalMarks,
                        FinalGPA = finalGpa,
                        FinalGrade = finalGrade,
                        IsPassed = isPassed,
                        PromotionStatus = isPassed ? PromotionStatus.Promoted : PromotionStatus.NotPromoted
                    };

                    foreach (var detail in subjectDetails)
                    {
                        newResult.Details.Add(detail);
                    }

                    await _unitOfWork.FinalResultRepository.AddAsync(newResult, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }

            await RecalculateRankingsAsync(academicYearId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        await _resultAuditService.LogAsync(nameof(AcademicYear), academicYearId, ResultAuditAction.Calculated, _currentUserService.UserId, cancellationToken: cancellationToken);

        return await GetByAcademicYearAsync(academicYearId, null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<FinalResultDto>> PublishAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        var results = await _unitOfWork.FinalResultRepository.GetByAcademicYearAsync(academicYearId, cancellationToken);

        if (results.Count == 0)
        {
            throw new BadRequestException("Final results have not been calculated for this academic year yet.");
        }

        if (results.Any(x => x.IsPublished))
        {
            throw new BadRequestException("This academic year's final results are already published.");
        }

        var now = DateTime.UtcNow;
        var publishedBy = _currentUserService.UserId;

        foreach (var result in results)
        {
            var tracked = await _unitOfWork.FinalResultRepository.GetByIdTrackedAsync(result.Id, cancellationToken);
            if (tracked is null)
                continue;

            tracked.IsPublished = true;
            tracked.PublishedAt = now;
            tracked.PublishedBy = publishedBy;

            _unitOfWork.FinalResultRepository.Update(tracked);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _resultAuditService.LogAsync(nameof(AcademicYear), academicYearId, ResultAuditAction.Published, publishedBy, cancellationToken: cancellationToken);

        return await GetByAcademicYearAsync(academicYearId, null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UnpublishAsync(int academicYearId, CancellationToken cancellationToken = default)
    {
        var results = await _unitOfWork.FinalResultRepository.GetByAcademicYearAsync(academicYearId, cancellationToken);

        if (results.Count == 0)
        {
            throw new NotFoundException(nameof(FinalResult), academicYearId);
        }

        foreach (var result in results)
        {
            var tracked = await _unitOfWork.FinalResultRepository.GetByIdTrackedAsync(result.Id, cancellationToken);
            if (tracked is null)
                continue;

            tracked.IsPublished = false;
            tracked.PublishedAt = null;
            tracked.PublishedBy = null;

            _unitOfWork.FinalResultRepository.Update(tracked);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _resultAuditService.LogAsync(nameof(AcademicYear), academicYearId, ResultAuditAction.Unpublished, _currentUserService.UserId, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<FinalResultDto> GetStudentFinalResultAsync(int studentId, int academicYearId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FinalResultRepository.GetByStudentAndYearAsync(studentId, academicYearId, cancellationToken)
            ?? throw new NotFoundException(nameof(FinalResult), $"student {studentId}, academic year {academicYearId}");

        return _mapper.Map<FinalResultDto>(entity);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<FinalResultDto>> GetByAcademicYearAsync(int academicYearId, int? classId, CancellationToken cancellationToken = default)
    {
        var entities = classId.HasValue
            ? await _unitOfWork.FinalResultRepository.GetByAcademicYearAndClassAsync(academicYearId, classId.Value, cancellationToken)
            : await _unitOfWork.FinalResultRepository.GetByAcademicYearAsync(academicYearId, cancellationToken);

        return _mapper.Map<IReadOnlyList<FinalResultDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MeritEntryDto>> GetClassMeritListAsync(int academicYearId, int classId, CancellationToken cancellationToken = default)
    {
        var results = await _unitOfWork.FinalResultRepository.GetByAcademicYearAndClassAsync(academicYearId, classId, cancellationToken);
        return BuildMeritList(results, x => x.ClassPosition);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<MeritEntryDto>> GetSectionMeritListAsync(int academicYearId, int sectionId, CancellationToken cancellationToken = default)
    {
        var results = await _unitOfWork.FinalResultRepository.GetByAcademicYearAndSectionAsync(academicYearId, sectionId, cancellationToken);
        return BuildMeritList(results, x => x.SectionPosition);
    }

    /// <summary>Recomputes Merit (year-wide), Class and Section positions for every FinalResult of an academic year. Only passed students are ranked.</summary>
    private async Task RecalculateRankingsAsync(int academicYearId, CancellationToken cancellationToken)
    {
        var results = await _unitOfWork.FinalResultRepository.GetByAcademicYearAsync(academicYearId, cancellationToken);

        await AssignPositionsAsync(results.Where(x => x.IsPassed), cancellationToken, (r, pos) => r.MeritPosition = pos);

        foreach (var classGroup in results.GroupBy(x => x.Student?.ClassId))
        {
            await AssignPositionsAsync(classGroup.Where(x => x.IsPassed), cancellationToken, (r, pos) => r.ClassPosition = pos);
        }

        foreach (var sectionGroup in results.GroupBy(x => x.Student?.SectionId))
        {
            await AssignPositionsAsync(sectionGroup.Where(x => x.IsPassed), cancellationToken, (r, pos) => r.SectionPosition = pos);
        }

        foreach (var failed in results.Where(x => !x.IsPassed))
        {
            var tracked = await _unitOfWork.FinalResultRepository.GetByIdTrackedAsync(failed.Id, cancellationToken);
            if (tracked is null)
                continue;

            tracked.MeritPosition = null;
            tracked.ClassPosition = null;
            tracked.SectionPosition = null;
            _unitOfWork.FinalResultRepository.Update(tracked);
        }
    }

    /// <summary>Ranks a group of results by FinalGPA desc then FinalMarks desc, assigning sequential 1-based positions via the supplied setter.</summary>
    private async Task AssignPositionsAsync(
        IEnumerable<FinalResult> group,
        CancellationToken cancellationToken,
        Action<FinalResult, int> setPosition)
    {
        var ordered = group
            .OrderByDescending(x => x.FinalGPA)
            .ThenByDescending(x => x.FinalMarks)
            .ToList();

        var position = 1;

        foreach (var result in ordered)
        {
            var tracked = await _unitOfWork.FinalResultRepository.GetByIdTrackedAsync(result.Id, cancellationToken);
            if (tracked is null)
                continue;

            setPosition(tracked, position);
            _unitOfWork.FinalResultRepository.Update(tracked);
            position++;
        }
    }

    /// <inheritdoc />
    public async Task<FinalResultDto> SetRemarksAsync(int studentId, int academicYearId, string? teacherRemarks, string? principalRemarks, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.FinalResultRepository.GetByStudentAndYearAsync(studentId, academicYearId, cancellationToken)
            ?? throw new NotFoundException(nameof(FinalResult), $"student {studentId}, academic year {academicYearId}");

        var tracked = await _unitOfWork.FinalResultRepository.GetByIdTrackedAsync(entity.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(FinalResult), entity.Id);

        if (teacherRemarks is not null)
        {
            tracked.TeacherRemarks = teacherRemarks;
        }

        if (principalRemarks is not null)
        {
            tracked.PrincipalRemarks = principalRemarks;
        }

        _unitOfWork.FinalResultRepository.Update(tracked);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<FinalResultDto>(await _unitOfWork.FinalResultRepository.GetByStudentAndYearAsync(studentId, academicYearId, cancellationToken));
    }

    /// <summary>Maps a list of FinalResult entities into a position-ordered MeritEntryDto list.</summary>
    private static IReadOnlyList<MeritEntryDto> BuildMeritList(IReadOnlyList<FinalResult> results, Func<FinalResult, int?> positionSelector)
    {
        return results
            .OrderBy(x => positionSelector(x) ?? int.MaxValue)
            .Select(x => new MeritEntryDto
            {
                Position = positionSelector(x) ?? 0,
                StudentId = x.StudentId,
                StudentName = x.Student?.FullName ?? string.Empty,
                RollNo = x.Student?.RollNo ?? string.Empty,
                ClassName = x.Student?.SchoolClass?.Name ?? string.Empty,
                SectionName = x.Student?.Section?.Name ?? string.Empty,
                TotalMarks = x.FinalMarks,
                GPA = x.FinalGPA,
                Grade = x.FinalGrade,
                IsPassed = x.IsPassed
            })
            .ToList();
    }
}
