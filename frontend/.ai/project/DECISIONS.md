# Decisions

## Purpose

This file is a running log of architecture and product decisions made during
this project — both backend decisions the frontend must respect, and frontend
decisions made along the way.

Before proposing a different approach to something listed here, read this
file. If a decision seems wrong, ask before overriding it — do not silently
pick a different pattern.

When a new significant decision is made during frontend work, add a new entry
at the bottom using the same format. Never delete old entries, even if later
superseded — mark them "Superseded by #N" instead.

---

## Format

Each entry has: an id, a short title, the date, status, the context that led
to it, the decision itself, and the consequences (what this means for the
code you write).

---

### #1 — Platform target is a responsive web app, not a native app

**Status:** Accepted

**Context:** The system needs to work for both mobile and desktop users.

**Decision:** Build a single responsive React (Vite) web application using
Tailwind CSS breakpoints. Do not build a separate native mobile app (no React
Native / Capacitor / Expo) and do not build a native desktop app (no
Electron / Tauri). "Mobile support" means the website itself must be fully
usable in a mobile browser, not that an installable app exists.

**Consequences:**
- Every page/table/form must be tested at mobile width, not just desktop.
- Tables must collapse to card/list layout on small screens — never rely on
  horizontal scroll as the only mobile solution unless explicitly noted in
  `UI_UX_GUIDELINES.md`.
- No native-only APIs (camera roll, push notifications, biometrics) unless
  this decision is revisited.
- If "installable app" behavior is wanted later, the next step is a PWA
  manifest + service worker on top of this same codebase — not a rewrite.

---

### #2 — Financial records use state-transition endpoints, not generic edit/delete

**Status:** Accepted

**Context:** Invoice, Payment, and Receipt are financial/audit-sensitive
records. Allowing free-form edit or delete on them would destroy the audit
trail (e.g. silently changing a payment amount after the fact).

**Decision:** These entities do not have a generic "Update" or "Delete" API.
Instead they expose specific action endpoints:
- Invoice: `Create`, `Cancel` (blocked if any payment exists)
- Payment: `Create` ("Collect"), `Void` (reverses the invoice balance, voids
  the linked receipt)
- Receipt: `Void` only (normally triggered indirectly via Payment Void)

**Consequences:**
- The UI must not show a generic "Edit" button on Invoice/Payment/Receipt
  rows. Show specific actions instead: "Cancel Invoice," "Void Payment,"
  "Reprint Receipt."
- A void/cancel action always requires a reason (`CancellationReason` /
  `Reason` / `VoidReason` fields are required, not optional) — the form must
  enforce this.
- "Delete" in the generic sense does not exist for these three entities. Do
  not build a delete button for them.

---

### #3 — Invoice uses optimistic concurrency (RowVersion) — the frontend must handle 409 Conflict

**Status:** Accepted

**Context:** Two staff members (e.g. two cashiers) can act on the same
invoice at nearly the same time (one collecting a payment while another
cancels it). Without a concurrency check, one action could silently
overwrite the other.

**Decision:** `Invoice` has a `RowVersion` column enforced server-side. If a
save conflicts with another concurrent change, the API returns
**`409 Conflict`** with a message like "This invoice was modified by another
user. Please refresh and try again."

**Consequences:**
- The global API client / error interceptor must special-case `409`: show a
  toast with the server's message and automatically refetch the affected
  record (via TanStack Query `invalidateQueries`), rather than treating it
  as a generic error.
- Never let the user "retry" a 409 by blindly resubmitting the same stale
  form data — force a refetch first so they see current state.
- This currently applies to Invoice Cancel and Payment Collect specifically.
  Treat any `409` from any endpoint the same way as a general pattern.

---

### #4 — Soft delete vs. no-delete, by entity

**Status:** Accepted

**Context:** Different entities have different deletion semantics on the
backend, and the UI must not offer an action the backend will reject.

**Decision:**
- Most master data (FeeCategory, FeeType, FeeStructure, StudentFeeConcession,
  LateFineRule, etc.) uses **soft delete** — a `Delete` button exists, and
  deleted rows disappear from lists but are not physically removed.
- Some master-data deletes are **conditionally blocked** by business rules
  (e.g. a FeeCategory cannot be deleted while it still has FeeTypes; a
  FeeType cannot be deleted while used in any FeeStructure or Invoice). These
  return `400 Bad Request` with an explanatory message — show it as-is in a
  toast, do not reinterpret it.
- `Payment` and `Receipt` have **no delete at all**, soft or hard — see
  Decision #2. `Invoice` has no delete either, only `Cancel`.

**Consequences:**
- Do not assume every entity in the system supports the same "row actions"
  set. Check `API_REFERENCE.md` per entity before adding a Delete button.
- Always show the backend's 400 error message directly to the user for
  blocked deletes — it already explains why (e.g. "Cannot delete a fee
  category that has associated fee types.").

---

### #5 — Invoice list is paginated; most other list endpoints are not (yet)

**Status:** Accepted

**Context:** Invoice volume grows every month per student, so it needs
server-side pagination from day one. Most other master-data lists (Fee
Category, Fee Type, Late Fine Rules, etc.) are small in practice (tens to
low hundreds of rows) and currently return the full list.

**Decision:** Only `GET /api/invoices` currently returns a paginated
`PagedResult<T>` shape (`items`, `totalCount`, `pageNumber`, `pageSize`,
`totalPages`, `hasPreviousPage`, `hasNextPage`). All other Fee Management
list endpoints return a plain array.

**Consequences:**
- Build one reusable "paginated table" component and one reusable "plain
  list table" component (or one component with a pagination prop that can be
  toggled) — do not assume every table needs server pagination wiring.
- If a plain-array list grows large enough to need pagination later, that is
  a backend change to request, not something to fake entirely client-side
  for a large dataset.

---

### #6 — Permission model is string-based, checked via JWT claims

**Status:** Accepted

**Context:** See `AUTH_PERMISSION.md`.

**Decision:** Every protected endpoint requires a specific permission string
(e.g. `FeeCategoryView`, `PaymentCollect`, `InvoiceCancel`), defined centrally
in the backend's `PermissionNames` class and issued to the user as JWT role
claims after login.

**Consequences:**
- The frontend must fetch/decode the permission list once at login and store
  it (e.g. in an auth store), then gate every button/menu/route by the exact
  matching permission string — not by role name. A user's role can carry any
  combination of permissions; never hardcode "if role === Accountant show
  fee menu."
- Permission string naming convention is `{Resource}{Action}`, e.g.
  `FeeStructureEdit`, `ConcessionApprove`. Match these exactly — see
  `API_REFERENCE.md` for the full list per endpoint.

---

### #7 — Money fields: always 2 decimal places, generated document numbers follow fixed formats

**Status:** Accepted

**Context:** Consistent formatting matters for a financial module.

**Decision:**
- All monetary `decimal` fields are `decimal(18,2)` server-side. Always
  format for display with exactly 2 decimal places (e.g. `1,500.00`), never
  round differently client-side than server-side.
- Document numbers are server-generated, not user-editable, in these fixed
  formats:
  - Invoice: `INV-{AcademicYearName}-{6-digit sequence}`
  - Payment: `PAY-{yyyyMM}-{6-digit sequence}`
  - Receipt: `RCPT-{yyyyMM}-{6-digit sequence}`

**Consequences:**
- Do not build an "Invoice Number" input field on the Create Invoice form —
  it is assigned by the server and only shown after creation.
- When building a search/filter, treat these number formats as opaque
  strings (prefix search is fine; do not try to parse/reconstruct them
  client-side).

---

### #8 — Public/anonymous endpoints are isolated under `/api/public/*` and exist only for the Landing Page

**Status:** Accepted

**Context:** The Landing Page renders before login, so it cannot call any of
the normal permission-protected endpoints. A separate, deliberately
anonymous set of read-only endpoints was added instead of relaxing
permissions on existing controllers.

**Decision:** A dedicated `PublicController` at `/api/public/*` exposes only
three read-only, aggregate/non-sensitive endpoints (`school-info`, `stats`,
`notices`) with no `[Authorize]`/`[PermissionAuthorize]` at all. No other
controller in the system is anonymous.

**Consequences:**
- Never attach a JWT token to a call under `/api/public/*` — some backend
  setups reject an unexpected `Authorization` header on an anonymous
  endpoint, and even where they don't, it's a signal something is wired
  wrong on the frontend.
- Never reuse a `/api/public/*` endpoint inside an authenticated page just
  because it's convenient (e.g. don't use `/api/public/stats` on the
  Dashboard — use the real, permission-checked dashboard/report endpoints
  there instead, once they exist).
- If the Landing Page ever needs one more piece of public data, add it to
  `PublicInfoService`/`PublicController` following the same
  aggregate-only, no-PII rule from `BUSINESS_RULES.md` → Landing Page
  Rules — do not widen an existing authenticated endpoint's access instead.

---

## Open Questions (not yet decided — ask before assuming)

- Does the Dashboard need dedicated aggregate endpoints (total students,
  today's collection, etc.), or should the frontend compute these
  client-side from existing list endpoints? Not yet confirmed with backend —
  see `MODULES.md` Phase 7.
- Is a PWA manifest wanted for "Add to Home Screen" support, or is
  browser-only access sufficient long-term? See Decision #1.
