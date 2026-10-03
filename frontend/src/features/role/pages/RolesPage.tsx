import { useEffect, useMemo, useState } from "react";
import { Check, KeyRound, Loader2, Plus, Search, Shield, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import {
  useAssignPermissionsToRole,
  useCreatePermission,
  useCreateRole,
  useDeletePermission,
  useDeleteRole,
  usePermissionsCatalog,
  useRolePermissions,
  useRoles,
  useUpdatePermission,
  useUpdateRole,
} from "../hooks/useRoleData";

function normalizeName(value: string) {
  return value.trim();
}

export function RolesPage() {
  const { hasPermission } = usePermissions();
  const canViewRoles = hasPermission(Permission.RoleView);
  const canViewPermissions = hasPermission(Permission.PermissionView);
  const canManageRoles = hasPermission(Permission.RoleCreate) || hasPermission(Permission.RoleEdit) || hasPermission(Permission.RoleDelete);
  const canManagePermissions = hasPermission(Permission.PermissionCreate) || hasPermission(Permission.PermissionEdit) || hasPermission(Permission.PermissionDelete);
  const canAssignPermissions = hasPermission(Permission.RoleAssignPermission);

  const { data: roles = [], isPending: rolesPending, isError: rolesError } = useRoles();
  const { data: permissions = [], isPending: permissionsPending, isError: permissionsError } = usePermissionsCatalog();
  const [selectedRoleId, setSelectedRoleId] = useState<number | null>(null);
  const [roleSearch, setRoleSearch] = useState("");
  const [permissionSearch, setPermissionSearch] = useState("");
  const [roleForm, setRoleForm] = useState({ name: "", description: "" });
  const [editingRoleId, setEditingRoleId] = useState<number | null>(null);
  const [permissionForm, setPermissionForm] = useState({ name: "" });
  const [editingPermissionId, setEditingPermissionId] = useState<number | null>(null);

  const selectedRole = useMemo(
    () => roles.find((role) => role.id === selectedRoleId) ?? roles[0] ?? null,
    [roles, selectedRoleId]
  );

  const selectedRolePermissions = useRolePermissions(selectedRole?.id ?? null);
  const [selectedPermissionIds, setSelectedPermissionIds] = useState<number[]>([]);

  useEffect(() => {
    setSelectedPermissionIds((selectedRolePermissions.data ?? []).map((permission) => permission.id));
  }, [selectedRolePermissions.data]);

  useEffect(() => {
    if (!selectedRoleId && roles.length > 0) {
      setSelectedRoleId(roles[0].id);
    }
  }, [roles, selectedRoleId]);

  useEffect(() => {
    if (selectedRole) {
      setRoleForm({
        name: selectedRole.name,
        description: selectedRole.description ?? "",
      });
    }
  }, [selectedRole]);

  const filteredRoles = useMemo(
    () =>
      roles.filter((role) =>
        [role.name, role.description ?? ""].join(" ").toLowerCase().includes(roleSearch.toLowerCase())
      ),
    [roleSearch, roles]
  );

  const filteredPermissions = useMemo(
    () =>
      permissions.filter((permission) =>
        [permission.name].join(" ").toLowerCase().includes(permissionSearch.toLowerCase())
      ),
    [permissionSearch, permissions]
  );

  const createRoleMutation = useCreateRole();
  const updateRoleMutation = useUpdateRole();
  const deleteRoleMutation = useDeleteRole();
  const createPermissionMutation = useCreatePermission();
  const updatePermissionMutation = useUpdatePermission();
  const deletePermissionMutation = useDeletePermission();
  const assignPermissionsMutation = useAssignPermissionsToRole();

  if (!canViewRoles && !canViewPermissions) {
    return (
      <Card className="mx-auto max-w-3xl">
        <CardContent className="p-6 text-sm text-destructive">
          You do not have permission to view role or access settings.
        </CardContent>
      </Card>
    );
  }

  async function handleCreateOrUpdateRole() {
    const name = normalizeName(roleForm.name);
    if (!name) {
      toast.error("Role name is required.");
      return;
    }

    try {
      if (editingRoleId) {
        await updateRoleMutation.mutateAsync({
          id: editingRoleId,
          payload: {
            id: editingRoleId,
            name,
            description: roleForm.description.trim() || null,
          },
        });
        toast.success("Role updated successfully.");
      } else {
        await createRoleMutation.mutateAsync({
          name,
          description: roleForm.description.trim() || null,
        });
        toast.success("Role created successfully.");
      }

      setRoleForm({ name: "", description: "" });
      setEditingRoleId(null);
    } catch (error) {
      toast.error((error as Error)?.message ?? "Unable to save this role.");
    }
  }

  async function handleDeleteRole(id: number) {
    try {
      await deleteRoleMutation.mutateAsync(id);
      toast.success("Role deleted.");
      if (selectedRoleId === id) {
        setSelectedRoleId(null);
      }
    } catch (error) {
      toast.error((error as Error)?.message ?? "Unable to delete this role.");
    }
  }

  async function handleCreateOrUpdatePermission() {
    const name = normalizeName(permissionForm.name);
    if (!name) {
      toast.error("Permission name is required.");
      return;
    }

    try {
      if (editingPermissionId) {
        await updatePermissionMutation.mutateAsync({
          id: editingPermissionId,
          payload: {
            id: editingPermissionId,
            name,
          },
        });
        toast.success("Permission updated successfully.");
      } else {
        await createPermissionMutation.mutateAsync({ name });
        toast.success("Permission created successfully.");
      }

      setPermissionForm({ name: "" });
      setEditingPermissionId(null);
    } catch (error) {
      toast.error((error as Error)?.message ?? "Unable to save this permission.");
    }
  }

  async function handleDeletePermission(id: number) {
    try {
      await deletePermissionMutation.mutateAsync(id);
      toast.success("Permission deleted.");
    } catch (error) {
      toast.error((error as Error)?.message ?? "Unable to delete this permission.");
    }
  }

  async function handleSaveAssignments() {
    if (!selectedRole) {
      toast.error("Select a role to update its permissions.");
      return;
    }

    const permissionIds = [...new Set(selectedPermissionIds)].sort((a, b) => a - b);

    try {
      await assignPermissionsMutation.mutateAsync({
        roleId: selectedRole.id,
        permissionIds,
      });
      toast.success(`${selectedRole.name} access was updated.`);
    } catch (error) {
      toast.error((error as Error)?.message ?? "Unable to save role permissions.");
    }
  }

  function togglePermission(permissionId: number) {
    if (!selectedRole || !canAssignPermissions) return;

    setSelectedPermissionIds((current) => {
      const next = new Set(current);
      if (next.has(permissionId)) {
        next.delete(permissionId);
      } else {
        next.add(permissionId);
      }
      return [...next];
    });
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Roles & access</h1>
          <p className="text-sm text-muted-foreground">
            Manage roles, permissions, and assign access dynamically across the school system.
          </p>
        </div>
      </div>

      <div className="grid gap-4 md:grid-cols-3">
        <Card>
          <CardContent className="flex items-center justify-between p-4">
            <div>
              <p className="text-sm text-muted-foreground">Roles</p>
              <p className="text-2xl font-semibold">{roles.length}</p>
            </div>
            <div className="rounded-xl bg-primary/10 p-2 text-primary"><Shield className="h-5 w-5" /></div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="flex items-center justify-between p-4">
            <div>
              <p className="text-sm text-muted-foreground">Permissions</p>
              <p className="text-2xl font-semibold">{permissions.length}</p>
            </div>
            <div className="rounded-xl bg-primary/10 p-2 text-primary"><KeyRound className="h-5 w-5" /></div>
          </CardContent>
        </Card>
        <Card>
          <CardContent className="flex items-center justify-between p-4">
            <div>
              <p className="text-sm text-muted-foreground">Assigned</p>
              <p className="text-2xl font-semibold">{selectedRolePermissions.data?.length ?? 0}</p>
            </div>
            <div className="rounded-xl bg-primary/10 p-2 text-primary"><Check className="h-5 w-5" /></div>
          </CardContent>
        </Card>
      </div>

      <div className="grid gap-6 xl:grid-cols-[0.9fr_1.1fr]">
        <div className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle>Roles</CardTitle>
              <CardDescription>Create and manage access groups for users.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="relative">
                <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                <input
                  value={roleSearch}
                  onChange={(event) => setRoleSearch(event.target.value)}
                  className="w-full rounded-md border bg-background py-2 pl-9 pr-3"
                  placeholder="Search roles"
                />
              </div>

              {rolesPending ? (
                <div className="flex items-center justify-center py-8 text-sm text-muted-foreground">
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" /> Loading roles…
                </div>
              ) : rolesError ? (
                <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">
                  Unable to load role data.
                </div>
              ) : filteredRoles.length === 0 ? (
                <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">
                  No roles found.
                </div>
              ) : (
                <div className="space-y-2">
                  {filteredRoles.map((role) => (
                    <button
                      key={role.id}
                      type="button"
                      onClick={() => setSelectedRoleId(role.id)}
                      className={`flex w-full items-start justify-between rounded-xl border p-3 text-left transition ${selectedRole?.id === role.id ? "border-primary bg-primary/5" : "hover:bg-accent"}`}
                    >
                      <div>
                        <p className="font-medium">{role.name}</p>
                        <p className="text-sm text-muted-foreground">{role.description || "No description"}</p>
                      </div>
                      <div className="flex items-center gap-2">
                        <span className="rounded-full bg-muted px-2 py-1 text-xs">{selectedPermissionIds.length}</span>
                        {canManageRoles ? (
                          <>
                            <Button
                              type="button"
                              variant="ghost"
                              size="sm"
                              onClick={(event) => {
                                event.stopPropagation();
                                setEditingRoleId(role.id);
                                setRoleForm({ name: role.name, description: role.description ?? "" });
                              }}
                            >
                              Edit
                            </Button>
                            <Button
                              type="button"
                              variant="ghost"
                              size="sm"
                              className="text-destructive"
                              onClick={(event) => {
                                event.stopPropagation();
                                void handleDeleteRole(role.id);
                              }}
                            >
                              <Trash2 className="mr-1 h-4 w-4" /> Delete
                            </Button>
                          </>
                        ) : null}
                      </div>
                    </button>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>{editingRoleId ? "Edit role" : "Create role"}</CardTitle>
              <CardDescription>Define reusable access groups for your users.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div>
                <label className="mb-1 block text-sm font-medium">Role name</label>
                <input
                  value={roleForm.name}
                  onChange={(event) => setRoleForm((current) => ({ ...current, name: event.target.value }))}
                  className="w-full rounded-md border bg-background px-3 py-2"
                  placeholder="e.g. Head Teacher"
                />
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Description</label>
                <textarea
                  rows={3}
                  value={roleForm.description}
                  onChange={(event) => setRoleForm((current) => ({ ...current, description: event.target.value }))}
                  className="w-full rounded-md border bg-background px-3 py-2"
                  placeholder="Short summary of this access group"
                />
              </div>
              {canManageRoles ? (
                <div className="flex gap-2">
                  <Button onClick={() => void handleCreateOrUpdateRole()} disabled={createRoleMutation.isPending || updateRoleMutation.isPending}>
                    {createRoleMutation.isPending || updateRoleMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                    {editingRoleId ? "Save role" : "Create role"}
                  </Button>
                  {editingRoleId ? (
                    <Button
                      variant="outline"
                      onClick={() => {
                        setEditingRoleId(null);
                        setRoleForm({ name: selectedRole?.name ?? "", description: selectedRole?.description ?? "" });
                      }}
                    >
                      Cancel
                    </Button>
                  ) : null}
                </div>
              ) : null}
            </CardContent>
          </Card>
        </div>

        <div className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle>{selectedRole ? `${selectedRole.name} permissions` : "Select a role"}</CardTitle>
              <CardDescription>
                {selectedRole
                  ? "Enable or disable access rights for this role. Changes are saved instantly when you click update permissions."
                  : "Choose a role from the list to manage access."}
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              {selectedRole ? (
                <>
                  <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                    <div className="rounded-md border bg-muted/40 px-3 py-2 text-sm text-muted-foreground">
                      {selectedRole.description || "No description available"}
                    </div>
                    {canAssignPermissions ? (
                      <Button onClick={() => void handleSaveAssignments()} disabled={assignPermissionsMutation.isPending}>
                        {assignPermissionsMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Check className="mr-2 h-4 w-4" />}
                        Update permissions
                      </Button>
                    ) : null}
                  </div>

                  <div className="relative">
                    <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                    <input
                      value={permissionSearch}
                      onChange={(event) => setPermissionSearch(event.target.value)}
                      className="w-full rounded-md border bg-background py-2 pl-9 pr-3"
                      placeholder="Search permission"
                    />
                  </div>

                  {selectedRolePermissions.isPending || permissionsPending ? (
                    <div className="flex items-center justify-center py-8 text-sm text-muted-foreground">
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" /> Loading permissions…
                    </div>
                  ) : permissionsError ? (
                    <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">
                      Unable to load permissions catalog.
                    </div>
                  ) : (
                    <div className="grid gap-3 md:grid-cols-2">
                      {filteredPermissions.map((permission) => {
                        const checked = selectedPermissionIds.includes(permission.id);
                        return (
                          <label
                            key={permission.id}
                            className={`flex cursor-pointer items-start gap-3 rounded-xl border p-3 transition ${checked ? "border-primary bg-primary/5" : "hover:bg-accent"}`}
                          >
                            <input
                              type="checkbox"
                              checked={checked}
                              disabled={!canAssignPermissions}
                              onChange={() => togglePermission(permission.id)}
                              className="mt-1 h-4 w-4 accent-primary"
                            />
                            <div className="min-w-0">
                              <p className="truncate font-medium">{permission.name}</p>
                              <p className="text-xs text-muted-foreground">Permission ID #{permission.id}</p>
                            </div>
                          </label>
                        );
                      })}
                    </div>
                  )}
                </>
              ) : (
                <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">
                  Pick a role to configure its access rights.
                </div>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Permission catalog</CardTitle>
              <CardDescription>Add a new permission or update existing ones for system-wide access control.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div>
                <label className="mb-1 block text-sm font-medium">Permission name</label>
                <input
                  value={permissionForm.name}
                  onChange={(event) => setPermissionForm({ name: event.target.value })}
                  className="w-full rounded-md border bg-background px-3 py-2"
                  placeholder="e.g. Student.Create"
                />
              </div>
              {canManagePermissions ? (
                <div className="flex gap-2">
                  <Button onClick={() => void handleCreateOrUpdatePermission()} disabled={createPermissionMutation.isPending || updatePermissionMutation.isPending}>
                    {createPermissionMutation.isPending || updatePermissionMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                    {editingPermissionId ? "Save permission" : "Add permission"}
                  </Button>
                  {editingPermissionId ? (
                    <Button
                      variant="outline"
                      onClick={() => {
                        setEditingPermissionId(null);
                        setPermissionForm({ name: "" });
                      }}
                    >
                      Cancel
                    </Button>
                  ) : null}
                </div>
              ) : null}

              <div className="space-y-2">
                {permissions.map((permission) => (
                  <div key={permission.id} className="flex items-center justify-between rounded-xl border p-3">
                    <div>
                      <p className="font-medium">{permission.name}</p>
                    </div>
                    <div className="flex gap-2">
                      {canManagePermissions ? (
                        <>
                          <Button
                            type="button"
                            variant="ghost"
                            size="sm"
                            onClick={() => {
                              setEditingPermissionId(permission.id);
                              setPermissionForm({ name: permission.name });
                            }}
                          >
                            Edit
                          </Button>
                          <Button
                            type="button"
                            variant="ghost"
                            size="sm"
                            className="text-destructive"
                            onClick={() => void handleDeletePermission(permission.id)}
                          >
                            <Trash2 className="mr-1 h-4 w-4" /> Delete
                          </Button>
                        </>
                      ) : null}
                    </div>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
