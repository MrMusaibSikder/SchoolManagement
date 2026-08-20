# Project Structure

## Root

```
SchoolERP/
├── .ai/                    Root AI documentation
├── frontend/
│   ├── .ai/                Frontend AI documentation (detailed)
│   ├── src/
│   │   ├── features/       Feature modules (auth, dashboard, academic, …)
│   │   ├── landing/        Public landing page
│   │   ├── layouts/        AppShell
│   │   ├── components/ui/  Shadcn primitives
│   │   ├── lib/            API clients, utilities
│   │   └── config/         Environment config
│   └── package.json
└── backend/
    ├── src/
    │   ├── SchoolERP.Api/           Controllers, auth, middleware
    │   ├── SchoolERP.Application/   DTOs, interfaces, validators
    │   ├── SchoolERP.Domain/        Entities, enums
    │   └── SchoolERP.Infrastructure/ Services, repositories, EF
    └── SchoolERP.sln
```

## Frontend Feature Module Pattern

```
features/<module>/
├── api/           Axios calls
├── hooks/         TanStack Query hooks
├── types/         TypeScript interfaces
├── pages/         Route pages
├── components/    Module-specific UI (optional)
├── schemas/       Zod schemas (forms)
└── index.ts       Public exports
```

## Backend Layer Pattern

```
Features/<Name>/
├── DTOs/
├── Interfaces/
└── Validators/

Controllers/<Name>Controller.cs  →  Service  →  Repository
```

## Key API Clients

| Client | File | Use |
|--------|------|-----|
| Public | `frontend/src/lib/api/public-client.ts` | Anonymous `/api/public/*` |
| Auth | `frontend/src/lib/api/auth-client.ts` | Bearer token, auto 401 redirect |
