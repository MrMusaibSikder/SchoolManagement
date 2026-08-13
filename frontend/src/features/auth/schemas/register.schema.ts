import { z } from "zod";

/**
 * Client-side validation for the registration form.
 *
 * Mirrors the backend registration validation rules:
 * - username: required, max 100
 * - email: required, valid email, max 150
 * - password: required, min 8, must contain uppercase + lowercase + digit
 * - confirmPassword: must match password
 *
 * The backend remains the source of truth.
 * New public registrations are assigned the Student role by the backend.
 */
export const registerSchema = z
  .object({
    username: z
      .string()
      .trim()
      .min(1, "Username is required.")
      .max(100, "Username must be at most 100 characters."),

    email: z
      .string()
      .trim()
      .min(1, "Email is required.")
      .max(150, "Email must be at most 150 characters.")
      .email("A valid email address is required."),

    password: z
      .string()
      .min(8, "Password must be at least 8 characters long.")
      .regex(
        /[A-Z]/,
        "Password must contain at least one uppercase letter.",
      )
      .regex(
        /[a-z]/,
        "Password must contain at least one lowercase letter.",
      )
      .regex(/[0-9]/, "Password must contain at least one digit."),

    confirmPassword: z
      .string()
      .min(1, "Please confirm your password."),
  })
  .refine((val) => val.confirmPassword === val.password, {
    message: "Password confirmation does not match the password.",
    path: ["confirmPassword"],
  });

export type RegisterFormValues = z.infer<typeof registerSchema>;