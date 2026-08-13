import { useMutation } from "@tanstack/react-query";
import { forgotPasswordApi } from "../api/auth.api";
import type { ForgotPasswordDto } from "../types/auth.types";

/**
 * TanStack Query mutation for the forgot-password request. The backend always
 * returns 204 (it does not reveal whether the email exists), so the UI shows a
 * generic "check your email" success message regardless.
 */
export function useForgotPassword() {
  return useMutation({
    mutationFn: (payload: ForgotPasswordDto) => forgotPasswordApi(payload),
  });
}
