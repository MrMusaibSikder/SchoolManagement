# Modules

## Purpose

This file is the single source of truth for which modules exist, what each one
contains, what it depends on, and the order in which the frontend should be
built.

Before starting a new module, always check this file.

Never start a module that depends on an incomplete module.

Never build modules in a different order without explicit approval.

---

# How To Read This File

Each module lists:

- **Sub-features** — the screens/entities inside that module
- **Depends On** — modules that must exist first (shared dropdowns, auth, etc.)
- **Backend Status** — Completed / Partial / Not Started
- **Priority** — the recommended build order (lower number = build first)

Backend Status reflects the ASP.NET Core API only. Frontend status is tracked
separately in `CURRENT_PROGRESS.md`.

---

# Phase 0 — Foundation (build first, everything depends on this)

| Priority | Module | Sub-features | Depends On | Backend Status |
|---|---|---|---|---|
| 0.1 | Authentication | Login, Refresh Token, Logout, Change Password, Forgot/Reset Password | None | Completed |
| 0.1b | Current User Profile | `GET /api/CurrentUser/profile` — returns `{ userId, username, email, isActive, roles[], permissions[] }`. Also `/api/CurrentUser/roles` and `/api/CurrentUser/permissions` as narrower alternatives. **This is the endpoint that resolves the permission-source question** — call it right after login (and on app load, if a valid token exists) and store `permissions[]` in the auth store; gate all UI by these strings, not by `roles[]`. | Authentication | Completed |
| 0.2 | App Shell | Layout, Sidebar, Topbar, Route Guard, Permission Guard, Toast/Notification system, Error Boundary | Authentication | Completed |
| 0.3 | Dashboard (shell only) | Empty widget grid, role-aware layout | App Shell | Partial (see Dashboard Rules) |
| 0.4 | Landing Page (Public Homepage) | Public route `/` — Hero (school name/logo), Stats section (student/teacher/employee count), Public Notices list, Login CTA, Footer with contact info | App Shell (reuse Navbar/Footer/Button styling only — **not** auth) + new `/api/public/*` backend endpoints | Backend: To Be Added — see `API_REFERENCE.md` → Public (Anonymous) Endpoints |

**Note on 0.4 — this module has no functional dependency on Authentication.**
It is a fully public, unauthenticated route rendered for anyone who opens the
app before logging in. It is listed after App Shell only so it can reuse the
same shared components (buttons, cards, layout primitives), not because auth
must work first. It does, however, require three new anonymous backend
endpoints that do not exist yet — do not start this module until those are
confirmed live (see the Public Endpoints section of `API_REFERENCE.md`).

Never call an authenticated endpoint from this page and never attach a JWT
token on its requests — a visitor viewing the Landing Page has not logged in
yet.

---

# Phase 1 — Core Setup / Master Data

| Priority | Module | Sub-features | Depends On | Backend Status |
|---|---|---|---|---|
| 1.1 | School | School profile (single record) | Foundation | Completed |
| 1.2 | Academic Year | List, Create, Edit, Set Current | Foundation | Completed |
| 1.3 | School Class | List, Create, Edit, Delete | Academic Year | Completed |
| 1.4 | Section | List, Create, Edit, Delete (belongs to a Class) | School Class | Completed |
| 1.5 | Subject | List, Create, Edit, Delete | Foundation | Completed |
| 1.6 | Class ↔ Subject Assignment | Assign/Remove subjects per class | School Class, Subject | Completed |
| 1.7 | Designation | List, Create, Edit, Delete (for Employees) | Foundation | Completed |
| 1.8 | Role & Permission | Role CRUD, Permission list, Assign permissions to role | Foundation | Completed |
| 1.9 | User Management | User CRUD, Assign role to user | Role & Permission | Completed |

---

# Phase 2 — People

| Priority | Module | Sub-features | Depends On | Backend Status |
|---|---|---|---|---|
| 2.1 | Guardian | List, Create, Edit, Delete, Search | Foundation | Completed |
| 2.2 | Student | List, Create, Edit, Delete, Search, Photo upload, Link Guardian(s) | School Class, Section, Guardian | Completed |
| 2.3 | Teacher | List, Create, Edit, Delete | Designation, User | Completed |
| 2.4 | Employee | List, Create, Edit, Delete, Photo upload | Designation, User | Completed |
| 2.5 | Subject ↔ Teacher Assignment | Assign/Remove teachers per subject | Subject, Teacher | Completed |

---

# Phase 3 — Attendance

| Priority | Module | Sub-features | Depends On | Backend Status |
|---|---|---|---|---|
| 3.1 | Student Attendance | Mark daily attendance (bulk, per class/section), edit, history | Student, School Class, Section | Completed |
| 3.2 | Employee Attendance | Mark daily attendance, bulk entry, history | Employee | Completed |
| 3.3 | Attendance Report | Read-only aggregate/report views | Student Attendance, Employee Attendance | Completed |

---

# Phase 4 — Examination & Result

Build in this exact order — each step depends on the previous one's data existing.

| Priority | Module | Sub-features | Depends On | Backend Status |
|---|---|---|---|---|
| 4.1 | Exam Type | List, Create, Edit, Delete | Foundation | Completed |
| 4.2 | Grade Setup | Configurable grading bands (A+, A, B, ... / GPA) | Foundation | Completed |
| 4.3 | Exam | List, Create, Edit, Delete, Publish, Complete, Cancel | Exam Type, Academic Year, School Class | Completed |
| 4.4 | Exam Schedule | Subject-wise date/time per exam | Exam, Subject | Completed |
| 4.5 | Marks Entry (Result) | Bulk mark entry per exam schedule, edit | Exam Schedule, Student | Completed |
| 4.6 | Exam Weight Setup | Weight configuration per exam type (for Final Result calculation) | Exam Type | Completed |
| 4.7 | Exam Result | Calculate, Publish, Unlock (aggregate per-exam result) | Marks Entry, Grade Setup | Completed |
| 4.8 | Final Result | Calculate, Publish, Unlock (year-wide weighted result) | Exam Result, Exam Weight Setup | Completed |
| 4.9 | Result Audit Trail | Read-only history of calculation/publish/unlock actions | Exam Result, Final Result | Completed |
| 4.10 | Transcript | Full academic-year or full-history printable transcript | Final Result, Exam Result, Student Attendance | Completed |
| 4.11 | Progress Report | Per-exam progress report / marksheet | Exam Result | Completed |

---

# Phase 5 — Fee Management ⭐ (fully built and end-to-end tested — build this frontend early)

This entire module's backend is complete, unit-tested via manual QA, and
already includes: state-transition endpoints (not generic edit/delete for
financial records), optimistic-concurrency handling (409 responses),
concession approval workflow, recurring invoice generation, automatic late
fine application, PDF receipts, and two report endpoints. See
`API_REFERENCE.md` → Fee Management section for full endpoint detail and
`BUSINESS_RULES.md` → Fee Management Rules for the state machine and
constraints the UI must respect.

| Priority | Module | Sub-features | Depends On | Backend Status |
|---|---|---|---|---|
| 5.1 | Fee Category | List, Create, Edit, Delete | Foundation | Completed |
| 5.2 | Fee Type | List, Create, Edit, Delete (belongs to Fee Category) | Fee Category | Completed |
| 5.3 | Fee Structure | List (filter by year/class/active), Create with items, Edit with item add/update/remove, Delete | Academic Year, School Class, Section, Fee Type | Completed |
| 5.4 | Student Fee Concession | List by student, Pending approvals queue, Create, Edit, Approve, Delete | Student, Fee Type, Academic Year | Completed |
| 5.5 | Invoice | Paged list (filter by status/student/year), Detail, Create (manual), Cancel, Bulk Monthly Generation | Student, Fee Structure, Academic Year | Completed |
| 5.6 | Payment | Detail, List by invoice, Collect (create), Void | Invoice | Completed |
| 5.7 | Receipt | Detail, List by payment, Void, **Download PDF** | Payment | Completed |
| 5.8 | Late Fine Rule | List by academic year, Create, Edit, Delete | Academic Year, Fee Type | Completed |
| 5.9 | Late Fine Auto-Apply | Trigger button (admin-run) — recalculates fines on all overdue invoices | Invoice, Late Fine Rule | Completed |
| 5.10 | Fee Reports | Collection Summary (date range + method breakdown), Defaulter Report (by class) | Invoice, Payment | Completed |

---

# Phase 6 — Communication

| Priority | Module | Sub-features | Depends On | Backend Status |
|---|---|---|---|---|
| 6.1 | Notice | List, Create, Edit, Delete, Publish, Audience targeting | Foundation | Completed |
| 6.2 | SMS Template | List, Create, Edit, Delete, Placeholder preview | Foundation | Completed |
| 6.3 | SMS Log | Read-only list, filter/search | SMS Template | Completed |

---

# Phase 6.5 — Employee Payroll

Added after the initial `MODULES.md` draft — the backend has a working
`EmployeeSalaryController` that wasn't captured in the original phase list.
See `API_REFERENCE_AUTOGENERATED.md` → EmployeeSalaryController for the
full endpoint contract.

| Priority | Module | Sub-features | Depends On | Backend Status |
|---|---|---|---|---|
| 6.5.1 | Employee Salary | List, Create, Edit, Delete | Employee | Completed |

---

# Phase 7 — Dashboard (final pass) & Settings

| Priority | Module | Sub-features | Depends On | Backend Status |
|---|---|---|---|---|
| 7.1 | Dashboard (full) | Role-based widgets pulling from every module above (student count, today's attendance, fee collection today, pending fees, recent notices, upcoming exams) | All modules above | Partial — dedicated dashboard aggregate endpoints not yet confirmed; verify with backend before wiring widgets |
| 7.2 | Settings | School profile edit, general app settings | School | Partial |

---

# Cross-Cutting (not a module, needed by all of the above)

- Searchable dropdown / combobox component for every foreign key (Student, Guardian, FeeType, AcademicYear, SchoolClass, Section, Employee, etc.)
- Reusable paginated table component
- Reusable form + Zod schema pattern per DTO
- Global 401 → redirect to login, 403 → access-denied page, 409 → "someone else updated this, refresh and retry" toast (this exact case happens on Invoice cancel and Payment collect — see DECISIONS.md #3)
- File/Image upload component (Student photo, Employee photo)

---

# Rule

Never mark a module "Completed" in `CURRENT_PROGRESS.md` until every
sub-feature listed here for it is implemented, validated, permission-checked,
and responsive.

If a module's backend status is "Partial" or "Not Started," stop and ask
before building its frontend — do not guess the missing API shape.
