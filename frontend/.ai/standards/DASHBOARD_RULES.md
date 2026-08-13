# Dashboard Rules

## Purpose

This document defines the design, layout, and development standards for the Dashboard.

The Dashboard is the first screen users see after login.

It must provide a clear overview of the system while remaining clean, modern, responsive, and easy to use.

Never build a simple statistics page.

Build a professional enterprise dashboard.

---

# Dashboard Objectives

The dashboard should

- Display important information at a glance
- Reduce unnecessary navigation
- Help users complete common tasks quickly
- Show only relevant information based on user role
- Maintain a premium and modern appearance

---

# Dashboard Layout

Desktop

--------------------------------------------------------
Header
--------------------------------------------------------
Welcome Banner
--------------------------------------------------------
Statistics Cards
--------------------------------------------------------
Charts
--------------------------------------------------------
Quick Actions | Recent Activities
--------------------------------------------------------
Notice Board | Academic Calendar
--------------------------------------------------------

Mobile

Header

↓

Welcome Card

↓

Statistics

↓

Charts

↓

Quick Actions

↓

Notices

↓

Activities

↓

Calendar

---

# Header

Always display

- School Logo
- School Name
- Global Search
- Notification Icon
- User Profile
- Logout

Optional

- Theme Toggle
- Language Switch

---

# Welcome Section

Display

- User Name
- User Role
- School Name
- Current Academic Year
- Current Session
- Current Date

Example

Good Morning, Musaib

Welcome back to School Management System.

---

# Statistics Cards

Show key statistics.

Examples

- Total Students
- Total Teachers
- Total Employees
- Total Guardians
- Total Classes
- Total Sections
- Today's Attendance
- Pending Fees
- Collected Fees
- Active Users

Every card should include

- Icon
- Title
- Value
- Short Description
- Trend (Optional)

Use reusable StatisticsCard component.

---

# Charts

Support

- Student Growth
- Attendance Trend
- Fee Collection
- Result Analysis

Use lightweight chart libraries.

Charts must be responsive.

---

# Quick Actions

Provide shortcuts for common tasks.

Examples

- Add Student
- Add Teacher
- Take Attendance
- Collect Fee
- Create Notice
- Create Exam
- View Reports

Show only actions allowed by user permissions.

---

# Recent Activities

Display recent system activities.

Examples

- New Student Added
- Teacher Updated
- Fee Collected
- Attendance Submitted
- Result Published

Show latest activities first.

---

# Notice Board

Display

- Latest Notices
- Important Announcements
- Upcoming Events

Allow users to view full notice details.

---

# Academic Calendar

Display

- Holidays
- Exams
- Events
- Meetings

Highlight today's date.

---

# Role-Based Dashboard

Super Admin

- Full Statistics
- All Modules
- Reports
- Analytics

School Admin

- School Statistics
- Academic Summary
- Fee Summary

Teacher

- Assigned Classes
- Today's Routine
- Attendance
- Upcoming Exams

Student

- Attendance
- Results
- Routine
- Notices

Guardian

- Child Information
- Attendance
- Results
- Fee Status
- Notices

Accountant

- Fee Collection
- Due Amount
- Payment Summary

Every user should only see relevant widgets.

---

# Widget Rules

Each widget must

- Be reusable
- Support loading state
- Support empty state
- Handle API errors
- Be responsive

Never hardcode data.

Always load from API.

---

# Card Design

Use

- Rounded Corners
- Soft Shadow
- Clean Background
- Consistent Padding
- Hover Effect
- Smooth Transition

Avoid

- Bright colors
- Heavy borders
- Cluttered layouts

---

# Responsive Design

Desktop

4 Cards Per Row

Laptop

3 Cards Per Row

Tablet

2 Cards Per Row

Mobile

1 Card Per Row

Avoid horizontal scrolling.

---

# Loading State

Use Skeleton Loader.

Never leave empty spaces.

---

# Empty State

If no data exists

Show

- Friendly Message
- Illustration
- Refresh Button

---

# Error State

If dashboard data fails

Display

- Friendly Error Message
- Retry Button

Never expose technical errors.

---

# Performance

Load dashboard widgets independently.

Do not block the entire page if one widget fails.

Cache dashboard requests using TanStack Query.

Lazy load heavy widgets such as charts.

---

# Accessibility

Support

- Keyboard Navigation
- Screen Readers
- Focus States
- High Contrast

---

# Final Rules

The dashboard must be

- Modern
- Clean
- Responsive
- Fast
- Reusable
- Accessible
- Permission Based
- Production Ready

It should feel comparable to premium commercial School ERP systems used in Bangladesh.

## Standard Dashboard Widgets

Required Widgets

- Welcome Card
- Statistics Cards
- Attendance Summary
- Fee Summary
- Student Growth Chart
- Result Analysis Chart
- Quick Actions
- Recent Activities
- Notice Board
- Academic Calendar
- Upcoming Exams
- Today's Birthdays
- Weather (Optional)
- System Status (Optional)