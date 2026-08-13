namespace SchoolERP.Application.Common.Exceptions;

/// <summary>
/// Thrown by a service when a requested entity cannot be located.
/// Typically translated to an HTTP 404 by the API's exception middleware.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object key)
        : base($"Entity \"{entityName}\" with key ({key}) was not found.")
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }
}
