/**
 * Frontend mirror of backend PermissionNames constants.
 * Keep in sync with SchoolERP.Domain.Constants.PermissionNames.
 */
export const Permission = {
  StudentView: "Student.View",
  StudentCreate: "Student.Create",
  TeacherView: "Teacher.View",
  EmployeeView: "Employee.View",
  GuardianView: "Guardian.View",
  GuardianCreate: "Guardian.Create",
  InvoiceView: "Invoice.View",
  PaymentView: "Payment.View",
  PaymentCollect: "Payment.Collect",
  AttendanceReportView: "AttendanceReport.View",
  StudentAttendanceCreate: "StudentAttendance.Create",
  NoticeView: "Notice.View",
  ExamView: "Exam.View",
  AcademicYearView: "AcademicYear.View",
  ProgressReportView: "ProgressReport.View",
  FeeReportView: "FeeReport.View",
} as const;

export type PermissionName = (typeof Permission)[keyof typeof Permission];

export const AppRole = {
  Admin: "Admin",
  Teacher: "Teacher",
  Student: "Student",
  Accountant: "Accountant",
} as const;

export type AppRoleName = (typeof AppRole)[keyof typeof AppRole];
