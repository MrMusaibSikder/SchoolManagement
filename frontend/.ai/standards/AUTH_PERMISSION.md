# Authentication & Permission Rules

## Purpose

This document defines authentication and authorization rules for the School Management System.

The frontend must always respect backend authentication and permission rules.

Never implement authentication or permission logic independently.

Always use backend responses as the source of truth.

---

# Authentication

The system uses

- JWT Authentication
- Refresh Token (if available)
- Role-Based Access Control (RBAC)

Never store sensitive information outside secure storage.

---

# Login

After successful login

- Save JWT Token
- Save User Information
- Save Role & Permissions
- Redirect to Dashboard

If login fails

- Show friendly error message
- Keep entered username
- Clear password field

---

# Logout

On logout

- Clear token
- Clear cached user data
- Clear permissions
- Redirect to Login

---

# Protected Routes

Every protected page requires authentication.

If the user is not authenticated

Redirect to Login.

Never allow direct access.

---

# Permission Rules

Permissions control

- Pages
- Menus
- Buttons
- Actions
- Reports

Frontend should hide unauthorized UI.

Backend remains responsible for final authorization.

---

# Role Examples

- Super Admin
- School Admin
- Principal
- Vice Principal
- Teacher
- Accountant
- Employee
- Librarian
- Receptionist
- Guardian
- Student

---

# Menu Permission

Only show menus the user can access.

Do not display hidden modules.

---

# Page Permission

Before opening a page

Check permission.

If access is denied

Show

403 - Access Denied

---

# Button Permission

Hide buttons the user cannot use.

Examples

- Add
- Edit
- Delete
- Export
- Print
- Approve
- Publish

---

# Action Permission

Every action must check permission.

Examples

Student

- Create
- Update
- Delete
- View

Result

- Publish
- Lock
- Edit

Fee

- Collect
- Refund
- Print Receipt

---

# Dashboard Permission

Dashboard widgets must be role-based.

Examples

Teacher

- Own Classes
- Today's Routine
- Attendance

Guardian

- Child Information
- Fee Status
- Results

Student

- Profile
- Attendance
- Results

---

# Route Guard

Every protected route should

- Check authentication
- Check required permission
- Redirect or show 403 if unauthorized

---

# API Requests

Attach JWT automatically to every protected request.

Handle

401 Unauthorized

↓

Redirect to Login

Handle

403 Forbidden

↓

Show Access Denied

---

# Session Handling

If token expires

- Clear session
- Redirect to Login
- Show session expired message

---

# Permission Components

Create reusable components such as

- PermissionGuard
- ProtectedRoute
- RoleGuard

Reuse them across the application.

---

# Security Rules

Never

- Trust frontend permission alone
- Hardcode permissions
- Expose hidden menus through UI

Always rely on backend authorization.

---

# Final Rule

Authentication ensures who the user is.

Authorization determines what the user can do.

Always keep both secure, consistent, and synchronized with the backend.