import { useEffect, useState } from "react";
import { api, ApiError } from "../../lib/api";
import { useAuth } from "../../lib/auth";
import type { PermissionResponse, RoleResponse, RolePermissionsResponse } from "../../lib/types";
import { Badge, Button, Card, EmptyState, ErrorBanner, PageHeader, Spinner } from "../../components/ui";

export default function RolesPage() {
  const { hasPermission } = useAuth();
  const [roles, setRoles] = useState<RoleResponse[]>([]);
  const [catalog, setCatalog] = useState<PermissionResponse[]>([]);
  const [selectedRoleId, setSelectedRoleId] = useState<string | null>(null);
  const [rolePermissions, setRolePermissions] = useState<RolePermissionsResponse | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const canManage = hasPermission("roles.manage");

  useEffect(() => {
    Promise.all([
      api.get<RoleResponse[]>("/api/admin/roles"),
      api.get<PermissionResponse[]>("/api/admin/permissions"),
    ])
      .then(([r, p]) => {
        setRoles(r);
        setCatalog(p);
        if (r.length > 0) setSelectedRoleId(r[0].id);
      })
      .catch((err) => setError(err instanceof ApiError ? err.message : "Failed to load."))
      .finally(() => setIsLoading(false));
  }, []);

  useEffect(() => {
    if (!selectedRoleId) return;
    api
      .get<RolePermissionsResponse>(`/api/admin/roles/${selectedRoleId}/permissions`)
      .then(setRolePermissions)
      .catch((err) => setError(err instanceof ApiError ? err.message : "Failed to load role permissions."));
  }, [selectedRoleId]);

  async function toggle(permissionName: string, currentlyAssigned: boolean) {
    if (!selectedRoleId) return;
    setError(null);
    try {
      const updated = currentlyAssigned
        ? await api.del<RolePermissionsResponse>(`/api/admin/roles/${selectedRoleId}/permissions/${permissionName}`).then(
            () => api.get<RolePermissionsResponse>(`/api/admin/roles/${selectedRoleId}/permissions`),
          )
        : await api.post<RolePermissionsResponse>(`/api/admin/roles/${selectedRoleId}/permissions`, { permissionName });
      setRolePermissions(updated);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to update permission.");
    }
  }

  const selectedRole = roles.find((r) => r.id === selectedRoleId);
  const isSuperAdminRole = selectedRole?.name === "SuperAdmin";

  return (
    <div>
      <PageHeader title="Roles & Permissions" subtitle="Control what each role can do" />
      <ErrorBanner message={error} />
      {isLoading ? (
        <Spinner />
      ) : roles.length === 0 ? (
        <EmptyState message="No roles found." />
      ) : (
        <div className="grid grid-cols-[200px_1fr] gap-6">
          <Card className="h-fit p-2">
            {roles.map((r) => (
              <button
                key={r.id}
                onClick={() => setSelectedRoleId(r.id)}
                className={`block w-full rounded-lg px-3 py-2 text-left text-sm font-medium transition-colors ${
                  r.id === selectedRoleId ? "bg-brand-50 text-brand-700" : "text-slate-600 hover:bg-slate-50"
                }`}
              >
                {r.name}
              </button>
            ))}
          </Card>
          <Card className="p-5">
            {!rolePermissions ? (
              <Spinner />
            ) : isSuperAdminRole ? (
              <p className="rounded-lg bg-brand-50 px-4 py-3 text-sm text-brand-700">
                SuperAdmin automatically has every permission (seeded on every startup, plus a runtime bypass) — it can't be
                edited here.
              </p>
            ) : (
              <div className="space-y-1">
                {catalog.map((perm) => {
                  const assigned = rolePermissions.permissions.includes(perm.name);
                  return (
                    <div key={perm.id} className="flex items-center justify-between rounded-lg px-2 py-2 hover:bg-slate-50">
                      <div>
                        <p className="text-sm font-medium text-ink">{perm.name}</p>
                        {perm.description && <p className="text-xs text-slate-400">{perm.description}</p>}
                      </div>
                      <div className="flex items-center gap-2">
                        <Badge tone={assigned ? "green" : "slate"}>{assigned ? "Granted" : "Not granted"}</Badge>
                        {canManage && (
                          <Button variant={assigned ? "secondary" : "primary"} onClick={() => toggle(perm.name, assigned)}>
                            {assigned ? "Revoke" : "Grant"}
                          </Button>
                        )}
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </Card>
        </div>
      )}
    </div>
  );
}
