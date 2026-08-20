# Backend Structure

## Solution

`backend/SchoolERP.sln`

## Projects

| Project | Purpose |
|---------|---------|
| `SchoolERP.Api` | HTTP layer — controllers, JWT auth, middleware |
| `SchoolERP.Application` | Business contracts — DTOs, service interfaces, validators, AutoMapper profiles |
| `SchoolERP.Domain` | Entities, enums, constants (`PermissionNames`) |
| `SchoolERP.Infrastructure` | EF Core, repositories, service implementations, seeders |

## Controller Route Convention

```csharp
[Route("api/[controller]")]
```

Example: `StudentsController` → `/api/Students`

Special routes:

- `PublicController` → `/api/public/*`
- `FeeReportsController` → `/api/fee-reports/*`

## Finding an API

1. Search `backend/src/SchoolERP.Api/Controllers/`
2. Read action method + DTO in `SchoolERP.Application/Features/`
3. Check `[PermissionAuthorize(PermissionNames.*)]` for required permission

## Dashboard-Related Endpoints

| Widget | Endpoint | Response |
|--------|----------|----------|
| Stats counts | `GET /api/public/stats` | `{ totalStudents, totalTeachers, totalEmployees }` |
| Invoices | `GET /api/Invoices?pageSize=5` | `PagedResult<InvoiceListDto>` |
| Attendance | `GET /api/AttendanceReport/admin-dashboard` | `AdminDashboardAttendanceDto` |
| Upcoming exams | `GET /api/Exam/upcoming?count=5` | `UpcomingExamDto[]` |
| Recent notices | `GET /api/Notice/recent?count=5` | `NoticeDto[]` |

## Seeded Data

See `backend/src/SchoolERP.Infrastructure/Persistence/Seed/` — default admin user and sample school data.
