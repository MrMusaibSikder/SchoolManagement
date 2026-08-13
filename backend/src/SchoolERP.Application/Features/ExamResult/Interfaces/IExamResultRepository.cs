using SchoolERP.Application.Common.Interfaces;

namespace SchoolERP.Application.Features.ExamResult.Interfaces;

/// <summary>
/// Repository contract for <see cref="SchoolERP.Domain.Entities.ExamResult"/>
/// entities. Extends the generic repository with ExamResult-specific data
/// access members. Contains database operations only; calculation, ranking
/// and publishing rules live in <c>IExamResultService</c>.
/// </summary>
public interface IExamResultRepository : IGenericRepository<SchoolERP.Domain.Entities.ExamResult>
{
    /// <summary>Gets the aggregate result for one student in one exam, with Student eagerly loaded, or null.</summary>
    Task<SchoolERP.Domain.Entities.ExamResult?> GetByStudentAndExamAsync(
        int studentId,
        int examId,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every aggregate result for an exam, with Student (+Class/Section) eagerly loaded.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.ExamResult>> GetByExamAsync(
        int examId,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every aggregate result for an exam restricted to one class, with Student eagerly loaded.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.ExamResult>> GetByExamAndClassAsync(
        int examId,
        int classId,
        CancellationToken cancellationToken = default);

    /// <summary>Gets every aggregate result for an exam restricted to one section, with Student eagerly loaded.</summary>
    Task<IReadOnlyList<SchoolERP.Domain.Entities.ExamResult>> GetByExamAndSectionAsync(
        int examId,
        int sectionId,
        CancellationToken cancellationToken = default);
}
