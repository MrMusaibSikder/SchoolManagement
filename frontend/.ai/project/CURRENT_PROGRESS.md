# Current Progress

## Purpose

This file tracks the current development status of the frontend.

Before starting any new task: Always read this file.

After completing a task: Always update this file.

---

# Project Status

**Project Name:** School Management System

**Backend:** ✅ Completed

**Frontend:** 🟡 In Progress

---

# Completed

- Project Setup
- Frontend Architecture
- UI/UX Guidelines
- Coding Standards
- Module Standards
- Project Documentation

---

# Current Task

✅ **Authentication** — Phase 1 — **COMPLETE & VERIFIED**

JWT-based login flow (reads real backend through the Vite `/api` proxy) plus
the full registration & password-management flows wired to the existing
backend `AuthController` endpoints (no duplicated backend APIs):
- `POST /api/Auth/login` → `{ userId, username, email, roles[], token,
  expiresAt, refreshToken, refreshTokenExpiresAt }` — mirrored exactly in
  `src/features/auth/types/auth.types.ts` from `AuthController.cs` +
  `LoginResponseDto`.
- `POST /api/Auth/register` (anonymous) → `RegisterResponseDto`
  (`{ userId, username, email, role }`). `RegisterPage.tsx` at `/register`
  with RHF+Zod validation mirroring the backend FluentValidation password
  rule (min 8, uppercase + lowercase + digit), a fixed self-registration role
  set (Student / Guardian / Teacher), show/hide password toggles, and
  redirect to `/login` on success.
- `POST /api/Auth/change-password` (Bearer) → 204. `ChangePasswordPage.tsx`
  at `/change-password`, guarded by `ProtectedRoute`; requires current
  password, new password differs from current, confirm matches.
- `POST /api/Auth/forgot-password` (anonymous) → always 204 (does not reveal
  whether the email exists). `ForgotPasswordPage.tsx` at `/forgot-password`
  shows a neutral "check your email" success state.
- `POST /api/Auth/reset-password` (anonymous) → 204. `ResetPasswordPage.tsx`
  at `/reset-password` reads the emailed `token` from the query string
  (`?token=…`) and submits the new password pair.
- All four flows reuse the existing `publicApiClient` (register/forgot/reset —
  anonymous) / `authApiClient` (change — Bearer), use relative `/Auth/*`
  paths (no double `/api`), and reuse `getAuthErrorMessage` for friendly
  error messages.
- New TanStack Query mutations: `useRegister`, `useForgotPassword`,
  `useResetPassword`, `useChangePassword`; new Zod schemas: `register`,
  `change-password`, `forgot-password`, `reset-password`; DTO types added to
  `auth.types.ts`; all exported from `features/auth/index.ts`.
- `src/App.tsx` routes: `/`, `/login`, `/register`, `/forgot-password`,
  `/reset-password` (public), `/change-password` (protected).

## Verification

- `tsc -b` (typecheck) ✅ | `npm run build` (vite build) ✅ | `eslint .` ✅.
- Login already verified end-to-end against the real backend (valid seeded
  login `admin`/`Admin@123` → 200 with JWT+refresh; invalid → 401). The new
  register/forgot/reset/change flows call the *existing* `AuthController`
  endpoints (verified present in `AuthController.cs`) through the same Vite
  proxy path.
- Session persisted in `localStorage` via `src/features/auth/store/auth.store.ts`
  (restored on reload only if the access token is still valid).
- Authed axios client `src/lib/api/auth-client.ts` — auto-attaches the Bearer
  token and, on a 401, clears the stored session and redirects to `/login`.
- `AuthProvider` (`src/features/auth/context/`) exposes `session`,
  `isAuthenticated`, `login`, `logout`; `useAuth`/`useLogin` hooks consume it.
- `LoginPage` (`src/features/auth/pages/LoginPage.tsx`) — react-hook-form +
  Zod validation + show/hide password + loading state + friendly 401/422/
  network error messages via `getAuthErrorMessage`.
- `ProtectedRoute` (`src/features/auth/components/ProtectedRoute.tsx`) — guards
  authed routes, redirects to `/login` preserving the original path.
- Routing wired in `src/App.tsx` (`/` → LandingPage, `/login` → LoginPage,
  wrapped in `AuthProvider`); `src/main.tsx` mounts `QueryClientProvider` +
  `BrowserRouter` + Sonner `<Toaster>`.
- Backend refresh-token (`POST /api/Auth/refresh-token`) and logout
  (`POST /api/Auth/logout`) functions are stubbed in `auth.api.ts` — silent
  auto-refresh interceptor is a documented follow-up (see Notes).

## Verification (real backend, end-to-end)

- Backend running on `https://localhost:7083` (dev cert trusted) — also HTTP
  `:5053` (307 → HTTPS). Frontend dev server on `http://localhost:5173` with
  `/api` proxy → `https://localhost:7083` (`secure:false` for the self-signed
  dev cert). Confirmed listening via `netstat` (PID 22520) and
  `GET /api/public/stats` → 200.
- Request path confirmed: browser sends `POST http://localhost:5173/api/Auth/login`
  (API base URL is the relative `/api`), Vite proxies it to
  `https://localhost:7083/api/Auth/login` — same-origin, so CORS is avoided
  (backend CORS policy `_myAllowSpecificOrigins` only allows :5173/:3000).
- **Valid login** (seeded `admin` / `Admin@123` from `UserSeeder.cs`) through
  the proxy returns a real session:
  `{ userId:1, username:"admin", email:"admin@schoolerp.com", roles:["Admin"],
  token:<JWT>, expiresAt, refreshToken, refreshTokenExpiresAt }` — matches the
  frontend `LoginResponseDto`/`AuthSession` shape exactly.
- **Invalid login** returns `401 {"message":"Invalid username/email or password."}`
  — handled by `getAuthErrorMessage()` in the login form.
- Frontend auth types/endpoints cross-checked against the real
  `SchoolERP.Api/Controllers/AuthController.cs` and its DTOs/validator.
- `tsc -b` (typecheck) ✅ | `vite build` (1998 modules) ✅ | `eslint .` ✅ (0 errors).

## Notes

- **Root-cause fix (404 on login):** `auth.api.ts` originally posted to
  `/api/Auth/login` while `publicApiClient`/`authApiClient` already set
  `baseURL = /api` (env `VITE_API_BASE_URL=/api`), so axios joined them into
  `/api/api/Auth/login` → 404. Changed the three endpoints to relative paths
  (`/Auth/login`, `/Auth/refresh-token`, `/Auth/logout`), matching the landing
  `public.api.ts` convention. Verified: `npm run build` passes, and live
  through the Vite proxy invalid login → 401 and valid seeded login
  (`admin`/`Admin@123`) → 200 with a real JWT + refresh token. The browser
  now sends `POST http://localhost:5173/api/Auth/login` (no double `/api`).
- Silent **refresh-token** auto-refresh on 401 (DECISIONS.md/logic in
  `auth-client.ts`) is deliberately left as a follow-up — the current
  interceptor clears session + redirects and works for the first pass.
- `POST /api/Auth/logout` is fire-and-forget (best effort); local session is
  always cleared regardless of network result.
- `src/pages/LoginPlaceholder.tsx` is a leftover placeholder and can be
  removed once the app shell/dashboard route target exists.

## Foundation (completed)

- Added centralized API config (`src/config/env.ts`, `VITE_API_BASE_URL`,
  default `https://localhost:7083`) — points at HTTPS directly to avoid the
  backend's 307 HTTP→HTTPS redirect and the cross-origin cert handshake it
  triggers in the browser.
- Added no-auth public Axios client (`src/lib/api/public-client.ts`) — a bare
  axios instance that never sends an `Authorization` header (DECISIONS.md #8).
- Wired TanStack Query (`QueryClientProvider`) + React Router
  (`BrowserRouter`) in `src/main.tsx`; routes `/` → `LandingPage`,
  `/login` → placeholder (`src/pages/LoginPlaceholder.tsx`).
- Added Vite dev proxy `/api` → `https://localhost:7083` with `secure:false`
  (trusts the self-signed ASP.NET Core dev cert; avoids CORS in dev).
- Added `src/landing/` feature: `api/public.api.ts`, `hooks/`
  (`useSchoolInfo`, `usePublicStats`, `usePublicNotices`), `types/`
  (`public.types.ts` — `summary?: string | null` nullable), `pages/`
  (`LandingPage`), `components/` (`HeroSection`, `StatsPlaque`, `NoticeBoard`
  with conditional summary + priority pin colors, `PublicFooter`).
- Extended `src/index.css` with Landing Page theme tokens (`bg-paper`,
  `text-ink`, `bg-gold`, `bg-cork`, `text-forest`, `bg-rust`, `font-display`,
  `animate-fade-up`) — the shadcn theme and dark mode are preserved.

## Verification (real backend, end-to-end)

- Backend running on `https://localhost:7083` (dev cert trusted). Frontend dev
  server on `http://localhost:5173` with `/api` proxy → HTTPS backend.
- Verified all three public endpoints return **real data** through the Vite
  proxy (the same path the browser uses):
  - `GET /api/public/school-info` → 200
    `{"name":"Badalpara High School And College","address":"Badalpara","phone":"01681439385","email":"badalpara@gmail.com"}`
  - `GET /api/public/stats` → 200 `{"totalStudents":1,"totalTeachers":0,"totalEmployees":1}`
  - `GET /api/public/notices?take=5` → 200 `[]` (renders the empty state)
- Headless Chrome DOM dump confirmed the app mounts and renders the full
  LandingPage (hero, stats plaque, notice board, footer) with correct
  loading skeletons while data is in flight.
- Loading / empty / error states all handled in the components (skeletons,
  "No public notices at the moment.", friendly error message).
- `public-client.ts` confirmed to send **no** `Authorization` header
  (DECISIONS.md #8 compliance).
- `tsc -b` (typecheck) ✅ | `vite build` (136 modules, 553ms) ✅ | `eslint .` ✅ (0 errors).

## Notes

- Notices are currently empty in the DB, so the empty state is what renders —
  the card layout + priority pin colors are exercised as soon as an
  `Everyone`-audience notice is published.
- Starced `totalStudents`/`totalEmployees` are live counts from the DB; the
  numbers will change as data is seeded. No fake/mock data anywhere.

---

# Next Priority (After Authentication)

1. Dashboard
2. Academic Management
3. Student Module
4. Guardian Module
5. Teacher Module
6. Employee Module
7. Attendance Module
8. Examination Module
9. Result Module

---

# Development Rules

For every module:

✔ Analyze backend APIs first
✔ Reuse existing components
✔ Follow UI guidelines
✔ Add validation
✔ Connect APIs
✔ Test responsiveness
✔ Check permissions
✔ Handle loading and errors

Only then mark the module complete.

---

# Module Status

| Module | Status |
|---------|--------|
| Landing Page | ✅ Completed |
| Authentication | ✅ Completed |
| Dashboard | ⏳ Pending |
| Academic | ⏳ Pending |
| Student | ⏳ Pending |
| Guardian | ⏳ Pending |
| Teacher | ⏳ Pending |
| Employee | ⏳ Pending |
| Attendance | ⏳ Pending |
| Examination | ⏳ Pending |
| Result | ⏳ Pending |
| Fee Management | ⏳ Pending |
| Reports | ⏳ Pending |
| Settings | ⏳ Pending |

---

# When A Module Is Finished

Update Status: Pending → In Progress → Completed

Example:
Student: ⏳ Pending → 🚧 In Progress → ✅ Completed

---

# Important

Never rebuild a completed module.

Always continue from the last completed task.

If implementation is interrupted, resume from the current module instead of starting over.