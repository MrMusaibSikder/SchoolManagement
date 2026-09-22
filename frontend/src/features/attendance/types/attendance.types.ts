export const AttendanceStatus = {
  Present: 1,
  Absent: 2,
  Late: 3,
  Leave: 4,
  HalfDay: 5,
} as const;

export type AttendanceStatusValue = (typeof AttendanceStatus)[keyof typeof AttendanceStatus];

export const ATTENDANCE_STATUS_OPTIONS: Array<{
  value: AttendanceStatusValue;
  label: string;
}> = [
  { value: AttendanceStatus.Present, label: "Present" },
  { value: AttendanceStatus.Absent, label: "Absent" },
  { value: AttendanceStatus.Late, label: "Late" },
  { value: AttendanceStatus.Leave, label: "Leave" },
  { value: AttendanceStatus.HalfDay, label: "Half day" },
];

export function attendanceStatusLabel(status: number | null | undefined) {
  return ATTENDANCE_STATUS_OPTIONS.find((item) => item.value === status)?.label ?? "—";
}

export function todayIsoDate() {
  const now = new Date();
  const month = String(now.getMonth() + 1).padStart(2, "0");
  const day = String(now.getDate()).padStart(2, "0");
  return `${now.getFullYear()}-${month}-${day}`;
}

export interface StudentAttendanceDto {
  id: number;
  studentId: number;
  studentName: string;
  attendanceDate: string;
  status: AttendanceStatusValue;
  remarks?: string | null;
}

export interface StudentAttendanceItemDto {
  studentId: number;
  status: AttendanceStatusValue;
  remarks?: string | null;
}

export interface BulkStudentAttendanceDto {
  classId: number;
  sectionId: number;
  attendanceDate: string;
  attendance: StudentAttendanceItemDto[];
}

export interface EmployeeAttendanceDto {
  id: number;
  employeeId: number;
  attendanceDate: string;
  checkIn?: string | null;
  checkOut?: string | null;
  status: AttendanceStatusValue;
}

export interface EmployeeAttendanceItemDto {
  employeeId: number;
  status: AttendanceStatusValue;
  checkIn?: string | null;
  checkOut?: string | null;
}

export interface BulkEmployeeAttendanceDto {
  attendanceDate: string;
  attendance: EmployeeAttendanceItemDto[];
}

export interface DashboardAttendanceDto {
  totalStudents: number;
  presentToday: number;
  absentToday: number;
  lateToday: number;
  leaveToday: number;
  attendancePercentage: number;
}

export interface AdminDashboardAttendanceDto {
  totalStudents: number;
  totalPresent: number;
  totalAbsent: number;
  totalLate: number;
  totalLeave: number;
  attendancePercentage: number;
}

export interface ClassAttendanceSummaryDto {
  classId: number;
  sectionId: number;
  totalStudents: number;
  present: number;
  absent: number;
  late: number;
  leave: number;
  percentage: number;
}

export interface AttendanceTrendDto {
  date: string;
  present: number;
  absent: number;
  late: number;
  leave: number;
}
