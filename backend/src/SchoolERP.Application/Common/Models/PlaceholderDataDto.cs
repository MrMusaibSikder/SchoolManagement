namespace SchoolERP.Application.Common.Models;

/// <summary>
/// Holds the actual values that can be substituted into a message template's
/// <c>{{Placeholder}}</c> tokens by <see cref="Interfaces.Services.IPlaceholderReplacementService"/>.
/// Shared across features (SmsTemplate, Notice, and any future messaging
/// feature) so the placeholder vocabulary stays consistent project-wide.
/// All members are optional: only the placeholders present in a given template
/// need to be supplied by the caller.
/// </summary>
public class PlaceholderDataDto
{
    /// <summary>Value for <c>{{StudentName}}</c>.</summary>
    public string? StudentName { get; set; }

    /// <summary>Value for <c>{{GuardianName}}</c>.</summary>
    public string? GuardianName { get; set; }

    /// <summary>Value for <c>{{TeacherName}}</c>.</summary>
    public string? TeacherName { get; set; }

    /// <summary>Value for <c>{{EmployeeName}}</c>.</summary>
    public string? EmployeeName { get; set; }

    /// <summary>Value for <c>{{Class}}</c>.</summary>
    public string? Class { get; set; }

    /// <summary>Value for <c>{{Section}}</c>.</summary>
    public string? Section { get; set; }

    /// <summary>Value for <c>{{Roll}}</c>.</summary>
    public string? Roll { get; set; }

    /// <summary>Value for <c>{{AttendanceStatus}}</c>.</summary>
    public string? AttendanceStatus { get; set; }

    /// <summary>Value for <c>{{Date}}</c>. Defaults to today (short date) when not supplied.</summary>
    public string? Date { get; set; }

    /// <summary>Value for <c>{{Time}}</c>. Defaults to now (short time) when not supplied.</summary>
    public string? Time { get; set; }

    /// <summary>Value for <c>{{SchoolName}}</c>.</summary>
    public string? SchoolName { get; set; }
}
