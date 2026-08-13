# Business Rules

## Purpose

This document defines the business rules of the School Management System.

The frontend must always follow these rules.

Do not assume business logic.

Do not invent new workflows.

If a business rule is unclear, ask before implementing.

---

# General Rules

- Backend is the source of truth.
- Frontend follows backend APIs.
- Never implement business logic in frontend.
- Validate user input before sending requests.
- Show meaningful success and error messages.
- Hide unauthorized features.

---

# School Context

The application is designed for schools in Bangladesh.

Use

- Academic Year
- Session
- Class
- Section
- Roll Number
- Admission Number

throughout the application.

---

# Student Rules

A student belongs to

- One School
- One Academic Year
- One Session
- One Class
- One Section

A student may have

- One or Multiple Guardians
- Photo
- Documents

Admission Number is unique.

Roll Number is unique within a class and section.

---

# Guardian Rules

A guardian can be linked with multiple students.

Guardian information should not be duplicated.

When creating a student

If guardian already exists

↓

Select existing guardian.

Otherwise

↓

Create guardian

↓

Automatically select the new guardian.

---

# Teacher Rules

A teacher belongs to a department.

A teacher can teach multiple classes and subjects.

A teacher may also be a class teacher.

---

# Employee Rules

Employees are different from teachers.

Employee modules should not mix with teacher modules.

---

# Attendance Rules

Attendance can be taken for

- Students
- Teachers
- Employees

Attendance is marked once per day.

Duplicate attendance is not allowed.

---

# Examination Rules

Each exam belongs to

- Academic Year
- Session
- Class

Results are generated after marks are published.

---

# Fee Management Rules ⭐

This is the most business-rule-heavy module in the system. Read this section
fully before building any Fee Management screen. Full endpoint detail is in
`API_REFERENCE.md` → Fee Management.

## 1. The building blocks and how they relate

```
FeeCategory (e.g. "Academic", "Transport")
   └─ FeeType (e.g. "Tuition Fee", monthly, under "Academic")

FeeStructure (e.g. "Class 5 – 2026 Fee Structure")
   ├─ belongs to one AcademicYear + one SchoolClass (+ optionally one Section)
   └─ has FeeStructureItems (one row per FeeType, with an Amount)

Invoice (one bill issued to one Student)
   ├─ optionally generated from a FeeStructure
   └─ has InvoiceItems (one row per FeeType, with Original/Discount/Fine/Net amount)

Payment (money actually collected against one Invoice)
   └─ has exactly one Receipt

StudentFeeConcession (a discount/waiver for one Student + one FeeType + one AcademicYear)

LateFineRule (fine to apply when an Invoice passes its due date)
```

A **FeeStructure with no Section** applies to every section of that class. A
FeeStructure **with a Section** applies only to that section. When both could
match a student, the section-specific one takes priority (this matters for
"apply this structure" style UI, e.g. bulk invoice generation).

## 2. Invoice status is a one-way state machine

```
Draft → Issued → PartiallyPaid → Paid
                       ↓
                   Overdue (due date passed, still unpaid/partially paid)
                       ↓
                   (payment resumes → back to PartiallyPaid/Paid)

Issued/PartiallyPaid/Overdue → Cancelled  (only if AmountPaid = 0)
```

Rules the UI must enforce or reflect:

- An invoice can only be **cancelled** while `amountPaid = 0`. If any payment
  exists (even a small one), the Cancel action must be disabled/hidden — the
  correct flow is to void the payment(s) first, not to cancel the invoice.
- `status` moves to `PartiallyPaid` or `Paid` automatically whenever a
  payment is collected — the frontend never sets status directly, it is
  always a side effect of a Payment or Cancel action.
- `status` moves to `Overdue` automatically by the "Apply Late Fines"
  background action once the due date has passed — this is not a manual
  status a user picks from a dropdown.
- There is no "Delete Invoice." The only destructive action is Cancel, and
  only under the condition above.

## 3. Payments can never exceed the invoice balance

- `payment.amount` must be `<= invoice.balanceDue` at the moment of
  submission. The backend enforces this (400 if violated), but the frontend
  should also validate it client-side for a fast, friendly error — pull the
  current `balanceDue` when opening the "Collect Payment" form and show it
  next to the amount field.
- A `transactionId` is required for every payment method except `Cash`
  (Bank Transfer, Card, Mobile Banking, Cheque). The form should conditionally
  require this field based on the selected method.
- Collecting a payment always produces exactly one Receipt automatically —
  there is no separate "create receipt" step in the UI.

## 4. Voiding, not deleting

- Neither Payment nor Receipt can be deleted, ever — only **voided**, and
  voiding always requires a reason.
- Voiding a Payment automatically: reverses the amount back into the
  invoice's `balanceDue`, moves the invoice status back toward
  `Issued`/`PartiallyPaid` as appropriate, and voids the linked Receipt.
  This is one action (`POST /api/payments/{id}/void`) — do not build a
  separate "void the receipt too" step for the normal flow.
- Voiding a Receipt directly (`POST /api/receipts/{id}/void`) is an
  edge-case-only action (e.g. a reprint mistake) and does **not** touch the
  invoice balance. Do not present this as the primary way to reverse a
  payment in the UI — label it clearly as a receipt-only action, or omit it
  from the main Payment screens entirely and only expose it from a Receipts
  admin screen if one exists.
- A voided Payment/Receipt must be visually distinct in every list/detail
  view (e.g. a red "Voided" badge, strikethrough amount) — never show it
  identically to an active one.

## 5. Concessions (discounts/waivers)

- A concession is defined per (Student, FeeType, AcademicYear) — one row per
  combination, not a general "student discount."
- Three types: **Percentage Discount** (0–100% of the item amount), **Fixed
  Amount Discount** (a flat currency amount, capped at the item amount), and
  **Full Exemption** (the entire item amount is waived — no `value` needed).
- If `requiresApproval` is set to `false` at creation, the concession is
  **immediately active** — do not show an "awaiting approval" state for it.
  If `true`, it sits in the "Pending Approvals" queue until an authorized
  user (permission `ConcessionApprove`) approves it.
- Concessions are applied automatically wherever a fee item is calculated
  for that student/fee type/year (both manual invoice math shown to staff
  and the monthly bulk-generation job) — the UI never manually types in a
  "discount" on an invoice item; it is always sourced from an existing
  concession record.

## 6. Late fines

- A `LateFineRule` belongs to an AcademicYear and is either **global**
  (`feeTypeId = null`, applies to every fee type that year) or **specific**
  to one FeeType. A specific rule always takes priority over the global one
  for that fee type.
- Three fine types: **Fixed** (flat amount once grace period passes),
  **Percentage** (percentage of the item's original amount), **Daily
  Accrual** (multiplies per day overdue, past the grace period) —
  `MaxFineAmount`, if set, caps the total regardless of type.
- Fines are **recalculated from scratch** every time the "Apply Late Fines"
  action runs, not accumulated — running it twice in one day does not double
  the fine. This means it is safe to expose as a repeatable admin action
  (button, or later a scheduled job), not something that needs a
  "have I already run this today" guard in the UI.
- The grace period is measured from the invoice's due date, per fee-type
  rule, not a single blanket number.

## 7. Recurring (bulk) invoice generation

- The "Generate Monthly Invoices" action only includes fee items whose
  `FeeType.frequency = Monthly`. Termly/Yearly fee items are **not** included
  and must be invoiced manually (via the normal Create Invoice form) until a
  separate bulk trigger for them exists.
- It is idempotent per (student, period, fee structure) — running it again
  for a month that was already generated simply skips students who already
  have that invoice; it does not create duplicates or error out. This makes
  it safe to expose as a plain "Generate" button the admin can click again
  without fear, rather than something that needs a confirmation dialog
  warning about duplicates.
- The response is a summary object (counts + per-student errors), always
  returned with `200 OK` even when some students failed — **the UI must
  read the response body**, not just check the HTTP status, to know whether
  everything succeeded. Display the summary counts and, if `failed > 0`,
  the error list.

## 8. Reports

- **Collection Summary** only counts `Completed` payments — voided, failed,
  and refunded payments are excluded from the totals. If displaying a
  "gross vs. net" figure is ever wanted, that is a backend change to
  request, not something to approximate client-side by re-including voided
  payments.
- **Defaulter Report** only includes invoices that are unpaid or partially
  paid **and** past their due date (`Overdue`-eligible) — a `Cancelled`
  invoice never appears here, and neither does a fully `Paid` one.

---

# Landing Page Rules ⭐

The Landing Page (`/`) is the **only** part of the frontend that renders
before login, for an anonymous visitor. It must be treated as public
information only — nothing shown here can require, imply, or leak anything
private.

- Only aggregate counts are shown (total students, total teachers, total
  employees) — never a list of individual names, roll numbers, admission
  numbers, contact info, or any other per-person data.
- Only notices with `audience = Everyone` may appear. A notice targeted at
  `Students`, `Teachers`, `Employees`, or `Guardians` specifically must never
  render on this page, even if it is published — those audiences see it
  after logging in, inside the authenticated Notice module.
- An expired (`expiryDate` in the past) or archived notice must never appear
  here, even though it may still exist in the database for internal records.
- This page must call only the `/api/public/*` endpoints (see
  `API_REFERENCE.md`) and must never attach a JWT token to those requests —
  it is, by definition, rendered for someone who is not logged in yet.
- The only interactive element that leads deeper into the app is the
  "Login" call-to-action, which routes to `/login`. Do not add any other
  authenticated-feeling UI (e.g. a search box that queries student records)
  to this page.

---

# Notice Rules

Notices can target

- Everyone
- Students
- Teachers
- Employees
- Guardians

---

# Report Rules

Reports should support

- Search
- Filter
- Export
- Print

Only users with permission can access reports.

---

# Dashboard Rules

Dashboard should show only information relevant to the logged-in user.

Example

Admin

↓

All statistics

Teacher

↓

Own classes and attendance

Guardian

↓

Own children

Student

↓

Own profile and results

---

# Search Rules

Search should always be fast.

Prefer server-side search.

Use debounce.

---

# Delete Rules

Always ask for confirmation before deleting.

Never delete immediately.

**Exception:** Invoice, Payment, and Receipt have no "delete" concept at all
— see Fee Management Rules above. Do not add a delete confirmation dialog
for these three; use their specific Cancel/Void actions instead, each of
which already requires a typed reason as its confirmation step.

---

# File Upload Rules

Validate

- File Type
- File Size

Show preview whenever applicable.

---

# Permission Rules

Every page, button, menu, report, and action must respect user permissions.

Never show unauthorized actions.

---

# Final Rule

Always build the frontend according to these business rules.

Do not create workflows that conflict with the backend.
