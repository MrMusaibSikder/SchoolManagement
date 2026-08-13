using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.AcademicYear.Interfaces;
using SchoolERP.Application.Features.AttendanceReport.Interfaces;
using SchoolERP.Application.Features.Authentication.Interfaces;
using SchoolERP.Application.Features.Authentication.Services;
using SchoolERP.Application.Features.Authorization.Interfaces;
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
using SchoolERP.Application.Features.FeeReports.Interfaces;
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
using SchoolERP.Application.Features.ProgressReport.Interfaces;
using SchoolERP.Application.Features.Public.Interfaces;
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
using SchoolERP.Application.Features.Transcript.Interfaces;
using SchoolERP.Application.Features.User.Interfaces;
using SchoolERP.Application.Features.UserRole.Interfaces;
using SchoolERP.Infrastructure.Jwt;
using SchoolERP.Infrastructure.Repositories;
using SchoolERP.Infrastructure.Services;
using SchoolERP.Shared;

namespace SchoolERP.Infrastructure
{
    /// <summary>
    /// Registers Infrastructure-layer services: JWT auth, the Unit of Work,
    /// every feature repository and every feature service.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {

            // JWT Settings
            services.Configure<JwtSettings>(config.GetSection("Jwt"));

            // Email + Password Reset Settings
            services.Configure<EmailSettings>(config.GetSection("Email"));
            services.Configure<PasswordResetSettings>(config.GetSection("PasswordReset"));

            // JWT Service
            services.AddScoped<IJwtService, JwtService>();
            // File Handel
            services.Configure<FileStorageSettings>(
                      config.GetSection("FileStorage"));
            // Password Hasher
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // Unit Of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories

            services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
            services.AddScoped<IClassSubjectRepository, ClassSubjectRepository>();
            services.AddScoped<IDesignationRepository, DesignationRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeAttendanceRepository, EmployeeAttendanceRepository>();
            services.AddScoped<IEmployeeSalaryRepository, EmployeeSalaryRepository>();
            services.AddScoped<IExamRepository, ExamRepository>();
            services.AddScoped<IExamScheduleRepository, ExamScheduleRepository>();
            services.AddScoped<IExamTypeRepository, ExamTypeRepository>();
            services.AddScoped<IGuardianRepository, GuardianRepository>();
            services.AddScoped<INoticeRepository, NoticeRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
            services.AddScoped<IResultRepository, ResultRepository>();
            services.AddScoped<IExamResultRepository, ExamResultRepository>();
            services.AddScoped<IExamWeightSetupRepository, ExamWeightSetupRepository>();
            services.AddScoped<IExamWeightItemRepository, ExamWeightItemRepository>();
            services.AddScoped<IFinalResultRepository, FinalResultRepository>();
            services.AddScoped<IGradeSetupRepository, GradeSetupRepository>();
            services.AddScoped<IResultAuditLogRepository, ResultAuditLogRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<ISchoolRepository, SchoolRepository>();
            services.AddScoped<ISchoolClassRepository, SchoolClassRepository>();
            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<ISmsLogRepository, SmsLogRepository>();
            services.AddScoped<ISmsTemplateRepository, SmsTemplateRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<IStudentAttendanceRepository, StudentAttendanceRepository>();
            services.AddScoped<IStudentGuardianRepository, StudentGuardianRepository>();
            services.AddScoped<ISubjectRepository, SubjectRepository>();
            services.AddScoped<ISubjectTeacherRepository, SubjectTeacherRepository>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            //new repository create
            services.AddScoped<IFeeCategoryRepository, FeeCategoryRepository>();
            services.AddScoped<IFeeTypeRepository, FeeTypeRepository>();
            services.AddScoped<IFeeStructureRepository, FeeStructureRepository>();
            services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IReceiptRepository, ReceiptRepository>();
            services.AddScoped<ILateFineRuleRepository, LateFineRuleRepository>();
            services.AddScoped<IStudentFeeConcessionRepository, StudentFeeConcessionRepository>();
            // Services
            services.AddScoped<IProgressReportService, ProgressReportService>();
            services.AddScoped<ITranscriptPdfService, TranscriptPdfService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IAcademicYearService, AcademicYearService>();
            services.AddScoped<IClassSubjectService, ClassSubjectService>();
            services.AddScoped<IDesignationService, DesignationService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IEmployeeAttendanceService, EmployeeAttendanceService>();
            services.AddScoped<IEmployeeSalaryService, EmployeeSalaryService>();
            services.AddScoped<IExamService, ExamService>();
            services.AddScoped<IExamScheduleService, ExamScheduleService>();
            services.AddScoped<IExamTypeService, ExamTypeService>();
            services.AddScoped<IGuardianService, GuardianService>();
            services.AddScoped<INoticeService, NoticeService>();
            services.AddScoped<IPermissionService, PermissionService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IExamResultService, ExamResultService>();
            services.AddScoped<IExamWeightSetupService, ExamWeightSetupService>();
            services.AddScoped<IFinalResultService, FinalResultService>();
            services.AddScoped<IGradeSetupService, GradeSetupService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IRolePermissionService, RolePermissionService>();
            services.AddScoped<ISchoolService, SchoolService>();
            services.AddScoped<ISchoolClassService, SchoolClassService>();
            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<ISmsLogService, SmsLogService>();
            services.AddScoped<ISmsTemplateService, SmsTemplateService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IStudentAttendanceService, StudentAttendanceService>();
            services.AddScoped<IStudentGuardianService, StudentGuardianService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<ISubjectTeacherService, SubjectTeacherService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserAccessService, UserAccessService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPlaceholderReplacementService, PlaceholderReplacementService>();
            services.AddScoped<IGradeLookupService, GradeLookupService>();
            services.AddScoped<IResultAuditService, ResultAuditService>();
            services.AddScoped<ITranscriptService, TranscriptService>();
            services.AddScoped<IAttendanceReportService, AttendanceReportService>();
            services.AddScoped<IFeeCategoryService, FeeCategoryService>();
            services.AddScoped<IFeeTypeService, FeeTypeService>();
            services.AddScoped<IFeeStructureService, FeeStructureService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IReceiptService, ReceiptService>();
            services.AddScoped<ILateFineRuleService, LateFineRuleService>();
            services.AddScoped<IStudentFeeConcessionService, StudentFeeConcessionService>();
            services.AddScoped<ICurrentEmployeeService, CurrentEmployeeService>();
            // For Invoice monthly
            services.AddScoped<IInvoiceGenerationService, InvoiceGenerationService>();
            services.AddScoped<IFeeReportService, FeeReportService>();
            services.AddScoped<ILateFineApplicationService, LateFineApplicationService>();
            services.AddScoped<IReceiptPdfService, ReceiptPdfService>();
            //public data
            services.AddScoped<IPublicInfoService, PublicInfoService>();
            return services;
        }
    }
}
