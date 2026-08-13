namespace SchoolERP.Application.Features.Result.DTOs;

/// <summary>
/// Input model for entering/updating marks for an entire class in one call
/// (one exam schedule = one subject + one class + one exam). Existing entries
/// are updated (upsert); new ones are created. Used for both "Bulk Marks
/// Entry" and "Bulk Update".
/// </summary>
public class BulkMarkEntryDto
{
    public int ExamScheduleId { get; set; }

    /// <summary>Id of the teacher entering the marks. Must be assigned to the schedule's subject.</summary>
    public int TeacherId { get; set; }

    public List<MarkEntryItemDto> Entries { get; set; } = new();
}
