# Module Standards

## Purpose

Every module must follow the same structure, UI, and workflow.

Never create a module differently unless I explicitly request it.

All modules must feel like part of one application.

---

# Standard Module Structure

Every CRUD module should contain

- List Page
- Create Page
- Edit Page
- Details Page
- Delete Confirmation
- API Service
- Validation Schema
- Types
- Hooks
- Route

---

# List Page

Every list page must support

- Search
- Filter
- Pagination
- Sorting
- Status Badge
- Action Menu
- Refresh
- Export (If Required)

Use AppTable.

---

# Create Page

Every create page must include

- React Hook Form
- Zod Validation
- Loading State
- Submit Button
- Reset Button
- Cancel Button
- Success Toast
- Error Handling

---

# Edit Page

Load existing data.

Show loading while fetching.

Prefill all fields.

Support image/file replacement if applicable.

---

# Details Page

Display information in read-only mode.

Group data into sections.

Show audit information if available.

---

# Delete

Never delete immediately.

Always show confirmation dialog.

Refresh the list after successful deletion.

---

# Foreign Key Fields

Never use text inputs.

Always use searchable dropdowns.

If related data does not exist

Show

Create New

After creating

Automatically reload dropdown

Automatically select new item

Example

Student → Guardian

Teacher → Department

Employee → Designation

Subject → Category

---

# Image Upload

If a module supports images

Always provide

- Preview
- Replace
- Remove
- Validation
- Loading

---

# Table Actions

Every table should contain

View

Edit

Delete

Additional actions only when required.

---

# Search

Search should be server-side.

Use debounce.

Don't call API on every key press.

---

# Filter

Use reusable filter component.

Keep filters after page refresh whenever possible.

---

# Validation

Every field must have validation.

Never allow invalid submissions.

Show user-friendly error messages.

---

# Permission

Hide unauthorized

Pages

Buttons

Actions

Menus

Never rely only on frontend permission.

---

# Loading

Every module must show

Loading

Skeleton

Empty State

Error State

Success Feedback

---

# Responsive

Every module must work on

Desktop

Laptop

Tablet

Mobile

---

# Reusable Components

Always reuse

AppButton

AppInput

AppSelect

AppTable

AppModal

AppDrawer

AppBadge

AppLoader

EmptyState

ErrorState

PermissionGuard

Never create duplicate components.

---

# Module Completion Checklist

Before completing any module

✓ UI matches design system

✓ Responsive

✓ API connected

✓ Validation complete

✓ Permission checked

✓ Loading works

✓ Error handled

✓ Success message shown

✓ No TypeScript errors

✓ No ESLint errors

✓ Build successful

Only then consider the module complete.