# Frontend Architecture

## Purpose

This document defines the official frontend architecture for the School Management System.

Every feature, page, component, hook, service, utility, and API integration must follow this architecture.

Never violate this architecture unless explicitly instructed.

---

# Architecture Philosophy

The frontend must be

Enterprise Ready

Scalable

Maintainable

Reusable

Modular

Feature Driven

Performance Optimized

Easy to Understand

Easy to Extend

Future Proof

The project should continue growing for years without requiring major restructuring.

---

# Architectural Principles

Always

Separate UI from Business Logic

Separate API from Components

Separate Validation from Forms

Separate Routing from Pages

Separate Shared Components from Feature Components

Separate Feature State from Global State

Never mix responsibilities.

One file should have one responsibility.

---

# Project Structure

The project should follow Feature Based Architecture.

Example

src/

app/

assets/

components/

features/

hooks/

layouts/

lib/

pages/

routes/

services/

store/

types/

utils/

constants/

styles/

contexts/

providers/

config/

Each folder has one clear responsibility.

---

# Folder Responsibilities

app/

Application configuration.

Application providers.

Global initialization.

Global wrappers.

App.tsx

main.tsx

Theme

Query Client

Router

Authentication Provider

---

assets/

Static files.

Images

Icons

Fonts

Illustrations

Videos

Logos

SVG

Never place business files here.

---

components/

Reusable shared UI components.

Example

Button

Input

Modal

Drawer

Table

Card

Badge

Avatar

Search

Pagination

Date Picker

Uploader

This folder must never contain business logic.

---

features/

Business modules.

Every module has its own folder.

Example

Student

Teacher

Guardian

Employee

Attendance

Fees

Payroll

Exam

Results

Library

Inventory

Settings

Each feature contains

Pages

Components

Hooks

API

Validation

Types

Constants

Utils

Feature level logic.

---

hooks/

Reusable hooks.

Example

useAuth

usePagination

usePermission

useDebounce

useDialog

useSearch

useUpload

useExport

Never create duplicate hooks.

---

layouts/

Application layouts.

Example

Main Layout

Dashboard Layout

Authentication Layout

Landing Layout

Report Layout

Print Layout

---

pages/

Top level route pages only.

Business logic should remain inside features.

Pages should be lightweight.

---

routes/

Centralized routing.

Authentication routes.

Protected routes.

Public routes.

Role based routes.

Permission routes.

Lazy loading.

---

services/

Global API services.

Axios instance.

Authentication.

Refresh Token.

File Upload.

Notification.

Global APIs.

Feature specific APIs should remain inside features.

---

store/

Global state.

Only truly global data belongs here.

Theme

Authentication

Language

Current School

Never store feature state globally without reason.

---

types/

Global TypeScript types.

Enums.

Interfaces.

Generic models.

API response models.

Pagination models.

Never duplicate types.

---

utils/

Reusable helper functions.

Date formatting.

Currency formatting.

Export.

Validation helper.

String helper.

File helper.

Image helper.

Permission helper.

---

constants/

Application constants.

Routes

Roles

Permissions

Colors

Storage Keys

API Constants

Never hardcode repeated values.

---

styles/

Global styles.

Tailwind overrides.

Fonts.

Animations.

Theme variables.

---

contexts/

React Context providers.

Authentication

Theme

Notification

School

Language

Only use Context for global cross-cutting concerns.

---

providers/

React Providers.

Query Provider

Theme Provider

Router Provider

Auth Provider

Error Boundary

Notification Provider

Application wrappers belong here.

---

config/

Application configuration.

Environment

API URL

Feature Flags

Application Settings

Never hardcode configuration values.

---

# Feature Architecture

Every feature should follow the same structure.

Feature/

components/

pages/

api/

hooks/

schemas/

types/

constants/

utils/

index.ts

Keep every feature isolated.

Avoid dependencies between unrelated features.

---

# Dependency Rules

Pages

↓

Feature Components

↓

Shared Components

↓

Hooks

↓

API

↓

Backend

Never reverse this dependency order.

---

# Import Rules

Prefer absolute imports.

Avoid long relative paths.

Group imports consistently.

External Libraries

↓

Internal Modules

↓

Components

↓

Hooks

↓

Types

↓

Styles

Maintain import order across the project.

---

# Reusability Rules

Before creating any component

Search the project.

If a reusable version already exists,

reuse it.

Never create duplicate components.

---

# Final Architecture Rule

Architecture is more important than speed.

A clean architecture saves hundreds of hours in future development.

Never sacrifice architecture for quick implementation.
---

# Feature Module Architecture

Every business module must follow one consistent structure.

Never create custom structures for different modules.

Every module must be independent, maintainable, and scalable.

Each module should contain everything required for that feature.

Example modules

Student

Guardian

Teacher

Employee

Class

Section

Subject

Attendance

Routine

Exam

Result

Payroll

Library

Inventory

Transport

Settings

Every module follows exactly the same architecture.

---

# Standard Module Structure

features/

student/

api/

components/

hooks/

pages/

schemas/

types/

constants/

utils/

index.ts

Never place feature code outside its module unless it is reusable.

---

# Pages Folder

The pages folder contains route-level pages.

Examples

StudentListPage

StudentCreatePage

StudentEditPage

StudentDetailsPage

StudentProfilePage

StudentReportPage

StudentImportPage

StudentExportPage

Pages should only

Build Layout

Connect Components

Handle Navigation

Never write heavy business logic inside pages.

---

# Components Folder

Contains feature-specific UI.

Example

StudentForm

StudentTable

StudentCard

StudentStatistics

StudentFilter

StudentSearch

StudentAvatar

StudentProfileHeader

StudentTimeline

StudentAttendanceCard

StudentGuardianCard

StudentFeeSummary

StudentResultSummary

StudentQuickActions

StudentDocuments

StudentImageUploader

StudentStatusBadge

StudentDialogs

StudentDrawer

StudentDeleteDialog

Every component must have one responsibility.

---

# API Folder

Contains only API communication.

Example

student.api.ts

Functions

getStudents()

getStudent()

createStudent()

updateStudent()

deleteStudent()

restoreStudent()

searchStudents()

exportStudents()

importStudents()

uploadStudentImage()

Never call Axios directly from Components.

Always use API files.

---

# Hooks Folder

Contains feature hooks.

Examples

useStudents()

useStudent()

useCreateStudent()

useUpdateStudent()

useDeleteStudent()

useSearchStudents()

useStudentStatistics()

useStudentPermission()

Hooks should combine

API

Query

Mutation

Business Logic

Never duplicate hooks.

---

# Validation Folder

Contains all validation.

Example

student.schema.ts

Use

Zod

Never validate directly inside components.

Validation must remain reusable.

---

# Types Folder

Contains feature types.

Example

Student.ts

CreateStudentDto.ts

UpdateStudentDto.ts

StudentDetails.ts

StudentStatistics.ts

StudentFilter.ts

StudentReport.ts

Never duplicate backend DTOs.

Create frontend models when necessary.

---

# Constants Folder

Contains module constants.

Examples

Student Status

Blood Group

Gender

Admission Type

Guardian Relation

Dropdown Options

Never hardcode repeated values.

---

# Utils Folder

Contains helper functions.

Examples

Student Name Formatter

Age Calculator

Profile Image Helper

Admission Number Formatter

Export Helper

Import Helper

Date Formatter

Keep utilities pure.

---

# Index File

Every feature should expose public APIs.

Example

export *

Pages

Components

Hooks

API

Types

Keep imports clean.

---

# CRUD Standard

Every module must support

List

Create

Edit

Delete

Restore

View Details

Search

Filter

Sorting

Pagination

Bulk Selection

Bulk Delete

Import

Export

Print

Permission

Validation

Responsive Design

Loading

Skeleton

Error State

Empty State

Success Feedback

If backend supports additional operations,

implement them.

---

# Smart Forms

Every create/edit form should support

Auto Save (Future Ready)

Real Time Validation

Foreign Key Dropdown

Related Entity Creation

Image Preview

File Upload

Reset

Cancel

Unsaved Changes Warning

Keyboard Navigation

Confirmation before leaving

---

# Related Entity Creation

When a foreign key exists

Always provide

Search

Dropdown

Quick Create

Refresh List

Auto Select New Record

Example

Student

↓

Guardian

↓

Create Guardian

↓

Save

↓

Auto Select

↓

Continue Student Form

Apply this pattern to all modules.

---

# File Upload Standard

If backend accepts files,

support

Image Preview

Drag & Drop

Replace

Delete

Download

Compression (Optional)

Validation

Loading

Retry

---

# List Page Standard

Every list page must contain

Breadcrumb

Page Title

Description

Statistics

Search

Filters

Quick Actions

Table

Pagination

Bulk Actions

Export

Import

Floating Action Button (Optional)

Permission Based Actions

Never create plain tables.

---

# Details Page Standard

Every details page should contain

Profile Header

Summary Cards

Basic Information

Related Information

Timeline

History

Attachments

Activity Log

Quick Actions

Print

Export

Responsive Layout

---

# Performance Rule

Feature modules should load independently.

Use lazy loading.

Avoid importing unnecessary components.

Optimize bundle size.

Cache API requests.

Prefetch important data where appropriate.

---

# Final Module Rule

Each feature module must be capable of being developed, tested, maintained, and extended independently.

No module should tightly depend on another module except through officially defined APIs.

A developer should understand the module structure within minutes without reading additional documentation.
---

# API Layer Architecture

## Purpose

This document defines how every frontend module communicates with the backend.

The backend is already completed.

Never redesign the backend.

Never modify API endpoints.

Never change request DTOs.

Never change response DTOs.

Always consume the existing backend APIs.

---

# API Design Principles

Every API request must be

Secure

Reusable

Typed

Maintainable

Cancelable

Cached

Error Handled

Permission Aware

Production Ready

Never call axios directly inside components.

---

# API Layer Structure

Every feature must have its own API layer.

Example

features/

student/

api/

student.api.ts

guardian/

api/

guardian.api.ts

teacher/

api/

teacher.api.ts

employee/

api/

employee.api.ts

Never mix APIs from different modules.

---

# Axios Instance

Use one centralized axios instance.

The axios instance is responsible for

Base URL

Authorization Header

JWT Token

Refresh Token

Request Interceptor

Response Interceptor

Timeout

Global Error Handling

Never create multiple axios instances.

---

# Request Flow

React Component

↓

React Hook

↓

TanStack Query

↓

Feature API

↓

Axios Instance

↓

Backend API

Never bypass this flow.

---

# Response Flow

Backend

↓

Axios

↓

Feature API

↓

TanStack Query

↓

React Hook

↓

UI Component

---

# Authentication

Every protected request must automatically include

Authorization

Bearer Token

Never manually attach JWT inside components.

Axios should do this automatically.

---

# Refresh Token

If backend supports Refresh Token

Automatically

Detect Expired Token

Refresh Token

Retry Request

Continue User Session

Never ask the user to login again unless refresh fails.

---

# Query Strategy

Use TanStack Query for all server state.

Never manually manage API loading state using useState.

Always use

useQuery()

useMutation()

invalidateQueries()

prefetchQuery()

keepPreviousData()

queryClient

---

# Query Keys

Every feature must have centralized query keys.

Example

students

student

teachers

teacher

attendance

attendance-report

Never hardcode query keys.

---

# Cache Strategy

List Pages

Cache

Single Details

Cache

Dashboard

Cache

Dropdown Data

Long Cache

Settings

Long Cache

Statistics

Short Cache

Reports

No Cache unless required

Always invalidate cache after

Create

Update

Delete

Restore

Import

Bulk Update

---

# Pagination Strategy

Backend pagination must be used.

Never load all records.

Support

Page

Page Size

Search

Filter

Sorting

Total Records

Never implement client-side pagination for large datasets.

---

# Search Strategy

Search must be server-side whenever supported.

Use

Debounce

Minimum Character Validation

Clear Search

Search should not fire on every keystroke.

---

# Filter Strategy

Filters should support

Status

Date

Academic Year

Department

Class

Section

Session

Gender

Custom Filters

Filters must synchronize with URL when appropriate.

---

# File Upload Strategy

Support

Image Upload

Document Upload

Multiple Files

Drag & Drop

Preview

Progress Bar

Retry Upload

Replace File

Delete File

Show upload progress.

Never freeze the UI.

---

# Download Strategy

Support

PDF

Excel

Images

Documents

Progress

Retry

Permission Validation

---

# Error Handling

Every API call must handle

400

401

403

404

409

422

500

Network Error

Timeout

Display user-friendly messages.

Never expose raw server exceptions.

---

# Retry Policy

Retry automatically only for

Temporary Network Failure

Timeout

Do not retry

Validation Errors

Permission Errors

Authentication Errors

---

# Optimistic Updates

Use optimistic updates only when

Safe

Simple

Reversible

Otherwise

Invalidate Queries

Refetch

---

# Background Refresh

Important dashboard data should refresh automatically.

Dropdown data should not refresh unnecessarily.

Avoid excessive API requests.

---

# Permission Awareness

Never call APIs that the user cannot access.

Hide restricted actions before the request.

Respect backend authorization.

---

# API Security

Never

Store JWT in unsafe places.

Log sensitive information.

Expose API URLs unnecessarily.

Expose backend error messages.

Always sanitize user input before sending requests.

---

# API Performance

Avoid duplicate requests.

Reuse cached data.

Prefetch frequently used data.

Cancel outdated requests.

Lazy load heavy resources.

Minimize payload size.

Never request unnecessary data.

---

# API Naming Convention

Examples

getStudents

getStudent

createStudent

updateStudent

deleteStudent

restoreStudent

importStudents

exportStudents

uploadStudentImage

downloadStudentReport

Names must clearly describe the action.

---

# Final API Rule

The frontend must never assume backend behavior.

Always follow the backend API contract.

If an API does not exist,

do not invent one.

Instead,

report the missing endpoint and wait for further instructions.

The frontend architecture must always remain fully compatible with the existing ASP.NET Core Web API.
---

# Authentication & Permission Architecture

## Purpose

This document defines the complete authentication, authorization, routing, dashboard selection, and permission architecture.

Authentication and authorization must remain centralized.

Never duplicate authentication logic.

Never bypass permission validation.

Always respect backend authorization.

---

# Authentication Flow

The application startup flow must always follow this order.

Application Start

↓

Initialize Providers

↓

Load Environment

↓

Initialize Query Client

↓

Check Stored Authentication

↓

Validate Token

↓

Load Current User

↓

Load User Roles

↓

Load User Permissions

↓

Load School Information

↓

Render Application

Never render protected pages before authentication is verified.

---

# Public Application Flow

Unauthenticated users should always enter through the Landing Page.

Landing Page

↓

About School

↓

Notice Board

↓

Events

↓

Gallery

↓

Contact

↓

Login

↓

Forgot Password

↓

Authenticate

↓

Dashboard

Never redirect directly to Login when opening the application.

Landing Page is always the first experience.

---

# Authentication Pages

Authentication pages include

Login

Forgot Password

Reset Password

Change Password

Unauthorized

Session Expired

404

403

Authentication pages should use Authentication Layout.

Never display Dashboard Layout before login.

---

# JWT Authentication

Authentication uses

JWT Access Token

Refresh Token (if backend supports)

User Information

Role Information

Permission Information

School Information

The frontend should never decode business rules from the token.

Always use backend responses.

---

# Login Flow

User enters credentials

↓

Validate Form

↓

Call Login API

↓

Receive JWT

↓

Store Authentication

↓

Load Current User

↓

Load Roles

↓

Load Permissions

↓

Redirect Based On Role

Never hardcode redirect destinations.

---

# Logout Flow

Logout

↓

Clear Access Token

↓

Clear Refresh Token

↓

Clear Query Cache

↓

Clear User State

↓

Clear Permission State

↓

Redirect To Landing Page

Never leave sensitive information in memory.

---

# Protected Routes

Protected pages require

Valid Authentication

Valid Permission

Valid Role

Active User

Active School

Unauthorized users must never access protected pages.

---

# Public Routes

Public routes include

Landing

Login

Forgot Password

Contact

About

Gallery

Notice

Admission Information

FAQ

Public routes should never require authentication.

---

# Role Based Dashboard

Every role has its own dashboard.

Examples

Super Admin

Admin

Principal

Teacher

Employee

Student

Guardian

Each dashboard should contain only relevant information.

Never show unnecessary widgets.

---

# Dynamic Sidebar

Sidebar must be generated dynamically.

Menu visibility depends on

Role

Permission

School Configuration

Module Availability

Never hardcode sidebar items.

---

# Sidebar Features

Support

Nested Menus

Search

Collapse

Expand

Permission Filtering

Badge Count

Icons

Active State

Remember Collapse State

Responsive Drawer

Smooth Animation

---

# Permission Architecture

Every UI element should respect permissions.

Permissions include

View

Create

Update

Delete

Restore

Approve

Reject

Print

Export

Import

Assign

Generate Report

Permission checks should apply to

Routes

Sidebar

Buttons

Tables

Dialogs

Forms

Reports

Dashboard Widgets

Never show unauthorized actions.

---

# Route Guards

Every protected route should validate

Authentication

Permission

Role

School Context

Session

Only then render the page.

---

# User Session

Maintain

Current User

Current School

Current Role

Current Permissions

Current Theme

Current Language

Restore session automatically after refresh.

---

# Unauthorized Access

If user lacks permission

Show

403 Page

Friendly Message

Back Button

Home Button

Request Access (Future)

Never expose restricted content.

---

# Multi Layout Architecture

The application should support multiple layouts.

Landing Layout

Authentication Layout

Dashboard Layout

Report Layout

Print Layout

Error Layout

Each layout has a dedicated responsibility.

---

# Navigation Rules

Navigation must always be predictable.

Every page should include

Breadcrumb

Page Title

Current Location

Back Navigation (where applicable)

Quick Actions

Never allow users to get lost.

---

# School Context

If the system supports multiple schools

Load School Context after authentication.

All data should respect the selected school.

Never mix school data.

---

# Session Expiration

If authentication expires

Attempt Refresh Token

If refresh succeeds

Continue normally.

If refresh fails

Redirect to Login

Preserve intended destination if possible.

---

# Error Pages

Provide professional pages for

401 Unauthorized

403 Forbidden

404 Not Found

500 Internal Error

Maintenance Mode (Future)

Pages should remain consistent with the application theme.

---

# Security Rules

Never expose

JWT

Refresh Token

Sensitive User Data

Permission Structure

Internal APIs

Never trust frontend validation.

Backend remains the final authority.

---

# Final Authentication Rule

Authentication, authorization, routing, layouts, dashboard selection, and permissions must work together as one unified system.

The user should only see what they are authorized to access.

Everything else should remain completely hidden.
---

# State Management Architecture

## Purpose

This document defines how application state should be managed.

Use the simplest solution possible.

Never store server state inside React Context.

Never duplicate state.

Always separate

Server State

Client State

UI State

Application State

---

# State Categories

The frontend contains four different types of state.

1.

Server State

(API Data)

↓

TanStack Query

2.

Global Application State

↓

React Context

3.

Component State

↓

useState

4.

Form State

↓

React Hook Form

Never mix these responsibilities.

---

# Server State

Server state includes

Students

Teachers

Employees

Attendance

Results

Fees

Dashboard

Reports

Settings

School Information

Always use

TanStack Query

Never use useEffect + useState for API requests.

---

# Client State

Client state includes

Sidebar Collapse

Current Theme

Current Language

Current School

Current User

Notifications

Current Academic Year

These may use Context.

---

# Component State

Use local state only for

Dialog Open

Drawer Open

Tab Selection

Accordion

Popover

Dropdown

Hover

Temporary UI

Never store API data here.

---

# Form State

Every form must use

React Hook Form

Zod Resolver

Never manually synchronize form values.

React Hook Form manages the entire form.

---

# Global Context

Context should only contain

Authentication

Theme

Language

School

Notification

App Settings

Never place business module data inside Context.

---

# Query Client

Create one global Query Client.

Configure

Retry

Cache

Stale Time

Garbage Collection

Background Refetch

Window Focus

Reconnect

Offline Support (Future)

Do not create multiple Query Clients.

---

# Query Cache

Cache should be intelligent.

Examples

Dropdown Data

Long Cache

Statistics

Medium Cache

Dashboard

Short Cache

Reports

No Cache

Search Results

Temporary Cache

Always invalidate affected queries after mutations.

---

# Query Keys

Every module should define centralized query keys.

Example

studentKeys

teacherKeys

employeeKeys

guardianKeys

attendanceKeys

feesKeys

Never use string literals throughout the project.

---

# Mutation Strategy

Every mutation should

Show Loading

Disable Button

Handle Errors

Show Success Toast

Invalidate Queries

Refresh Data

Close Dialog (if needed)

Never manually refresh pages.

---

# Optimistic Update

Only use optimistic updates when

Safe

Predictable

Easy to Rollback

Otherwise

Invalidate

Refetch

Synchronize

---

# Prefetch Strategy

Prefetch commonly used data.

Examples

Dropdown Lists

Current User

Dashboard

Academic Year

School Settings

Permission List

Avoid unnecessary waiting.

---

# Lazy Loading

Lazy load

Pages

Charts

Large Reports

Heavy Components

Never lazy load

Buttons

Inputs

Small Components

Common UI

---

# Theme Management

Support

Light Theme

Dark Theme (Future)

System Theme (Future)

Theme should persist after refresh.

Never hardcode colors.

Use semantic design tokens.

---

# Language Management

Application should support future localization.

Default

English

Future

Bangla

Arabic

Others

Never hardcode reusable labels inside components.

---

# School Context

If multiple schools exist

Store Current School

Current Academic Year

Current Session

Current Branch

Every request should respect current context.

---

# Notification State

Manage

Unread Count

Notification List

Read Status

Real Time Updates (Future)

Never fetch notifications repeatedly.

---

# Error Boundary

Wrap application with

Global Error Boundary

Feature Error Boundary (Optional)

If a component crashes

Display friendly fallback UI.

Never crash the whole application.

---

# Offline Support

Future Ready

Detect Offline

Queue Requests (Future)

Notify User

Reconnect Automatically

---

# Performance Strategy

Avoid unnecessary renders.

Use

Memoization

Lazy Loading

Query Caching

Code Splitting

Stable References

Debounced Search

Virtual Lists (Future)

Never optimize prematurely.

Optimize where needed.

---

# Final State Rule

Every state must have a clear owner.

Server Data

↓

TanStack Query

Application Settings

↓

Context

Temporary UI

↓

useState

Forms

↓

React Hook Form

Never break these responsibilities.
---

# Enterprise Scalability Architecture

## Purpose

The frontend architecture must support continuous growth without major restructuring.

The project should remain maintainable even after adding hundreds of modules, thousands of components, and millions of records.

Never build only for today's requirements.

Always design for future expansion.

---

# Feature Independence

Every feature must be completely independent.

Each feature should contain

API

Pages

Components

Hooks

Schemas

Types

Constants

Utilities

Tests (Future)

Documentation (Future)

Removing one feature must not break another feature.

---

# Dynamic Module Registration

The application should support adding new modules with minimal changes.

When creating a new module

Register Route

Register Sidebar

Register Permission

Register Dashboard Widget (Optional)

Register Report (Optional)

Register Search (Optional)

No other part of the application should require modification.

---

# Dynamic Sidebar Architecture

Sidebar should be generated dynamically.

Sidebar items must depend on

Role

Permission

School Configuration

Installed Modules

Feature Availability

Never hardcode menus.

Support

Nested Menus

Badges

Icons

Groups

Search

Collapse

Favorites (Future)

Recent Pages (Future)

---

# Dashboard Engine

Dashboard should be modular.

Each widget should be an independent component.

Examples

Attendance Widget

Fee Widget

Student Widget

Teacher Widget

Notice Widget

Calendar Widget

Activity Widget

Chart Widget

Quick Action Widget

Widgets should load independently.

A failed widget must never break the entire dashboard.

---

# Dynamic Form Engine

Forms should follow one reusable pattern.

Every form should support

Validation

Loading

Image Upload

Foreign Key Dropdown

Quick Create Related Data

Permission Check

Responsive Layout

Reusable Sections

Future features

Autosave

Draft

Version History

---

# Dynamic Table Engine

Every table should use one reusable table system.

Required features

Server Pagination

Server Search

Server Filter

Sorting

Column Visibility

Export

Print

Bulk Actions

Sticky Header

Responsive

Loading

Skeleton

Empty State

Error State

Permission Based Actions

Never build tables from scratch if a reusable table exists.

---

# Smart Foreign Key Workflow

Whenever a relation exists

Automatically detect

Foreign Key

Load Dropdown Data

Enable Search

Support Quick Create

Refresh List

Auto Select Newly Created Record

This workflow should be applied consistently across the application.

---

# Import & Export Architecture

Every module should be ready for

Excel Import

Excel Export

PDF Export

Print

CSV (Optional)

Bulk Import Validation

Import Error Report

Download Template

Import History (Future)

---

# Report Architecture

Reports should be independent from CRUD pages.

Every report should support

Filter

Search

Date Range

Export

Print

Permission

Pagination (if required)

School Branding

Report Header

Generated By

Generated Date

Reports should never duplicate CRUD pages.

---

# Notification Architecture

Support

Toast

System Notification

Real-Time Notification (Future)

Email Notification (Future)

SMS Notification (Future)

Push Notification (Future)

Notifications should be modular.

---

# Performance Architecture

Always optimize

Images

API Requests

Bundle Size

Components

Rendering

Queries

Memory Usage

Never fetch unnecessary data.

Never render unnecessary components.

---

# Code Splitting

Always lazy load

Pages

Feature Modules

Large Dialogs

Charts

Reports

Heavy Components

Never lazy load

Buttons

Inputs

Icons

Badges

Utility Components

---

# Reusable Design System

Never redesign existing components.

Reuse

Button

Input

Table

Dialog

Drawer

Card

Badge

Avatar

Uploader

Select

Search

Filter

Pagination

Toast

Loader

Skeleton

Empty State

Error State

Everything must belong to one unified design system.

---

# Logging Strategy

Development

Console logs allowed only for debugging.

Production

No console logs.

Errors should be handled through centralized error handling.

---

# Build Quality

The project must always compile successfully.

Never leave

TypeScript Errors

ESLint Errors

Build Errors

Unused Imports

Unused Variables

Commented Production Code

Temporary Code

Mock Data

Demo Components

Debug Statements

---

# AI Development Rules

Before writing code

Understand the requirement.

Analyze the project.

Analyze dependencies.

Reuse existing components.

Plan the implementation.

Only then write code.

After implementation

Review

Refactor

Optimize

Validate

Test mentally

Never stop after writing code.

---

# Future Ready Architecture

The project should support future integration of

Multi School

Multi Campus

Multi Language

Dark Mode

Offline Mode

PWA

Push Notifications

AI Assistant

Chat Module

Video Classes

Online Exams

Online Payment

Biometric Attendance

Cloud Storage

Audit Logs

Analytics

No architectural changes should be required for these future enhancements.

---

# Enterprise Quality Checklist

Every completed feature must satisfy

✓ Clean Architecture

✓ Reusable Components

✓ Type Safety

✓ Responsive Design

✓ Accessibility

✓ Validation

✓ API Integration

✓ Permission Check

✓ Loading State

✓ Skeleton

✓ Empty State

✓ Error Handling

✓ Success Feedback

✓ Image Preview

✓ File Upload

✓ Search

✓ Filter

✓ Pagination

✓ Export

✓ Print

✓ Mobile Support

✓ Tablet Support

✓ Desktop Support

✓ No Build Errors

✓ No TypeScript Errors

✓ Production Ready

---

# Final Architecture Principle

Always think like a Senior Frontend Architect.

Do not optimize only for today's task.

Optimize for the next five years.

Every decision must improve

Scalability

Maintainability

Readability

Performance

Developer Experience

User Experience

Business Growth

Never compromise architecture for speed.

Quality always comes first.

This document is the official architecture reference for the entire School Management Frontend.

