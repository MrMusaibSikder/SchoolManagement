import { useEffect, useState } from "react";
import { api, ApiError } from "../../lib/api";
import { useAuth } from "../../lib/auth";
import type { EligibleUserResponse, PagedResult, StudentResponse } from "../../lib/types";
import { Badge, Button, Card, EmptyState, ErrorBanner, Field, Input, Modal, PageHeader, Spinner } from "../../components/ui";

const emptyCreateForm = { userId: "", studentId: "", firstName: "", lastName: "" };
const emptyEditForm = { firstName: "", lastName: "", dateOfBirth: "", profileImageUrl: "" };
const emptyContactForm = { phone: "", email: "", address: "" };

export default function StudentsPage() {
  const { hasPermission } = useAuth();
  const [data, setData] = useState<PagedResult<StudentResponse> | null>(null);
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [createOpen, setCreateOpen] = useState(false);
  const [createForm, setCreateForm] = useState(emptyCreateForm);
  const [eligibleUsers, setEligibleUsers] = useState<EligibleUserResponse[] | null>(null);
  const [editing, setEditing] = useState<StudentResponse | null>(null);
  const [editForm, setEditForm] = useState(emptyEditForm);
  const [contactEditing, setContactEditing] = useState<StudentResponse | null>(null);
  const [contactForm, setContactForm] = useState(emptyContactForm);

  async function load(pageOverride?: number) {
    setIsLoading(true);
    try {
      const result = await api.get<PagedResult<StudentResponse>>("/api/admin/students", {
        search,
        page: pageOverride ?? page,
        pageSize: 10,
      });
      setData(result);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to load students.");
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
      const users = await api.get<EligibleUserResponse[]>("/api/admin/students/eligible-users");
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
      await api.post("/api/admin/students", createForm);
      setCreateOpen(false);
      setCreateForm(emptyCreateForm);
      setPage(1);
      load(1);
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to create student.");
    }
  }

  function openEdit(student: StudentResponse) {
    setEditing(student);
    setEditForm({
      firstName: student.firstName,
      lastName: student.lastName,
      dateOfBirth: student.dateOfBirth ? student.dateOfBirth.slice(0, 10) : "",
      profileImageUrl: student.profileImageUrl ?? "",
    });
  }

  async function handleEditSave() {
    if (!editing) return;
    setError(null);
    try {
      await api.put(`/api/admin/students/${editing.id}/profile`, {
        firstName: editForm.firstName,
        lastName: editForm.lastName,
        dateOfBirth: editForm.dateOfBirth || null,
      });
      // Separate endpoint on the backend (PUT .../profile-image).
      await api.put(`/api/admin/students/${editing.id}/profile-image`, { profileImageUrl: editForm.profileImageUrl || null });
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

  function openContactEdit(student: StudentResponse) {
    setContactEditing(student);
    setContactForm({
      phone: student.phone ?? "",
      email: student.email ?? "",
      address: student.address ?? "",
    });
  }

  async function handleContactSave() {
    if (!contactEditing) return;
    setError(null);
    try {
      await api.put(`/api/admin/students/${contactEditing.id}/contact`, {
        phone: contactForm.phone || null,
        email: contactForm.email || null,
        address: contactForm.address || null,
      });
      setContactEditing(null);
      load();
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Failed to save contact info.");
    }
  }

  const canCreate = hasPermission("students.create");
  const canUpdate = hasPermission("students.update");
  const canDelete = hasPermission("students.delete");

  return (
    <div>
      <PageHeader
        title="Students"
        subtitle="Promote a registered user into a student profile"
        action={canCreate && <Button onClick={openCreate}>+ New student</Button>}
      />
      <ErrorBanner message={error} />
      <div className="mb-4">
        <Input
          placeholder="Search by name, student id, or email…"
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
          <EmptyState message="No students yet." />
        ) : (
          <table className="w-full text-left text-sm">
            <thead className="border-b border-slate-100 text-xs uppercase tracking-wide text-slate-400">
              <tr>
                <th className="px-5 py-3">Name</th>
                <th className="px-5 py-3">Student ID</th>
                <th className="px-5 py-3">Guardian</th>
                <th className="px-5 py-3">Status</th>
                <th className="px-5 py-3 text-right">Actions</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map((s) => (
                <tr key={s.id} className="border-b border-slate-50 last:border-0">
                  <td className="px-5 py-3 font-medium text-ink">
                    {s.firstName} {s.lastName}
                  </td>
                  <td className="px-5 py-3 text-slate-500">{s.studentId}</td>
                  <td className="px-5 py-3 text-slate-500">{s.guardianName ?? "—"}</td>
                  <td className="px-5 py-3">
                    <Badge tone={s.isActive ? "green" : "slate"}>{s.isActive ? "Active" : "Inactive"}</Badge>
                  </td>
                  <td className="px-5 py-3">
                    <div className="flex justify-end gap-2">
                      {canUpdate && (
                        <>
                          <button onClick={() => openEdit(s)} className="text-xs font-medium text-brand-600 hover:underline">
                            Edit
                          </button>
                          <button onClick={() => openContactEdit(s)} className="text-xs font-medium text-brand-600 hover:underline">
                            Contact
                          </button>
                          <button
                            onClick={() =>
                              runAction(
                                (id) => (s.isActive ? api.post(`/api/admin/students/${id}/deactivate`) : api.post(`/api/admin/students/${id}/activate`)),
                                s.id,
                              )
                            }
                            className="text-xs font-medium text-slate-500 hover:underline"
                          >
                            {s.isActive ? "Deactivate" : "Activate"}
                          </button>
                        </>
                      )}
                      {canDelete && (
                        <button
                          onClick={() => runAction((id) => api.del(`/api/admin/students/${id}`), s.id)}
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
        <Modal title="New student" onClose={() => setCreateOpen(false)}>
          <div className="space-y-3">
            <p className="rounded-lg bg-slate-50 px-3 py-2 text-xs text-slate-500">
              Only users who registered with the <strong>Student</strong> role and don't already have a profile show up
              here.
            </p>
            <Field label="User">
              {eligibleUsers === null ? (
                <div className="py-2 text-sm text-slate-400">Loading eligible users…</div>
              ) : eligibleUsers.length === 0 ? (
                <div className="rounded-lg bg-amber-50 px-3 py-2 text-xs text-amber-700">
                  No eligible users found. Ask someone to register (Student is the default role) first, or check that
                  everyone who registered as Student hasn't already been promoted.
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
            <Field label="Student ID">
              <Input value={createForm.studentId} onChange={(e) => setCreateForm({ ...createForm, studentId: e.target.value })} />
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
                Create student
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
            <Field label="Date of birth">
              <Input type="date" value={editForm.dateOfBirth} onChange={(e) => setEditForm({ ...editForm, dateOfBirth: e.target.value })} />
            </Field>
            <Field label="Profile image URL">
              <Input
                value={editForm.profileImageUrl}
                onChange={(e) => setEditForm({ ...editForm, profileImageUrl: e.target.value })}
                placeholder="https://cdn.example.com/students/photo.jpg"
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
      {contactEditing && (
        <Modal title={`Contact — ${contactEditing.firstName} ${contactEditing.lastName}`} onClose={() => setContactEditing(null)}>
          <div className="space-y-3">
            <Field label="Phone">
              <Input value={contactForm.phone} onChange={(e) => setContactForm({ ...contactForm, phone: e.target.value })} />
            </Field>
            <Field label="Email">
              <Input type="email" value={contactForm.email} onChange={(e) => setContactForm({ ...contactForm, email: e.target.value })} />
            </Field>
            <Field label="Address">
              <Input value={contactForm.address} onChange={(e) => setContactForm({ ...contactForm, address: e.target.value })} />
            </Field>
            <div className="flex justify-end gap-2 pt-2">
              <Button variant="secondary" onClick={() => setContactEditing(null)}>
                Cancel
              </Button>
              <Button onClick={handleContactSave}>Save changes</Button>
            </div>
          </div>
        </Modal>
      )}
    </div>
  );
}
