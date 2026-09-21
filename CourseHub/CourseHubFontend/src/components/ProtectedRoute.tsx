import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../lib/auth";
import { Spinner } from "./ui";

/**
 * Wraps a set of routes so they require a logged-in user, and optionally
 * one of a specific set of roles. Not a security boundary by itself —
 * the API enforces authorization for real (see PermissionAuthorizationHandler
 * server-side); this is purely about not showing the wrong dashboard shell.
 */
export function ProtectedRoute({ allowRoles }: { allowRoles?: string[] }) {
  const { currentUser, isLoading, homeRoute } = useAuth();

  if (isLoading) return <Spinner />;
  if (!currentUser) return <Navigate to="/login" replace />;

  if (allowRoles && !allowRoles.some((r) => currentUser.roles.includes(r))) {
    return <Navigate to={homeRoute()} replace />;
  }

  return <Outlet />;
}
