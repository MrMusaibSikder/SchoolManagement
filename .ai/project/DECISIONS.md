# Decisions (Root)

Cross-cutting decisions for the SchoolERP monorepo.

## 1. Monorepo Documentation

- **Root `.ai/`** — project-wide context, module order, overall progress.
- **`frontend/.ai/`** — detailed frontend standards, UI rules, API integration.
- Keep both in sync when module status changes.

## 2. Backend Is Source of Truth

Never invent DTO shapes. Read controllers and DTOs under `backend/src/`.

## 3. API Base URL

Frontend axios clients use `baseURL: '/api'`. Endpoint paths must be relative (`/Students`, not `/api/Students`).

## 4. No AcademicSessions Backend

There is no `AcademicSessionsController`. Academic periods are managed via `AcademicYearsController` only.

## 5. Paged List Endpoints

Several list endpoints return `PagedResult<T>` with `{ items, totalCount, pageNumber, pageSize }`. Frontend must unwrap `.items`.

## 6. Dashboard Data Strategy

- Counts: prefer `GET /api/public/stats` (lightweight, no full list fetch).
- Fees: `GET /api/Invoices?pageSize=5` (paged).
- Attendance: `GET /api/AttendanceReport/admin-dashboard`.
- Exams: `GET /api/Exam/upcoming?count=5`.
- Notices: `GET /api/Notice/recent?count=5`.

## 7. Permission-Based UI

Resolve permissions from `GET /api/CurrentUser/profile` — gate UI by `permissions[]`, not `roles[]`. (Follow-up task.)
