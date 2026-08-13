namespace SchoolERP.Domain.Enums;

/// <summary>Attendance status used for both student and employee attendance.</summary>
public enum AttendanceStatus
{
    Present = 1,
    Absent = 2,
    Late = 3,
    Leave = 4,
    HalfDay = 5
}
