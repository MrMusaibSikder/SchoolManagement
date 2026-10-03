import { useMemo, useState } from "react";
import {
  ChevronLeft,
  ChevronRight,
  KeyRound,
  Loader2,
  Search,
  ShieldCheck,
  UserRound,
  UserRoundPlus,
  X,
} from "lucide-react";
import { toast } from "sonner";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { AppRole } from "@/lib/permissions";
import {
  useAccessRoles,
  useAccessPermissions,
  useAccessUsers,
  useAssignPermissionsToRole,
  useAssignRoleToUser,
  useRemovePermissionFromRole,
  useRemoveRoleFromUser,
  useRolePermissionAssignments,
  useUserPermissions,
  useUserRoles,
} from "../hooks/useRoleData";

const PAGE_SIZE = 10;

export function RolesPage() {
  const { hasRole, isPending: profilePending } = usePermissions();
  const isAdmin = hasRole(AppRole.Admin);

  const usersQuery = useAccessUsers(isAdmin);
  const rolesQuery = useAccessRoles(isAdmin);

  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [selectedUserId, setSelectedUserId] = useState<number | null>(null);
  const [roleToAssign, setRoleToAssign] = useState("");
  const [selectedPermissionRoleId, setSelectedPermissionRoleId] = useState<number | null>(null);
  const [permissionSearch, setPermissionSearch] = useState("");
  const [permissionDraft, setPermissionDraft] = useState<{
    roleId: number;
    permissionIds: number[];
  } | null>(null);

  const filteredUsers = useMemo(() => {
    const normalizedSearch = search.trim().toLowerCase();
    return (usersQuery.data ?? []).filter((user) =>
      [user.username, user.email].some((value) =>
        value.toLowerCase().includes(normalizedSearch)
      )
    );
  }, [search, usersQuery.data]);

  const pageCount = Math.max(1, Math.ceil(filteredUsers.length / PAGE_SIZE));
  const currentPage = Math.min(page, pageCount);
  const pagedUsers = filteredUsers.slice(
    (currentPage - 1) * PAGE_SIZE,
    currentPage * PAGE_SIZE
  );
  const selectedUser =
    pagedUsers.find((user) => user.id === selectedUserId) ?? pagedUsers[0] ?? null;

  const userRolesQuery = useUserRoles(selectedUser?.id ?? null, isAdmin);
  const userPermissionsQuery = useUserPermissions(selectedUser?.id ?? null, isAdmin);
  const permissionsQuery = useAccessPermissions(isAdmin);
  const permissionRole =
    (rolesQuery.data ?? []).find((role) => role.id === selectedPermissionRoleId) ??
    rolesQuery.data?.[0] ??
    null;
  const rolePermissionsQuery = useRolePermissionAssignments(
    permissionRole?.id ?? null,
    isAdmin
  );
  const assignRole = useAssignRoleToUser();
  const removeRole = useRemoveRoleFromUser();
  const assignPermissions = useAssignPermissionsToRole();
  const removePermission = useRemovePermissionFromRole();

  const assignedRoleIds = new Set((userRolesQuery.data ?? []).map((role) => role.id));
  const assignableRoles = (rolesQuery.data ?? []).filter(
    (role) => !assignedRoleIds.has(role.id)
  );

  const groupedPermissions = useMemo(() => {
    const groups = new Map<string, string[]>();
    for (const permission of userPermissionsQuery.data ?? []) {
      const [resource, ...actionParts] = permission.name.split(".");
      const action = actionParts.join(".") || "Access";
      const actions = groups.get(resource) ?? [];
      actions.push(action);
      groups.set(resource, actions);
    }
    return [...groups.entries()].sort(([left], [right]) =>
      left.localeCompare(right)
    );
  }, [userPermissionsQuery.data]);

  const rolePermissionIds = useMemo(
    () => (rolePermissionsQuery.data ?? []).map((permission) => permission.id),
    [rolePermissionsQuery.data]
  );
  const selectedRolePermissionIds =
    permissionDraft !== null &&
    permissionRole !== null &&
    permissionDraft.roleId === permissionRole.id
      ? permissionDraft.permissionIds
      : rolePermissionIds;
  const filteredPermissionCatalog = useMemo(() => {
    const normalizedSearch = permissionSearch.trim().toLowerCase();
    return (permissionsQuery.data ?? []).filter((permission) =>
      permission.name.toLowerCase().includes(normalizedSearch)
    );
  }, [permissionSearch, permissionsQuery.data]);
  const groupedPermissionCatalog = useMemo(() => {
    const groups = new Map<string, typeof filteredPermissionCatalog>();
    for (const permission of filteredPermissionCatalog) {
      const [resource] = permission.name.split(".");
      groups.set(resource, [...(groups.get(resource) ?? []), permission]);
    }
    return [...groups.entries()].sort(([left], [right]) =>
      left.localeCompare(right)
    );
  }, [filteredPermissionCatalog]);
  const rolePermissionDirty =
    selectedRolePermissionIds.length !== rolePermissionIds.length ||
    selectedRolePermissionIds.some((id) => !rolePermissionIds.includes(id));

  function goToPage(nextPage: number) {
    setPage(nextPage);
    const nextPageUsers = filteredUsers.slice(
      (nextPage - 1) * PAGE_SIZE,
      nextPage * PAGE_SIZE
    );
    setSelectedUserId(nextPageUsers[0]?.id ?? null);
  }

  async function handleAssignRole() {
    if (!selectedUser || !roleToAssign) return;

    try {
      await assignRole.mutateAsync({
        userId: selectedUser.id,
        roleId: Number(roleToAssign),
      });
      setRoleToAssign("");
      toast.success("Role assigned. User access now follows this role's permissions.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Unable to assign this role.");
    }
  }

  async function handleRemoveRole(roleId: number, roleName: string) {
    if (!selectedUser) return;
    if (!window.confirm(`Remove the ${roleName} role from ${selectedUser.username}?`)) {
      return;
    }
    try {
      await removeRole.mutateAsync({ userId: selectedUser.id, roleId });
      toast.success("Role removed. The user's effective permissions were updated.");
    } catch (error) {
      toast.error(error instanceof Error ? error.message : "Unable to remove this role.");
    }
  }

  function toggleRolePermission(permissionId: number) {
    if (!permissionRole) return;
    const selected = new Set(selectedRolePermissionIds);
    if (selected.has(permissionId)) selected.delete(permissionId);
    else selected.add(permissionId);
    setPermissionDraft({
      roleId: permissionRole.id,
      permissionIds: [...selected],
    });
  }

  function toggleFilteredRolePermissions() {
    if (!permissionRole) return;
    const selected = new Set(selectedRolePermissionIds);
    const allFilteredSelected = filteredPermissionCatalog.every((permission) =>
      selected.has(permission.id)
    );
    for (const permission of filteredPermissionCatalog) {
      if (allFilteredSelected) selected.delete(permission.id);
      else selected.add(permission.id);
    }
    setPermissionDraft({
      roleId: permissionRole.id,
      permissionIds: [...selected],
    });
  }

  async function handleSaveRolePermissions() {
    if (!permissionRole) return;

    const currentIds = new Set(selectedRolePermissionIds);
    const originalIds = new Set(rolePermissionIds);
    const toAdd = [...currentIds].filter((id) => !originalIds.has(id));
    const toRemove = [...originalIds].filter((id) => !currentIds.has(id));

    if (
      toRemove.length > 0 &&
      !window.confirm(
        `Revoke ${toRemove.length} permission${toRemove.length === 1 ? "" : "s"} from ${permissionRole.name}? This changes access for every user assigned to this role.`
      )
    ) {
      return;
    }

    try {
      if (toAdd.length > 0) {
        await assignPermissions.mutateAsync({
          roleId: permissionRole.id,
          permissionIds: toAdd,
        });
      }
      for (const permissionId of toRemove) {
        await removePermission.mutateAsync({
          roleId: permissionRole.id,
          permissionId,
        });
      }
      setPermissionDraft(null);
      toast.success(`${permissionRole.name} permissions updated.`);
    } catch (error) {
      setPermissionDraft(null);
      toast.error(
        error instanceof Error
          ? error.message
          : "Some role permission changes could not be saved. The latest assignments are being refreshed."
      );
    }
  }

  if (profilePending) {
    return (
      <div className="flex min-h-48 items-center justify-center text-sm text-muted-foreground">
        <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
        Checking administrator access…
      </div>
    );
  }

  if (!isAdmin) {
    return (
      <Card className="mx-auto max-w-3xl">
        <CardContent className="p-6 text-sm text-destructive">
          Access denied. Only an administrator can manage user roles.
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <header>
        <div className="flex items-center gap-2">
          <div className="rounded-xl bg-primary/10 p-2 text-primary">
            <ShieldCheck aria-hidden="true" className="h-5 w-5" />
          </div>
          <h1 className="font-display text-2xl font-semibold">User role access</h1>
        </div>
        <p className="mt-2 max-w-3xl text-sm text-muted-foreground">
          Assign roles to users. Each user receives the permissions attached to
          their assigned roles. You can also add or revoke permissions on a
          role; those changes apply to every user assigned that role.
        </p>
      </header>

      <div className="grid gap-6 lg:grid-cols-[minmax(0,0.9fr)_minmax(0,1.1fr)]">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center justify-between gap-3">
              <span>Users</span>
              <Badge variant="secondary">{filteredUsers.length}</Badge>
            </CardTitle>
            <CardDescription>Find a user, then manage their assigned roles.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="relative">
              <Search
                aria-hidden="true"
                className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground"
              />
              <input
                aria-label="Search users"
                value={search}
                onChange={(event) => {
                  setSearch(event.target.value);
                  setPage(1);
                }}
                placeholder="Search username or email"
                className="w-full rounded-md border bg-background py-2 pl-9 pr-3 text-sm"
              />
            </div>

            {usersQuery.isPending ? (
              <div className="flex items-center justify-center py-10 text-sm text-muted-foreground">
                <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
                Loading users…
              </div>
            ) : usersQuery.isError ? (
              <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">
                Unable to load users. Please refresh and try again.
              </div>
            ) : pagedUsers.length === 0 ? (
              <div className="rounded-lg border border-dashed p-8 text-center text-sm text-muted-foreground">
                <UserRound className="mx-auto mb-2 h-5 w-5" />
                No users match your search.
              </div>
            ) : (
              <div className="space-y-2">
                {pagedUsers.map((user) => (
                  <button
                    key={user.id}
                    type="button"
                    onClick={() => setSelectedUserId(user.id)}
                    className={`w-full rounded-xl border p-3 text-left transition focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring ${
                      selectedUser?.id === user.id
                        ? "border-primary bg-primary/5"
                        : "hover:bg-accent"
                    }`}
                  >
                    <span className="flex items-start justify-between gap-3">
                      <span className="min-w-0">
                        <span className="block truncate font-medium">{user.username}</span>
                        <span className="block truncate text-sm text-muted-foreground">
                          {user.email}
                        </span>
                      </span>
                      <Badge variant={user.isActive ? "secondary" : "outline"}>
                        {user.isActive ? "Active" : "Inactive"}
                      </Badge>
                    </span>
                  </button>
                ))}
              </div>
            )}

            {!usersQuery.isPending && !usersQuery.isError && filteredUsers.length > 0 && (
              <div className="flex flex-col gap-3 border-t pt-4 sm:flex-row sm:items-center sm:justify-between">
                <p className="text-sm text-muted-foreground">
                  Showing {(currentPage - 1) * PAGE_SIZE + 1}–
                  {Math.min(currentPage * PAGE_SIZE, filteredUsers.length)} of{" "}
                  {filteredUsers.length}
                </p>
                <div className="flex items-center gap-2">
                  <Button
                    type="button"
                    variant="outline"
                    className="h-9 px-3"
                    aria-label="Previous page"
                    disabled={currentPage <= 1}
                    onClick={() => goToPage(Math.max(1, currentPage - 1))}
                  >
                    <ChevronLeft aria-hidden="true" className="h-4 w-4" />
                  </Button>
                  <span className="min-w-20 text-center text-sm">
                    Page {currentPage} of {pageCount}
                  </span>
                  <Button
                    type="button"
                    variant="outline"
                    className="h-9 px-3"
                    aria-label="Next page"
                    disabled={currentPage >= pageCount}
                    onClick={() => goToPage(Math.min(pageCount, currentPage + 1))}
                  >
                    <ChevronRight aria-hidden="true" className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            )}
          </CardContent>
        </Card>

        <div className="space-y-6">
          <Card>
            <CardHeader>
              <CardTitle>
                {selectedUser ? `Roles for ${selectedUser.username}` : "Select a user"}
              </CardTitle>
              <CardDescription>
                Add a role to grant its access or remove a role to revoke that
                role's access from this user.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-5">
              {!selectedUser ? (
                <div className="rounded-lg border border-dashed p-8 text-center text-sm text-muted-foreground">
                  Select a user from the list to manage their roles.
                </div>
              ) : (
                <>
                  <div className="flex items-center gap-3 rounded-xl bg-muted/40 p-4">
                    <div className="rounded-full bg-background p-2 text-muted-foreground">
                      <UserRound aria-hidden="true" className="h-5 w-5" />
                    </div>
                    <div className="min-w-0">
                      <p className="truncate font-medium">{selectedUser.username}</p>
                      <p className="truncate text-sm text-muted-foreground">{selectedUser.email}</p>
                    </div>
                  </div>

                  {rolesQuery.isError || userRolesQuery.isError ? (
                    <p className="rounded-lg border border-destructive/20 bg-destructive/5 p-3 text-sm text-destructive">
                      Unable to load role assignments.
                    </p>
                  ) : userRolesQuery.isPending || rolesQuery.isPending ? (
                    <div className="flex items-center text-sm text-muted-foreground">
                      <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
                      Loading assigned roles…
                    </div>
                  ) : (
                    <div className="space-y-2">
                      <h3 className="text-sm font-medium">Assigned roles</h3>
                      {userRolesQuery.data?.length ? (
                        userRolesQuery.data.map((role) => (
                          <div
                            key={role.id}
                            className="flex items-center justify-between gap-3 rounded-lg border p-3"
                          >
                            <div className="min-w-0">
                              <p className="font-medium">{role.name}</p>
                              {role.description ? (
                                <p className="text-sm text-muted-foreground">{role.description}</p>
                              ) : null}
                            </div>
                            <Button
                              type="button"
                              variant="outline"
                              className="h-9 shrink-0 px-3 text-destructive hover:text-destructive"
                              disabled={removeRole.isPending}
                              aria-label={`Remove ${role.name} role`}
                              onClick={() => void handleRemoveRole(role.id, role.name)}
                            >
                              {removeRole.isPending ? (
                                <Loader2 aria-hidden="true" className="h-4 w-4 animate-spin" />
                              ) : (
                                <X aria-hidden="true" className="h-4 w-4" />
                              )}
                              <span className="ml-1">Remove</span>
                            </Button>
                          </div>
                        ))
                      ) : (
                        <p className="rounded-lg border border-dashed p-4 text-sm text-muted-foreground">
                          No roles are assigned to this user.
                        </p>
                      )}
                    </div>
                  )}

                  <div className="flex flex-col gap-2 sm:flex-row">
                    <select
                      aria-label="Role to assign"
                      value={roleToAssign}
                      onChange={(event) => setRoleToAssign(event.target.value)}
                      className="min-w-0 flex-1 rounded-md border bg-background px-3 py-2 text-sm"
                      disabled={rolesQuery.isPending || assignableRoles.length === 0}
                    >
                      <option value="">
                        {assignableRoles.length ? "Choose a role to add" : "All roles assigned"}
                      </option>
                      {assignableRoles.map((role) => (
                        <option key={role.id} value={role.id}>
                          {role.name}
                        </option>
                      ))}
                    </select>
                    <Button
                      type="button"
                      className="shrink-0"
                      disabled={!roleToAssign || assignRole.isPending}
                      onClick={() => void handleAssignRole()}
                    >
                      {assignRole.isPending ? (
                        <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
                      ) : (
                        <UserRoundPlus aria-hidden="true" className="mr-2 h-4 w-4" />
                      )}
                      Assign role
                    </Button>
                  </div>
                </>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <KeyRound aria-hidden="true" className="h-5 w-5 text-primary" />
                Effective permissions
              </CardTitle>
              <CardDescription>
                Read-only permissions inherited from this user&apos;s assigned roles.
                Change access by changing the user&apos;s roles above.
              </CardDescription>
            </CardHeader>
            <CardContent>
              {!selectedUser ? (
                <p className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">
                  Select a user to review their effective access.
                </p>
              ) : userPermissionsQuery.isPending ? (
                <div className="flex items-center justify-center py-8 text-sm text-muted-foreground">
                  <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
                  Loading effective permissions…
                </div>
              ) : userPermissionsQuery.isError ? (
                <p className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">
                  Unable to load this user&apos;s effective permissions.
                </p>
              ) : groupedPermissions.length === 0 ? (
                <p className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">
                  This user currently has no permissions through their assigned roles.
                </p>
              ) : (
                <div className="grid gap-3 sm:grid-cols-2">
                  {groupedPermissions.map(([resource, actions]) => (
                    <div key={resource} className="rounded-xl border p-4">
                      <h3 className="font-medium">{resource}</h3>
                      <div className="mt-2 flex flex-wrap gap-2">
                        {[...actions].sort((left, right) => left.localeCompare(right)).map((action) => (
                          <Badge key={`${resource}.${action}`} variant="secondary">
                            {action}
                          </Badge>
                        ))}
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle>Role permissions</CardTitle>
              <CardDescription>
                Select a role, then add or revoke the permissions that belong to
                it. Changes affect every user assigned to that role.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              {rolesQuery.isError ? (
                <p className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">
                  Unable to load roles.
                </p>
              ) : rolesQuery.isPending ? (
                <div className="flex items-center text-sm text-muted-foreground">
                  <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
                  Loading roles…
                </div>
              ) : !permissionRole ? (
                <p className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">
                  Create a role before assigning permissions.
                </p>
              ) : (
                <>
                  <div className="flex flex-col gap-3 sm:flex-row">
                    <select
                      aria-label="Role to configure"
                      value={permissionRole.id}
                      onChange={(event) => {
                        setSelectedPermissionRoleId(Number(event.target.value));
                        setPermissionDraft(null);
                      }}
                      className="min-w-0 flex-1 rounded-md border bg-background px-3 py-2 text-sm"
                    >
                      {(rolesQuery.data ?? []).map((role) => (
                        <option key={role.id} value={role.id}>
                          {role.name}
                        </option>
                      ))}
                    </select>
                    <Button
                      type="button"
                      variant="outline"
                      disabled={
                        filteredPermissionCatalog.length === 0 ||
                        rolePermissionsQuery.isPending ||
                        permissionsQuery.isPending
                      }
                      onClick={toggleFilteredRolePermissions}
                    >
                      {filteredPermissionCatalog.every((permission) =>
                        selectedRolePermissionIds.includes(permission.id)
                      )
                        ? "Revoke filtered"
                        : "Add filtered"}
                    </Button>
                  </div>

                  <div className="flex flex-wrap items-center justify-between gap-3">
                    <p className="text-sm text-muted-foreground">
                      {selectedRolePermissionIds.length} permission
                      {selectedRolePermissionIds.length === 1 ? "" : "s"} assigned
                    </p>
                    <Button
                      type="button"
                      disabled={
                        !rolePermissionDirty ||
                        rolePermissionsQuery.isPending ||
                        permissionsQuery.isPending ||
                        assignPermissions.isPending ||
                        removePermission.isPending
                      }
                      onClick={() => void handleSaveRolePermissions()}
                    >
                      {assignPermissions.isPending || removePermission.isPending ? (
                        <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
                      ) : (
                        <ShieldCheck aria-hidden="true" className="mr-2 h-4 w-4" />
                      )}
                      Save role permissions
                    </Button>
                  </div>

                  <div className="relative">
                    <Search
                      aria-hidden="true"
                      className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground"
                    />
                    <input
                      aria-label="Search permissions"
                      value={permissionSearch}
                      onChange={(event) => setPermissionSearch(event.target.value)}
                      placeholder="Search permissions"
                      className="w-full rounded-md border bg-background py-2 pl-9 pr-3 text-sm"
                    />
                  </div>

                  {permissionsQuery.isError || rolePermissionsQuery.isError ? (
                    <p className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">
                      Unable to load permissions for this role.
                    </p>
                  ) : permissionsQuery.isPending || rolePermissionsQuery.isPending ? (
                    <div className="flex items-center justify-center py-8 text-sm text-muted-foreground">
                      <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
                      Loading role permissions…
                    </div>
                  ) : groupedPermissionCatalog.length === 0 ? (
                    <p className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">
                      No permissions match this search.
                    </p>
                  ) : (
                    <div className="max-h-[32rem] space-y-4 overflow-y-auto pr-1">
                      {groupedPermissionCatalog.map(([resource, permissions]) => (
                        <section key={resource} className="rounded-xl border p-4">
                          <h3 className="mb-3 font-medium">{resource}</h3>
                          <div className="grid gap-2 sm:grid-cols-2">
                            {permissions.map((permission) => {
                              const checked = selectedRolePermissionIds.includes(
                                permission.id
                              );
                              return (
                                <label
                                  key={permission.id}
                                  className={`flex cursor-pointer items-start gap-3 rounded-lg border p-3 text-sm transition ${
                                    checked
                                      ? "border-primary bg-primary/5"
                                      : "hover:bg-accent"
                                  }`}
                                >
                                  <input
                                    type="checkbox"
                                    checked={checked}
                                    onChange={() =>
                                      toggleRolePermission(permission.id)
                                    }
                                    className="mt-0.5 h-4 w-4 accent-primary"
                                  />
                                  <span className="break-all">{permission.name}</span>
                                </label>
                              );
                            })}
                          </div>
                        </section>
                      ))}
                    </div>
                  )}
                </>
              )}
            </CardContent>
          </Card>
        </div>
      </div>
    </div>
  );
}
