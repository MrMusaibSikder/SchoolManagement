# Project Context

## Project Name

School Management System (SchoolERP)

## Repository Layout

```
SchoolERP/
├── frontend/          React 19 + TypeScript + Vite + Tailwind + Shadcn
├── backend/           ASP.NET Core Web API + SQL Server
└── .ai/               Root AI documentation (this folder)
```

## Technology Stack

| Layer | Stack |
|-------|-------|
| Frontend | React 19, TypeScript, Vite, Tailwind CSS, Shadcn UI, TanStack Query, React Hook Form, Zod, Axios |
| Backend | ASP.NET Core Web API, SQL Server, JWT, FluentValidation, AutoMapper |
| Auth | JWT Bearer + refresh tokens |

## Goals

Build a commercial-quality School ERP for Bangladesh — modern, responsive, permission-aware, and production-ready.

## Backend Status

**Completed.** All business APIs exist. Frontend consumes them as-is.

## Frontend Status

**In progress.** See `CURRENT_PROGRESS.md` for module-by-module status.

## Dev Setup

| Service | URL |
|---------|-----|
| Backend HTTPS | `https://localhost:7083` |
| Backend HTTP | `http://localhost:5053` (redirects to HTTPS) |
| Frontend dev | `http://localhost:5173` |
| API proxy | Vite proxies `/api` → backend |

Default seeded admin: `admin` / `Admin@123`

## AI Agent Scope

- **Default:** frontend only (`frontend/src/`)
- **Backend changes:** only when user explicitly requests
- **Documentation:** maintain both root `.ai/` and `frontend/.ai/`
