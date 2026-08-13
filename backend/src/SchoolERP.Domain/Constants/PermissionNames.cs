namespace SchoolERP.Domain.Constants;

/// <summary>
/// Central, compile-time-safe catalogue of every permission name used by the
/// permission-based authorization system. Using these constants (instead of raw
/// strings) with <c>[PermissionAuthorize]</c> avoids typos between the attribute
/// usage on controllers and the <see cref="Entities.Permission"/> rows seeded in
/// the database.
/// </summary>
public static class PermissionNames
{
    // FeeCategory
    public const string FeeCategoryView = "FeeCategory.View";
    public const string FeeCategoryCreate = "FeeCategory.Create";
    public const string FeeCategoryEdit = "FeeCategory.Edit";
    public const string FeeCategoryDelete = "FeeCategory.Delete";

    // FeeType
    public const string FeeTypeView = "FeeType.View";
    public const string FeeTypeCreate = "FeeType.Create";
    public const string FeeTypeEdit = "FeeType.Edit";
    public const string FeeTypeDelete = "FeeType.Delete";

    // FeeStructure
    public const string FeeStructureView = "FeeStructure.View";
    public const string FeeStructureCreate = "FeeStructure.Create";
    public const string FeeStructureEdit = "FeeStructure.Edit";
    public const string FeeStructureDelete = "FeeStructure.Delete";

    // Invoice
    public const string InvoiceView = "Invoice.View";
    public const string InvoiceCreate = "Invoice.Create";
    public const string InvoiceCancel = "Invoice.Cancel";

    // Payment
    public const string PaymentView = "Payment.View";
    public const string PaymentCollect = "Payment.Collect";
    public const string PaymentVoid = "Payment.Void";

    // Receipt
    public const string ReceiptView = "Receipt.View";
    public const string ReceiptVoid = "Receipt.Void";

    // LateFineRule
    public const string LateFineRuleView = "LateFineRule.View";
    public const string LateFineRuleManage = "LateFineRule.Manage";

    // StudentFeeConcession
    public const string ConcessionView = "Concession.View";
    public const string ConcessionCreate = "Concession.Create";
    public const string ConcessionEdit = "Concession.Edit";
    public const string ConcessionApprove = "Concession.Approve";
    public const string ConcessionDelete = "Concession.Delete";
    //Fee REport
    // PermissionNames.cs এ যোগ করুন
    public const string FeeReportView = "FeeReport.View";

    //Academic year
    public const string AcademicYearView = "AcademicYear.View";
    public const string AcademicYearCreate = "AcademicYear.Create";
    public const string AcademicYearEdit = "AcademicYear.Edit";
    public const string AcademicYearDelete = "AcademicYear.Delete";

    // School
    public const string SchoolView = "School.View";
    public const string SchoolCreate = "School.Create";
    public const string SchoolUpdate = "School.Edit";
    public const string SchoolDelete = "School.Delete";
    // User management
    public const string UserView = "User.View";
    public const string UserCreate = "User.Create";
    public const string UserEdit = "User.Edit";
    public const string UserDelete = "User.Delete";

    // Role management
    public const string RoleView = "Role.View";
    public const string RoleCreate = "Role.Create";
    public const string RoleEdit = "Role.Edit";
    public const string RoleDelete = "Role.Delete";
    public const string RoleAssignPermission = "Role.AssignPermission";

    // Permission management
    public const string PermissionView = "Permission.View";
    public const string PermissionCreate = "Permission.Create";
    public const string PermissionEdit = "Permission.Edit";
    public const string PermissionDelete = "Permission.Delete";

    // User <-> Role assignment
    public const string UserRoleAssign = "UserRole.Assign";
    public const string UserRoleRemove = "UserRole.Remove";
    public const string UserRoleView = "UserRole.View";

    // Student
    public const string StudentView = "Student.View";
    public const string StudentCreate = "Student.Create";
    public const string StudentEdit = "Student.Edit";
    public const string StudentDelete = "Student.Delete";

    // Teacher
    public const string TeacherView = "Teacher.View";
    public const string TeacherCreate = "Teacher.Create";
    public const string TeacherEdit = "Teacher.Edit";
    public const string TeacherDelete = "Teacher.Delete";

    // Employee
    public const string EmployeeView = "Employee.View";
    public const string EmployeeCreate = "Employee.Create";
    public const string EmployeeEdit = "Employee.Edit";
    public const string EmployeeDelete = "Employee.Delete";

    // Designation
    public const string DesignationView = "Designation.View";
    public const string DesignationCreate = "Designation.Create";
    public const string DesignationEdit = "Designation.Edit";
    public const string DesignationDelete = "Designation.Delete";

    // Guardian
    public const string GuardianView = "Guardian.View";
    public const string GuardianCreate = "Guardian.Create";
    public const string GuardianEdit = "Guardian.Edit";
    public const string GuardianDelete = "Guardian.Delete";

    // SchoolClass
    public const string SchoolClassView = "SchoolClass.View";
    public const string SchoolClassCreate = "SchoolClass.Create";
    public const string SchoolClassEdit = "SchoolClass.Edit";
    public const string SchoolClassDelete = "SchoolClass.Delete";

    // Section
    public const string SectionView = "Section.View";
    public const string SectionCreate = "Section.Create";
    public const string SectionEdit = "Section.Edit";
    public const string SectionDelete = "Section.Delete";

    // Subject
    public const string SubjectView = "Subject.View";
    public const string SubjectCreate = "Subject.Create";
    public const string SubjectEdit = "Subject.Edit";
    public const string SubjectDelete = "Subject.Delete";

    // ClassSubject (class <-> subject assignment)
    public const string ClassSubjectView = "ClassSubject.View";
    public const string ClassSubjectAssign = "ClassSubject.Assign";
    public const string ClassSubjectRemove = "ClassSubject.Remove";

    // SubjectTeacher (subject <-> teacher assignment)
    public const string SubjectTeacherView = "SubjectTeacher.View";
    public const string SubjectTeacherAssign = "SubjectTeacher.Assign";
    public const string SubjectTeacherRemove = "SubjectTeacher.Remove";

    // StudentAttendance
    public const string StudentAttendanceView = "StudentAttendance.View";
    public const string StudentAttendanceCreate = "StudentAttendance.Create";
    public const string StudentAttendanceEdit = "StudentAttendance.Edit";
    public const string StudentAttendanceDelete = "StudentAttendance.Delete";

    // EmployeeAttendance
    public const string EmployeeAttendanceView = "EmployeeAttendance.View";
    public const string EmployeeAttendanceCreate = "EmployeeAttendance.Create";
    public const string EmployeeAttendanceEdit = "EmployeeAttendance.Edit";
    public const string EmployeeAttendanceDelete = "EmployeeAttendance.Delete";

    // AttendanceReport (read-only dashboards/reports)
    public const string AttendanceReportView = "AttendanceReport.View";

    // SmsTemplate
    public const string SmsTemplateView = "SmsTemplate.View";
    public const string SmsTemplateCreate = "SmsTemplate.Create";
    public const string SmsTemplateEdit = "SmsTemplate.Edit";
    public const string SmsTemplateDelete = "SmsTemplate.Delete";

    // SmsLog (read-only; no Edit permission since logs are immutable)
    public const string SmsLogView = "SmsLog.View";
    public const string SmsLogCreate = "SmsLog.Create";
    public const string SmsLogDelete = "SmsLog.Delete";

    // Notice
    public const string NoticeView = "Notice.View";
    public const string NoticeCreate = "Notice.Create";
    public const string NoticeEdit = "Notice.Edit";
    public const string NoticeDelete = "Notice.Delete";
    public const string NoticePublish = "Notice.Publish";

    // EmployeeSalary 
    public const string EmployeeSalaryView = "EmployeeSalary.View";
    public const string EmployeeSalaryCreate = "EmployeeSalary.Create";
    public const string EmployeeSalaryUpdate = "EmployeeSalary.Update";
    public const string EmployeeSalaryDelete = "EmployeeSalary.Delete";

    // ExamType
    public const string ExamTypeView = "ExamType.View";
    public const string ExamTypeCreate = "ExamType.Create";
    public const string ExamTypeEdit = "ExamType.Edit";
    public const string ExamTypeDelete = "ExamType.Delete";

    // Exam
    public const string ExamView = "Exam.View";
    public const string ExamCreate = "Exam.Create";
    public const string ExamEdit = "Exam.Edit";
    public const string ExamDelete = "Exam.Delete";
    public const string ExamPublish = "Exam.Publish";
    public const string ExamComplete = "Exam.Complete";
    public const string ExamCancel = "Exam.Cancel";

    // ExamSchedule
    public const string ExamScheduleView = "ExamSchedule.View";
    public const string ExamScheduleCreate = "ExamSchedule.Create";
    public const string ExamScheduleEdit = "ExamSchedule.Edit";
    public const string ExamScheduleDelete = "ExamSchedule.Delete";

    // Marks Entry (Result module: subject-level mark entry)
    public const string MarksEntryView = "MarksEntry.View";
    public const string MarksEntryCreate = "MarksEntry.Create";
    public const string MarksEntryEdit = "MarksEntry.Edit";
    public const string MarksEntryDelete = "MarksEntry.Delete";
    public const string MarksEntryPublish = "MarksEntry.Publish";

    // Exam Result (aggregate exam-level result: calculation, publishing, reports)
    public const string ResultView = "Result.View";
    public const string ResultCalculate = "Result.Calculate";
    public const string ResultPublish = "Result.Publish";
    public const string ResultUnlock = "Result.Unlock";

    // Exam Weight Setup
    public const string WeightSetupView = "WeightSetup.View";
    public const string WeightSetupManage = "WeightSetup.Manage";

    // Final Result (weighted, year-wide result)
    public const string FinalResultView = "FinalResult.View";
    public const string FinalResultCalculate = "FinalResult.Calculate";
    public const string FinalResultPublish = "FinalResult.Publish";
    public const string FinalResultUnlock = "FinalResult.Unlock";

    // Grade Setup (configurable grading bands)
    public const string GradeSetupView = "GradeSetup.View";
    public const string GradeSetupManage = "GradeSetup.Manage";

    // Result Audit Trail
    public const string ResultAuditView = "ResultAudit.View";

    // Transcript
    public const string TranscriptView = "Transcript.View";

    // Progress Report
    public const string ProgressReportView = "ProgressReport.View";

    // Fee (already present in seed data)
    public const string FeeManage = "Fee.Manage";

    /// <summary>
    /// Returns every permission name declared as a constant on this class, via
    /// reflection. Used by the permission seeder so newly added constants are
    /// automatically picked up without having to maintain a second list by hand.
    /// </summary>
    public static IReadOnlyList<string> GetAll()
    {
        return typeof(PermissionNames)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!)
            .ToList();
    }
}
