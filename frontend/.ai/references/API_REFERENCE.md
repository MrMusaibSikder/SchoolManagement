# API Reference

## Purpose

This file lists the actual backend endpoints, request/response shapes, and
required permissions — module by module.

Never guess an endpoint path, field name, or status code. If a module below
is marked "NOT YET DOCUMENTED," stop and ask for its real endpoint list
(ideally exported from Swagger) before building that module's frontend. Do
not invent a plausible-looking endpoint to fill the gap.

All endpoints are prefixed with the API base URL (see environment config).
All protected endpoints require an `Authorization: Bearer {token}` header.

---

# How To Use This File

For each endpoint you are about to call from a form, table, or action button:

1. Find it below by module.
2. Confirm the exact path, method, and required permission.
3. Match the request body to the DTO fields listed — do not add or omit
   fields.
4. Handle every listed response status, not just 200.

---

# Global Response Conventions

- Success bodies for single-item responses are the DTO itself, e.g.
  `{ id, name, ... }`.
- Success bodies for error responses are
  `{ success: false, message: string }`, except FluentValidation failures
  which are `{ success: false, errors: [{ field, message }] }`.
- `401` → not authenticated → redirect to login.
- `403` → authenticated but missing permission → show access-denied.
- `404` → resource not found → show not-found state, do not treat as a
  generic error toast.
- `409` → concurrency conflict (currently only on Invoice/Payment
  operations) → see `DECISIONS.md` #3: refetch and prompt retry, do not
  resubmit blindly.
- `422` → FluentValidation failure → map `errors[].field` to the matching
  form field; show `errors[].message` inline, not as a toast.
- `499` → client cancelled the request (e.g. navigated away) → ignore,
  no user-facing error needed.
- `500` → unexpected error → show a generic "something went wrong" message,
  never show the raw error to the user.

---

# Fee Management ⭐ (fully documented — backend complete)

## Fee Category — `/api/feecategories`

| Action | Method & Path | Permission | Notes |
|---|---|---|---|
| List | `GET /api/feecategories` | `FeeCategoryView` | Returns active categories ordered by `displayOrder`. Array of `FeeCategoryDto`. |
| Get by id | `GET /api/feecategories/{id}` | `FeeCategoryView` | 404 if not found. |
| Create | `POST /api/feecategories` | `FeeCategoryCreate` | Body: `CreateFeeCategoryDto { name, description?, displayOrder, isActive }`. Returns 201 + `FeeCategoryDto`. |
| Update | `PUT /api/feecategories/{id}` | `FeeCategoryEdit` | Body: `UpdateFeeCategoryDto { id, name, description?, displayOrder, isActive }`. Route id and body id must match (400 if not). |
| Delete | `DELETE /api/feecategories/{id}` | `FeeCategoryDelete` | Soft delete. 400 if this category still has FeeTypes attached. |

## Fee Type — `/api/feetypes`

| Action | Method & Path | Permission | Notes |
|---|---|---|---|
| List | `GET /api/feetypes` | `FeeTypeView` | Lightweight list — `FeeTypeListDto[]` (includes `feeCategoryName`, not full detail). |
| Get by id | `GET /api/feetypes/{id}` | `FeeTypeView` | Full detail — `FeeTypeDto`. |
| Create | `POST /api/feetypes` | `FeeTypeCreate` | Body: `CreateFeeTypeDto { name, code, description?, feeCategoryId, frequency, isMandatory, isRefundable, defaultDueDayOfMonth?, defaultGracePeriodDays }`. `frequency` enum: `OneTime=1, Monthly=2, Termly=3, Yearly=4`. `defaultDueDayOfMonth` must be null for `OneTime`, required (1–31) otherwise. |
| Update | `PUT /api/feetypes/{id}` | `FeeTypeEdit` | Body: `UpdateFeeTypeDto` (same shape + `id`, `isActive`). |
| Delete | `DELETE /api/feetypes/{id}` | `FeeTypeDelete` | Soft delete. 400 if used in any FeeStructure or Invoice — deactivate (`isActive = false` via Update) instead in that case. |

## Fee Structure — `/api/feestructures`

| Action | Method & Path | Permission | Notes |
|---|---|---|---|
| List | `GET /api/feestructures?academicYearId=&schoolClassId=&isActive=` | `FeeStructureView` | All query params optional. Returns `FeeStructureListDto[]` with `itemCount` and computed `totalAmount`. |
| Get by id | `GET /api/feestructures/{id}` | `FeeStructureView` | Full detail incl. `items[]` — `FeeStructureDto`. |
| Create | `POST /api/feestructures` | `FeeStructureCreate` | Body: `CreateFeeStructureDto { name, description?, academicYearId, schoolClassId, sectionId?, isTemplate, effectiveFrom, effectiveTo?, clonedFromId?, items: CreateFeeStructureItemDto[] }`. Each item: `{ feeTypeId, amount, isOptional, sortOrder }`. `items` must be non-empty; no duplicate `feeTypeId` within the list. 400 if a structure already exists for the same class/section/year combination. |
| Update | `PUT /api/feestructures/{id}` | `FeeStructureEdit` | Body: `UpdateFeeStructureDto { id, name, description?, isActive, sectionId?, isTemplate, effectiveFrom, effectiveTo?, items: UpdateFeeStructureItemDto[] }`. Each item: `{ id?, feeTypeId, amount, isOptional, sortOrder, isDeleted }`. **`id: null` = new item; `isDeleted: true` = remove that existing item.** At least one non-deleted item must remain. |
| Delete | `DELETE /api/feestructures/{id}` | `FeeStructureDelete` | Soft delete. 400 if any invoices already reference it — deactivate instead. |

## Invoice — `/api/invoices`

| Action | Method & Path | Permission | Notes |
|---|---|---|---|
| Paged list | `GET /api/invoices?pageNumber=1&pageSize=20&status=&studentId=&academicYearId=` | `InvoiceView` | All filters optional. `status` enum: `Draft=1, Issued=2, PartiallyPaid=3, Paid=4, Overdue=5, Cancelled=6`. Returns `PagedResult<InvoiceListDto>`. |
| Get by id | `GET /api/invoices/{id}` | `InvoiceView` | Full detail incl. `items[]` — `InvoiceDto`. |
| By student | `GET /api/invoices/student/{studentId}` | `InvoiceView` | Full unpaged list for one student — `InvoiceListDto[]`. |
| Create (manual) | `POST /api/invoices` | `InvoiceCreate` | Body: `CreateInvoiceDto { studentId, academicYearId, feeStructureId?, invoiceDate, dueDate, month?, year?, notes?, items: CreateInvoiceItemDto[] }`. `dueDate >= invoiceDate`. `invoiceNumber`/`status`/amounts are server-assigned — do not send them. |
| Cancel | `POST /api/invoices/{id}/cancel` | `InvoiceCancel` | Body: `CancelInvoiceDto { cancellationReason }` (required, non-empty). 400 if the invoice already has any payment recorded — void the payment(s) first. **Can return 409** — see Decision #3. |
| Bulk generate monthly | `POST /api/invoices/generate-monthly` | `InvoiceCreate` | Body: `GenerateMonthlyInvoicesDto { academicYearId, month (1-12), year, schoolClassId?, dueDate, invoiceDate? }`. Idempotent — safe to re-run for the same month; already-invoiced students are skipped, not duplicated. Only includes `Monthly`-frequency fee items. Returns `InvoiceGenerationResultDto { totalStudentsEvaluated, invoicesCreated, skippedAlreadyInvoiced, skippedNoMonthlyItems, failed, errors[] }` — **always 200**, check the counts/`errors[]` in the body rather than relying on HTTP status for partial failures. |
| Apply late fines | `POST /api/invoices/apply-late-fines?asOfDate=` | `InvoiceCreate` | `asOfDate` optional, defaults to today. Idempotent, safe to run repeatedly/daily. Returns `LateFineApplicationResultDto` — same "always 200, check the body" pattern as above. |

## Payment — `/api/payments`

| Action | Method & Path | Permission | Notes |
|---|---|---|---|
| Get by id | `GET /api/payments/{id}` | `PaymentView` | Full detail — `PaymentDto`. |
| By invoice | `GET /api/payments/invoice/{invoiceId}` | `PaymentView` | Payment history for one invoice — `PaymentListDto[]`. |
| Collect | `POST /api/payments` | `PaymentCollect` | Body: `CreatePaymentDto { invoiceId, studentId, amount, paymentDate, method, transactionId?, remarks? }`. `method` enum: `Cash=1, BankTransfer=2, Card=3, MobileBanking=4, Cheque=5`. **`amount` must not exceed the invoice's current `balanceDue`** (400 if it does). `transactionId` is required for every method except `Cash`. Creates the Payment **and** a Receipt, and updates the Invoice's `amountPaid`/`balanceDue`/`status` atomically. **Can return 409** — see Decision #3. |
| Void | `POST /api/payments/{id}/void` | `PaymentVoid` | Body: `VoidPaymentDto { reason }` (required). Reverses the invoice balance and voids the linked receipt. 400 if already voided. **Can return 409.** |

## Receipt — `/api/receipts`

| Action | Method & Path | Permission | Notes |
|---|---|---|---|
| Get by id | `GET /api/receipts/{id}` | `ReceiptView` | `ReceiptDto`. |
| By payment | `GET /api/receipts/payment/{paymentId}` | `ReceiptView` | `ReceiptDto`. |
| Void | `POST /api/receipts/{id}/void` | `ReceiptVoid` | Body: `VoidReceiptDto { voidReason }`. **Edge-case only** — prefer voiding via `POST /api/payments/{id}/void`, which keeps the invoice balance in sync; this endpoint does not touch the invoice. |
| Download PDF | `GET /api/receipts/{id}/pdf` | `ReceiptView` | Returns `application/pdf` binary (not JSON) — request as a blob/file download, filename `Receipt-{id}.pdf`. Shows a red "VOIDED" watermark if the receipt has been voided. Note: the "Current Balance Due" line on the PDF reflects the invoice's balance *at PDF-generation time*, not a historical snapshot — if reprinted after another payment, the number will have moved on. |

## Late Fine Rule — `/api/latefinerules`

| Action | Method & Path | Permission | Notes |
|---|---|---|---|
| By academic year | `GET /api/latefinerules/academic-year/{academicYearId}` | `LateFineRuleView` | `LateFineRuleDto[]`. |
| Create | `POST /api/latefinerules` | `LateFineRuleManage` | Body: `CreateLateFineRuleDto { academicYearId, feeTypeId?, type, amount, gracePeriodDays, maxFineAmount?, isActive }`. `type` enum: `Fixed=1, Percentage=2, DailyAccrual=3`. `feeTypeId: null` = applies to all fee types for that year (a "global" rule). `amount` for `Percentage` must be 0–100. One rule per (year, feeType) combination — 400/409-style validation error if a duplicate is attempted. |
| Update | `PUT /api/latefinerules/{id}` | `LateFineRuleManage` | Body: `UpdateLateFineRuleDto { id, type, amount, gracePeriodDays, maxFineAmount?, isActive }`. `academicYearId`/`feeTypeId` cannot be changed — create a new rule instead. |
| Delete | `DELETE /api/latefinerules/{id}` | `LateFineRuleManage` | Soft delete. |

## Student Fee Concession — `/api/studentfeeconcessions`

| Action | Method & Path | Permission | Notes |
|---|---|---|---|
| By student | `GET /api/studentfeeconcessions/student/{studentId}` | `ConcessionView` | `StudentFeeConcessionListDto[]`. |
| Pending approvals | `GET /api/studentfeeconcessions/pending-approvals` | `ConcessionApprove` | Approver's queue — `StudentFeeConcessionListDto[]` where `requiresApproval && !isApproved`. |
| Create | `POST /api/studentfeeconcessions` | `ConcessionCreate` | Body: `CreateStudentFeeConcessionDto { studentId, feeTypeId, academicYearId, type, value?, reason, requiresApproval, validFrom?, validTo? }`. `type` enum: `PercentageDiscount=1, FixedAmountDiscount=2, FullExemption=3`. `value` required unless `FullExemption`; for `PercentageDiscount` must be 0–100. **If `requiresApproval` is `false`, the concession is immediately active/approved** — do not show an "approve" step for it in the UI. One concession per (student, feeType, academicYear). |
| Update | `PUT /api/studentfeeconcessions/{id}` | `ConcessionEdit` | Body: `UpdateStudentFeeConcessionDto { id, type, value?, reason, validFrom?, validTo?, isActive }`. |
| Approve | `POST /api/studentfeeconcessions/{id}/approve` | `ConcessionApprove` | No body — the id comes from the route. 400 if already approved. |
| Delete | `DELETE /api/studentfeeconcessions/{id}` | `ConcessionDelete` | Soft delete. |

## Fee Reports — `/api/fee-reports`

| Action | Method & Path | Permission | Notes |
|---|---|---|---|
| Collection summary | `GET /api/fee-reports/collection-summary?dateFrom=&dateTo=` | `PaymentView` | Both dates required (`yyyy-MM-dd`). Returns `FeeCollectionSummaryDto { dateFrom, dateTo, totalCollected, totalTransactions, averageTransactionAmount, dailyBreakdown[], methodBreakdown[] }` — use `dailyBreakdown` for a trend chart, `methodBreakdown` for a pie/bar chart of Cash vs Bank vs Card etc. Only counts `Completed` payments (voided/failed excluded). |
| Defaulters | `GET /api/fee-reports/defaulters?asOfDate=&schoolClassId=` | `InvoiceView` | Both params optional (`asOfDate` defaults to today). Returns `DefaulterReportDto { asOfDate, totalDefaulters, totalOverdueAmount, defaulters[] }` — one row per student with `overdueInvoiceCount`, `totalOverdueAmount`, `oldestDueDate`, `daysOverdue`. Sorted by `totalOverdueAmount` descending. |

---

# Public (Anonymous) Endpoints — no auth required

These exist only to feed the pre-login **Landing Page** (`MODULES.md` Phase
0.4). No `[Authorize]`/`[PermissionAuthorize]` on the backend controller —
never attach an `Authorization` header on these three calls, and never call
them from inside an authenticated page (use the normal authenticated
equivalents there instead, e.g. the full paginated Notice list).

| Action | Method & Path | Permission | Notes |
|---|---|---|---|
| School info | `GET /api/public/school-info` | None (anonymous) | `PublicSchoolInfoDto { name, address?, logoUrl?, phone?, email? }`. Single-school assumption — always returns the one `School` row. **⚠️ `logoUrl`/`phone`/`email` field names are not yet confirmed against the real `School` entity — verify before relying on them; if they don't exist on `School`, this DTO needs adjusting first.** |
| Stats | `GET /api/public/stats` | None (anonymous) | `PublicStatsDto { totalStudents, totalTeachers, totalEmployees }`. Counts exclude soft-deleted rows. |
| Public notices | `GET /api/public/notices?take=5` | None (anonymous) | `PublicNoticeDto[] { id, title, summary, publishDate, priority }`. Only notices where `isPublished = true`, `isArchived = false`, `audience = Everyone`, and (`expiryDate` is null or `expiryDate >= now`) — an expired or archived notice must never appear here even if it was once published. Sorted by `publishDate` descending. `priority` is the `NoticePriority` enum as a string (`Low`/`Medium`/`High`/... — confirm exact member names from the enum before styling a priority badge). `summary` is `description` truncated to ~150 characters with `...` appended — it is **not** a separate database field, so don't build a "summary" input anywhere; it's server-computed from `description`. |

---

# Every Other Module — See API_REFERENCE_AUTOGENERATED.md

**Update:** every module previously listed here as undocumented is now
covered in **`API_REFERENCE_AUTOGENERATED.md`** (same folder), machine-
extracted directly from the backend source (`Controllers/*.cs`,
`Application/Features/*/DTOs/*.cs`, `Domain/Enums/*.cs`). It has full
routes, HTTP verbs, permission names, request/response DTO fields, and
enum values for all 43 controllers except Fee Management and Public
(which stay here, hand-written, with the extra business-rule detail this
file provides).

When building any non-Fee-Management module: read the relevant section of
`API_REFERENCE_AUTOGENERATED.md` for the contract, and cross-check
`BUSINESS_RULES.md` for any behavioral rules before assuming a field's
purpose. That file also explains its own extraction caveats (regex-based,
not a compiler) — if something looks off or missing there, verify against
the actual controller file before guessing.

Dashboard aggregate endpoints (total students, today's collection, etc.)
are still unconfirmed to exist — see `DECISIONS.md` Open Questions.

---

# Rule

Always check this file before implementing a frontend feature.

Never create a fake or "probably correct" endpoint. If a module you need is
in the "NOT YET DOCUMENTED" list, stop and ask for its Swagger export first.
