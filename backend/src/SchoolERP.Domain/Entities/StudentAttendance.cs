using SchoolERP.Domain.Common;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Domain.Entities;

/// <summary>Represents a single day's attendance record for a student.</summary>
public class StudentAttendance : BaseEntity
{
    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public DateTime AttendanceDate { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Remarks { get; set; }
}
