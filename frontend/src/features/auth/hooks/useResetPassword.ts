import { useMutation } from "@tanstack/react-query";
import { resetPasswordApi } from "../api/auth.api";
import type { ResetPasswordDto } from "../types/auth.types";

/**
 * TanStack Query mutation for the reset-password request. Completes a password
 * reset using the token received via the forgot-password email.
 */
export function useResetPassword() {
  return useMutation({
    mutationFn: (payload: ResetPasswordDto) => resetPasswordApi(payload),
  });
}
