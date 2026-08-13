using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Transcript.DTOs;
using SchoolERP.Application.Features.Transcript.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Builds student transcripts purely from existing published
/// <see cref="ExamResult"/> and <see cref="FinalResult"/> data plus their
/// underlying <see cref="Result"/> (mark entry) rows, and the existing
/// Attendance module (via the Unit of Work) — introduces no new storage of
/// its own beyond the two remark fields already added to
/// <see cref="FinalResult"/>. Only published results are included, matching
/// the "only published results are visible" rule used elsewhere.
/// </summary>
public class TranscriptService : ITranscriptService
{
    private readonly IUnitOfWork _unitOfWork;

    public TranscriptService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<TranscriptDto> GetStudentTranscriptAsync(int studentId, CancellationToken cancellationToken = default)
    {
        var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Student), studentId);

        var allExamResults = await _unitOfWork.ExamResultRepository.GetAllAsync(cancellationToken);
        var studentExamResults = allExamResults.Where(x => x.StudentId == studentId && x.IsPublished).ToList();

        var allFinalResults = await _unitOfWork.FinalResultRepository.GetAllAsync(cancellationToken);
        var studentFinalResults = allFinalResults.Where(x => x.StudentId == studentId && x.IsPublished).ToList();

        return await BuildTranscriptAsync(student, studentExamResults, studentFinalResults, dateFrom: null, dateTo: null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TranscriptDto> GetAcademicYearTranscriptAsync(int studentId, int academicYearId, CancellationToken cancellationToken = default)
    {
        var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId, cancellationToken)
            ?? throw new NotFoundException(nameof(Student), studentId);

        var academicYear = await _unitOfWork.AcademicYearRepository.GetByIdAsync(academicYearId, cancellationToken)
            ?? throw new NotFoundException(nameof(AcademicYear), academicYearId);

        var allExams = await _unitOfWork.ExamRepository.GetAllAsync(cancellationToken);
        var examIdsForYear = allExams.Where(x => x.AcademicYearId == academicYearId).Select(x => x.Id).ToHashSet();

        var allExamResults = await _unitOfWork.ExamResultRepository.GetAllAsync(cancellationToken);
        var studentExamResults = allExamResults
            .Where(x => x.StudentId == studentId && x.IsPublished && examIdsForYear.Contains(x.ExamId))
            .ToList();

        var allFinalResults = await _unitOfWork.FinalResultRepository.GetAllAsync(cancellationToken);
        var studentFinalResults = allFinalResults
            .Where(x => x.StudentId == studentId && x.AcademicYearId == academicYearId && x.IsPublished)
            .ToList();

        return await BuildTranscriptAsync(student, studentExamResults, studentFinalResults, academicYear.StartDate, academicYear.EndDate, cancellationToken);
    }

    /// <summary>Assembles the full printable TranscriptDto from already-filtered published exam/final results, in a small, fixed number of batched queries.</summary>
    private async Task<TranscriptDto> BuildTranscriptAsync(
        Student student,
        IReadOnlyList<ExamResult> examResults,
        IReadOnlyList<FinalResult> finalResults,
        DateTime? dateFrom,
        DateTime? dateTo,
        CancellationToken cancellationToken)
    {
        var schoolClass = await _unitOfWork.SchoolClassRepository.GetByIdAsync(student.ClassId, cancellationToken);
        var section = student.SectionId.HasValue
     ? await _unitOfWork.SectionRepository.GetByIdAsync(student.SectionId.Value, cancellationToken)
     : null;

        var allExams = await _unitOfWork.ExamRepository.GetAllWithDetailsAsync(cancellationToken);
        var examLookup = allExams.ToDictionary(x => x.Id);

        var allAcademicYears = await _unitOfWork.AcademicYearRepository.GetAllAsync(cancellationToken);
        var yearLookup = allAcademicYears.ToDictionary(x => x.Id);

        var allSubjects = await _unitOfWork.SubjectRepository.GetAllAsync(cancellationToken);
        var subjectLookup = allSubjects.ToDictionary(x => x.Id);

        // Single batched fetch of every mark entry the student has ever had,
        // grouped in-memory by exam — avoids querying per exam (N+1).
        var allMarks = await _unitOfWork.ResultRepository.GetByStudentAsync(student.Id, cancellationToken);
        var marksByExam = allMarks.GroupBy(x => x.ExamSchedule!.ExamId).ToDictionary(g => g.Key, g => g.ToList());

        var optionalSubjectIds = await _unitOfWork.ClassSubjectRepository.GetOptionalSubjectIdsAsync(student.ClassId, cancellationToken);

        var examHistory = examResults
            .OrderBy(x => x.PublishedAt)
            .Select(er =>
            {
                examLookup.TryGetValue(er.ExamId, out var exam);
                var subjects = BuildSubjectRows(marksByExam.TryGetValue(er.ExamId, out var marks) ? marks : new List<Result>(), subjectLookup, optionalSubjectIds);

                return new TranscriptExamEntryDto
                {
                    ExamId = er.ExamId,
                    ExamName = exam?.Name ?? string.Empty,
                    ExamTypeName = exam?.ExamType?.Name ?? string.Empty,
                    AcademicYearId = exam?.AcademicYearId ?? 0,
                    AcademicYearName = exam is not null && yearLookup.TryGetValue(exam.AcademicYearId, out var ey) ? ey.Name : string.Empty,
                    GPA = er.GPA,
                    Grade = er.Grade,
                    IsPassed = er.IsPassed,
                    PublishedAt = er.PublishedAt,
                    Subjects = subjects
                };
            })
            .ToList();

        var yearSummaries = finalResults
            .OrderBy(x => x.AcademicYearId)
            .Select(fr => new TranscriptYearSummaryDto
            {
                AcademicYearId = fr.AcademicYearId,
                AcademicYearName = yearLookup.TryGetValue(fr.AcademicYearId, out var y) ? y.Name : string.Empty,
                FinalGPA = fr.FinalGPA,
                FinalGrade = fr.FinalGrade,
                IsPassed = fr.IsPassed,
                PromotionStatus = fr.PromotionStatus,
                ClassPosition = fr.ClassPosition,
                SectionPosition = fr.SectionPosition,
                MeritPosition = fr.MeritPosition,
                PublishedAt = fr.PublishedAt,
                Subjects = fr.Details.Select(d => new TranscriptSubjectDto
                {
                    SubjectId = d.SubjectId,
                    SubjectName = subjectLookup.TryGetValue(d.SubjectId, out var subj) ? subj.Name : string.Empty,
                    MarksObtained = d.FinalMarks,
                    Grade = d.FinalGradeLabel,
                    GPA = d.FinalGradePoint,
                    IsPassed = d.FinalGradeLabel != "F",
                    IsOptional = d.IsOptional
                }).ToList(),
                TeacherRemarks = fr.TeacherRemarks,
                PrincipalRemarks = fr.PrincipalRemarks
            })
            .ToList();

        // Graph-ready trends: prefer year-level GPA/position when available, fall back to per-exam entries otherwise.
        var gpaHistory = yearSummaries.Count > 0
            ? yearSummaries.Select(y => new GpaHistoryPointDto { Label = y.AcademicYearName, GPA = y.FinalGPA }).ToList()
            : examHistory.Select(e => new GpaHistoryPointDto { Label = e.ExamName, GPA = e.GPA }).ToList();

        var positionHistory = yearSummaries.Select(y => new PositionHistoryPointDto
        {
            AcademicYearName = y.AcademicYearName,
            ClassPosition = y.ClassPosition,
            SectionPosition = y.SectionPosition,
            MeritPosition = y.MeritPosition
        }).ToList();

        var cgpa = yearSummaries.Count == 0 ? 0m : Math.Round(yearSummaries.Average(y => y.FinalGPA), 2);
        var overallPassed = yearSummaries.Count > 0 && yearSummaries.All(y => y.IsPassed);

        var summary = new TranscriptSummaryDto
        {
            TotalExamsIncluded = examHistory.Count,
            TotalAcademicYearsIncluded = yearSummaries.Count,
            CGPA = cgpa,
            HighestYearGPA = yearSummaries.Count > 0 ? yearSummaries.Max(y => y.FinalGPA) : 0m,
            LowestYearGPA = yearSummaries.Count > 0 ? yearSummaries.Min(y => y.FinalGPA) : 0m,
            OverallPassed = overallPassed,
            LatestPromotionStatus = yearSummaries.Count > 0 ? yearSummaries[^1].PromotionStatus : null
        };

        var attendanceSummary = await BuildAttendanceSummaryAsync(student.Id, dateFrom, dateTo, cancellationToken);

        return new TranscriptDto
        {
            StudentId = student.Id,
            StudentName = student.FullName,
            RollNo = student.RollNo,
            ClassName = schoolClass?.Name ?? string.Empty,
            SectionName = section?.Name ?? string.Empty,
            Summary = summary,
            ExamHistory = examHistory,
            YearSummaries = yearSummaries,
            GpaHistory = gpaHistory,
            PositionHistory = positionHistory,
            AttendanceSummary = attendanceSummary,
            GeneratedAt = DateTime.UtcNow
        };
    }

    /// <summary>Projects a single exam's raw mark entries into subject-wise transcript rows.</summary>
    private static IReadOnlyList<TranscriptSubjectDto> BuildSubjectRows(
        IReadOnlyList<Result> marks,
        IReadOnlyDictionary<int, Subject> subjectLookup,
        IReadOnlyList<int> optionalSubjectIds)
    {
        return marks.Select(m =>
        {
            var subjectId = m.ExamSchedule!.SubjectId;

            return new TranscriptSubjectDto
            {
                SubjectId = subjectId,
                SubjectName = subjectLookup.TryGetValue(subjectId, out var subject) ? subject.Name : string.Empty,
                MarksObtained = m.MarksObtained + m.GraceMarks,
                Grade = m.Grade ?? string.Empty,
                GPA = m.GPA ?? 0,
                IsPassed = m.IsPassed,
                IsOptional = optionalSubjectIds.Contains(subjectId)
            };
        }).ToList();
    }

    /// <summary>Builds the attendance summary, reusing the existing Attendance module read-only. Uses the academic year's date range when scoped, or the student's full history otherwise.</summary>
    private async Task<TranscriptAttendanceSummaryDto> BuildAttendanceSummaryAsync(int studentId, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken)
    {
        var from = fromDate ?? DateTime.MinValue;
        var to = toDate ?? DateTime.UtcNow;

        var records = await _unitOfWork.StudentAttendanceRepository.GetStudentHistoryAsync(studentId, from, to, cancellationToken);

        if (records.Count == 0)
        {
            return new TranscriptAttendanceSummaryDto();
        }

        var present = records.Count(r => r.Status == AttendanceStatus.Present);
        var absent = records.Count(r => r.Status == AttendanceStatus.Absent);
        var late = records.Count(r => r.Status == AttendanceStatus.Late);
        var leave = records.Count(r => r.Status == AttendanceStatus.Leave);

        return new TranscriptAttendanceSummaryDto
        {
            TotalDays = records.Count,
            PresentDays = present,
            AbsentDays = absent,
            LateDays = late,
            LeaveDays = leave,
            AttendancePercentage = records.Count == 0 ? 0 : Math.Round(present * 100m / records.Count, 2)
        };
    }
}
