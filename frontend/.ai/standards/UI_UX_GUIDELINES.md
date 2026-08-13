# UI / UX Guidelines

## Purpose

This document defines the official UI/UX standards for the School Management Frontend.

Every page, component, layout, form, table, dashboard, dialog, and interaction must follow these standards.

These rules are mandatory.

Never ignore them.

The objective is to build an enterprise-grade commercial School ERP frontend suitable for real schools in Bangladesh.

---

# Design Philosophy

The application must feel

- Modern
- Clean
- Professional
- Lightweight
- Fast
- Premium
- Consistent
- Responsive
- Accessible
- Enterprise Ready

The interface should create confidence and trust.

Every screen should look polished.

Avoid experimental layouts.

Avoid outdated designs.

Avoid unnecessary decorations.

Keep the interface simple but powerful.

---

# Target Users

The application will be used by

- Super Admin
- School Admin
- Principal
- Vice Principal
- Teacher
- Accountant
- HR
- Employee
- Student
- Guardian
- Receptionist
- Librarian

Every user should immediately understand how to use the application without training.

---

# Design Inspiration

The overall experience should be inspired by modern enterprise software.

The UI should feel similar to commercial School ERP platforms commonly used in Bangladesh while maintaining a modern SaaS appearance.

The interface should never look like

- Bootstrap Admin Template
- Old ERP Software
- University Final Year Project
- Default React Template

The application must have its own professional identity.

---

# Design Principles

Always prioritize

Clarity

Consistency

Accessibility

Readability

Performance

Scalability

Maintainability

User Productivity

Every UI element must have a purpose.

If something does not improve usability, do not add it.

---

# Visual Identity

The interface should feel

Professional

Elegant

Organized

Minimal

Premium

Calm

Balanced

Friendly

Every page should maintain the same visual language.

Never mix multiple design styles.

---

# Layout Philosophy

Every page should follow the same structure.

Page Header

↓

Breadcrumb

↓

Page Actions

↓

Statistics (Optional)

↓

Filters

↓

Content

↓

Pagination

↓

Footer (Optional)

Never change this layout unless necessary.

---

# User Experience Principles

The user should never feel lost.

Every action must provide feedback.

Every important action must have confirmation.

Every page should clearly communicate

Where the user is

What the user can do

What happened

What should happen next

---

# Navigation Principles

Navigation should always be predictable.

Users should reach any page within a few clicks.

Use

Sidebar

Top Navigation

Breadcrumb

Quick Actions

Search

Never create confusing navigation.

---

# Consistency Rules

Buttons must always look identical.

Forms must always follow the same spacing.

Tables must always use the same layout.

Dialogs must always use the same structure.

Cards must always follow the same style.

Typography must remain consistent.

Colors must remain consistent.

Icons must remain consistent.

Spacing must remain consistent.

Never create different styles for similar components.

---

# Information Hierarchy

The user should immediately identify

Primary Information

Secondary Information

Actions

Warnings

Errors

Statistics

Use spacing, typography and color to create hierarchy.

Do not rely only on colors.

---

# White Space Rules

Always use generous spacing.

Never overcrowd the interface.

Allow the content to breathe.

Spacing improves readability.

Large enterprise applications prioritize spacing over decoration.

---

# User Productivity

Every page should reduce user effort.

Minimize unnecessary clicks.

Reduce typing whenever possible.

Use

Dropdowns

Auto Complete

Searchable Select

Default Values

Quick Actions

Recently Used Data

Smart Suggestions

The application should help users complete work faster.

---

# Smart Workflow

Whenever possible,

allow users to create related data without leaving the current page.

Example

Student Form

↓

Guardian does not exist

↓

Open Create Guardian Dialog

↓

Save Guardian

↓

Automatically select Guardian

↓

Continue Student Form

Apply the same behavior across all modules.

---

# Enterprise Quality Standard

Every page should look like it belongs to the same application.

There should be no difference in design quality between

Dashboard

Student Module

Teacher Module

Reports

Settings

Authentication

Landing Page

Everything should feel unified.

---

# Final Principle

Before completing any UI,

ask yourself:

Is this easy to understand?

Is this beautiful?

Is this responsive?

Is this reusable?

Is this accessible?

Is this production ready?

If the answer is NO,

improve it before finishing.
---

# Color System

The entire application must use one unified design system.

Never use random colors.

Every color must have a purpose.

The application should look professional and trustworthy.

Primary colors should represent the school brand.

Success colors should indicate completed operations.

Warning colors should indicate actions requiring attention.

Danger colors should only represent destructive actions.

Information colors should represent neutral notifications.

Never use bright or flashy colors.

Always maintain visual balance.

Recommended Color Palette

Primary

Blue

Secondary

Slate

Success

Emerald

Warning

Amber

Danger

Red

Info

Sky

Background

White

Surface

Slate-50

Text Primary

Slate-900

Text Secondary

Slate-600

Border

Slate-200

Hover

Slate-100

Disabled

Slate-300

Always use Tailwind color tokens.

Never hardcode HEX colors inside components.

Theme colors should be managed centrally.

---

# Dark Mode

The application must support future dark mode.

Do not hardcode light-only colors.

Always use semantic color classes.

Background

Foreground

Primary

Secondary

Muted

Border

Card

Popover

Input

Ring

Sidebar

Dashboard

Chart

Notification

Toast

Everything must automatically adapt to dark mode.

---

# Typography

Typography must remain consistent across the application.

Use one font family throughout the project.

Recommended

Inter

Fallback

System UI

Hierarchy

H1

Page Title

H2

Section Title

H3

Card Title

H4

Widget Title

Body

Normal Content

Caption

Small Information

Never randomly change font sizes.

Never mix typography styles.

Maintain consistent line heights.

Text should always remain readable.

---

# Font Weight

Page Title

Bold

Section Title

Semi Bold

Card Title

Medium

Body

Regular

Caption

Regular

Buttons

Medium

Navigation

Medium

Tables

Regular

---

# Spacing System

Always use consistent spacing.

Never manually guess spacing.

Recommended spacing scale

4

8

12

16

20

24

32

40

48

64

Never overcrowd components.

Give breathing space between sections.

Cards should have consistent padding.

Forms should have equal spacing.

Tables should have proper margins.

---

# Border Radius

Use one consistent radius system.

Small Components

Rounded

Medium Components

Rounded Large

Cards

Rounded XL

Dialogs

Rounded XL

Images

Rounded Large

Buttons

Rounded Medium

Avoid sharp corners.

---

# Shadows

Shadows should be subtle.

Do not use heavy shadows.

Cards

Soft Shadow

Dialogs

Medium Shadow

Dropdown

Soft Shadow

Popover

Soft Shadow

Hover

Slight Elevation

Avoid excessive depth.

---

# Borders

Borders should be light.

Use borders only where needed.

Avoid unnecessary outlines.

Use subtle separators between sections.

---

# Layout Width

The application should use responsive containers.

Maximum width should remain readable.

Avoid extremely wide forms.

Tables may occupy full width.

Forms should remain centered when appropriate.

---

# Grid System

Always use responsive grid layouts.

Desktop

Multi Column

Tablet

Adaptive Grid

Mobile

Single Column

Never force fixed widths.

---

# Cards

Cards must follow one consistent design.

Each card should include

Title

Optional Description

Content

Actions

Proper Padding

Soft Shadow

Rounded Corners

Hover Effect (when applicable)

Cards should never feel crowded.

---

# Dashboard Cards

Dashboard statistics cards should contain

Icon

Title

Current Value

Optional Trend

Optional Percentage

Optional Mini Chart

Cards must remain visually balanced.

Do not overload cards with information.

---

# Dividers

Use dividers only when they improve readability.

Avoid unnecessary lines.

Prefer spacing over borders whenever possible.

---

# Hover Effects

Every interactive element must provide visual feedback.

Buttons

Cards

Table Rows

Sidebar Menu

Dropdown Items

Navigation

Use smooth transitions.

Avoid aggressive animations.

---

# Transition Standards

Use consistent transitions.

Fast

Hover

Medium

Dialog

Slow

Page Loading

Never animate everything.

Animations should improve usability.

---

# Focus State

Every interactive component must have a visible focus state.

Buttons

Inputs

Links

Dropdowns

Checkboxes

Radio Buttons

Accessibility is mandatory.

---

# Scroll Behavior

Avoid nested scrolling.

Pages should scroll naturally.

Tables may use horizontal scrolling on small devices.

Dialogs should have controlled scrolling.

---

# Theme Consistency

Landing Page

Authentication

Dashboard

CRUD Pages

Reports

Settings

Dialogs

Tables

Forms

Everything must share the same visual identity.

Users should instantly recognize the application.

---

# Final Theme Rule

Never introduce a new design style without updating the design system.

Consistency is more valuable than creativity.

Every screen must feel like part of one unified enterprise application.
---

# Form Design Standards

Forms are one of the most frequently used parts of the application.

Every form must be clean, consistent, responsive, and easy to complete.

The goal is to reduce user effort while preventing mistakes.

Never create long confusing forms.

Split large forms into logical sections.

Use Cards, Tabs, Accordions or Step Forms whenever necessary.

---

# Form Layout

Always use a consistent layout.

Recommended order

Page Header

↓

Description

↓

Form Card

↓

Form Sections

↓

Action Buttons

↓

Help Information (Optional)

---

# Form Sections

Group related information together.

Example

Student Information

Guardian Information

Academic Information

Address Information

Emergency Contact

Documents

Images

Never mix unrelated fields.

---

# Required Fields

Every required field must display

*

Example

Student Name *

Email *

Phone *

Never surprise users during submission.

Users should know required fields immediately.

---

# Labels

Every input must have a clear label.

Never use placeholders as labels.

Labels should always remain visible.

Use sentence case.

Good

Student Name

Bad

studentName

Bad

STUDENT NAME

---

# Placeholder

Placeholders should provide examples.

Example

Enter student name

Enter guardian phone number

Enter email address

Never repeat the label.

---

# Input Components

Use the appropriate component.

Text

Textarea

Number

Password

Date Picker

Time Picker

Checkbox

Radio

Switch

Searchable Select

Multi Select

File Upload

Image Upload

Color Picker (if needed)

Never use a text input for every field.

---

# Searchable Dropdown

Whenever a foreign key exists

Always use a searchable dropdown.

Never use a simple HTML select for large datasets.

Search should be instant.

Keyboard navigation should be supported.

---

# Smart Related Record Creation

Whenever related data does not exist

Allow users to create it without leaving the current page.

Example

Student

↓

Guardian Not Found

↓

Click "Create Guardian"

↓

Dialog Opens

↓

Save Guardian

↓

Automatically Select Guardian

↓

Continue Student Form

This behavior should be available in every module where applicable.

Examples

Student → Guardian

Teacher → Department

Employee → Designation

Subject → Category

Exam → Session

Routine → Classroom

Never force users to navigate away.

---

# Form Validation

Always validate before submission.

Validation should happen

On Blur

On Change (where appropriate)

Before Submit

Validation messages must be

Short

Friendly

Specific

Helpful

Never expose technical errors.

Example

Good

Phone number is required.

Bad

Validation failed.

---

# Submit Buttons

Buttons should clearly communicate their purpose.

Examples

Save

Update

Submit

Approve

Assign

Generate Report

Delete

Never use generic labels like

OK

Yes

Done

---

# Button Placement

Primary button

Right side

Secondary button

Left side

Danger button

Separated from primary action

---

# Loading State

Never leave users wondering.

During API requests

Disable buttons

Show loading spinner

Prevent duplicate submission

Keep previous data visible when possible.

Use Skeleton for page loading.

Use Spinner for button loading.

Never block the entire application.

---

# Skeleton Standards

Always prefer skeleton loaders instead of large spinners.

Skeleton should match the final layout.

Use Skeleton for

Dashboard

Cards

Tables

Forms

Profile Pages

Reports

Lists

Avoid layout shifting.

---

# Empty States

Every page must have a meaningful empty state.

Show

Illustration (optional)

Message

Short explanation

Primary action

Example

"No students found."

"Create your first student."

Never show blank pages.

---

# Error States

Every API error should be handled gracefully.

Display

Friendly title

Simple explanation

Retry button

Never expose server exceptions.

---

# Success Feedback

Successful actions must provide clear feedback.

Use toast notifications.

Recommended library

Sonner

Examples

Student created successfully.

Guardian updated successfully.

Attendance submitted successfully.

---

# Confirmation Dialog

Confirmation is required for

Delete

Bulk Delete

Reset

Approve

Reject

Archive

Restore

Dialogs should clearly explain the consequence.

---

# Image Upload

Every image uploader must support

Image Preview

Drag & Drop

Browse

Replace

Remove

File Validation

Size Validation

Type Validation

Loading Indicator

Default Placeholder

Broken Image Fallback

Image Preview should update instantly.

---

# Avatar

Student

Teacher

Employee

Guardian

Users without images should display

Initials

or

Default Avatar

Never show broken images.

---

# Tables

Every table must support

Search

Filter

Sorting

Pagination

Column Visibility

Bulk Selection

Bulk Action

Export

Print

Status Badge

Action Menu

Responsive Layout

Sticky Header (when appropriate)

Loading State

Empty State

Error State

Tables should remain fast even for large datasets.

---

# Status Badges

Never use plain text.

Display statuses using badges.

Examples

Active

Inactive

Pending

Approved

Rejected

Paid

Unpaid

Promoted

Graduated

Use consistent colors.

---

# Action Menu

Avoid too many buttons inside table rows.

Use a dropdown action menu.

Examples

View

Edit

Delete

Assign

Print

History

Download

Only show actions allowed by permissions.

---

# Final Component Rule

Every component must be

Reusable

Responsive

Accessible

Permission Aware

API Ready

Production Ready

If a component cannot be reused,
redesign it before implementation.
---

# Landing Page Standards

The application must not open directly to the login page.

Instead, users should first see a professional public landing page representing the school.

The landing page should build trust and provide important information before authentication.

---

# Landing Page Objectives

The landing page should

Introduce the school

Provide important statistics

Display achievements

Show announcements

Provide quick access to login

Support admissions

Support public visitors

Look modern and premium

Load quickly

Be fully responsive

---

# Landing Page Layout

Top Announcement Bar (Optional)

↓

Top Navigation

↓

Hero Section

↓

Quick Statistics

↓

About School

↓

Principal Message (Optional)

↓

Features

↓

Latest News

↓

Upcoming Events

↓

Gallery Preview

↓

Testimonials

↓

FAQ

↓

Contact Information

↓

Google Map

↓

Footer

---

# Hero Section

The hero section should contain

School Name

School Logo

Short Description

Background Illustration or Image

Primary Call To Action

Secondary Call To Action

Example

Login

Online Admission

Explore School

Contact Us

---

# Public Statistics

Display live statistics fetched from the backend.

Examples

Total Students

Total Teachers

Total Employees

Total Staff

Total Classes

Total Departments

Total Subjects

Current Academic Year

Success Rate

Passing Rate

Awards

Years of Excellence

These statistics should be displayed using premium statistic cards.

---

# About School

Display

History

Mission

Vision

Core Values

Short Introduction

Read More Button

---

# News Section

Display latest notices

Latest news

Events

Circulars

Admission Notices

Each card should contain

Image

Title

Publish Date

Short Description

Read More

---

# Gallery Section

Display

Campus Images

Events

Programs

Sports

Cultural Activities

Science Fair

Only show a limited number.

Provide View All button.

---

# Contact Section

Display

School Address

Phone

Email

Office Hours

Social Media

Google Maps

Contact Form

---

# Footer

Footer should contain

School Logo

Quick Links

Useful Links

Support

Privacy Policy

Terms

Contact

Copyright

Social Media

---

# Authentication Pages

Authentication should feel premium.

Never create a plain login page.

The login page should include

School Branding

Logo

Welcome Message

Background Illustration

Remember Me

Forgot Password

Show Password

Password Visibility Toggle

Loading State

Validation

Responsive Layout

Dark Mode Ready

---

# Forgot Password

Support

Email

Username

Phone (if backend supports)

OTP (if backend supports)

Reset Password

Success Feedback

---

# Dashboard Philosophy

Every dashboard should provide useful information immediately.

Users should understand their work within five seconds.

Avoid unnecessary widgets.

Avoid visual clutter.

---

# Dashboard Types

Admin Dashboard

Principal Dashboard

Teacher Dashboard

Employee Dashboard

Student Dashboard

Guardian Dashboard

Each dashboard should display only information relevant to that role.

---

# Dashboard Layout

Header

↓

Statistics Cards

↓

Charts

↓

Quick Actions

↓

Recent Activities

↓

Upcoming Events

↓

Recent Notices

↓

Recent Reports

↓

Calendar

↓

Tasks (Optional)

---

# Dashboard Statistics

Every role should have unique statistics.

Examples

Admin

Students

Teachers

Employees

Revenue

Attendance

Pending Fees

Pending Admissions

Teacher

Today's Classes

Attendance

Upcoming Exams

Assigned Subjects

Homework

Student

Attendance

Results

Assignments

Fees

Routine

Guardian

Children

Attendance

Results

Fees

Notifications

---

# Charts

Use clean and lightweight charts.

Recommended library

Recharts

Possible charts

Bar Chart

Line Chart

Area Chart

Pie Chart

Donut Chart

Avoid unnecessary animations.

Charts must be responsive.

---

# Calendar

Use a calendar widget to display

Events

Exams

Meetings

Holidays

Leave

Assignments

Calendar should support quick navigation.

---

# Sidebar

Sidebar must remain consistent across the application.

Support

Collapsible Mode

Nested Menu

Search

Permission Based Visibility

Active Menu Highlight

Icons

Badges

Smooth Animation

Responsive Drawer

Sidebar should remember collapse state.

---

# Navbar

Navbar should contain

Breadcrumb

Search

Notification

Messages (Optional)

Profile Menu

Theme Switch (Future Ready)

Language Switch (Future Ready)

School Selector (If Multi School)

---

# Notifications

Support

Unread Count

Mark as Read

Mark All Read

Navigate to Related Page

Time Ago Display

Permission Based Notifications

---

# Quick Actions

Every dashboard should provide shortcuts.

Examples

Create Student

Take Attendance

Create Teacher

Collect Fees

Create Exam

Generate Report

Create Employee

These actions should be permission based.

---

# Recent Activities

Display recent system activities.

Examples

Student Added

Attendance Submitted

Exam Created

Result Published

Fee Collected

Employee Joined

Only display activities relevant to the logged-in user.

---

# Dashboard Performance

Dashboard must load quickly.

Statistics should load independently.

Charts should lazy load if necessary.

Use skeleton loaders while loading.

Avoid blocking the page.

---

# Mobile Dashboard

Cards should stack vertically.

Sidebar should become a drawer.

Charts should resize automatically.

Tables should become scrollable.

Quick actions should remain accessible.

---

# Final Dashboard Rule

Every dashboard should answer three questions immediately.

What needs attention?

What happened recently?

What can I do next?

If the dashboard cannot answer these questions, redesign it before implementation.
---

# Accessibility Standards

The application must be usable by everyone.

Accessibility is mandatory.

Every interactive component must support

Keyboard Navigation

Visible Focus State

Screen Reader Labels

Proper HTML Semantics

ARIA Attributes (where necessary)

Color Contrast

Meaningful Icons

Form Labels

Accessible Tables

Accessible Dialogs

Never rely only on colors to communicate information.

Always provide text, icon, or badge.

---

# Loading Strategy

Never leave users waiting without feedback.

Use

Skeleton Loading

Button Loading

Progress Indicators

Lazy Loading

Incremental Loading

Background Fetching

Keep previous data visible whenever possible.

Recommended

TanStack Query Loading States

Shadcn Skeleton

CSS Spinner

Never block the whole application while loading one component.

---

# Skeleton Rules

Skeleton must match the final layout.

Use skeletons for

Dashboard

Tables

Cards

Forms

Profile

Reports

Charts

Sidebar

Do not show random placeholder blocks.

---

# Empty State Standards

Every page must have a meaningful empty state.

Every empty state should contain

Illustration or Icon

Title

Description

Primary Action

Example

No Students Found

Create your first student to get started.

Never display an empty white screen.

---

# Error Handling UI

Errors must be user-friendly.

Every error should contain

Friendly Title

Short Description

Retry Button

Back Button (if needed)

Technical details must never be shown to end users.

Log technical errors internally.

---

# Notification Standards

Use toast notifications for

Create

Update

Delete

Restore

Upload

Download

Import

Export

Login

Logout

Permission Errors

Recommended Library

Sonner

Toast should appear

Top Right

Auto Close

Pause on Hover

Success

Warning

Error

Info

---

# Dialog Standards

Every dialog must support

Keyboard Close

Escape Key

Backdrop Click (where appropriate)

Loading State

Validation

Responsive Layout

Scrollable Content

Confirmation before destructive actions.

---

# Drawer Standards

Use drawers for

Quick Create

Quick Edit

Quick View

Filters

Small Forms

Avoid using full pages for small tasks.

---

# Animation Standards

Animations should improve usability.

Never use excessive animations.

Use subtle transitions for

Buttons

Cards

Dropdowns

Sidebar

Dialogs

Hover

Navigation

Recommended duration

150ms

200ms

300ms

Avoid long animations.

---

# Micro Interaction Standards

Provide visual feedback for

Hover

Click

Selection

Loading

Success

Failure

Drag & Drop

Image Upload

Dropdown Selection

Forms

Micro interactions should make the application feel alive without being distracting.

---

# Search Experience

Every searchable page should support

Instant Search

Debounce

Highlight Matching Text

Clear Button

Search History (optional)

Search Suggestions (future)

Search must remain fast.

---

# Filter Experience

Filters should be

Easy to understand

Collapsible (if large)

Resettable

Persist while navigating

Support multiple filters

Never require unnecessary clicks.

---

# Pagination Standards

Support

Page Number

Next

Previous

Page Size

Total Records

Jump to Page

Keep current page after refresh whenever possible.

---

# Export Standards

Every report-ready module should support

Print

PDF

Excel

CSV (optional)

Export should respect

Current Filters

Current Sorting

Permissions

Selected Rows

---

# Print Standards

Print layouts should

Hide unnecessary UI

Show School Logo

Show Header

Show Footer

Show Print Date

Show Printed By

Fit A4 paper

Avoid cutting tables.

---

# Image Standards

Images must support

Lazy Loading

Preview

Fallback Image

Responsive Display

Error Handling

Consistent Ratio

Compression (if required)

Never stretch images.

---

# Performance Standards

Optimize

Rendering

Images

API Calls

Bundle Size

Queries

Re-renders

Memoization where appropriate.

Use lazy loading for pages.

Use code splitting.

Cache API responses.

Never fetch the same data multiple times unnecessarily.

---

# Security Standards

Never expose

JWT Token

Sensitive Data

Server Errors

Stack Traces

Internal IDs (where unnecessary)

Always respect backend permissions.

Hide unauthorized actions from the UI.

---

# Responsive Standards

Every page must work correctly on

Desktop

Laptop

Tablet

Mobile

Large Screen

No broken layout is acceptable.

---

# Enterprise UX Rules

Always minimize clicks.

Always simplify workflows.

Always reuse existing UI.

Always prioritize speed.

Always prioritize readability.

Always prioritize consistency.

Never surprise the user.

---

# Final Golden Rules

Before completing any feature, verify

✓ Responsive

✓ Accessible

✓ Validated

✓ Permission Based

✓ API Connected

✓ Loading Implemented

✓ Error Handling Complete

✓ Empty State Complete

✓ Skeleton Added

✓ Reusable Components Used

✓ No TypeScript Errors

✓ No Build Errors

✓ No Console Errors

✓ Production Ready

If any item is missing,

the task is NOT complete.

---

# Ultimate Objective

Build a frontend that feels like a premium commercial School ERP used by thousands of schools.

Every page should be clean.

Every interaction should be smooth.

Every component should be reusable.

Every feature should be scalable.

Every design decision should improve user productivity.

Never compromise quality.

Always think like a Senior Frontend Architect before writing code.