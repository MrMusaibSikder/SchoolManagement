import { useEffect, useMemo, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Loader2, Plus } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useStudents } from "@/features/academic/hooks/useAcademicData";
import { useAuth } from "@/features/auth/hooks/useAuth";
import { useCreateGuardian, useGuardian, useUpdateGuardian } from "../hooks/useGuardianData";
import type { CreateGuardianDto, GuardianFormState } from "../types/guardian.types";

const initialForm: GuardianFormState = {
  fullName: "",
  phoneNumber: "",
  email: "",
  address: "",
  occupation: "",
  linkedStudents: [],
};

export function GuardianFormPage() {
  const navigate = useNavigate();
  const { session } = useAuth();
  const { id } = useParams();
  const isEditing = Boolean(id);
  const { data: guardian, isPending: isGuardianPending } = useGuardian(isEditing ? Number(id) : null);
  const { data: students = [] } = useStudents();
  const createGuardianMutation = useCreateGuardian();
  const updateGuardianMutation = useUpdateGuardian();
  const [form, setForm] = useState(initialForm);
  const [errorMessage, setErrorMessage] = useState("");
  const canManage = session?.roles?.some((role) => role === "GuardianCreate" || role === "GuardianEdit" || role === "Admin" || role === "SuperAdmin" || role.toLowerCase().includes("admin")) ?? false;

  useEffect(() => {
    if (guardian) {
      setForm({
        fullName: guardian.fullName ?? "",
        phoneNumber: guardian.phoneNumber ?? "",
        email: guardian.email ?? "",
        address: guardian.address ?? "",
        occupation: guardian.occupation ?? "",
        linkedStudents: [],
      });
    }
  }, [guardian]);

  const studentCountLabel = useMemo(() => {
    const count = form.linkedStudents.length;
    return count > 0 ? `${count} student${count > 1 ? "s" : ""} linked` : "No students linked yet";
  }, [form.linkedStudents.length]);

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setErrorMessage("");
    const payload: CreateGuardianDto = {
      fullName: form.fullName.trim(),
      phoneNumber: form.phoneNumber.trim(),
      email: form.email.trim() || null,
      address: form.address.trim() || null,
      occupation: form.occupation.trim() || null,
    };

    try {
      if (isEditing && id) {
        await updateGuardianMutation.mutateAsync({ id: Number(id), payload: { id: Number(id), ...payload } });
      } else {
        await createGuardianMutation.mutateAsync(payload);
      }
      navigate("/guardians");
    } catch {
      setErrorMessage("Could not save the guardian record. Please check the details and try again.");
    }
  }

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">{isEditing ? "Edit guardian" : "Add guardian"}</h1>
          <p className="text-sm text-muted-foreground">Record guardian contact details and link them to students.</p>
        </div>
        <Link to="/guardians" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">
          Back to list
        </Link>
      </div>

      {isGuardianPending && isEditing ? <div className="flex items-center justify-center py-12 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading guardian details…</div> : null}

      <Card>
        <CardHeader>
          <CardTitle>Guardian information</CardTitle>
          <CardDescription>Use the form below to save the guardian profile and connect it with students.</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-6">
            {errorMessage ? <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</div> : null}
            <div className="grid gap-4 md:grid-cols-2">
              <div>
                <label className="mb-1 block text-sm font-medium">Full name</label>
                <input required value={form.fullName} onChange={(event) => setForm((value) => ({ ...value, fullName: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Phone number</label>
                <input required value={form.phoneNumber} onChange={(event) => setForm((value) => ({ ...value, phoneNumber: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Email</label>
                <input type="email" value={form.email} onChange={(event) => setForm((value) => ({ ...value, email: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Occupation</label>
                <input value={form.occupation} onChange={(event) => setForm((value) => ({ ...value, occupation: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
            </div>

            <div>
              <label className="mb-1 block text-sm font-medium">Address</label>
              <textarea value={form.address} onChange={(event) => setForm((value) => ({ ...value, address: event.target.value }))} className="min-h-24 w-full rounded-md border bg-background px-3 py-2" />
            </div>

            <div className="rounded-lg border p-4">
              <div className="flex items-center justify-between gap-3">
                <div>
                  <h2 className="text-lg font-semibold">Student links</h2>
                  <p className="text-sm text-muted-foreground">{studentCountLabel}</p>
                </div>
                <Button type="button" variant="outline" onClick={() => setForm((value) => ({ ...value, linkedStudents: [...value.linkedStudents, { studentId: 0, relationship: "Father" }] }))}><Plus className="mr-2 h-4 w-4" />Add link</Button>
              </div>
              <div className="mt-4 space-y-3">
                {form.linkedStudents.map((entry, index) => (
                  <div key={`${entry.studentId}-${index}`} className="grid gap-3 rounded-lg border p-3 md:grid-cols-[1.2fr_0.8fr]">
                    <select value={entry.studentId} onChange={(event) => setForm((value) => ({ ...value, linkedStudents: value.linkedStudents.map((item, itemIndex) => itemIndex === index ? { ...item, studentId: Number(event.target.value) } : item) }))} className="rounded-md border bg-background px-3 py-2">
                      <option value={0}>Select student</option>
                      {students.map((student) => <option key={student.id} value={student.id}>{student.fullName} • Roll {student.rollNo}</option>)}
                    </select>
                    <input value={entry.relationship} onChange={(event) => setForm((value) => ({ ...value, linkedStudents: value.linkedStudents.map((item, itemIndex) => itemIndex === index ? { ...item, relationship: event.target.value } : item) }))} placeholder="Relationship" className="rounded-md border bg-background px-3 py-2" />
                  </div>
                ))}
              </div>
            </div>

            <div className="flex gap-2">
              <Button type="submit" disabled={createGuardianMutation.isPending || updateGuardianMutation.isPending || !canManage}>
                {createGuardianMutation.isPending || updateGuardianMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                {isEditing ? "Save changes" : "Create guardian"}
              </Button>
              <Button type="button" variant="outline" onClick={() => navigate("/guardians")}>Cancel</Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
