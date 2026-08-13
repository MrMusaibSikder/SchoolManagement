using Microsoft.AspNetCore.Http;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Application.Features.Notice.DTOs;

/// <summary>Input model for updating an existing Notice record.</summary>
public class UpdateNoticeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public NoticePriority Priority { get; set; }
    public NoticeAudience Audience { get; set; }
    public bool SendSms { get; set; }
    public bool SendEmail { get; set; }

    /// <summary>Optional replacement attachment file. Leave null to keep the existing attachment.</summary>
    public IFormFile? AttachmentFile { get; set; }

    /// <summary>Set to true to remove the existing attachment without uploading a new one.</summary>
    public bool RemoveAttachment { get; set; }
}
