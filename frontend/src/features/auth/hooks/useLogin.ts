import { useMutation } from "@tanstack/react-query";
import { loginApi } from "../api/auth.api";
import type { LoginRequestDto } from "../types/auth.types";

/**
 * TanStack Query mutation for the login request. The actual session
 * persistence + navigation is handled by the AuthProvider (which calls this
 * hook), keeping the UI decoupled from storage concerns.
 *
 * Returns the raw LoginResponseDto on success so the caller can build the
 * AuthSession and store it.
 */
export function useLogin() {
  return useMutation({
    mutationFn: (payload: LoginRequestDto) => loginApi(payload),
  });
}
