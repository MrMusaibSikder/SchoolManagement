# CourseHubBackend

Single-institute Course & Training Management backend. .NET 8, ASP.NET Core Web API,
Clean Architecture, PostgreSQL/EF Core 8, dynamic role-based authorization.

CourseHub is **not** multi-tenant. There's exactly one institute; `Institution` exists
only to power the public landing page (name, logo, description, contact info) — it is
not referenced by `User`, `Teacher`, `Student`, `Course`, `Batch`, or `Enrollment`.

## Status

- **Domain**: `BaseEntity`, domain exceptions, enums, and every core entity
  (`Institution`, `User`, `Role`, `Permission`, `UserRole`, `RolePermission`, `Teacher`,
  `Student`, `Course`, `Batch`, `Enrollment`, `RefreshToken`, `PasswordResetToken`).
  None of these except `Institution` itself carry an `InstitutionId`.
- **Infrastructure**: `CourseHubDbContext`, Fluent API configurations, a design-time
  DbContext factory for `dotnet ef`, `AddInfrastructure(...)` DI extension, password
  hashing (ASP.NET Core Identity's `PasswordHasher<TUser>`), JWT issuance with role
  claims, secure token generation/hashing, EF repositories, a development-only email
  sender, and **startup seeding** (`DatabaseSeeder`) that guarantees the default roles
  and the single Institution landing-page row exist.
- **Application**: `IAuthenticationService` (register/login/refresh/logout/
  change-password/forgot-reset-password/current-user), a public institution-profile
  service, FluentValidation validators, DTOs, and every Application-layer abstraction.
- **API**: `AuthController` (all auth endpoints) and `PublicController` (landing page),
  JWT Bearer authentication wired into the pipeline, Swagger configured for
  Bearer-token testing, and an interim domain-exception-to-HTTP-status filter
  (superseded later by full global exception handling).

Not yet implemented: permission-based authorization enforcement on non-auth
controllers, full global exception handling, remaining domain controllers/DTOs
(Course/Batch/Enrollment CRUD), full Swagger docs, Docker.

## Roles & bootstrapping

Four system roles are seeded automatically on first startup: `SuperAdmin`, `Admin`,
`Teacher`, `Student` (protected — `IsSystemRole = true`; more can be created dynamically
later through role management). Nobody can self-register as `Admin` or `SuperAdmin`
through the public registration form:

- **`POST /api/auth/register`** defaults new accounts to **`Student`**. Passing
  `"requestedRole": "Teacher"` assigns `Teacher` instead. `"Admin"` is rejected by
  validation — it's not self-selectable.
- **SuperAdmin bootstrap**: pass the correct `superAdminCode` in the register request
  and the account gets `SuperAdmin` instead, regardless of `requestedRole`. The code is
  compared against `Seed:SuperAdminInviteCode`, which must be set via User Secrets/
  environment variable — it is **never** stored in `appsettings.json` and is empty by
  default (meaning nobody can bootstrap a SuperAdmin until you explicitly set it).

## Authentication endpoints

| Method | Route | Auth required |
|---|---|---|
| POST | `/api/auth/register` | No |
| POST | `/api/auth/login` | No |
| POST | `/api/auth/refresh` | No |
| POST | `/api/auth/logout` | Yes |
| POST | `/api/auth/change-password` | Yes |
| POST | `/api/auth/forgot-password` | No |
| POST | `/api/auth/reset-password` | No |
| GET | `/api/auth/me` | Yes |
| GET | `/api/public/institution` | No |

`register`/`login`/`forgot-password` no longer take an `institutionId` — email is
globally unique.

## Local setup

1. Copy `src/CourseHub.API/appsettings.Development.json.example` to
   `src/CourseHub.API/appsettings.Development.json` and fill in your real local
   PostgreSQL password. This file is git-ignored and must never be committed with
   a real password.

2. Set secrets via User Secrets (never in appsettings.*.json):
   ```
   cd src/CourseHub.API
   dotnet user-secrets set "Authentication:Jwt:SecretKey" "<a random string, 32+ characters>"
   dotnet user-secrets set "Seed:SuperAdminInviteCode" "<a random string only you know>"
   ```
   The app throws a clear startup error if the JWT secret is missing/too short —
   intentional fail-fast behavior. `SuperAdminInviteCode` can stay empty in dev if you
   don't need a SuperAdmin account yet.

3. Restore and build:
   ```
   dotnet restore CourseHubBackend.sln
   dotnet build CourseHubBackend.sln
   ```

4. Install the EF Core CLI tool if you don't already have it:
   ```
   dotnet tool install --global dotnet-ef --version 8.0.11
   ```

5. **The schema changed since the previous (multi-tenant) version** — the old
   `InitialCreate` migration has been deleted because it no longer matches the model.
   If you already have a local database from before, drop it, then generate a fresh
   migration:
   ```
   dotnet ef migrations add InitialCreate --project src\CourseHub.Infrastructure --startup-project src\CourseHub.API --output-dir Persistence\Migrations
   dotnet ef database update --project src\CourseHub.Infrastructure --startup-project src\CourseHub.API
   ```

6. Run the app — startup seeding creates the four system roles and the single
   Institution row automatically:
   ```
   dotnet run --project src/CourseHub.API
   ```

7. Run the tests:
   ```
   dotnet test CourseHubBackend.sln
   ```
   Integration tests hit the real configured PostgreSQL database end-to-end — they
   need steps 1–2 and a running app-seeded database first.

## Security

- `appsettings.json` never contains a real secret — only placeholders/non-secret
  defaults (the seeded institution name/slug/description are public information, so
  those are fine to keep there).
- `appsettings.Development.json` is git-ignored; use the `.example` file as a template.
- JWT signing key and the SuperAdmin invite code live only in User Secrets (dev) or
  environment variables/secret manager (production) — never in any committed file.
- Passwords are hashed with ASP.NET Core Identity's `PasswordHasher<TUser>` (PBKDF2,
  salted). Refresh tokens and password-reset tokens are stored only as SHA-256 hashes.
- Login and forgot-password responses are intentionally generic to avoid account
  enumeration. Register's duplicate-email response is not generic (the caller is
  actively creating that account, so confirming it's taken isn't an enumeration risk).
- Never commit real database passwords, JWT signing keys, invite codes, or cloud
  storage credentials.
