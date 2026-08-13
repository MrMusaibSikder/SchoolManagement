/**
 * Centralized auth session persistence.
 *
 * Stores the JWT access token + refresh token in localStorage so a browser
 * refresh restores the session (see DECISIONS.md #6 — JWT + refresh token,
 * and .ai/standards/AUTH_PERMISSION.md). All reads/writes/clears go through
 * these helpers so the storage mechanism can be swapped (e.g. to
 * sessionStorage or an httpOnly cookie) later without touching feature code.
 */
import type { AuthSession } from "../types/auth.types";

const SESSION_KEY = "schoolerp.auth.session";

export function getStoredSession(): AuthSession | null {
  try {
    const raw = localStorage.getItem(SESSION_KEY);
    if (!raw) return null;
    const parsed = JSON.parse(raw) as AuthSession;
    // Basic sanity check: must have a token and sane numeric userId.
    if (
      !parsed ||
      typeof parsed.token !== "string" ||
      parsed.token.length === 0 ||
      typeof parsed.userId !== "number"
    ) {
      return null;
    }
    return parsed;
  } catch {
    return null;
  }
}

export function setStoredSession(session: AuthSession): void {
  try {
    localStorage.setItem(SESSION_KEY, JSON.stringify(session));
  } catch {
    // Storage unavailable (e.g. privacy mode) — fail silently; the in-memory
    // session in the AuthProvider still works for the current page load.
  }
}

export function clearStoredSession(): void {
  try {
    localStorage.removeItem(SESSION_KEY);
  } catch {
    // no-op
  }
}

/** Returns true if the stored token is still valid (not expired). */
export function isSessionExpired(session: AuthSession): boolean {
  if (!session.expiresAt) return true;
  const expiresAt = new Date(session.expiresAt).getTime();
  if (Number.isNaN(expiresAt)) return true;
  return expiresAt <= Date.now();
}
