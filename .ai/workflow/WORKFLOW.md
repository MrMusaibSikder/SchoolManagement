# Development Workflow (Root)

## Before Any Task

1. Read `.ai/project/CURRENT_PROGRESS.md`
2. Read `.ai/project/MODULES.md` — confirm dependencies are met
3. If frontend work: read `frontend/.ai/` standards in order (see `frontend/AGENTS.md`)

## Implementation Steps

```
Understand → Analyze backend API → Analyze existing frontend code
→ Plan → Implement → Validate → Update progress docs → Review
```

## Per Module Checklist

- [ ] Backend controller + DTO verified
- [ ] Types match backend (camelCase JSON)
- [ ] API service uses correct path and response shape
- [ ] TanStack Query hooks with loading/error/empty states
- [ ] Form validation (RHF + Zod) where applicable
- [ ] Responsive layout
- [ ] Permission gates (when auth profile wired)
- [ ] `tsc -b` + `vite build` pass
- [ ] Update `CURRENT_PROGRESS.md` (root + frontend)

## Do Not

- Modify backend without explicit user request
- Duplicate existing components
- Mark modules complete with placeholder UI
- Create modules with no backend API
