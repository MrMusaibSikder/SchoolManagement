namespace CourseHub.Application.Common.Interfaces;

/// <summary>
/// Abstraction for sending transactional emails. No production email
/// provider is wired up yet — see CourseHub.Infrastructure/Email for the
/// development implementation and where a real provider (SendGrid, Amazon
/// SES, SMTP, etc.) should be plugged in later.
/// </summary>
public interface IEmailSender
{
    Task SendPasswordResetEmailAsync(string toEmail, string resetLink, CancellationToken cancellationToken = default);
}
