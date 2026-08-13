import { useMutation } from "@tanstack/react-query";
import { registerApi } from "../api/auth.api";
import type {
  RegisterRequestDto,
  RegisterResponseDto,
} from "../types/auth.types";

/**
 * TanStack Query mutation for the registration request. Returns the created
 * user's summary on success so the caller can navigate to login.
 */
export function useRegister() {
  return useMutation({
    mutationFn: (payload: RegisterRequestDto) => registerApi(payload),
  });
}

export type { RegisterResponseDto };
