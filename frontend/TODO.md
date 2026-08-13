# Auth Features — Registration / Change Password / Forgot / Reset — TODO

## Backend confirmed (all exist in AuthController.cs — connect, don't duplicate)
- `POST /api/Auth/register` (anonymous) → RegisterResponseDto
- `POST /api/Auth/change-password` ([Authorize]) → 204
- `POST /api/Auth/forgot-password` (anonymous) → 204 (always, no account leak)
- `POST /api/Auth/reset-password` (anonymous) → 204 / 400 invalid token

## Steps
1. [x] Inspect backend AuthController.cs + AuthService.cs + DTOs + validators
2. [ ] Extend `src/features/auth/types/auth.types.ts` (Register/ChangePassword/Forgot/Reset DTOs)
3. [ ] Add `register/change-password/forgot-password/reset-password` APIs to `src/features/auth/api/auth.api.ts`
4. [ ] Add Zod schemas: `register.schema.ts`, `change-password.schema.ts`, `forgot-password.schema.ts`, `reset-password.schema.ts`
5. [ ] Add TanStack Query hooks: `useRegister.ts`, `useChangePassword.ts`, `useForgotPassword.ts`, `useResetPassword.ts`
6. [ ] Create pages: `RegisterPage.tsx`, `ForgotPasswordPage.tsx`, `ResetPasswordPage.tsx`, `ChangePasswordPage.tsx`
7. [ ] Wire navigation links between Login/Register/Forgot/Reset
8. [ ] Export new pages/hooks/types from `src/features/auth/index.ts`
9. [ ] Add routes in `src/App.tsx` (`/register`, `/forgot-password`, `/reset-password`, guarded `/change-password`)
10. [ ] Run `npm run build` and `npm run lint`
11. [ ] Test each flow against the real backend through the Vite proxy
12. [ ] Update `.ai/project/CURRENT_PROGRESS.md`
