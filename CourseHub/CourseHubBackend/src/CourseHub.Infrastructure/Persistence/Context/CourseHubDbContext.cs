using CourseHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CourseHub.Infrastructure.Persistence.Context;

public class CourseHubDbContext : DbContext
{
    public CourseHubDbContext(DbContextOptions<CourseHubDbContext> options)
        : base(options)
    {
    }

    public DbSet<Institution> Institutions => Set<Institution>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    public DbSet<Teacher> Teachers => Set<Teacher>();

    public DbSet<Student> Students => Set<Student>();

    public DbSet<Course> Courses => Set<Course>();

    public DbSet<Batch> Batches => Set<Batch>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CourseHubDbContext).Assembly);
    }
}
