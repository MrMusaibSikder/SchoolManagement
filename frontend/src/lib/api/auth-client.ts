/**
 * Authenticated Axios instance for protected endpoints.
 *
 * This client automatically attaches the JWT access token (from the auth
 * store) to every request, and centralizes 401 handling (session expiry →
 * clear session + redirect to /login). It is deliberately separate from
 * `public-client.ts`, which must NEVER send an Authorization header
 * (DECISIONS.md #8).
 *
 * Note on token refresh: the backend exposes POST /api/Auth/refresh-token.
 * A full silent-refresh interceptor (queuing requests while one refresh is in
 * flight, retrying after refresh) is a follow-up; for now a 401 clears the
 * session and redirects to login. The refresh endpoint is still wired in
 * `auth.api.ts` so it can be used explicitly.
 */
import axios, { AxiosError } from "axios";
import { API_BASE_URL, API_TIMEOUT_MS } from "@/config/env";
import {
  clearStoredSession,
  getStoredSession,
} from "@/features/auth/store/auth.store";

/** Reads the current access token synchronously from the store. */
export function getAccessToken(): string | null {
  return getStoredSession()?.token ?? null;
}

/** Redirect once to /login (guard against multiple simultaneous 401s). */
let redirecting = false;
function redirectToLogin(): void {
  if (redirecting) return;
  redirecting = true;
  clearStoredSession();
  // Avoid a hard reload if we're already inside the router.
  if (window.location.pathname !== "/login") {
    window.location.assign("/login");
  }
  setTimeout(() => {
    redirecting = false;
  }, 300);
}

export const authApiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: API_TIMEOUT_MS,
  headers: {
    "Content-Type": "application/json",
  },
});

// Request interceptor: attach the Bearer token if present.
authApiClient.interceptors.request.use((config) => {
  const token = getAccessToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor: centralize 401 handling.
authApiClient.interceptors.response.use(
  (response) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      redirectToLogin();
    }
    return Promise.reject(error);
  }
);
