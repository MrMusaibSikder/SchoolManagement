# Coding Standards

## Purpose

This document defines the coding standards for the School Management Frontend.

Always follow these rules.

Never violate them unless explicitly instructed.

---

# Tech Stack

Always use

- React 19
- TypeScript
- Vite
- Tailwind CSS
- Shadcn UI
- TanStack Query
- React Hook Form
- Zod
- Axios
- React Router DOM

Never use JavaScript.

---

# General Rules

Always

- Write clean code
- Write reusable code
- Use TypeScript
- Keep components small
- Reuse existing code
- Remove unused code
- Use meaningful names

Never

- Duplicate code
- Use any
- Hardcode values
- Leave console.log
- Leave commented code

---

# Naming Rules

Components

PascalCase

Examples

StudentForm

GuardianTable

DashboardCard

Functions

camelCase

Examples

getStudents

createTeacher

calculateGrade

Variables

camelCase

Constants

UPPER_CASE

Example

MAX_FILE_SIZE

Folders

lowercase

Example

students

teachers

attendance

---

# Component Rules

One component = One responsibility.

If a component becomes too large

Split it.

Prefer reusable components.

---

# API Rules

Never call axios inside pages.

Always

Page

↓

Hook

↓

API Service

↓

Backend

---

# Form Rules

Always use

React Hook Form

+

Zod

Every form must include

Validation

Loading

Submit State

Error Message

Success Message

---

# Table Rules

Every module must reuse

AppTable

Support

Search

Filter

Pagination

Sorting

Actions

Never create custom tables.

---

# UI Rules

Use existing components first.

Examples

AppButton

AppInput

AppModal

AppTable

AppSelect

AppBadge

AppLoader

EmptyState

PermissionGuard

Never redesign existing UI.

---

# Responsive Rules

Every page must support

Desktop

Laptop

Tablet

Mobile

No horizontal scrolling.

---

# Permission Rules

Hide unauthorized

Pages

Buttons

Menus

Actions

Do not show disabled actions unless required.

---

# Performance Rules

Use

Lazy Loading

TanStack Query Cache

Memoization when needed

Never fetch duplicate data.

---

# File Structure

Each feature

pages/

components/

hooks/

services/

types/

validation/

constants/

Keep related files together.

---

# Before Commit

Check

✓ No TypeScript errors

✓ No ESLint errors

✓ Responsive

✓ API connected

✓ Validation works

✓ Loading works

✓ Error handled

✓ Permission checked

✓ Build successful

Only then consider the feature complete.