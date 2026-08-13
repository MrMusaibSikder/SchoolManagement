import { useCallback, useMemo, useState, type ReactNode } from "react";
import { useNavigate } from "react-router-dom";
import {
  clearStoredSession,
  getStoredSession,
  isSessionExpired,
  setStoredSession,
} from "../store/auth.store";
import { logoutApi } from "../api/auth.api";
import { AuthContext, type AuthContextValue } from "./auth-context";
import type { AuthSession, LoginResponseDto } from "../types/auth.types";

/**
 * Provides and persists the authenticated session. On mount it restores a
 * stored session only if the access token is still valid; expired sessions are
 * cleared immediately. `login` persists + sets the session; `logout` revokes
 * the refresh token server-side (best effort), clears the local session and
 * redirects to /login regardless of the API result.
 */
export function AuthProvider({ children }: { children: ReactNode }) {
  const navigate = useNavigate();
  const [session, setSession] = useState<AuthSession | null>(() => {
    const stored = getStoredSession();
    // Restore only if the access token has not already expired.
    if (stored && !isSessionExpired(stored)) {
      return stored;
    }
    if (stored) clearStoredSession();
    return null;
  });

  const login = useCallback((response: LoginResponseDto) => {
    const next: AuthSession = {
      userId: response.userId,
      username: response.username,
      email: response.email,
      roles: response.roles,
      token: response.token,
      expiresAt: response.expiresAt,
      refreshToken: response.refreshToken,
      refreshTokenExpiresAt: response.refreshTokenExpiresAt,
    };
    setStoredSession(next);
    setSession(next);
  }, []);

  const logout = useCallback(async () => {
    const current = getStoredSession();
    // Best-effort server-side revocation; clear locally regardless.
    try {
      if (current) {
        await logoutApi({ refreshToken: current.refreshToken });
      }
    } catch {
      // Backend unreachable, token already invalid, etc. — still log out locally.
    } finally {
      clearStoredSession();
      setSession(null);
      navigate("/login", { replace: true });
    }
  }, [navigate]);

  const value = useMemo<AuthContextValue>(
    () => ({ session, isAuthenticated: session !== null, login, logout }),
    [session, login, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
