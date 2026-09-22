/**
 * Frontend mirror of backend PermissionNames constants.
 * Keep in sync with SchoolERP.Domain.Constants.PermissionNames.
 */
export const Permission = {
  StudentView: "Student.View",
  StudentCreate: "Student.Create",
  TeacherView: "Teacher.View",
  EmployeeView: "Employee.View",
  EmployeeCreate: "Employee.Create",
  EmployeeEdit: "Employee.Edit",
  EmployeeDelete: "Employee.Delete",
  DesignationView: "Designation.View",
  UserView: "User.View",
  GuardianView: "Guardian.View",
  GuardianCreate: "Guardian.Create",
  InvoiceView: "Invoice.View",
  PaymentView: "Payment.View",
  PaymentCollect: "Payment.Collect",
  AttendanceReportView: "AttendanceReport.View",
  StudentAttendanceView: "StudentAttendance.View",
  StudentAttendanceCreate: "StudentAttendance.Create",
  StudentAttendanceEdit: "StudentAttendance.Edit",
  EmployeeAttendanceView: "EmployeeAttendance.View",
  EmployeeAttendanceCreate: "EmployeeAttendance.Create",
  EmployeeAttendanceEdit: "EmployeeAttendance.Edit",
  NoticeView: "Notice.View",
  ExamView: "Exam.View",
  ExamCreate: "Exam.Create",
  ExamEdit: "Exam.Edit",
  ExamDelete: "Exam.Delete",
  ExamPublish: "Exam.Publish",
  ExamComplete: "Exam.Complete",
  ExamCancel: "Exam.Cancel",
  ExamTypeView: "ExamType.View",
  ExamTypeCreate: "ExamType.Create",
  ExamTypeEdit: "ExamType.Edit",
  ExamTypeDelete: "ExamType.Delete",
  ExamScheduleView: "ExamSchedule.View",
  ExamScheduleCreate: "ExamSchedule.Create",
  ExamScheduleEdit: "ExamSchedule.Edit",
  ExamScheduleDelete: "ExamSchedule.Delete",
  GradeSetupView: "GradeSetup.View",
  GradeSetupManage: "GradeSetup.Manage",
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
