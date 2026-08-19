# CourseHubBackend

Single-institute Course & Training Management backend. .NET 8, ASP.NET Core Web API,
Clean Architecture, PostgreSQL/EF Core 8, dynamic role- and permission-based
authorization.

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
  **and permission** claims, secure token generation/hashing, EF repositories, a
  development-only email sender, and **startup seeding** (`DatabaseSeeder`) that
  guarantees the default roles, the global permission catalog, SuperAdmin's
  auto-granted-every-permission links, the other roles' default permission
  assignments, and the single Institution landing-page row all exist.
- **Application**: `IAuthenticationService` (register/login/refresh/logout/
  change-password/forgot-reset-password/current-user — now including effective
  permissions), `IPublicInstitutionService` (landing-page profile), `IPublicCatalogService`
  (public teachers/courses/stats), `IRolePermissionService` (admin role↔permission
  management), `ICourseService` (admin Courses CRUD), `ITeacherService` (admin Teachers
  CRUD), `IStudentService` (admin Students CRUD), `IBatchService` (admin Batches CRUD),
  FluentValidation validators, DTOs, and every Application-layer abstraction.
- **API**: `AuthController`, `PublicController`, `RolePermissionsController`,
  `CoursesController`, `TeachersController`, `StudentsController`, `BatchesController`;
  JWT Bearer authentication + **permission-based authorization** wired into the
  pipeline (see below); a global `IExceptionHandler` producing consistent
  `ProblemDetails` responses for every unhandled exception (replaces the earlier
  interim filter); Swagger configured for Bearer-token testing.

Not yet implemented: Enrollments CRUD, Users management CRUD, full Swagger
request/response examples, Docker.

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

## Authorization & permissions

Every user's JWT carries two kinds of claims: `role` (from `ClaimTypes.Role`) and a
custom `permission` claim per effective permission. Permissions are **resolved from
roles and baked into the token at login/register/refresh time** — not looked up from
the database on every request. Trade-off: a role's permission change takes effect on
the user's *next* login/refresh, not instantly. Accepted, since access tokens are
short-lived and refresh is cheap.

**SuperAdmin** is auto-granted every permission in the catalog by `DatabaseSeeder` on
every startup — real `RolePermission` rows, not a hardcoded list, so it never drifts
out of sync as new permissions are added. `PermissionAuthorizationHandler` also keeps
an `IsInRole(SuperAdmin)` bypass as a cheap safety net for the narrow window between a
brand-new permission being created and the next seeder run.

Protecting a new endpoint requires **no registration anywhere else** — a custom
`IAuthorizationPolicyProvider` (`PermissionPolicyProvider`) turns any permission-shaped
policy name into a requirement automatically:

```csharp
[HasPermission("courses.create")]
public async Task<IActionResult> CreateCourse(...) { ... }
```

To add a new permission: add one line to `SeedOptions.DefaultPermissions` (and
optionally `DefaultRolePermissions` for non-SuperAdmin roles) — nothing else to wire up.

**Global permission catalog so far:**

| Permission | Resource.Action | Default roles |
|---|---|---|
| `roles.manage` | Role.Manage | SuperAdmin, Admin |
| `roles.view` | Role.View | SuperAdmin, Admin |
| `permissions.view` | Permission.View | SuperAdmin, Admin |
| `courses.view` | Course.View | SuperAdmin, Admin, Teacher |
| `courses.create` | Course.Create | SuperAdmin, Admin |
| `courses.update` | Course.Update | SuperAdmin, Admin |
| `courses.delete` | Course.Delete | SuperAdmin, Admin |
| `teachers.view` | Teacher.View | SuperAdmin, Admin |
| `teachers.create` | Teacher.Create | SuperAdmin, Admin |
| `teachers.update` | Teacher.Update | SuperAdmin, Admin |
| `teachers.delete` | Teacher.Delete | SuperAdmin, Admin |
| `students.view` | Student.View | SuperAdmin, Admin |
| `students.create` | Student.Create | SuperAdmin, Admin |
| `students.update` | Student.Update | SuperAdmin, Admin |
| `students.delete` | Student.Delete | SuperAdmin, Admin |
| `batches.view` | Batch.View | SuperAdmin, Admin, Teacher |
| `batches.create` | Batch.Create | SuperAdmin, Admin |
| `batches.update` | Batch.Update | SuperAdmin, Admin |
| `batches.delete` | Batch.Delete | SuperAdmin, Admin |

## Error handling

Every unhandled exception (anywhere — controllers, Application, Infrastructure) is
caught once by `GlobalExceptionHandler` (.NET 8's `IExceptionHandler`) and converted to
an RFC 7807 `ProblemDetails` JSON response:

```json
{
  "status": 404,
  "title": "Not Found",
  "detail": "Course with key '...' was not found.",
  "instance": "/api/admin/courses/...",
  "traceId": "0HN...."
}
```

| Exception | Status |
|---|---|
| `AuthenticationException` | 401 |
| `UnauthorizedAccessException` | 401 |
| `NotFoundException` | 404 |
| `CourseHub.Domain.Exceptions.ValidationException` | 400 |
| `FluentValidation.ValidationException` | 400 |
| `DomainException` (base, catch-all) | 400 |
| anything else | 500 — generic message only; the real exception is always logged server-side, never leaked to the client |

`Development` environment only: `exceptionType`/`stackTrace` are also included, for
debugging. Requests to routes matching no controller also get a `ProblemDetails` 404
body (via `UseStatusCodePages()`), not an empty response.

## Endpoints

### Auth (`/api/auth`) — public unless noted

| Method | Route | Auth required |
|---|---|---|
| POST | `/register` | No |
| POST | `/login` | No |
| POST | `/refresh` | No |
| POST | `/logout` | Yes |
| POST | `/change-password` | Yes |
| POST | `/forgot-password` | No |
| POST | `/reset-password` | No |
| GET | `/me` | Yes |

`register`/`login`/`forgot-password` no longer take an `institutionId` — email is
globally unique.

### Public catalog (`/api/public`) — always unauthenticated

| Method | Route | Returns |
|---|---|---|
| GET | `/institution` | Landing-page profile (name, logo, description, contact) |
| GET | `/teachers` | Active teachers who opted into public display — `ProfileImageUrl` returned as stored in the DB, no phone/email |
| GET | `/courses` | Active + public courses — `ThumbnailUrl` returned as stored in the DB |
| GET | `/stats` | Aggregate counts only (`TotalTeachers`, `TotalStudents`, `TotalCourses`, `TotalActiveBatches`, `TotalEnrollments`) — never identifies individuals; feeds a landing-page stats section/graph |

### Admin — Roles & Permissions (`/api/admin`)

| Method | Route | Permission |
|---|---|---|
| GET | `/permissions` | `permissions.view` |
| GET | `/roles/{roleId}/permissions` | `roles.view` |
| POST | `/roles/{roleId}/permissions` | `roles.manage` |
| DELETE | `/roles/{roleId}/permissions/{permissionName}` | `roles.manage` |

### Admin — Courses (`/api/admin/courses`)

| Method | Route | Permission |
|---|---|---|
| GET | `/` (`?search=&page=&pageSize=`) | `courses.view` |
| GET | `/{id}` | `courses.view` |
| POST | `/` | `courses.create` |
| PUT | `/{id}` | `courses.update` |
| PUT | `/{id}/thumbnail` | `courses.update` |
| POST | `/{id}/activate` \| `/deactivate` | `courses.update` |
| POST | `/{id}/publish` \| `/unpublish` | `courses.update` |
| DELETE | `/{id}` | `courses.delete` |

`DELETE` is a **soft delete** (calls `Course.Deactivate()`) — it never removes the row.
`Batch.CourseId` has `DeleteBehavior.Restrict` against `Course`, so a hard delete on a
course with any batches would fail with a raw FK-constraint error; soft-deleting also
keeps every batch's/enrollment's history intact and simply hides the course from the
public catalog (which filters on `IsActive`).

`Course.Code` is unique — `POST`/`PUT` reject a duplicate code with a 400
`ValidationException` before ever hitting the database's unique index.

### Admin — Teachers (`/api/admin/teachers`)

| Method | Route | Permission |
|---|---|---|
| GET | `/` (`?search=&page=&pageSize=`) | `teachers.view` |
| GET | `/{id}` | `teachers.view` |
| POST | `/` | `teachers.create` |
| PUT | `/{id}/profile` | `teachers.update` |
| PUT | `/{id}/contact` | `teachers.update` |
| PUT | `/{id}/profile-image` | `teachers.update` |
| POST | `/{id}/activate` \| `/deactivate` | `teachers.update` |
| POST | `/{id}/publish-profile` \| `/unpublish-profile` | `teachers.update` |
| DELETE | `/{id}` | `teachers.delete` |

A Teacher profile always **promotes an existing User** — `POST` takes a `userId`, not a
new email/password. The service enforces, in order: the user must exist, the user must
already hold the `Teacher` role (assign it first via registration's `requestedRole` or
future role management), the user must not already have a teacher profile
(`Teacher.UserId` is unique), and `employeeId` must be unique. Each check throws a
friendly 400/404 instead of letting the DB's unique indexes reject it as a raw
constraint violation.

`DELETE` is a soft delete (`Teacher.Deactivate()`), for the same "predictable contract
across every admin resource" reasoning as Courses — even though no FK currently
references `Teacher.Id` (batch-to-teacher assignment is intentionally deferred, per the
comment on the `Batch` entity), employee history and the public-profile audit trail
still matter.

### Admin — Students (`/api/admin/students`)

| Method | Route | Permission |
|---|---|---|
| GET | `/` (`?search=&page=&pageSize=`) | `students.view` |
| GET | `/{id}` | `students.view` |
| POST | `/` | `students.create` |
| PUT | `/{id}/profile` | `students.update` |
| PUT | `/{id}/contact` | `students.update` |
| PUT | `/{id}/guardian` | `students.update` |
| PUT | `/{id}/profile-image` | `students.update` |
| POST | `/{id}/activate` \| `/deactivate` | `students.update` |
| POST | `/{id}/publish-profile` \| `/unpublish-profile` | `students.update` |
| DELETE | `/{id}` | `students.delete` |

Same "promote an existing User" pattern as Teachers (`POST` takes a `userId`, checks
the user holds the `Student` role, rejects a second profile for the same user, and
enforces `studentId` uniqueness before ever hitting the DB's unique indexes). There is
**no public students listing anywhere** — `publish-profile`/`unpublish-profile` toggle
`Student.IsProfilePublic` for future use, but nothing currently reads it publicly (see
Phase 11's privacy design in `PublicCatalogService`).

`DELETE` is a soft delete and, unlike Teacher's version of the same decision, is backed
by a **real FK today**: `Enrollment.StudentId` has `DeleteBehavior.Restrict` against
`Student` (see `EnrollmentConfiguration`) specifically so enrollment history survives —
a hard delete on any enrolled student would fail with a raw FK-constraint error.

### Admin — Batches (`/api/admin/batches`)

| Method | Route | Permission |
|---|---|---|
| GET | `/` (`?search=&courseId=&page=&pageSize=`) | `batches.view` |
| GET | `/{id}` | `batches.view` |
| POST | `/` | `batches.create` |
| PUT | `/{id}` | `batches.update` (Name, Code) |
| PUT | `/{id}/schedule` | `batches.update` (StartDate, EndDate) |
| PUT | `/{id}/capacity` | `batches.update` |
| POST | `/{id}/activate` \| `/deactivate` | `batches.update` |
| DELETE | `/{id}` | `batches.delete` |

`POST` requires an existing **and active** `courseId` — creating a batch under a
soft-deleted course is rejected with a 400, since that would silently schedule a new
cohort under something the admin already retired. `Batch.Code` is unique, checked the
same way as `Course.Code`. `PUT .../schedule` delegates the `EndDate >= StartDate` rule
to the domain (`Batch.SetSchedule`) rather than duplicating it in the request
validator — one source of truth.

`DELETE` is a soft delete, backed by a real FK just like Student:
`Enrollment.BatchId` also has `DeleteBehavior.Restrict` against `Batch` (see
`EnrollmentConfiguration`) — a hard delete on a batch with any enrollments would fail
with a raw FK-constraint error.

Teacher-to-batch assignment is **intentionally not modeled yet** (see the comment on
the `Batch` entity) — a future phase will add it as a separate relationship, since a
batch may eventually have multiple instructors.

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

5. Apply migrations (schema already includes `Permission`/`RolePermission` — no new
   migration was needed for Phases 9–12, only seed data + application code):
   ```
   dotnet ef database update --project src\CourseHub.Infrastructure --startup-project src\CourseHub.API
   ```
   If you're setting this up fresh and there's no `InitialCreate` migration yet:
   ```
   dotnet ef migrations add InitialCreate --project src\CourseHub.Infrastructure --startup-project src\CourseHub.API --output-dir Persistence\Migrations
   dotnet ef database update --project src\CourseHub.Infrastructure --startup-project src\CourseHub.API
   ```

6. Run the app — startup seeding creates the four system roles, the global permission
   catalog, SuperAdmin's full permission set, Admin's/Teacher's default permissions, and
   the single Institution row automatically:
   ```
   dotnet run --project src/CourseHub.API
   ```

7. Run the tests:
   ```
   dotnet test CourseHubBackend.sln
   ```
   Integration tests hit the real configured PostgreSQL database end-to-end — they
   need steps 1–2 and a running app-seeded database first.

## How to smoke-test permissions & Courses CRUD manually

1. Register with the correct `superAdminCode` → decode the returned JWT (e.g.
   jwt.io) → confirm `permission` claims are present for every seeded permission.
2. `GET /api/admin/permissions` with that token → 200, lists the full catalog.
3. `GET /api/admin/roles/{superAdminRoleId}/permissions` → every permission is a real
   assigned row, not just a runtime bypass.
4. Register a second account with `"requestedRole": "Teacher"` → its token should only
   carry the `courses.view` permission claim → `GET /api/admin/courses` succeeds,
   `POST /api/admin/courses` returns 403.
5. As SuperAdmin/Admin: `POST /api/admin/courses` with a unique `code` → 201. Retry
   with the same `code` → 400 `ValidationException` ("A course with code '...' already
   exists."). `GET /api/admin/courses/{id}` → 200. `PUT .../publish` then
   `GET /api/public/courses` (no auth) → the course now appears. `DELETE /api/admin/courses/{id}`
   → 204, then `GET /api/admin/courses/{id}` still returns the course (now
   `IsActive: false`) — confirms it's a soft delete, not a row removal.
6. Register a third account with `"requestedRole": "Teacher"` → note its returned
   `id` (this is the `userId` to promote). As SuperAdmin/Admin:
   `POST /api/admin/teachers` with that `userId` + a unique `employeeId` → 201. Retry
   with the same `userId` → 400 ("This user already has a teacher profile."). Try
   promoting a `Student`-role user → 400 ("...does not have the Teacher role.").
   `POST /api/admin/teachers/{id}/publish-profile` then `GET /api/public/teachers`
   (no auth) → the teacher now appears with their `profileImageUrl` field (null until
   `PUT .../profile-image` is called), but never their phone/email.
7. Register a fourth account with `"requestedRole": "Teacher"` is not needed — a
   default `POST /api/auth/register` already defaults to `Student`. Note its `id`. As
   SuperAdmin/Admin: `POST /api/admin/students` with that `userId` + a unique
   `studentId` → 201. `GET /api/admin/students/{id}` → 200. Confirm
   `GET /api/public/*` has no students endpoint at all — students are never listed
   publicly. `DELETE /api/admin/students/{id}` → 204, then `GET /api/admin/students/{id}`
   still returns the student (now `isActive: false`) — soft delete, not a row removal.
8. As SuperAdmin/Admin: `POST /api/admin/batches` with a real `courseId` (from step 5)
   and a unique `code` → 201. Try again with a `courseId` belonging to a course you
   deactivated → 400 ("...is not active — reactivate it before adding new batches.").
   `PUT /api/admin/batches/{id}/schedule` with `endDate` earlier than `startDate` → 400
   from the domain. `GET /api/admin/batches?courseId={courseId}` → only that course's
   batches. `DELETE /api/admin/batches/{id}` → 204, then `GET /api/admin/batches/{id}`
   still returns it (now `isActive: false`) — soft delete.

## Security

- `appsettings.json` never contains a real secret — only placeholders/non-secret
  defaults (the seeded institution name/slug/description and permission catalog are
  public information, so those are fine to keep there).
- `appsettings.Development.json` is git-ignored; use the `.example` file as a template.
- JWT signing key and the SuperAdmin invite code live only in User Secrets (dev) or
  environment variables/secret manager (production) — never in any committed file.
- Passwords are hashed with ASP.NET Core Identity's `PasswordHasher<TUser>` (PBKDF2,
  salted). Refresh tokens and password-reset tokens are stored only as SHA-256 hashes.
- Login and forgot-password responses are intentionally generic to avoid account
  enumeration. Register's duplicate-email response is not generic (the caller is
  actively creating that account, so confirming it's taken isn't an enumeration risk).
- 500 responses never leak exception details/stack traces outside `Development` — see
  "Error handling" above.
- Never commit real database passwords, JWT signing keys, invite codes, or cloud
  storage credentials.
