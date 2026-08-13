# Error Handling

## Purpose

This document defines how the frontend should handle all errors.

Never expose technical errors to users.

Always provide a friendly and actionable experience.

---

# General Rules

Always

- Handle API errors
- Handle validation errors
- Handle network errors
- Handle permission errors
- Handle unexpected errors

Never

- Show raw backend exceptions
- Show stack traces
- Crash the application

---

# API Errors

Handle common HTTP status codes.

## 400 Bad Request

Show validation or request error.

## 401 Unauthorized

Redirect to Login.

Clear expired token.

Show

"Your session has expired. Please log in again."

---

## 403 Forbidden

Show

"You do not have permission to perform this action."

Do not display unauthorized UI.

---

## 404 Not Found

Show

"The requested data could not be found."

Provide a Back or Refresh option.

---

## 409 Conflict

Show a meaningful conflict message.

Example

"Admission Number already exists."

---

## 422 Validation Error

Display field-level validation messages.

Focus the first invalid input.

---

## 500 Internal Server Error

Show

"Something went wrong. Please try again later."

Do not display technical details.

---

## Network Error

Show

"Unable to connect to the server."

Provide a Retry button.

---

# Loading

While waiting for an API

- Show Skeleton
- Disable submit button
- Prevent duplicate requests

Never leave a blank page.

---

# Empty State

When no data exists

Show

- Illustration or Icon
- Title
- Short Description
- Action Button

Example

"No students found."

"Create Student"

---

# Form Errors

Always

- Highlight invalid fields
- Display validation message below the field
- Keep user input
- Focus the first invalid field

---

# File Upload Errors

Handle

- Invalid file type
- File too large
- Upload failed

Show clear messages.

---

# Image Errors

If an image fails to load

Display a default placeholder.

Never show a broken image.

---

# Delete Errors

If deletion fails

Keep the dialog open.

Display the reason.

Allow retry.

---

# Toast Messages

Use Toast Notifications.

Success

Green

Warning

Orange

Error

Red

Information

Blue

Avoid browser alerts.

---

# Retry

Allow retry for

- Network Error
- Timeout
- Temporary Server Error

Do not retry validation errors.

---

# Logging

Log errors only in development.

Remove unnecessary console logs in production.

---

# Final Rule

Every page must gracefully handle

- Loading
- Success
- Empty
- Error

The application should never leave the user confused.