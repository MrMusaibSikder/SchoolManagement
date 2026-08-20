import { useEffect, useRef, useState } from "react";
import type { ChangeEvent, FormEvent } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Loader2, Plus, Upload, UserRound } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useCreateEmployee, useEmployee, useUpdateEmployee } from "../hooks/useEmployeeData";

type EmployeeFormState = {
  employeeCode: string;
  fullName: string;
  phone: string;
  email: string;
  joiningDate: string;
  isActive: boolean;
  designationId: string;
  userId: string;
};

const initialForm: EmployeeFormState = {
  employeeCode: "",
  fullName: "",
  phone: "",
  email: "",
  joiningDate: "",
  isActive: true,
  designationId: "",
  userId: "",
};

export function EmployeeFormPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditing = Boolean(id);
  const { data: employee, isPending: isEmployeePending } = useEmployee(isEditing ? Number(id) : null);
  const createEmployeeMutation = useCreateEmployee();
  const updateEmployeeMutation = useUpdateEmployee();
  const [form, setForm] = useState(initialForm);
  const [photoFile, setPhotoFile] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState("");
  const objectUrlRef = useRef<string | null>(null);

  useEffect(() => {
    if (employee) {
      setForm({
        employeeCode: employee.employeeCode ?? "",
        fullName: employee.fullName ?? "",
        phone: employee.phone ?? "",
        email: employee.email ?? "",
        joiningDate: employee.joiningDate ? employee.joiningDate.slice(0, 10) : "",
        isActive: employee.isActive ?? true,
        designationId: employee.designationId ? String(employee.designationId) : "",
        userId: employee.userId ? String(employee.userId) : "",
      });
      setPreviewUrl(employee.employeePhoto ?? null);
      setPhotoFile(null);
    }
  }, [employee]);

  useEffect(() => {
    return () => {
      if (objectUrlRef.current) {
        URL.revokeObjectURL(objectUrlRef.current);
      }
    };
  }, []);

  function handlePhotoChange(event: ChangeEvent<HTMLInputElement>) {
    const nextFile = event.target.files?.[0] ?? null;
    setPhotoFile(nextFile);

    if (objectUrlRef.current) {
      URL.revokeObjectURL(objectUrlRef.current);
      objectUrlRef.current = null;
    }

    if (nextFile) {
      objectUrlRef.current = URL.createObjectURL(nextFile);
      setPreviewUrl(objectUrlRef.current);
      return;
    }

    setPreviewUrl(employee?.employeePhoto ?? null);
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault();
    setErrorMessage("");

    const payload = {
      employeeCode: form.employeeCode.trim(),
      fullName: form.fullName.trim(),
      phone: form.phone.trim(),
      email: form.email.trim() || null,
      joiningDate: form.joiningDate,
      isActive: form.isActive,
      designationId: Number(form.designationId),
      userId: form.userId ? Number(form.userId) : null,
    };

    try {
      if (isEditing && id) {
        await updateEmployeeMutation.mutateAsync({
          id: Number(id),
          payload: { ...payload, id: Number(id) },
          photoFile,
        });
      } else {
        await createEmployeeMutation.mutateAsync({ payload, photoFile });
      }
      navigate("/employees");
    } catch {
      setErrorMessage("Could not save the employee record. Please verify the information and try again.");
    }
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">{isEditing ? "Edit employee" : "Add employee"}</h1>
          <p className="text-sm text-muted-foreground">Capture employee details and upload a profile photo.</p>
        </div>
        <Link to="/employees" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">
          Back to list
        </Link>
      </div>

      {isEmployeePending && isEditing ? (
        <div className="flex items-center justify-center py-12 text-sm text-muted-foreground">
          <Loader2 className="mr-2 h-4 w-4 animate-spin" />
          Loading employee details…
        </div>
      ) : null}

      <Card>
        <CardHeader>
          <CardTitle>Employee information</CardTitle>
          <CardDescription>Upload a profile photo. Existing images are shown in edit mode and replaced only when you pick a new one.</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-6">
            {errorMessage ? <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</div> : null}

            <div className="flex flex-col gap-4 rounded-lg border bg-muted/20 p-4 sm:flex-row sm:items-center">
              <div className="flex h-20 w-20 items-center justify-center overflow-hidden rounded-full border bg-background text-muted-foreground">
                {previewUrl ? (
                  <img src={previewUrl} alt="Employee preview" className="h-full w-full object-cover" />
                ) : (
                  <UserRound className="h-8 w-8" />
                )}
              </div>

              <label className="flex cursor-pointer items-center gap-2 rounded-md border border-dashed bg-background px-3 py-2 text-sm text-muted-foreground">
                <Upload className="h-4 w-4" />
                <span>{photoFile ? photoFile.name : previewUrl ? "Change photo" : "Choose photo"}</span>
                <input type="file" accept="image/*" className="hidden" onChange={handlePhotoChange} />
              </label>
            </div>

            <div className="grid gap-4 md:grid-cols-2">
              <div>
                <label className="mb-1 block text-sm font-medium">Employee code</label>
                <input required value={form.employeeCode} onChange={(event) => setForm((current) => ({ ...current, employeeCode: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">Full name</label>
                <input required value={form.fullName} onChange={(event) => setForm((current) => ({ ...current, fullName: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">Phone</label>
                <input required value={form.phone} onChange={(event) => setForm((current) => ({ ...current, phone: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">Email</label>
                <input type="email" value={form.email} onChange={(event) => setForm((current) => ({ ...current, email: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">Joining date</label>
                <input type="date" required value={form.joiningDate} onChange={(event) => setForm((current) => ({ ...current, joiningDate: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">Designation ID</label>
                <input type="number" required min="1" value={form.designationId} onChange={(event) => setForm((current) => ({ ...current, designationId: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">User ID</label>
                <input type="number" min="1" value={form.userId} onChange={(event) => setForm((current) => ({ ...current, userId: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>

              <div className="flex items-center justify-start pt-8">
                <label className="flex items-center gap-2 text-sm font-medium">
                  <input type="checkbox" checked={form.isActive} onChange={(event) => setForm((current) => ({ ...current, isActive: event.target.checked }))} className="h-4 w-4" />
                  Active employee
                </label>
              </div>
            </div>

            <div className="flex gap-2">
              <Button type="submit" disabled={createEmployeeMutation.isPending || updateEmployeeMutation.isPending}>
                {createEmployeeMutation.isPending || updateEmployeeMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                {isEditing ? "Save changes" : "Create employee"}
              </Button>
              <Button type="button" variant="outline" onClick={() => navigate("/employees")}>Cancel</Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
