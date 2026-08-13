import { createContext } from "react";
import type { AuthSession, LoginResponseDto } from "../types/auth.types";

/**
 * Shape of the auth context value. Defined separately from the provider so the
 * `useAuth` hook (in its own file, for fast-refresh) can import it without
 * pulling in the provider component.
 */
export interface AuthContextValue {
  /** The persisted auth session, or null when logged out. */
  session: AuthSession | null;
  /** Convenience flag equivalent to `session !== null`. */
  isAuthenticated: boolean;
  /** Builds + persists an AuthSession from a login response. */
  login: (response: LoginResponseDto) => void;
  /** Calls POST /api/Auth/logout (best-effort), clears the session, redirects. */
  logout: () => Promise<void>;
}

export const AuthContext = createContext<AuthContextValue | undefined>(
  undefined
);
