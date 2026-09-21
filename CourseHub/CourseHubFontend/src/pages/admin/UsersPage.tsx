import { useEffect, useState } from "react";
import { api, ApiError } from "../../lib/api";
import { useAuth } from "../../lib/auth";
import type { PagedResult, RoleResponse, UserResponse } from "../../lib/types";
import { Badge, Button, Card, EmptyState, ErrorBanner, Input, Modal, PageHeader, Spinner } from "../../components/ui";

export default function UsersPage() {
  const { hasPermission } = useAuth();
  const [data, setData] = useState<PagedResult<UserResponse> | null>(null);
  const [roles, setRoles] = useState<RoleResponse[]>([]);
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [rolesModalUser, setRolesModalUser] = useState<UserResponse | null>(null);
  const [roleToAdd, setRoleToAdd] = useState("");

  const canManage = hasPermission("users.manage");

  useEffect(() => {
    // Roles list drives the "add a role" dropdown. SuperAdmin is
    // excluded — assigning it here always gets rejected by the API (see
    // README: it's invite-code-only), so there's no point offering it.
    api
      .get<RoleResponse[]>("/api/admin/roles")
      .then((r) => setRoles(r.filter((role) => role.name !== "SuperAdmin")))
      .catch(() => {});
  }, []);

  async function load(pageOverride?: number) {
    setIsLoading(true);
    try {
      const result = await api.get<PagedResult<UserResponse>>("/api/admin/users", {
        search,
        page: pageOverride ?? page,
        pageSize: 10,
      });
      setData(result);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load users.");
    } finally {
      setIsLoading(false);
    }
  }

  function handleSearchEnter() {
    setPage(1);
    load(1);
  }

  useEffect(() => {
    load();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [page]);

  function openRolesModal(user: UserResponse) {
    setRolesModalUser(user);
  }

  // Keeps the "add a role" dropdown's default selection correct even
  // when `roles` finishes loading after the modal is already open, or
  // after adding/removing a role refreshes rolesModalUser — without this,
  // roleToAdd could keep pointing at a role no longer in the filtered
  // list (e.g. one just added), while the <select> visually shows a
  // different option than what's actually selected in state.
  useEffect(() => {
    if (!rolesModalUser) {
      setRoleToAdd("");
      return;
    }
    const firstAssignable = roles.find((r) => !rolesModalUser.roles.includes(r.name));
    setRoleToAdd(firstAssignable?.name ?? "");
  }, [roles, rolesModalUser]);

  async function runAction(action: (id: string) => Promise<unknown>, id: string) {
    setError(null);
    try {
      await action(id);
      load();
      if (rolesModalUser?.id === id) {
        // Refresh the modal's own view of the user too, so newly
        // added/removed roles show up without closing and reopening it.
        const updated = await api.get<UserResponse>(`/api/admin/users/${id}`);
        setRolesModalUser(updated);
      }
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Action failed.");
    }
  }

  async function handleAddRole() {
    if (!rolesModalUser || !roleToAdd) return;
    await runAction((id) => api.post(`/api/admin/users/${id}/roles`, { roleName: roleToAdd }), rolesModalUser.id);
  }

  function statusTone(status: string): "green" | "slate" | "rose" {
    if (status === "Active") return "green";
    if (status === "Suspended") return "rose";
    return "slate";
  }

  return (
    <div>
      <PageHeader title="Users" subtitle="Every account, regardless of role or profile" />
      <ErrorBanner message={error} />
      <div className="mb-4">
        <Input
          placeholder="Search by name or email…"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          onKeyDown={(e) => e.key === "Enter" && handleSearchEnter()}
          className="max-w-xs"
        />
      </div>
      <Card>
        {isLoading ? (
          <Spinner />
        ) : !data || data.items.length === 0 ? (
          <EmptyState message="No users found." />
        ) : (
          <table className="w-full text-left text-sm">
            <thead className="border-b border-slate-100 text-xs uppercase tracking-wide text-slate-400">
              <tr>
                <th className="px-5 py-3">Name</th>
                <th className="px-5 py-3">Email</th>
                <th className="px-5 py-3">Roles</th>
                <th className="px-5 py-3">Status</th>
                <th className="px-5 py-3 text-right">Actions</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map((u) => (
                <tr key={u.id} className="border-b border-slate-50 last:border-0">
                  <td className="px-5 py-3 font-medium text-ink">
                    {u.firstName} {u.lastName}
                  </td>
                  <td className="px-5 py-3 text-slate-500">{u.email}</td>
                  <td className="px-5 py-3">
                    <div className="flex flex-wrap gap-1">
                      {u.roles.map((r) => (
                        <Badge key={r} tone={r === "SuperAdmin" ? "blue" : "slate"}>
                          {r}
                        </Badge>
                      ))}
                    </div>
                  </td>
                  <td className="px-5 py-3">
                    <Badge tone={statusTone(u.status)}>{u.status}</Badge>
                  </td>
                  <td className="px-5 py-3">
                    <div className="flex justify-end gap-2">
                      {canManage && (
                        <>
                          <button onClick={() => openRolesModal(u)} className="text-xs font-medium text-brand-600 hover:underline">
                            Roles
                          </button>
                          {u.status === "Active" ? (
                            <button
                              onClick={() => runAction((id) => api.post(`/api/admin/users/${id}/deactivate`), u.id)}
                              className="text-xs font-medium text-slate-500 hover:underline"
                            >
                              Deactivate
                            </button>
                          ) : (
                            <button
                              onClick={() => runAction((id) => api.post(`/api/admin/users/${id}/activate`), u.id)}
                              className="text-xs font-medium text-slate-500 hover:underline"
                            >
                              Activate
                            </button>
                          )}
                          {u.status !== "Suspended" && (
                            <button
                              onClick={() => runAction((id) => api.post(`/api/admin/users/${id}/suspend`), u.id)}
                              className="text-xs font-medium text-rose-600 hover:underline"
                            >
                              Suspend
                            </button>
                          )}
                        </>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </Card>
      {data && data.totalPages > 1 && (
        <div className="mt-4 flex items-center justify-center gap-3 text-sm text-slate-500">
          <Button variant="secondary" disabled={page <= 1} onClick={() => setPage((p) => p - 1)}>
            Previous
          </Button>
          <span>
            Page {data.page} of {data.totalPages}
          </span>
          <Button variant="secondary" disabled={page >= data.totalPages} onClick={() => setPage((p) => p + 1)}>
            Next
          </Button>
        </div>
      )}

      {rolesModalUser && (
        <Modal title={`Roles — ${rolesModalUser.firstName} ${rolesModalUser.lastName}`} onClose={() => setRolesModalUser(null)}>
          <div className="space-y-4">
            <div>
              <p className="mb-2 text-xs font-medium text-slate-500">Current roles</p>
              {rolesModalUser.roles.length === 0 ? (
                <p className="text-sm text-slate-400">No roles assigned.</p>
              ) : (
                <div className="space-y-1.5">
                  {rolesModalUser.roles.map((r) => (
                    <div key={r} className="flex items-center justify-between rounded-lg bg-slate-50 px-3 py-2">
                      <span className="text-sm font-medium text-ink">{r}</span>
                      {canManage && r !== "SuperAdmin" && (
                        <button
                          onClick={() => runAction((id) => api.del(`/api/admin/users/${id}/roles/${r}`), rolesModalUser.id)}
                          className="text-xs font-medium text-rose-600 hover:underline"
                        >
                          Remove
                        </button>
                      )}
                    </div>
                  ))}
                </div>
              )}
            </div>
            {canManage && roles.some((r) => !rolesModalUser.roles.includes(r.name)) && (
              <div>
                <p className="mb-2 text-xs font-medium text-slate-500">Add a role</p>
                <div className="flex gap-2">
                  <select
                    value={roleToAdd}
                    onChange={(e) => setRoleToAdd(e.target.value)}
                    className="flex-1 rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-100"
                  >
                    {roles
                      .filter((r) => !rolesModalUser.roles.includes(r.name))
                      .map((r) => (
                        <option key={r.id} value={r.name}>
                          {r.name}
                        </option>
                      ))}
                  </select>
                  <Button onClick={handleAddRole}>Add</Button>
                </div>
              </div>
            )}
          </div>
        </Modal>
      )}
    </div>
  );
}
