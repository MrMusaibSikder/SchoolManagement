using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Default <see cref="IPlaceholderReplacementService"/> implementation. Performs
/// simple, fast token substitution using <see cref="string.Replace(string, string)"/>
/// rather than a regex engine, since the placeholder vocabulary is a small,
/// fixed set known up front.
/// </summary>
public class PlaceholderReplacementService : IPlaceholderReplacementService
{
    /// <inheritdoc />
    public string Replace(string template, PlaceholderDataDto data)
    {
        if (string.IsNullOrEmpty(template))
            return template;

        var map = BuildReplacementMap(data);

        var result = template;

        foreach (var (token, value) in map)
        {
            result = result.Replace(token, value, StringComparison.OrdinalIgnoreCase);
        }

        return result;
    }

    /// <inheritdoc />
    public IReadOnlyList<string> GetSupportedPlaceholders()
    {
        return BuildReplacementMap(new PlaceholderDataDto())
            .Select(x => x.Token)
            .ToList();
    }

    /// <summary>
    /// Builds the token -&gt; value map for a given data set. Centralizing the
    /// token vocabulary here means adding a new placeholder only ever requires a
    /// change in one place.
    /// </summary>
    private static IReadOnlyList<(string Token, string Value)> BuildReplacementMap(PlaceholderDataDto data)
    {
        return new List<(string Token, string Value)>
        {
            ("{{StudentName}}", data.StudentName ?? string.Empty),
            ("{{GuardianName}}", data.GuardianName ?? string.Empty),
            ("{{TeacherName}}", data.TeacherName ?? string.Empty),
            ("{{EmployeeName}}", data.EmployeeName ?? string.Empty),
            ("{{Class}}", data.Class ?? string.Empty),
            ("{{Section}}", data.Section ?? string.Empty),
            ("{{Roll}}", data.Roll ?? string.Empty),
            ("{{AttendanceStatus}}", data.AttendanceStatus ?? string.Empty),
            ("{{Date}}", data.Date ?? DateTime.Today.ToString("dd-MM-yyyy")),
            ("{{Time}}", data.Time ?? DateTime.Now.ToString("hh:mm tt")),
            ("{{SchoolName}}", data.SchoolName ?? string.Empty)
        };
    }
}
