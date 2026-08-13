using SchoolERP.Application.Features.AcademicYear.Interfaces;
using SchoolERP.Application.Features.AttendanceReport.Interfaces;
using SchoolERP.Application.Features.ClassSubject.Interfaces;
using SchoolERP.Application.Features.Designation.Interfaces;
using SchoolERP.Application.Features.Employee.Interfaces;
using SchoolERP.Application.Features.EmployeeAttendance.Interfaces;
using SchoolERP.Application.Features.EmployeeSalary.Interfaces;
using SchoolERP.Application.Features.Exam.Interfaces;
using SchoolERP.Application.Features.ExamResult.Interfaces;
using SchoolERP.Application.Features.ExamSchedule.Interfaces;
using SchoolERP.Application.Features.ExamType.Interfaces;
using SchoolERP.Application.Features.ExamWeightItem.Interfaces;
using SchoolERP.Application.Features.ExamWeightSetup.Interfaces;
using SchoolERP.Application.Features.FeeCategory.Interfaces;
using SchoolERP.Application.Features.FeeStructure.Interfaces;
using SchoolERP.Application.Features.FeeType.Interfaces;
using SchoolERP.Application.Features.FinalResult.Interfaces;
using SchoolERP.Application.Features.GradeSetup.Interfaces;
using SchoolERP.Application.Features.Guardian.Interfaces;
using SchoolERP.Application.Features.Invoice.Interfaces;
using SchoolERP.Application.Features.LateFineRule.Interfaces;
using SchoolERP.Application.Features.Notice.Interfaces;
using SchoolERP.Application.Features.PasswordResetToken.Interfaces;
using SchoolERP.Application.Features.Payment.Interfaces;
using SchoolERP.Application.Features.Permission.Interfaces;
using SchoolERP.Application.Features.Receipt.Interfaces;
using SchoolERP.Application.Features.RefreshToken.Interfaces;
using SchoolERP.Application.Features.Result.Interfaces;
using SchoolERP.Application.Features.ResultAuditLog.Interfaces;
using SchoolERP.Application.Features.Role.Interfaces;
using SchoolERP.Application.Features.RolePermission.Interfaces;
using SchoolERP.Application.Features.School.Interfaces;
using SchoolERP.Application.Features.SchoolClass.Interfaces;
using SchoolERP.Application.Features.Section.Interfaces;
using SchoolERP.Application.Features.SmsLog.Interfaces;
using SchoolERP.Application.Features.SmsTemplate.Interfaces;
using SchoolERP.Application.Features.Student.Interfaces;
using SchoolERP.Application.Features.StudentAttendance.Interfaces;
using SchoolERP.Application.Features.StudentFeeConcession.Interfaces;
using SchoolERP.Application.Features.StudentGuardian.Interfaces;
using SchoolERP.Application.Features.Subject.Interfaces;
using SchoolERP.Application.Features.SubjectTeacher.Interfaces;
using SchoolERP.Application.Features.Teacher.Interfaces;
using SchoolERP.Application.Features.User.Interfaces;
using SchoolERP.Application.Features.UserRole.Interfaces;

namespace SchoolERP.Application.Common.Interfaces;

/// <summary>
/// Coordinates all feature repositories under a single EF Core <c>DbContext</c>
/// and exposes a single point to commit changes in one transaction.
/// </summary>
public interface IUnitOfWork : IDisposable, IAsyncDisposable 
{
    /// <summary>Repository for AcademicYear entities.</summary>
    IAcademicYearRepository AcademicYearRepository { get; }

    /// <summary>Repository for ClassSubject entities.</summary>
    IClassSubjectRepository ClassSubjectRepository { get; }

    /// <summary>Repository for Designation entities.</summary>
    IDesignationRepository DesignationRepository { get; }

    /// <summary>Repository for Employee entities.</summary>
    IEmployeeRepository EmployeeRepository { get; }

    /// <summary>Repository for EmployeeAttendance entities.</summary>
    IEmployeeAttendanceRepository EmployeeAttendanceRepository { get; }

    /// <summary>Repository for EmployeeSalary entities.</summary>
    IEmployeeSalaryRepository EmployeeSalaryRepository { get; }

    /// <summary>Repository for Exam entities.</summary>
    IExamRepository ExamRepository { get; }

    /// <summary>Repository for ExamSchedule entities.</summary>
    IExamScheduleRepository ExamScheduleRepository { get; }

    /// <summary>Repository for ExamType entities.</summary>
    IExamTypeRepository ExamTypeRepository { get; }

    /// <summary>Repository for FeeStructure entities.</summary>
    IFeeStructureRepository FeeStructureRepository { get; }

    /// <summary>Repository for FeeType entities.</summary>
    IFeeTypeRepository FeeTypeRepository { get; }

    /// <summary>Repository for Guardian entities.</summary>
    IGuardianRepository GuardianRepository { get; }

    /// <summary>Repository for Notice entities.</summary>
    INoticeRepository NoticeRepository { get; }

    /// <summary>Repository for Permission entities.</summary>
    IPermissionRepository PermissionRepository { get; }

    /// <summary> new Add Repository for RefreshToken entities.</summary>
    IRefreshTokenRepository RefreshTokenRepository { get; }

    /// <summary>new Add Repository for PasswordResetToken entities.</summary>
    IPasswordResetTokenRepository PasswordResetTokenRepository { get; }


    /// <summary>Repository for Result entities.</summary>

    IResultRepository ResultRepository { get; }

    /// <summary>Repository for ExamResult entities.</summary>
    IExamResultRepository ExamResultRepository { get; }

    /// <summary>Repository for ExamWeightSetup entities.</summary>
    IExamWeightSetupRepository ExamWeightSetupRepository { get; }

    /// <summary>Repository for ExamWeightItem entities.</summary>
    IExamWeightItemRepository ExamWeightItemRepository { get; }

    /// <summary>Repository for FinalResult entities.</summary>
    IFinalResultRepository FinalResultRepository { get; }

    /// <summary>Repository for GradeSetup entities.</summary>
    IGradeSetupRepository GradeSetupRepository { get; }

    /// <summary>Repository for ResultAuditLog entries.</summary>
    IResultAuditLogRepository ResultAuditLogRepository { get; }

    /// <summary>Repository for Role entities.</summary>
    IRoleRepository RoleRepository { get; }

    /// <summary>Repository for RolePermission entities.</summary>
    IRolePermissionRepository RolePermissionRepository { get; }

    /// <summary>Repository for School entities.</summary>
    ISchoolRepository SchoolRepository { get; }

    /// <summary>Repository for SchoolClass entities.</summary>
    ISchoolClassRepository SchoolClassRepository { get; }

    /// <summary>Repository for Section entities.</summary>
    ISectionRepository SectionRepository { get; }

    /// <summary>Repository for SmsLog entities.</summary>
    ISmsLogRepository SmsLogRepository { get; }

    /// <summary>Repository for SmsTemplate entities.</summary>
    ISmsTemplateRepository SmsTemplateRepository { get; }

    /// <summary>Repository for Student entities.</summary>
    IStudentRepository StudentRepository { get; }

    /// <summary>Repository for StudentAttendance entities.</summary>
    IStudentAttendanceRepository StudentAttendanceRepository { get; }

    /// <summary>Repository for StudentGuardian entities.</summary>
    IStudentGuardianRepository StudentGuardianRepository { get; }

    /// <summary>Repository for Subject entities.</summary>
    ISubjectRepository SubjectRepository { get; }

    /// <summary>Repository for SubjectTeacher entities.</summary>
    ISubjectTeacherRepository SubjectTeacherRepository { get; }

    /// <summary>Repository for Teacher entities.</summary>
    ITeacherRepository TeacherRepository { get; }

    /// <summary>Repository for User entities.</summary>
    IUserRepository UserRepository { get; }

    /// <summary>Repository for UserRole entities.</summary>
    IUserRoleRepository UserRoleRepository { get; }

    /// <summary>Repository for FeeCategory entities.</summary>
    IFeeCategoryRepository FeeCategoryRepository { get; }

    /// <summary>Repository for Invoice entities.</summary>
    IInvoiceRepository InvoiceRepository { get; }

    /// <summary>Repository for Payment entities.</summary>
    IPaymentRepository PaymentRepository { get; }

    /// <summary>Repository for Receipt entities.</summary>
    IReceiptRepository ReceiptRepository { get; }

    /// <summary>Repository for LateFineRule entities.</summary>
    ILateFineRuleRepository LateFineRuleRepository { get; }

    /// <summary>Repository for StudentFeeConcession entities.</summary>
    IStudentFeeConcessionRepository StudentFeeConcessionRepository { get; }


    /// <summary>Persists all pending changes tracked by the underlying DbContext.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    ///<summary>Transaction Add by Musaib Sikder</summary>
    Task BeginTransactionAsync(
    CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(
        CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(
        CancellationToken cancellationToken = default);

}

