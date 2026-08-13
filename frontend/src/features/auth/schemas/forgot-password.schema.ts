import { z } from "zod";

/**
 * Client-side validation for the forgot-password form. Mirrors the backend
 * FluentValidation rules (ForgotPasswordDtoValidator.cs): email is required
 * and must be a valid email address.
 *
 * UX layer only — the backend remains the source of truth.
 */
export const forgotPasswordSchema = z.object({
  email: z
    .string()
    .trim()
    .min(1, "Email is required.")
    .email("A valid email address is required."),
});

export type ForgotPasswordFormValues = z.infer<typeof forgotPasswordSchema>;
