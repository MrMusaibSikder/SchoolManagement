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
  management), `ICourseService`, `ITeacherService`, `IStudentService`, `IBatchService`,
  `IEnrollmentService` (admin CRUD for each — **Phase 12 is now complete**),
  FluentValidation validators, DTOs, and every Application-layer abstraction.
- **API**: `AuthController`, `PublicController`, `RolePermissionsController`,
  `CoursesController`, `TeachersController`, `StudentsController`, `BatchesController`,
  `EnrollmentsController`; JWT Bearer authentication + **permission-based
  authorization** wired into the pipeline (see below); a global `IExceptionHandler`
  producing consistent `ProblemDetails` responses for every unhandled exception
  (replaces the earlier interim filter); `/health/live` and `/health/ready` endpoints;
  automatic EF Core migration + seeding on startup; Swagger configured for
  Bearer-token testing.
- **Docker**: multi-stage `Dockerfile` (SDK build stage → slim ASP.NET Core runtime
  stage, non-root user, container `HEALTHCHECK`), `docker-compose.yml` (Postgres + API,
  `depends_on: condition: service_healthy`, fail-fast on missing secrets), `.env.example`.

Not yet implemented: Users management CRUD (promoting/demoting roles, deactivating
accounts directly), full Swagger request/response examples, a real `IEmailSender`
(needed before the forgot-password flow works outside Development).

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
| `enrollments.view` | Enrollment.View | SuperAdmin, Admin, Teacher |
| `enrollments.create` | Enrollment.Create | SuperAdmin, Admin |
| `enrollments.update` | Enrollment.Update | SuperAdmin, Admin, Teacher |
| `enrollments.delete` | Enrollment.Delete | SuperAdmin, Admin |

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

### Admin — Enrollments (`/api/admin/enrollments`)

| Method | Route | Permission |
|---|---|---|
| GET | `/` (`?studentId=&batchId=&status=&page=&pageSize=`) | `enrollments.view` |
| GET | `/{id}` | `enrollments.view` |
| POST | `/` | `enrollments.create` |
| POST | `/{id}/approve` (Pending → Active) | `enrollments.update` |
| POST | `/{id}/complete` (Active → Completed) | `enrollments.update` |
| POST | `/{id}/cancel` (Pending/Active → Cancelled) | `enrollments.update` |
| DELETE | `/{id}` | `enrollments.delete` |

This is the last link in Phase 12's dependency chain — `Enrollment` connects an
existing `Student` to an existing `Batch`. `POST` enforces, in order: the student must
exist and be active, the batch must exist and be active, the `(StudentId, BatchId)`
pair must not already exist (unique DB index — a student can't enroll in the same batch
twice), and if the batch has a `Capacity`, the batch must have a free seat (Pending +
Active enrollments count against the limit; Cancelled/Completed don't).

`Enrollment` has **no `IsActive`/soft-delete flag of its own** — its lifecycle is the
`Pending → Active → Completed` state machine plus `Cancel`, enforced entirely in the
domain (`Enrollment.Approve/Complete/Cancel`, each throwing a domain exception, mapped
to 400, for an invalid transition). `DELETE` maps to the same `Cancel()` call as
`POST /{id}/cancel` — kept as a separate route/permission only for REST consistency
with every other admin controller, not because it does anything different.

## Local setup

Prefer Docker? Skip straight to the [Docker](#docker) section below — it doesn't
need the .NET SDK, EF Core CLI, or a local Postgres install at all.

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

4. Make sure a PostgreSQL server is running and reachable at whatever
   `ConnectionStrings:DefaultConnection` in your `appsettings.Development.json` points
   to. You don't need to create the database or tables yourself — step 5 below
   creates the schema automatically.

5. Run the app:
   ```
   dotnet run --project src/CourseHub.API
   ```
   On startup the app **automatically applies any pending EF Core migrations**
   (`dbContext.Database.MigrateAsync()` in `Program.cs`) and then runs the idempotent
   seeder — the four system roles, the global permission catalog, SuperAdmin's full
   permission set, Admin's/Teacher's default permissions, and the single Institution
   row all exist after this, with no separate `dotnet ef database update` step
   required. (If you need the EF Core CLI anyway — e.g. to author a *new* migration
   after changing an entity — install it with
   `dotnet tool install --global dotnet-ef --version 8.0.11` and use
   `dotnet ef migrations add <Name> --project src\CourseHub.Infrastructure --startup-project src\CourseHub.API`.)

6. Run the tests:
   ```
   dotnet test CourseHubBackend.sln
   ```
   Integration tests hit the real configured PostgreSQL database end-to-end — they
   need steps 1–2 and a running app-seeded database first.

## Docker

Runs the whole stack — Postgres + the API, correctly wired together — in containers.
You only need [Docker Desktop](https://www.docker.com/products/docker-desktop/) (Mac/
Windows) or Docker Engine + the Compose plugin (Linux). No .NET SDK, no EF Core CLI, no
local Postgres install.

### What's in the repo

| File | Purpose |
|---|---|
| `Dockerfile` | Multi-stage build: SDK image compiles/publishes the API, then the much smaller ASP.NET Core *runtime* image (no compiler) is what actually ships. Runs as a non-root user; has a container `HEALTHCHECK` hitting `/health/live`. |
| `docker-compose.yml` | Defines two services — `postgres` and `api` — networked together, with a named volume so your data survives container restarts. |
| `.env.example` | Template for the secrets Compose needs (DB password, JWT signing key, SuperAdmin invite code). Copy it to `.env` and fill in real values — `.env` itself is git-ignored. |

### Run it

1. From the repo root (same folder as `docker-compose.yml`), copy the env template:
   ```
   cp .env.example .env
   ```
2. Open `.env` and set real values for `POSTGRES_PASSWORD` and `JWT_SECRET_KEY` (a
   comment in the file shows how to generate a good JWT key with `openssl rand -base64
   48`). Leave `SUPERADMIN_INVITE_CODE` empty if you don't need a SuperAdmin account
   yet — you can set it and restart later. Compose will **refuse to start** with a
   clear error message if you forget `POSTGRES_PASSWORD` or `JWT_SECRET_KEY` — this is
   intentional (same fail-fast philosophy as the JWT secret check in local dev).
3. Build and start everything:
   ```
   docker compose up --build
   ```
   What happens, in order: Compose builds the API image from the `Dockerfile`, starts
   a `postgres:16-alpine` container, waits until Postgres's own healthcheck reports
   healthy, *then* starts the `api` container (this ordering is `depends_on: condition:
   service_healthy` in `docker-compose.yml` — without it the API could try to connect
   before Postgres is accepting connections yet). The API then applies EF Core
   migrations and seeds default data automatically, exactly like local `dotnet run`
   (see "Local setup" step 5 above) — nothing extra to run by hand.
4. Once you see the API's log settle (or `docker compose ps` shows both services
   `healthy`), it's listening on **http://localhost:8080**. Try:
   ```
   curl http://localhost:8080/health/live
   curl http://localhost:8080/health/ready
   curl http://localhost:8080/api/public/institution
   ```

### Everyday commands

| Command | What it does |
|---|---|
| `docker compose up -d` | Start in the background (no attached logs). |
| `docker compose logs -f api` | Follow the API's logs. |
| `docker compose down` | Stop and remove the containers. Your database **data survives** (it's in the `postgres-data` named volume). |
| `docker compose down -v` | Stop and remove containers **and** the volume — wipes the database completely. Use this if you want a totally fresh start. |
| `docker compose up --build` | Rebuild the API image after you change code, then start. |
| `docker compose exec postgres psql -U coursehub -d CourseHubDb` | Open a `psql` shell inside the running Postgres container (adjust the username/db if you changed them in `.env`). |

### Notes

- **Swagger is off by default.** `ASPNETCORE_ENVIRONMENT` defaults to `Production` in
  `.env.example` (matching how the app behaves in local `Production` too — see
  `Program.cs`: `if (app.Environment.IsDevelopment()) { app.UseSwagger(); ... }`). Set
  `ASPNETCORE_ENVIRONMENT=Development` in your `.env` and re-run `docker compose up
  --build` if you want `/swagger` available for manual testing against the
  containerized API.
- **Password-reset emails won't actually send.** No real `IEmailSender` is wired up
  yet (see the Status section above) — outside `Development`,
  `NotConfiguredEmailSender` throws loudly rather than silently pretending to work.
  Everything else (register/login/CRUD/etc.) works fully.
- **This Compose setup is meant for local use and simple single-host deployments**,
  not a production cloud architecture. Before using it as a real production
  deployment: put a reverse proxy (nginx, Traefik, a cloud load balancer, etc.) in
  front of the `api` container to terminate TLS — the container itself only serves
  plain HTTP on port 8080; remove the `postgres` service's `ports:` mapping so the
  database isn't reachable from outside the Docker network at all; and source
  `POSTGRES_PASSWORD`/`JWT_SECRET_KEY`/`SUPERADMIN_INVITE_CODE` from your cloud
  provider's real secret manager instead of a `.env` file sitting on a server's disk.

## How to smoke-test permissions & Phase 12 CRUD manually

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
9. As SuperAdmin/Admin: `POST /api/admin/enrollments` with the `studentId` (from step
   7) and `batchId` (from step 8, use a fresh non-deactivated batch) → 201, `status:
   "Pending"`. Retry with the same pair → 400 ("already enrolled in this batch.").
   `POST /api/admin/enrollments/{id}/approve` → `status: "Active"`. Try
   `POST .../approve` again → 400 from the domain ("Only 'Pending' enrollments can be
   approved."). `POST .../complete` → `status: "Completed"`. Create a batch with
   `"capacity": 1`, enroll one student (Pending/Active counts against it), then try
   enrolling a second student in the same batch → 400 ("at full capacity").
   `DELETE /api/admin/enrollments/{id}` on a still-open enrollment → 200,
   `status: "Cancelled"` — confirms DELETE maps to Cancel, not a row removal.

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
