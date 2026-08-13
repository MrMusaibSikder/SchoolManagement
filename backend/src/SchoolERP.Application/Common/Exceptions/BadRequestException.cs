namespace SchoolERP.Application.Common.Exceptions;

/// <summary>
/// Thrown by a service when the caller's request is well-formed but violates a
/// business rule (e.g. wrong current password, expired/invalid token).
/// Translated to an HTTP 400 by the API's exception middleware.
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}
