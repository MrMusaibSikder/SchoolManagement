import { useEffect, useState } from "react";
import { api, ApiError } from "../../lib/api";
import { useAuth } from "../../lib/auth";
import type { EligibleUserResponse, PagedResult, TeacherResponse } from "../../lib/types";
import { Badge, Button, Card, EmptyState, ErrorBanner, Field, Input, Modal, PageHeader, Spinner, Textarea } from "../../components/ui";

const emptyCreateForm = { userId: "", employeeId: "", firstName: "", lastName: "" };
const emptyEditForm = { firstName: "", lastName: "", bio: "", profileImageUrl: "" };

export default function TeachersPage() {
  const { hasPermission } = useAuth();
  const [data, setData] = useState<PagedResult<TeacherResponse> | null>(null);
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [createOpen, setCreateOpen] = useState(false);
  const [createForm, setCreateForm] = useState(emptyCreateForm);
  const [eligibleUsers, setEligibleUsers] = useState<EligibleUserResponse[] | null>(null);
  const [editing, setEditing] = useState<TeacherResponse | null>(null);
  const [editForm, setEditForm] = useState(emptyEditForm);

  async function load(pageOverride?: number) {
    setIsLoading(true);
    try {
      const result = await api.get<PagedResult<TeacherResponse>>("/api/admin/teachers", {
        search,
        page: pageOverride ?? page,
        pageSize: 10,
      });
      setData(result);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load teachers.");
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

  async function openCreate() {
    setCreateForm(emptyCreateForm);
    setCreateOpen(true);
    setEligibleUsers(null);
    try {
      const users = await api.get<EligibleUserResponse[]>("/api/admin/teachers/eligible-users");
      setEligibleUsers(users);
      if (users.length > 0) {
        setCreateForm((f) => ({ ...f, userId: users[0].id, firstName: users[0].firstName, lastName: users[0].lastName }));
      }
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load eligible users.");
      setEligibleUsers([]);
    }
  }

  async function handleCreate() {
    setError(null);
    try {
      await api.post("/api/admin/teachers", createForm);
      setCreateOpen(false);
      setCreateForm(emptyCreateForm);
      setPage(1);
      load(1);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to create teacher.");
    }
  }

  function openEdit(teacher: TeacherResponse) {
    setEditing(teacher);
    setEditForm({
      firstName: teacher.firstName,
      lastName: teacher.lastName,
      bio: teacher.bio ?? "",
      profileImageUrl: teacher.profileImageUrl ?? "",
    });
  }

  async function handleEditSave() {
    if (!editing) return;
    setError(null);
    try {
      const { profileImageUrl, ...profileFields } = editForm;
      await api.put(`/api/admin/teachers/${editing.id}/profile`, profileFields);
      // Separate endpoint on the backend (PUT .../profile-image).
      await api.put(`/api/admin/teachers/${editing.id}/profile-image`, { profileImageUrl: profileImageUrl || null });
      setEditing(null);
      load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to save changes.");
    }
  }

  async function runAction(action: (id: string) => Promise<unknown>, id: string) {
    setError(null);
    try {
      await action(id);
      load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Action failed.");
    }
  }

  const canCreate = hasPermission("teachers.create");
  const canUpdate = hasPermission("teachers.update");
  const canDelete = hasPermission("teachers.delete");

  return (
    <div>
      <PageHeader
        title="Teachers"
        subtitle="Promote a registered user into a teacher profile"
        action={canCreate && <Button onClick={openCreate}>+ New teacher</Button>}
      />
      <ErrorBanner message={error} />
      <div className="mb-4">
        <Input
          placeholder="Search by name, employee id, or email…"
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
          <EmptyState message="No teachers yet." />
        ) : (
          <table className="w-full text-left text-sm">
            <thead className="border-b border-slate-100 text-xs uppercase tracking-wide text-slate-400">
              <tr>
                <th className="px-5 py-3">Name</th>
                <th className="px-5 py-3">Employee ID</th>
                <th className="px-5 py-3">Contact</th>
                <th className="px-5 py-3">Status</th>
                <th className="px-5 py-3 text-right">Actions</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map((t) => (
                <tr key={t.id} className="border-b border-slate-50 last:border-0">
                  <td className="px-5 py-3 font-medium text-ink">
                    {t.firstName} {t.lastName}
                  </td>
                  <td className="px-5 py-3 text-slate-500">{t.employeeId}</td>
                  <td className="px-5 py-3 text-slate-500">{t.email ?? t.phone ?? "—"}</td>
                  <td className="px-5 py-3">
                    <div className="flex gap-1.5">
                      <Badge tone={t.isActive ? "green" : "slate"}>{t.isActive ? "Active" : "Inactive"}</Badge>
                      <Badge tone={t.isProfilePublic ? "blue" : "slate"}>{t.isProfilePublic ? "Public" : "Private"}</Badge>
                    </div>
                  </td>
                  <td className="px-5 py-3">
                    <div className="flex justify-end gap-2">
                      {canUpdate && (
                        <>
                          <button onClick={() => openEdit(t)} className="text-xs font-medium text-brand-600 hover:underline">
                            Edit
                          </button>
                          <button
                            onClick={() =>
                              runAction(
                                (id) =>
                                  t.isProfilePublic
                                    ? api.post(`/api/admin/teachers/${id}/unpublish-profile`)
                                    : api.post(`/api/admin/teachers/${id}/publish-profile`),
                                t.id,
                              )
                            }
                            className="text-xs font-medium text-slate-500 hover:underline"
                          >
                            {t.isProfilePublic ? "Unpublish" : "Publish"}
                          </button>
                          <button
                            onClick={() =>
                              runAction(
                                (id) => (t.isActive ? api.post(`/api/admin/teachers/${id}/deactivate`) : api.post(`/api/admin/teachers/${id}/activate`)),
                                t.id,
                              )
                            }
                            className="text-xs font-medium text-slate-500 hover:underline"
                          >
                            {t.isActive ? "Deactivate" : "Activate"}
                          </button>
                        </>
                      )}
                      {canDelete && (
                        <button
                          onClick={() => runAction((id) => api.del(`/api/admin/teachers/${id}`), t.id)}
                          className="text-xs font-medium text-rose-600 hover:underline"
                        >
                          Delete
                        </button>
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

      {createOpen && (
        <Modal title="New teacher" onClose={() => setCreateOpen(false)}>
          <div className="space-y-3">
            <p className="rounded-lg bg-slate-50 px-3 py-2 text-xs text-slate-500">
              Only users who registered with the <strong>Teacher</strong> role and don't already have a profile show up
              here.
            </p>
            <Field label="User">
              {eligibleUsers === null ? (
                <div className="py-2 text-sm text-slate-400">Loading eligible users…</div>
              ) : eligibleUsers.length === 0 ? (
                <div className="rounded-lg bg-amber-50 px-3 py-2 text-xs text-amber-700">
                  No eligible users found. Ask someone to register with the Teacher role first (or check that everyone
                  who registered as Teacher hasn't already been promoted).
                </div>
              ) : (
                <select
                  value={createForm.userId}
                  onChange={(e) => {
                    const selected = eligibleUsers.find((u) => u.id === e.target.value);
                    setCreateForm({
                      ...createForm,
                      userId: e.target.value,
                      firstName: selected?.firstName ?? createForm.firstName,
                      lastName: selected?.lastName ?? createForm.lastName,
                    });
                  }}
                  className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-100"
                >
                  {eligibleUsers.map((u) => (
                    <option key={u.id} value={u.id}>
                      {u.firstName} {u.lastName} ({u.email})
                    </option>
                  ))}
                </select>
              )}
            </Field>
            <Field label="Employee ID">
              <Input value={createForm.employeeId} onChange={(e) => setCreateForm({ ...createForm, employeeId: e.target.value })} />
            </Field>
            <Field label="First name">
              <Input value={createForm.firstName} onChange={(e) => setCreateForm({ ...createForm, firstName: e.target.value })} />
            </Field>
            <Field label="Last name">
              <Input value={createForm.lastName} onChange={(e) => setCreateForm({ ...createForm, lastName: e.target.value })} />
            </Field>
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="secondary" onClick={() => setCreateOpen(false)}>
                Cancel
              </Button>
              <Button onClick={handleCreate} disabled={!createForm.userId}>
                Create teacher
              </Button>
            </div>
          </div>
        </Modal>
      )}

      {editing && (
        <Modal title={`Edit ${editing.firstName} ${editing.lastName}`} onClose={() => setEditing(null)}>
          <div className="space-y-3">
            <Field label="First name">
              <Input value={editForm.firstName} onChange={(e) => setEditForm({ ...editForm, firstName: e.target.value })} />
            </Field>
            <Field label="Last name">
              <Input value={editForm.lastName} onChange={(e) => setEditForm({ ...editForm, lastName: e.target.value })} />
            </Field>
            <Field label="Bio">
              <Textarea rows={3} value={editForm.bio} onChange={(e) => setEditForm({ ...editForm, bio: e.target.value })} />
            </Field>
            <Field label="Profile image URL">
              <Input
                value={editForm.profileImageUrl}
                onChange={(e) => setEditForm({ ...editForm, profileImageUrl: e.target.value })}
                placeholder="https://cdn.example.com/teachers/photo.jpg"
              />
            </Field>
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="secondary" onClick={() => setEditing(null)}>
                Cancel
              </Button>
              <Button onClick={handleEditSave}>Save changes</Button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
}
