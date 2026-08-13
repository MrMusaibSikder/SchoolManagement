import { z } from "zod";

/**
 * Client-side validation for the reset-password form. Mirrors the backend
 * FluentValidation rules (ResetPasswordDtoValidator.cs):
 * - token: required
 * - newPassword: required, min 8, uppercase + lowercase + digit
 * - confirmNewPassword: must match newPassword
 *
 * UX layer only — the backend remains the source of truth.
 */
export const resetPasswordSchema = z
  .object({
    token: z.string().trim().min(1, "Reset token is required."),
    newPassword: z
      .string()
      .min(8, "New password must be at least 8 characters long.")
      .regex(/[A-Z]/, "New password must contain at least one uppercase letter.")
      .regex(/[a-z]/, "New password must contain at least one lowercase letter.")
      .regex(/[0-9]/, "New password must contain at least one digit."),
    confirmNewPassword: z.string().min(1, "Please confirm your new password."),
  })
  .refine((val) => val.confirmNewPassword === val.newPassword, {
    message: "Password confirmation does not match the new password.",
    path: ["confirmNewPassword"],
  });

export type ResetPasswordFormValues = z.infer<typeof resetPasswordSchema>;
