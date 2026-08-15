# CourseHubBackend

Multi-tenant Course & Training Management SaaS backend. .NET 8, ASP.NET Core Web API,
Clean Architecture, PostgreSQL/EF Core 8, dynamic role/permission authorization.

## Status

Implemented so far:
- **Domain**: BaseEntity, domain exceptions, enums (`UserStatus`, `EnrollmentStatus`),
  and all 11 core entities (`Institution`, `User`, `Role`, `Permission`, `UserRole`,
  `RolePermission`, `Teacher`, `Student`, `Course`, `Batch`, `Enrollment`).
- **Infrastructure**: `CourseHubDbContext`, Fluent API configurations for every entity,
  a design-time DbContext factory for `dotnet ef`, and an `AddInfrastructure(...)`
  DI extension wired into `CourseHub.API`'s `Program.cs`.

Not yet implemented: EF Core migrations, authentication (password hashing/JWT),
permission-based authorization enforcement, DTOs/controllers, tests, Docker.
(EF migrations specifically need to be generated on a machine with NuGet access —
see below.)

## Local setup

1. Copy `src/CourseHub.API/appsettings.Development.json.example` to
   `src/CourseHub.API/appsettings.Development.json` and fill in your real local
   PostgreSQL password. This file is git-ignored and must never be committed with
   a real password.

2. Restore and build:
   ```
   dotnet restore CourseHubBackend.sln
   dotnet build CourseHubBackend.sln
   ```

3. Install the EF Core CLI tool if you don't already have it (matching the EF Core
   8.0.19 packages used by this solution):
   ```
   dotnet tool install --global dotnet-ef --version 8.0.19
   ```

4. Create the initial migration (Windows CMD):
   ```
   dotnet ef migrations add InitialCreate --project src\CourseHub.Infrastructure --startup-project src\CourseHub.API --output-dir Persistence\Migrations
   ```

5. Apply it to your local database:
   ```
   dotnet ef database update --project src\CourseHub.Infrastructure --startup-project src\CourseHub.API
   ```

6. Run the tests:
   ```
   dotnet test CourseHubBackend.sln
   ```

## Security

- `appsettings.json` never contains a real secret — only a placeholder connection string.
- `appsettings.Development.json` is git-ignored; use the `.example` file as a template.
- Never commit real database passwords, JWT signing keys, or cloud storage credentials.
