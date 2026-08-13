using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Application.Common.Models;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// SMTP-based <see cref="IEmailService"/> implementation using the built-in
/// <see cref="SmtpClient"/>. If no SMTP host is configured (e.g. in local
/// development), the email is logged instead of sent so the Forgot Password
/// flow can still be exercised end-to-end without a real mail server.
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string body, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_settings.Host))
        {
            // No SMTP server configured: log instead of failing the calling flow.
            _logger.LogInformation(
                "Email delivery skipped (no SMTP host configured). To: {ToEmail}, Subject: {Subject}, Body: {Body}",
                toEmail, subject, body);
            return;
        }

        using var message = new MailMessage
        {
            From = new MailAddress(_settings.FromAddress, _settings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        message.To.Add(toEmail);

        using var client = new SmtpClient(_settings.Host, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = string.IsNullOrWhiteSpace(_settings.Username)
                ? null
                : new NetworkCredential(_settings.Username, _settings.Password)
        };

        await client.SendMailAsync(message, cancellationToken);
    }
}
