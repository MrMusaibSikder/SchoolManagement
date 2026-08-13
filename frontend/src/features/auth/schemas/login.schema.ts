import { z } from "zod";

/**
 * Client-side validation for the login form. Mirrors the backend FluentValidation
 * rules (LoginRequestDtoValidator.cs): UsernameOrEmail and Password are required.
 * This is a UX layer only — the backend remains the source of truth.
 */
export const loginSchema = z.object({
  usernameOrEmail: z
    .string()
    .trim()
    .min(1, "Username or email is required."),
  password: z
    .string()
    .min(1, "Password is required."),
});

export type LoginFormValues = z.infer<typeof loginSchema>;
