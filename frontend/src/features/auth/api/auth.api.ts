/**
 * Authentication API layer — calls the real ASP.NET Core backend.
 *
 * Endpoints (see SchoolERP.Api/Controllers/AuthController.cs):
 *   POST /api/Auth/login          (anonymous)  → LoginResponseDto
 *   POST /api/Auth/refresh-token  (anonymous)  → RefreshTokenResponseDto
 *   POST /api/Auth/logout         ([Authorize]) → 204
 */
import type { AxiosError } from "axios";
import { publicApiClient } from "@/lib/api/public-client";
import { authApiClient } from "@/lib/api/auth-client";
import type {
  ChangePasswordDto,
  ForgotPasswordDto,
  LoginRequestDto,
  LoginResponseDto,
  LogoutRequestDto,
  RefreshTokenRequestDto,
  RefreshTokenResponseDto,
  RegisterRequestDto,
  RegisterResponseDto,
  ResetPasswordDto,
} from "../types/auth.types";

/** POST /api/Auth/login — returns the JWT + refresh token on success. */
export async function loginApi(
  payload: LoginRequestDto
): Promise<LoginResponseDto> {
  const { data } = await publicApiClient.post<LoginResponseDto>(
    "/Auth/login",
    payload
  );
  return data;
}

/** POST /api/Auth/refresh-token — exchanges a refresh token for a new pair. */
export async function refreshTokenApi(
  payload: RefreshTokenRequestDto
): Promise<RefreshTokenResponseDto> {
  const { data } = await publicApiClient.post<RefreshTokenResponseDto>(
    "/Auth/refresh-token",
    payload
  );
  return data;
}

/** POST /api/Auth/logout — revokes the refresh token (requires Bearer). */
export async function logoutApi(payload: LogoutRequestDto): Promise<void> {
  await authApiClient.post("/Auth/logout", payload);
}

/** POST /api/Auth/register — creates a user under an existing role. */
export async function registerApi(
  payload: RegisterRequestDto
): Promise<RegisterResponseDto> {
  const { data } = await publicApiClient.post<RegisterResponseDto>(
    "/Auth/register",
    payload
  );
  return data;
}

/** POST /api/Auth/change-password — updates the current user's password (Bearer). */
export async function changePasswordApi(
  payload: ChangePasswordDto
): Promise<void> {
  await authApiClient.post("/Auth/change-password", payload);
}

/** POST /api/Auth/forgot-password — requests a reset email (always 204). */
export async function forgotPasswordApi(
  payload: ForgotPasswordDto
): Promise<void> {
  await publicApiClient.post("/Auth/forgot-password", payload);
}

/** POST /api/Auth/reset-password — completes a reset with the emailed token. */
export async function resetPasswordApi(
  payload: ResetPasswordDto
): Promise<void> {
  await publicApiClient.post("/Auth/reset-password", payload);
}

/**
 * Extracts a human-friendly message from an Axios error for the login form.
 * The backend returns `{ message }` for 401 and FluentValidation `errors[]`
 * for 422/400.
 */
export function getAuthErrorMessage(error: unknown): string {
  const err = error as AxiosError<{ message?: string }>;
  const status = err?.response?.status;
  const data = err?.response?.data;

  if (status === 401) {
    return data?.message ?? "Invalid username/email or password.";
  }
  if (status === 422 || status === 400) {
    return data?.message ?? "Please check your input and try again.";
  }
  if (err?.code === "ECONNABORTED") {
    return "The request timed out. Please try again.";
  }
  return "Unable to connect to the server. Please try again.";
}
