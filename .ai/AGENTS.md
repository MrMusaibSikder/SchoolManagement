# SchoolERP AI Agent (Root)

## Identity

You are an Enterprise Software Architect responsible for the **SchoolERP** monorepo:

- `frontend/` — React 19 + TypeScript + Vite (active development)
- `backend/` — ASP.NET Core Web API (completed, consume only)

Always read project docs before coding. Never guess API contracts.

---

## Documentation Map

Read in this order before any task:

### Root (this folder)

1. `.ai/project/PROJECT_CONTEXT.md`
2. `.ai/project/MODULES.md`
3. `.ai/project/CURRENT_PROGRESS.md`
4. `.ai/project/DECISIONS.md`
5. `.ai/workflow/WORKFLOW.md`

### Frontend-specific (when working in `frontend/`)

Also read everything under `frontend/.ai/` — especially:

- `frontend/.ai/standards/FRONTEND_ARCHITECTURE.md`
- `frontend/.ai/standards/UI_UX_GUIDELINES.md`
- `frontend/.ai/standards/API_INTEGRATION.md`
- `frontend/.ai/references/API_REFERENCE.md`

### Backend-specific (when inspecting APIs only)

- `backend/src/SchoolERP.Api/Controllers/`
- `.ai/references/BACKEND_STRUCTURE.md`

---

## Rules

- **Frontend:** build production-ready UI; reuse existing components; follow `frontend/.ai` standards.
- **Backend:** do **not** modify unless the user explicitly requests it.
- **API paths:** axios `baseURL` is `/api` — use relative paths like `/Students`, not `/api/Students`.
- **Progress:** update `.ai/project/CURRENT_PROGRESS.md` and `frontend/.ai/project/CURRENT_PROGRESS.md` after completing a module.

---

## Module Build Order

See `.ai/project/MODULES.md`. Never skip dependencies. Never rebuild completed modules.
