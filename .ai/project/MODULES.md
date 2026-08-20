# Modules (Root)

Single source of truth for the **whole project**. Frontend build status is tracked in `CURRENT_PROGRESS.md`.

## Phase 0 — Foundation

| Priority | Module | Backend | Frontend |
|----------|--------|---------|----------|
| 0.1 | Authentication | ✅ | ✅ |
| 0.1b | Current User Profile / Permissions | ✅ | ⏳ |
| 0.2 | App Shell (layout, sidebar, guards) | ✅ | ✅ |
| 0.3 | Dashboard | Partial | 🚧 |
| 0.4 | Landing Page (public) | ✅ | ✅ |

## Phase 1 — Master Data

| Priority | Module | Backend | Frontend |
|----------|--------|---------|----------|
| 1.1 | School | ✅ | ⏳ |
| 1.2 | Academic Year | ✅ | ✅ |
| 1.3 | School Class | ✅ | ✅ |
| 1.4 | Section | ✅ | ✅ |
| 1.5 | Subject | ✅ | ✅ |
| 1.6 | Class ↔ Subject | ✅ | ⏳ |
| 1.7 | Designation | ✅ | ⏳ |
| 1.8 | Role & Permission | ✅ | ⏳ |
| 1.9 | User Management | ✅ | ⏳ |

## Phase 2 — People

| Priority | Module | Backend | Frontend |
|----------|--------|---------|----------|
| 2.1 | Guardian | ✅ | 🚧 |
| 2.2 | Student | ✅ | 🚧 |
| 2.3 | Teacher | ✅ | 🚧 (list in academic) |
| 2.4 | Employee | ✅ | ⏳ |
| 2.5 | Subject ↔ Teacher | ✅ | 🚧 |

## Phase 3–7

See `frontend/.ai/project/MODULES.md` for full detail (Attendance, Exam, Result, Fee, Communication, Payroll, Reports, Settings).

## Known Backend ↔ Frontend Notes

- **No `AcademicSessions` API** — backend only has `AcademicYears`. Do not build a separate sessions module.
- **Invoices list** returns `PagedResult` (`items`, `totalCount`), not a bare array.
- **Dashboard attendance** uses `GET /AttendanceReport/admin-dashboard` → `AdminDashboardAttendanceDto`.
- **Public stats** (`GET /api/public/stats`) provides student/teacher/employee counts without auth.

## Rule

Never mark a module complete until every sub-feature is implemented, validated, permission-checked, and responsive.
