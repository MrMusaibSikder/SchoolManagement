namespace CourseHub.Domain.Exceptions;

/// <summary>
/// Base type for all exceptions raised by the Domain layer.
/// Framework-independent: no dependency on ASP.NET Core or EF Core.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message)
        : base(message)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
