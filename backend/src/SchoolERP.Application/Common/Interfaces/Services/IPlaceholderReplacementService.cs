using SchoolERP.Application.Common.Models;

namespace SchoolERP.Application.Common.Interfaces.Services;

/// <summary>
/// Replaces <c>{{Placeholder}}</c> tokens inside a message template with actual
/// values. Used by the SMS Template feature (and reusable by Notice/Email
/// features later) instead of hardcoding string replacements in each service.
/// </summary>
public interface IPlaceholderReplacementService
{
    /// <summary>
    /// Replaces every known placeholder found in <paramref name="template"/> with
    /// the corresponding value from <paramref name="data"/>. Placeholders with no
    /// supplied value are replaced with an empty string. Unknown/unsupported
    /// tokens (not part of the placeholder vocabulary) are left untouched.
    /// </summary>
    string Replace(string template, PlaceholderDataDto data);

    /// <summary>Gets the full list of placeholder tokens supported by the system (e.g. "{{StudentName}}").</summary>
    IReadOnlyList<string> GetSupportedPlaceholders();
}
