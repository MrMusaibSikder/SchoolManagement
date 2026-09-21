import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import { api, ApiError } from "./api";
import { tokenStorage } from "./tokenStorage";
import type { AuthResponse, CurrentUser } from "./types";

interface AuthContextValue {
  currentUser: CurrentUser | null;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (params: {
    email: string;
    password: string;
    confirmPassword: string;
    firstName: string;
    lastName: string;
    requestedRole?: "Teacher" | "Student";
    superAdminCode?: string;
  }) => Promise<void>;
  logout: () => Promise<void>;
  hasPermission: (permission: string) => boolean;
  hasRole: (role: string) => boolean;
  /** Which dashboard shell this user should land on. */
  homeRoute: () => string;
}

const AuthContext = createContext<AuthContextValue | null>(null);

/** SuperAdmin bypasses every permission check server-side too (see the
 * backend's PermissionAuthorizationHandler) — mirrored here so the UI
 * doesn't hide admin-only screens/buttons from a SuperAdmin who simply
 * hasn't been seeded with every explicit permission row yet. */
function computeHasPermission(user: CurrentUser | null, permission: string): boolean {
  if (!user) return false;
  if (user.roles.includes("SuperAdmin")) return true;
  return user.permissions.includes(permission);
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [currentUser, setCurrentUser] = useState<CurrentUser | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  async function hydrate() {
    if (!tokenStorage.getAccessToken()) {
      setIsLoading(false);
      return;
    }
    try {
      const me = await api.get<CurrentUser>("/api/auth/me");
      setCurrentUser(me);
    } catch {
      tokenStorage.clear();
      setCurrentUser(null);
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    hydrate();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  async function login(email: string, password: string) {
    const auth = await api.post<AuthResponse>("/api/auth/login", { email, password });
    tokenStorage.setTokens(auth.accessToken, auth.refreshToken);
    const me = await api.get<CurrentUser>("/api/auth/me");
    setCurrentUser(me);
  }

  async function register(params: Parameters<AuthContextValue["register"]>[0]) {
    const auth = await api.post<AuthResponse>("/api/auth/register", params);
    tokenStorage.setTokens(auth.accessToken, auth.refreshToken);
    const me = await api.get<CurrentUser>("/api/auth/me");
    setCurrentUser(me);
  }

  async function logout() {
    const refreshToken = tokenStorage.getRefreshToken();
    try {
      if (refreshToken) {
        await api.post("/api/auth/logout", { refreshToken });
      }
    } catch (err) {
      // Best-effort — even if the server call fails, still clear local
      // state so the user isn't stuck "logged in" on their own device.
      if (!(err instanceof ApiError)) throw err;
    } finally {
      tokenStorage.clear();
      setCurrentUser(null);
    }
  }

  function hasPermission(permission: string) {
    return computeHasPermission(currentUser, permission);
  }

  function hasRole(role: string) {
    return currentUser?.roles.includes(role) ?? false;
  }

  function homeRoute() {
    if (hasRole("SuperAdmin") || hasRole("Admin")) return "/admin";
    if (hasRole("Teacher")) return "/teacher";
    return "/student";
  }

  return (
    <AuthContext.Provider
      value={{ currentUser, isLoading, login, register, logout, hasPermission, hasRole, homeRoute }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}
