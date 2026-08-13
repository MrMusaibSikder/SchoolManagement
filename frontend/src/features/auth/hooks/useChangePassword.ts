import { useMutation } from "@tanstack/react-query";
import { changePasswordApi } from "../api/auth.api";
import type { ChangePasswordDto } from "../types/auth.types";

/**
 * TanStack Query mutation for the change-password request. Requires an
 * authenticated session (the authApiClient attaches the Bearer token).
 */
export function useChangePassword() {
  return useMutation({
    mutationFn: (payload: ChangePasswordDto) => changePasswordApi(payload),
  });
}
