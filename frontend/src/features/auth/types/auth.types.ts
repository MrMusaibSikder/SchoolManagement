/**
 * Frontend types mirroring the backend ASP.NET Core authentication DTOs.
 * See SchoolERP/src/SchoolERP.Application/Features/Authentication/DTOs/.
 *
 * These intentionally mirror the backend contract exactly — do not invent or
 * omit fields (see .ai/standards/API_INTEGRATION.md).
 */

/** Request body for POST /api/Auth/login */
export interface LoginRequestDto {
  usernameOrEmail: string;
  password: string;
}

/** Response body for a successful POST /api/Auth/login */
export interface LoginResponseDto {
  userId: number;
  username: string;
  email: string;
  roles: string[];
  token: string; // JWT access token
  expiresAt: string; // UTC ISO datetime
  refreshToken: string;
  refreshTokenExpiresAt: string; // UTC ISO datetime
}

/** Request body for POST /api/Auth/refresh-token */
export interface RefreshTokenRequestDto {
  refreshToken: string;
}

/** Response body for a successful POST /api/Auth/refresh-token */
export interface RefreshTokenResponseDto {
  userId: number;
  username: string;
  email: string;
  roles: string[];
  token: string;
  expiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
}

/** Request body for POST /api/Auth/logout */
export interface LogoutRequestDto {
  refreshToken?: string | null;
}

/** Request body for POST /api/Auth/register */
export interface RegisterRequestDto {
  username: string;
  email: string;
  password: string;
  

}

/** Response body for a successful POST /api/Auth/register */
export interface RegisterResponseDto {
  userId: number;
  username: string;
  email: string;
  role: string;
}

/** Request body for POST /api/Auth/change-password (requires Bearer token) */
export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

/** Request body for POST /api/Auth/forgot-password */
export interface ForgotPasswordDto {
  email: string;
}

/** Request body for POST /api/Auth/reset-password */
export interface ResetPasswordDto {
  token: string;
  newPassword: string;
  confirmNewPassword: string;
}

/**
 * The authenticated session persisted locally so a browser refresh restores
 * the login (see DECISIONS.md — JWT + refresh token). Kept in one place so the
 * storage mechanism can change later without touching feature code.
 */
export interface AuthSession {
  userId: number;
  username: string;
  email: string;
  roles: string[];
  token: string;
  expiresAt: string;
  refreshToken: string;
  refreshTokenExpiresAt: string;
}
