# Report Rules

## Purpose

This document defines the standards for all reports in the School Management System.

Every report must follow the same structure, UI, and user experience.

Never create inconsistent reports.

---

# General Rules

Every report must

- Use existing backend APIs
- Support server-side filtering
- Be responsive
- Be printable
- Be exportable (if supported)
- Follow the project design system

Never generate report data manually.

Always use backend data.

---

# Common Features

Every report should support

- Search
- Filter
- Date Range
- Academic Year
- Session
- Class
- Section
- Status (if applicable)
- Refresh

---

# Report Layout

Every report page should contain

- Page Title
- Breadcrumb
- Filter Panel
- Report Table
- Summary Cards (if applicable)
- Export Actions
- Print Action

Keep the layout clean and easy to read.

---

# Report Types

Student Reports

- Student List
- Admission Report
- Student Profile
- Student Promotion
- Student Transfer

Teacher Reports

- Teacher List
- Attendance
- Workload

Employee Reports

- Employee List
- Attendance
- Payroll

Attendance Reports

- Daily Attendance
- Monthly Attendance
- Date Range Attendance

Examination Reports

- Marks Report
- Result Sheet
- Tabulation Sheet
- Merit List
- Progress Report

Finance Reports

- Fee Collection
- Due Report
- Payment History
- Salary Report

---

# Table Rules

Every report table must support

- Sorting
- Pagination
- Responsive Layout
- Sticky Header (when needed)
- Status Badges

Use AppTable whenever possible.

---

# Export

Support

- Print
- PDF
- Excel

Export should respect the currently applied filters.

---

# Print

Printed reports should

- Remove unnecessary UI
- Fit A4 pages
- Display School Name
- Display Report Title
- Display Print Date

---

# Empty State

If no records exist

Show

- Friendly Message
- Illustration
- Reset Filter Button

Never show an empty table without explanation.

---

# Loading

While loading

Show

- Skeleton Loader
- Progress Indicator

Never leave a blank screen.

---

# Error Handling

If report loading fails

- Show user-friendly error
- Allow retry
- Keep applied filters

---

# Performance

Use

- Server-side Pagination
- Server-side Filtering
- Lazy Loading
- Query Caching

Avoid loading large datasets at once.

---

# Security

Only users with permission can

- View Reports
- Export Reports
- Print Reports

Hide unauthorized actions.

---

# Final Rule

Every report must be

- Accurate
- Fast
- Responsive
- Printable
- Exportable
- Easy to read
- Consistent with the project design system