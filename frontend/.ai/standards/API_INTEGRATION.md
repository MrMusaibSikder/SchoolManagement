# API Integration Standards

## Purpose

This document defines the official API integration architecture for the School Management System frontend.

The backend is already completed using ASP.NET Core Web API.

The frontend must integrate with the existing APIs without modifying backend architecture, contracts, DTOs, routes, or business logic.

Frontend developers and AI agents must always follow this document when working with APIs.

---

# API Philosophy

Frontend is a consumer.

Backend is the source of truth.

Never redesign backend APIs.

Never modify request or response structures.

Never change HTTP methods.

Never create fake endpoints.

Always consume existing APIs exactly as they are.

If an API is missing or incorrect

↓

Report the issue

↓

Wait for backend update

↓

Do not invent temporary APIs.

---

# Backend Responsibility

Backend is responsible for

Authentication

Authorization

Business Logic

Validation

Database

Transactions

File Storage

Security

Audit Logs

Response Structure

Frontend must never duplicate backend business logic.

---

# Frontend Responsibility

Frontend is responsible for

Displaying Data

Sending Requests

Client-side Validation

Loading States

Caching

Error Handling

Notifications

Responsive UI

Permission-based Rendering

User Experience

---

# API Layer Architecture

Every API call must follow this flow

Page

↓

Feature Hook

↓

API Service

↓

Axios Client

↓

Backend API

UI components must never call Axios directly.

---

# Folder Structure

src/

services/

api/

auth.api.ts

student.api.ts

teacher.api.ts

attendance.api.ts

guardian.api.ts

employee.api.ts

result.api.ts

fees.api.ts

dashboard.api.ts

settings.api.ts

reports.api.ts

Each module should have its own API service.

Never place all APIs inside one file.

---

# Axios Configuration

Create one global Axios instance.

Responsibilities

Base URL

Authorization Header

Content-Type

Request Timeout

Interceptors

Error Handling

Token Refresh (Future)

Retry (Future)

Never create multiple Axios instances.

---

# Request Rules

Always use

GET

Read Data

POST

Create Data

PUT

Update Entire Resource

PATCH

Partial Update

DELETE

Delete Resource

Never misuse HTTP methods.

---

# Query Parameters

Always send filters using query parameters.

Examples

Search

Pagination

Sorting

Status

Academic Year

Class

Section

Date Range

Never concatenate URLs manually.

Always use parameter objects.

---

# Authentication

Every protected request must automatically include

Bearer Token

School Context (If Required)

Academic Year (If Required)

Current Session (If Required)

Never manually attach tokens inside individual API calls.

---

# API Response

Frontend should consume backend responses exactly as returned.

Never transform response globally.

Transformation should happen only when required inside feature hooks.

---

# Error Handling

Every request must handle

401 Unauthorized

403 Forbidden

404 Not Found

409 Conflict

422 Validation Error

500 Internal Server Error

503 Service Unavailable

Never expose backend exception messages directly to users.

Display friendly messages.

---

# Loading Strategy

Every API request must support

Loading

Skeleton

Retry

Cancellation

Background Refresh

Never leave users waiting without feedback.

---

# API Security

Never expose

Connection Strings

Secrets

JWT Secret

Internal IDs

Database Details

Stack Traces

Frontend should trust backend validation.

---

# Final API Rule

Frontend never owns business logic.

Frontend only consumes, displays, validates user input, and communicates with the backend.

Backend remains the single source of truth for all business operations.

---