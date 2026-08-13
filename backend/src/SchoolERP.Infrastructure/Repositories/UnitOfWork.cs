using Microsoft.EntityFrameworkCore.Storage;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.AcademicYear.Interfaces;
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
using SchoolERP.Infrastructure.Persistence.Context;

namespace SchoolERP.Infrastructure.Repositories;

/// <summary>
/// EF Core Unit of Work. Lazily instantiates each feature repository against a
/// single shared <see cref="SchoolERPDbContext"/> so all changes within a request
/// are committed together in one transaction via <see cref="SaveChangesAsync"/>.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly SchoolERPDbContext _context;
    private bool _disposed;

    private IAcademicYearRepository? _academicYearRepository;
    private IClassSubjectRepository? _classSubjectRepository;
    private IDesignationRepository? _designationRepository;
    private IEmployeeRepository? _employeeRepository;
    private IEmployeeAttendanceRepository? _employeeAttendanceRepository;
    private IEmployeeSalaryRepository? _employeeSalaryRepository;
    private IExamRepository? _examRepository;
    private IExamScheduleRepository? _examScheduleRepository;
    private IExamTypeRepository? _examTypeRepository;
    private IFeeStructureRepository? _feeStructureRepository;
    private IFeeTypeRepository? _feeTypeRepository;
    private IGuardianRepository? _guardianRepository;
    private INoticeRepository? _noticeRepository;
    private IPermissionRepository? _permissionRepository;
    private IRefreshTokenRepository? _refreshTokenRepository;
    private IPasswordResetTokenRepository? _passwordResetTokenRepository;
    private IResultRepository? _resultRepository;
    private IExamResultRepository? _examResultRepository;
    private IExamWeightSetupRepository? _examWeightSetupRepository;
    private IExamWeightItemRepository? _examWeightItemRepository;
    private IFinalResultRepository? _finalResultRepository;
    private IGradeSetupRepository? _gradeSetupRepository;
    private IResultAuditLogRepository? _resultAuditLogRepository;
    private IRoleRepository? _roleRepository;
    private IRolePermissionRepository? _rolePermissionRepository;
    private ISchoolRepository? _schoolRepository;
    private ISchoolClassRepository? _schoolClassRepository;
    private ISectionRepository? _sectionRepository;
    private ISmsLogRepository? _smsLogRepository;
    private ISmsTemplateRepository? _smsTemplateRepository;
    private IStudentRepository? _studentRepository;
    private IStudentAttendanceRepository? _studentAttendanceRepository;
    private IStudentGuardianRepository? _studentGuardianRepository;
    private ISubjectRepository? _subjectRepository;
    private ISubjectTeacherRepository? _subjectTeacherRepository;
    private ITeacherRepository? _teacherRepository;
    private IUserRepository? _userRepository;
    private IUserRoleRepository? _userRoleRepository;
    private IDbContextTransaction? _transaction;
    private IFeeCategoryRepository? _feeCategoryRepository;
    private IInvoiceRepository? _invoiceRepository;
    private IPaymentRepository? _paymentRepository;
    private IReceiptRepository? _receiptRepository;
    private ILateFineRuleRepository? _lateFineRuleRepository;
    private IStudentFeeConcessionRepository? _studentFeeConcessionRepository;

    public UnitOfWork(SchoolERPDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public IAcademicYearRepository AcademicYearRepository => _academicYearRepository ??= new AcademicYearRepository(_context);

    /// <inheritdoc />
    public IClassSubjectRepository ClassSubjectRepository => _classSubjectRepository ??= new ClassSubjectRepository(_context);

    /// <inheritdoc />
    public IDesignationRepository DesignationRepository => _designationRepository ??= new DesignationRepository(_context);

    /// <inheritdoc />
    public IEmployeeRepository EmployeeRepository => _employeeRepository ??= new EmployeeRepository(_context);

    /// <inheritdoc />
    public IEmployeeAttendanceRepository EmployeeAttendanceRepository => _employeeAttendanceRepository ??= new EmployeeAttendanceRepository(_context);

    /// <inheritdoc />
    public IEmployeeSalaryRepository EmployeeSalaryRepository => _employeeSalaryRepository ??= new EmployeeSalaryRepository(_context);

    /// <inheritdoc />
    public IExamRepository ExamRepository => _examRepository ??= new ExamRepository(_context);

    /// <inheritdoc />
    public IExamScheduleRepository ExamScheduleRepository => _examScheduleRepository ??= new ExamScheduleRepository(_context);

    /// <inheritdoc />
    public IExamTypeRepository ExamTypeRepository => _examTypeRepository ??= new ExamTypeRepository(_context);

    /// <inheritdoc />
    public IFeeStructureRepository FeeStructureRepository => _feeStructureRepository ??= new FeeStructureRepository(_context);

    /// <inheritdoc />
    public IFeeTypeRepository FeeTypeRepository => _feeTypeRepository ??= new FeeTypeRepository(_context);

    /// <inheritdoc />
    public IGuardianRepository GuardianRepository => _guardianRepository ??= new GuardianRepository(_context);

    /// <inheritdoc />
    public INoticeRepository NoticeRepository => _noticeRepository ??= new NoticeRepository(_context);

    /// <inheritdoc />
    public IPermissionRepository PermissionRepository => _permissionRepository ??= new PermissionRepository(_context);

    /// <inheritdoc />
    public IRefreshTokenRepository RefreshTokenRepository => _refreshTokenRepository ??= new RefreshTokenRepository(_context);

    /// <inheritdoc />
    public IPasswordResetTokenRepository PasswordResetTokenRepository => _passwordResetTokenRepository ??= new PasswordResetTokenRepository(_context);

    /// <inheritdoc />
    public IResultRepository ResultRepository => _resultRepository ??= new ResultRepository(_context);

    /// <inheritdoc />
    public IExamResultRepository ExamResultRepository => _examResultRepository ??= new ExamResultRepository(_context);

    /// <inheritdoc />
    public IExamWeightSetupRepository ExamWeightSetupRepository => _examWeightSetupRepository ??= new ExamWeightSetupRepository(_context);

    /// <inheritdoc />
    public IExamWeightItemRepository ExamWeightItemRepository => _examWeightItemRepository ??= new ExamWeightItemRepository(_context);

    /// <inheritdoc />
    public IFinalResultRepository FinalResultRepository => _finalResultRepository ??= new FinalResultRepository(_context);

    /// <inheritdoc />
    public IGradeSetupRepository GradeSetupRepository => _gradeSetupRepository ??= new GradeSetupRepository(_context);

    /// <inheritdoc />
    public IResultAuditLogRepository ResultAuditLogRepository => _resultAuditLogRepository ??= new ResultAuditLogRepository(_context);

    /// <inheritdoc />
    public IRoleRepository RoleRepository => _roleRepository ??= new RoleRepository(_context);

    /// <inheritdoc />
    public IRolePermissionRepository RolePermissionRepository => _rolePermissionRepository ??= new RolePermissionRepository(_context);

    /// <inheritdoc />
    public ISchoolRepository SchoolRepository => _schoolRepository ??= new SchoolRepository(_context);

    /// <inheritdoc />
    public ISchoolClassRepository SchoolClassRepository => _schoolClassRepository ??= new SchoolClassRepository(_context);

    /// <inheritdoc />
    public ISectionRepository SectionRepository => _sectionRepository ??= new SectionRepository(_context);

    /// <inheritdoc />
    public ISmsLogRepository SmsLogRepository => _smsLogRepository ??= new SmsLogRepository(_context);

    /// <inheritdoc />
    public ISmsTemplateRepository SmsTemplateRepository => _smsTemplateRepository ??= new SmsTemplateRepository(_context);

    /// <inheritdoc />
    public IStudentRepository StudentRepository => _studentRepository ??= new StudentRepository(_context);

    /// <inheritdoc />
    public IStudentAttendanceRepository StudentAttendanceRepository => _studentAttendanceRepository ??= new StudentAttendanceRepository(_context);

    /// <inheritdoc />
    public IStudentGuardianRepository StudentGuardianRepository => _studentGuardianRepository ??= new StudentGuardianRepository(_context);

    /// <inheritdoc />
    public ISubjectRepository SubjectRepository => _subjectRepository ??= new SubjectRepository(_context);

    /// <inheritdoc />
    public ISubjectTeacherRepository SubjectTeacherRepository => _subjectTeacherRepository ??= new SubjectTeacherRepository(_context);

    /// <inheritdoc />
    public ITeacherRepository TeacherRepository => _teacherRepository ??= new TeacherRepository(_context);

    /// <inheritdoc />
    public IUserRepository UserRepository => _userRepository ??= new UserRepository(_context);

    /// <inheritdoc />
    public IUserRoleRepository UserRoleRepository => _userRoleRepository ??= new UserRoleRepository(_context);


    /// <inheritdoc />
    public IFeeCategoryRepository FeeCategoryRepository => _feeCategoryRepository ??= new FeeCategoryRepository(_context);

    /// <inheritdoc />
    public IInvoiceRepository InvoiceRepository => _invoiceRepository ??= new InvoiceRepository(_context);

    /// <inheritdoc />
    public IPaymentRepository PaymentRepository => _paymentRepository ??= new PaymentRepository(_context);

    /// <inheritdoc />
    public IReceiptRepository ReceiptRepository => _receiptRepository ??= new ReceiptRepository(_context);

    /// <inheritdoc />
    public ILateFineRuleRepository LateFineRuleRepository => _lateFineRuleRepository ??= new LateFineRuleRepository(_context);

    /// <inheritdoc />
    public IStudentFeeConcessionRepository StudentFeeConcessionRepository => _studentFeeConcessionRepository ??= new StudentFeeConcessionRepository(_context);
    /// <inheritdoc />
    ///  /// <summary>
    /// Persists all pending changes to the database.
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    /// <inheritdoc/>
    /// <summary>
    /// Begins a new database transaction if one is not already active.
    /// </summary>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            return;
        }

        _transaction = await _context.Database
            .BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Persists all pending changes and commits the current transaction.
    /// Calling SaveChangesAsync separately before this is no longer required —
    /// this method guarantees changes are flushed before commit.
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            throw new InvalidOperationException("No active transaction to commit.");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);   // ➕ এখানে যোগ করা হলো — silent no-op bug আর সম্ভব না
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    /// <summary>
    /// Rolls back the transaction and clears the EF ChangeTracker
    /// to prevent stale entities from being saved later.
    /// </summary>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            return;
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);

            // Clear tracked entities so they do not retry on next SaveChanges
            _context.ChangeTracker.Clear();
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

   
  

    /// <summary>
    /// Safely disposes the transaction object.
    /// </summary>
    private async ValueTask DisposeTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    #region Dispose Pattern

    /// <summary>
    /// Synchronous dispose. Only disposes the transaction, NOT the DbContext.
    /// </summary>
    public void Dispose()
    {
        DisposeCore();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Asynchronous dispose. Only disposes the transaction, NOT the DbContext.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }

    private void DisposeCore()
    {
        if (_disposed)
        {
            return;
        }

        // Dispose only the transaction. 
        // Do NOT dispose _context here because it is managed by DI.
        _transaction?.Dispose();
        _transaction = null;

        _disposed = true;
    }

    private async ValueTask DisposeAsyncCore()
    {
        if (_disposed)
        {
            return;
        }

        await DisposeTransactionAsync();

        _disposed = true;
    }

    #endregion
}

