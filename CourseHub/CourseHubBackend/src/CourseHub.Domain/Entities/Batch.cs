using CourseHub.Domain.Common;
using CourseHub.Domain.Exceptions;

namespace CourseHub.Domain.Entities;

/// <summary>
/// Batch represents a specific running cohort/instance of a Course.
/// Teacher assignment is intentionally NOT modeled here — a batch may
/// eventually have multiple instructors, handled by a separate
/// relationship (e.g. BatchTeacher) in a later phase.
/// </summary>
public class Batch : BaseEntity
{
    public Guid CourseId { get; private set; }

    public string Name { get; private set; } = null!;

    public string Code { get; private set; } = null!;

    public DateTime StartDate { get; private set; }

    public DateTime? EndDate { get; private set; }

    /// <summary>
    /// Maximum number of students. Null means unlimited enrollment.
    /// </summary>
    public int? Capacity { get; private set; }

    public bool IsActive { get; private set; }

    private Batch()
    {
    }

    private Batch(Guid courseId, string name, string code, DateTime startDate, int? capacity)
    {
        CourseId = courseId;
        Name = name;
        Code = code;
        StartDate = startDate;
        Capacity = capacity;
        IsActive = true;
    }

    public static Batch Create(Guid courseId, string name, string code, DateTime startDate, int? capacity = null)
    {
        if (courseId == Guid.Empty)
        {
            throw new ValidationException("CourseId is required.");
        }

        var validatedName = ValidateRequired(name, "Name");
        var validatedCode = ValidateRequired(code, "Code");
        var validatedCapacity = ValidateCapacity(capacity);

        return new Batch(courseId, validatedName, validatedCode, startDate.ToUniversalTime(), validatedCapacity);
    }

    public void Update(string name, string code)
    {
        Name = ValidateRequired(name, "Name");
        Code = ValidateRequired(code, "Code");
        MarkAsUpdated();
    }

    public void SetSchedule(DateTime startDate, DateTime? endDate)
    {
        if (endDate.HasValue && endDate.Value < startDate)
        {
            throw new ValidationException("EndDate cannot be earlier than StartDate.");
        }

        StartDate = startDate.ToUniversalTime();
        EndDate = endDate?.ToUniversalTime();
        MarkAsUpdated();
    }

    public void SetCapacity(int? capacity)
    {
        Capacity = ValidateCapacity(capacity);
        MarkAsUpdated();
    }

    public void Activate()
    {
        IsActive = true;
        MarkAsUpdated();
    }

    public void Deactivate()
    {
        IsActive = false;
        MarkAsUpdated();
    }

    private static string ValidateRequired(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ValidationException($"{fieldName} is required.");
        }

        return value.Trim();
    }

    private static int? ValidateCapacity(int? capacity)
    {
        if (capacity.HasValue && capacity.Value <= 0)
        {
            throw new ValidationException("Capacity must be greater than zero when provided.");
        }

        return capacity;
    }
}
