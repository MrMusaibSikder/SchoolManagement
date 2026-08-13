# Development Workflow

## Purpose

This document defines how every task should be completed.

Never start coding immediately.

Always follow this workflow.

---

# Step 1 - Understand

Before writing any code

Read

- AGENTS.md
- PROJECT_CONTEXT.md
- MODULES.md
- CURRENT_PROGRESS.md

Understand

- Current task
- Backend APIs
- Module requirements
- Business rules

Never make assumptions.

---

# Step 2 - Analyze

Before creating anything

Check

- Existing components
- Existing hooks
- Existing API services
- Existing layouts

If something already exists

Reuse it.

Do not duplicate code.

---

# Step 3 - Plan

Before implementation

Identify

- Required pages
- Required components
- Required API endpoints
- Required validation
- Required permissions

Create a simple implementation plan first.

---

# Step 4 - Build

Build in this order

1. Types
2. Validation
3. API Service
4. Custom Hook
5. UI Components
6. Page
7. Routing

Do not skip steps.

---

# Step 5 - Connect

Connect the frontend with the existing backend.

Never modify backend APIs.

Never change request or response structures.

Use existing endpoints only.

---

# Step 6 - Test

Before finishing

Verify

- Validation
- API
- Loading
- Error handling
- Empty state
- Responsive layout
- Permissions

Fix issues before continuing.

---

# Step 7 - Review

Before marking complete

Confirm

✓ Clean code

✓ Reusable components

✓ Responsive UI

✓ TypeScript has no errors

✓ ESLint has no errors

✓ No duplicate code

✓ Production ready

---

# Step 8 - Update Progress

After completing a module

Update

CURRENT_PROGRESS.md

Change module status

Pending

↓

In Progress

↓

Completed

Never leave progress outdated.

---

# Development Order

Always build modules in this order

1. Authentication
2. Dashboard
3. Academic
4. Student
5. Guardian
6. Teacher
7. Employee
8. Attendance
9. Examination
10. Result
11. Fee Management
12. Reports
13. Settings

Do not jump randomly between modules unless requested.

---

# Final Rule

Think first.

Reuse existing code.

Follow project standards.

Write production-ready code.

Complete one module properly before starting the next.