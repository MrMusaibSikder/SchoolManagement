using Microsoft.EntityFrameworkCore;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Persistence.Context;

public class SchoolERPDbContext : DbContext
{
    public SchoolERPDbContext(DbContextOptions<SchoolERPDbContext> options)
        : base(options)
    {
    }

    // DbSets

    // Security
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    // Employee
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Designation> Designations => Set<Designation>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<EmployeeSalary> EmployeeSalaries => Set<EmployeeSalary>();
    public DbSet<EmployeeAttendance> EmployeeAttendances => Set<EmployeeAttendance>();

    // Academic
    public DbSet<School> Schools => Set<School>();
    public DbSet<SchoolClass> SchoolClasses => Set<SchoolClass>();
    public DbSet<Section> Sections => Set<Section>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<ClassSubject> ClassSubjects => Set<ClassSubject>();
    public DbSet<SubjectTeacher> SubjectTeachers => Set<SubjectTeacher>();

    // Student
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Guardian> Guardians => Set<Guardian>();
    public DbSet<StudentGuardian> StudentGuardians => Set<StudentGuardian>();
    public DbSet<StudentAttendance> StudentAttendances => Set<StudentAttendance>();

    // Exam
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<ExamType> ExamTypes => Set<ExamType>();
    public DbSet<Exam> Exams => Set<Exam>();
    public DbSet<ExamSchedule> ExamSchedules => Set<ExamSchedule>();
    public DbSet<Result> Results => Set<Result>();
    public DbSet<ExamResult> ExamResults => Set<ExamResult>();
    public DbSet<ExamWeightSetup> ExamWeightSetups => Set<ExamWeightSetup>();
    public DbSet<ExamWeightItem> ExamWeightItems => Set<ExamWeightItem>();
    public DbSet<FinalResult> FinalResults => Set<FinalResult>();
    public DbSet<FinalResultDetail> FinalResultDetails => Set<FinalResultDetail>();
    public DbSet<GradeSetup> GradeSetups => Set<GradeSetup>();
    public DbSet<ResultAuditLog> ResultAuditLogs => Set<ResultAuditLog>();

    // Finance
    public DbSet<FeeCategory> FeeCategories { get; set; }
    public DbSet<FeeType> FeeTypes { get; set; }
    public DbSet<FeeStructure> FeeStructures { get; set; }
    public DbSet<FeeStructureItem> FeeStructureItems { get; set; }
    public DbSet<StudentFeeConcession> StudentFeeConcessions { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceItem> InvoiceItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<LateFineRule> LateFineRules { get; set; }


    // Communication
    public DbSet<Notice> Notices => Set<Notice>();
    public DbSet<SmsTemplate> SmsTemplates => Set<SmsTemplate>();
    public DbSet<SmsLog> SmsLogs => Set<SmsLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations automatically
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SchoolERPDbContext).Assembly);
    }
}