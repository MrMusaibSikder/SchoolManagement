import { z } from "zod";

/**
 * Client-side validation for the change-password form. Mirrors the backend
 * FluentValidation rules (ChangePasswordDtoValidator.cs):
 * - currentPassword: required
 * - newPassword: required, min 8, uppercase + lowercase + digit, and must be
 *   different from the current password
 * - confirmNewPassword: must match newPassword
 *
 * UX layer only — the backend remains the source of truth.
 */
export const changePasswordSchema = z
  .object({
    currentPassword: z.string().min(1, "Current password is required."),
    newPassword: z
      .string()
      .min(8, "New password must be at least 8 characters long.")
      .regex(/[A-Z]/, "New password must contain at least one uppercase letter.")
      .regex(/[a-z]/, "New password must contain at least one lowercase letter.")
      .regex(/[0-9]/, "New password must contain at least one digit."),
    confirmNewPassword: z.string().min(1, "Please confirm your new password."),
  })
  .refine((val) => val.newPassword !== val.currentPassword, {
    message: "New password must be different from the current password.",
    path: ["newPassword"],
  })
  .refine((val) => val.confirmNewPassword === val.newPassword, {
    message: "Password confirmation does not match the new password.",
    path: ["confirmNewPassword"],
  });

export type ChangePasswordFormValues = z.infer<typeof changePasswordSchema>;
