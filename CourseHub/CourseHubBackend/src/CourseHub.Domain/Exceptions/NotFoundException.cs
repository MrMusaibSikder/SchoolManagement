namespace CourseHub.Domain.Exceptions;

/// <summary>
/// Raised when a requested entity does not exist.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} with key '{key}' was not found.")
    {
    }
}
