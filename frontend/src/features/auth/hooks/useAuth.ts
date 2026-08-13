import { useContext } from "react";
import {
  AuthContext,
  type AuthContextValue,
} from "../context/auth-context";

/**
 * Hook to consume the AuthContext. Throws if used outside the AuthProvider.
 */
export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return ctx;
}
