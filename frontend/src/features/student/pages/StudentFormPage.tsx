import { useEffect, useMemo, useRef, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { CalendarDays, ImagePlus, Loader2, Plus, Trash2, UserRoundPlus, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { MediaImage } from "@/components/common/MediaImage";
import { useSchoolClasses, useSections } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { useCreateGuardian, useGuardians } from "@/features/guardian/hooks/useGuardianData";
import type { CreateGuardianDto, GuardianDto } from "@/features/guardian/types/guardian.types";
import { Permission } from "@/lib/permissions";
import { useCreateStudent, useStudent, useUpdateStudent } from "../hooks/useStudentData";
import type { CreateStudentDto, StudentDto } from "../types/student.types";

type StudentFormState = {
  admissionNumber: string;
  fullName: string;
  dateOfBirth: string;
  rollNo: string;
  admissionDate: string;
  gender: string;
  bloodGroup: string;
  address: string;
  classId: string;
  sectionId: string;
  guardians: Array<{ guardianId?: number; guardianName?: string; relationship: string }>;
};

const getLocalDate = () => {
  const date = new Date();
  const offset = date.getTimezoneOffset() * 60_000;
  return new Date(date.getTime() - offset).toISOString().slice(0, 10);
};

const emptyForm: StudentFormState = {
  admissionNumber: "",
  fullName: "",
  dateOfBirth: "",
  rollNo: "",
  admissionDate: "",
  gender: "Male",
  bloodGroup: "",
  address: "",
  classId: "",
  sectionId: "",
  guardians: [{ guardianId: undefined, relationship: "Father" }],
};

function toStudentForm(student: StudentDto): StudentFormState {
  return {
    admissionNumber: student.admissionNumber?.startsWith("ADM-") ? student.admissionNumber.slice(4) : student.admissionNumber ?? "",
    fullName: student.fullName ?? "",
    dateOfBirth: student.dateOfBirth ? student.dateOfBirth.slice(0, 10) : "",
    rollNo: student.rollNo ?? "",
    admissionDate: student.admissionDate ? student.admissionDate.slice(0, 10) : "",
    gender: student.gender ?? "Male",
    bloodGroup: student.bloodGroup ?? "",
    address: student.address ?? "",
    classId: String(student.classId),
    sectionId: String(student.sectionId),
    guardians: student.guardians?.length
      ? student.guardians.map((guardian) => ({ guardianId: guardian.guardianId, guardianName: guardian.guardianName ?? undefined, relationship: guardian.relationship ?? "Father" }))
      : [{ guardianId: undefined, relationship: "Father" }],
  };
}

export function StudentFormPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditing = Boolean(id);
  const { data: student, isPending: isStudentPending } = useStudent(isEditing ? Number(id) : null);
  const { hasPermission } = usePermissions();
  const createStudentMutation = useCreateStudent();
  const updateStudentMutation = useUpdateStudent();
  const createGuardianMutation = useCreateGuardian();
  const { data: guardians = [], isPending: isGuardiansPending } = useGuardians();
  const { data: classes = [] } = useSchoolClasses();
  const { data: sections = [] } = useSections();
  const [blankForm] = useState(() => ({ ...emptyForm, admissionDate: getLocalDate() }));
  const [formOverride, setFormOverride] = useState<{ studentId: number | null; value: StudentFormState } | null>(null);
  const initialForm = useMemo(() => student ? toStudentForm(student) : blankForm, [blankForm, student]);
  const canCreateGuardian = hasPermission(Permission.GuardianCreate);
  const studentId = student?.id ?? null;
  const form = formOverride?.studentId === studentId ? formOverride.value : initialForm;
  const [photoFile, setPhotoFile] = useState<File | null>(null);
  const [errorMessage, setErrorMessage] = useState("");
  const [photoError, setPhotoError] = useState("");
  const [guardianDialogOpen, setGuardianDialogOpen] = useState(false);
  const [guardianError, setGuardianError] = useState("");
  const [newGuardian, setNewGuardian] = useState({ fullName: "", phoneNumber: "", email: "" });
  const photoInputRef = useRef<HTMLInputElement>(null);
  const dateOfBirthRef = useRef<HTMLInputElement>(null);
  const admissionDateRef = useRef<HTMLInputElement>(null);
  const [photoPreviewUrl, setPhotoPreviewUrl] = useState<string | null>(null);

  useEffect(() => {
    if (!photoPreviewUrl) return;
    return () => URL.revokeObjectURL(photoPreviewUrl);
  }, [photoPreviewUrl]);

  function setForm(update: (current: StudentFormState) => StudentFormState) {
    setFormOverride((current) => ({
      studentId,
      value: update(current?.studentId === studentId ? current.value : initialForm),
    }));
  }

  const availableSections = useMemo(() => sections.filter((section) => String(section.classId) === form.classId), [sections, form.classId]);
  const guardianOptions = useMemo(() => {
    const options = new Map<number, GuardianDto>(guardians.map((guardian) => [guardian.id, guardian]));
    form.guardians.forEach((guardian) => {
      if (guardian.guardianId && guardian.guardianName && !options.has(guardian.guardianId)) {
        options.set(guardian.guardianId, { id: guardian.guardianId, fullName: guardian.guardianName, phoneNumber: "" });
      }
    });
    return [...options.values()].sort((first, second) => first.fullName.localeCompare(second.fullName));
  }, [form.guardians, guardians]);

  function updateGuardian(index: number, changes: Partial<StudentFormState["guardians"][number]>) {
    setForm((value) => ({
      ...value,
      guardians: value.guardians.map((guardian, guardianIndex) => guardianIndex === index ? { ...guardian, ...changes } : guardian),
    }));
  }

  async function handleCreateGuardian(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setGuardianError("");
    const payload: CreateGuardianDto = {
      fullName: newGuardian.fullName.trim(),
      phoneNumber: newGuardian.phoneNumber.trim(),
      email: newGuardian.email.trim() || null,
    };

    try {
      const createdGuardian = await createGuardianMutation.mutateAsync(payload);
      setForm((value) => {
        const emptyGuardianIndex = value.guardians.findIndex((guardian) => !guardian.guardianId);
        if (emptyGuardianIndex < 0) {
          return { ...value, guardians: [...value.guardians, { guardianId: createdGuardian.id, guardianName: createdGuardian.fullName, relationship: "Father" }] };
        }
        return {
          ...value,
          guardians: value.guardians.map((guardian, index) => index === emptyGuardianIndex
            ? { ...guardian, guardianId: createdGuardian.id, guardianName: createdGuardian.fullName }
            : guardian),
        };
      });
      setNewGuardian({ fullName: "", phoneNumber: "", email: "" });
      setGuardianDialogOpen(false);
    } catch {
      setGuardianError("Could not create the guardian. Please check the details and try again.");
    }
  }

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setErrorMessage("");
    const payload: CreateStudentDto = {
      admissionNumber: `ADM-${form.admissionNumber.trim()}`,
      fullName: form.fullName,
      dateOfBirth: form.dateOfBirth,
      rollNo: form.rollNo,
      admissionDate: form.admissionDate,
      gender: form.gender,
      bloodGroup: form.bloodGroup || null,
      address: form.address || null,
      classId: Number(form.classId),
      sectionId: Number(form.sectionId),
      guardians: form.guardians.filter((guardian) => guardian.guardianId).map((guardian) => ({ guardianId: guardian.guardianId, relationship: guardian.relationship || null })),
    };

    try {
      if (isEditing && id) {
        await updateStudentMutation.mutateAsync({ id: Number(id), payload: { ...payload, id: Number(id) }, photoFile });
      } else {
        await createStudentMutation.mutateAsync({ payload, photoFile });
      }
      navigate("/students");
    } catch {
      setErrorMessage("Could not save the student record. Please verify the information and try again.");
    }
  }

  return (
    <div className="mx-auto max-w-6xl space-y-6 pb-8">
      <div className="flex flex-col gap-4 border-b pb-5 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <p className="text-xs font-semibold uppercase tracking-[0.12em] text-primary">Student records</p>
          <h1 className="mt-1 font-display text-2xl font-semibold">{isEditing ? "Edit student" : "Add student"}</h1>
          <p className="mt-1 text-sm text-muted-foreground">Student profile, enrollment, and guardian details.</p>
        </div>
        <Link to="/students" className="inline-flex h-10 items-center justify-center rounded-md border border-input bg-background px-4 text-sm font-medium transition hover:bg-accent">
          Back to list
        </Link>
      </div>

      {isStudentPending && isEditing ? <div className="flex items-center justify-center py-12 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading student details...</div> : null}

      <Card>
        <CardHeader className="border-b bg-muted/20">
          <CardTitle>Student information</CardTitle>
          <CardDescription>Complete the required fields, then add class placement and guardian details.</CardDescription>
        </CardHeader>
        <CardContent className="pt-6">
          <form onSubmit={handleSubmit} className="space-y-8">
            {errorMessage ? <div role="alert" className="rounded-md border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</div> : null}

            <section className="space-y-4">
              <h2 className="text-base font-semibold">Personal details</h2>
              <div className="grid gap-x-5 gap-y-4 md:grid-cols-2">
                <div>
                  <label htmlFor="admissionNumber" className="mb-1.5 block text-sm font-medium">Admission number</label>
                  <div className="flex overflow-hidden rounded-md border bg-background focus-within:ring-2 focus-within:ring-ring/30">
                    <span className="inline-flex items-center border-r bg-muted/50 px-3 text-sm font-semibold text-muted-foreground">ADM-</span>
                    <input id="admissionNumber" required value={form.admissionNumber} onChange={(event) => setForm((value) => ({ ...value, admissionNumber: event.target.value.replace(/^ADM-/i, "") }))} className="min-w-0 flex-1 bg-transparent px-3 py-2 text-sm outline-none" />
                  </div>
                </div>
                <div>
                  <label htmlFor="fullName" className="mb-1.5 block text-sm font-medium">Full name</label>
                  <input id="fullName" required value={form.fullName} onChange={(event) => setForm((value) => ({ ...value, fullName: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/30" />
                </div>
                <div>
                  <label htmlFor="dateOfBirth" className="mb-1.5 block text-sm font-medium">Date of birth</label>
                  <div className="relative">
                    <input ref={dateOfBirthRef} id="dateOfBirth" type="date" required value={form.dateOfBirth} onChange={(event) => setForm((value) => ({ ...value, dateOfBirth: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2 pr-11 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/30" />
                    <button type="button" aria-label="Open date of birth calendar" onClick={() => { const input = dateOfBirthRef.current; if (input?.showPicker) input.showPicker(); else input?.click(); }} className="absolute inset-y-0 right-0 inline-flex w-10 items-center justify-center text-muted-foreground hover:text-foreground"><CalendarDays className="h-4 w-4" /></button>
                  </div>
                </div>
                <div>
                  <label htmlFor="rollNo" className="mb-1.5 block text-sm font-medium">Roll number</label>
                  <input id="rollNo" required value={form.rollNo} onChange={(event) => setForm((value) => ({ ...value, rollNo: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/30" />
                </div>
                <div>
                  <label htmlFor="admissionDate" className="mb-1.5 block text-sm font-medium">Admission date</label>
                  <div className="relative">
                    <input ref={admissionDateRef} id="admissionDate" type="date" required value={form.admissionDate} onChange={(event) => setForm((value) => ({ ...value, admissionDate: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2 pr-11 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/30" />
                    <button type="button" aria-label="Open admission date calendar" onClick={() => { const input = admissionDateRef.current; if (input?.showPicker) input.showPicker(); else input?.click(); }} className="absolute inset-y-0 right-0 inline-flex w-10 items-center justify-center text-muted-foreground hover:text-foreground"><CalendarDays className="h-4 w-4" /></button>
                  </div>
                </div>
                <div>
                  <label htmlFor="gender" className="mb-1.5 block text-sm font-medium">Gender</label>
                  <select id="gender" value={form.gender} onChange={(event) => setForm((value) => ({ ...value, gender: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/30">
                    <option value="Male">Male</option>
                    <option value="Female">Female</option>
                    <option value="Other">Other</option>
                  </select>
                </div>
                <div>
                  <label htmlFor="bloodGroup" className="mb-1.5 block text-sm font-medium">Blood group</label>
                  <select id="bloodGroup" value={form.bloodGroup} onChange={(event) => setForm((value) => ({ ...value, bloodGroup: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/30">
                    <option value="">Select blood group</option>
                    {["A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-"].map((group) => <option key={group} value={group}>{group}</option>)}
                  </select>
                </div>
                <div className="md:row-span-2">
                  <span className="mb-1.5 block text-sm font-medium">Student photo</span>
                  <div className="flex min-h-28 items-center gap-4 rounded-md border border-dashed bg-muted/10 p-3">
                    <div className="h-20 w-20 shrink-0 overflow-hidden rounded-md border bg-muted">
                      <MediaImage src={photoPreviewUrl ?? student?.photo} alt={`${form.fullName || "Student"} photo`} />
                    </div>
                    <div className="min-w-0 space-y-2">
                      <p className="truncate text-sm font-medium">{photoFile?.name ?? (student?.photo ? "Current photo" : "No photo selected")}</p>
                      <div className="flex flex-wrap items-center gap-2">
                        <Button type="button" variant="outline" onClick={() => photoInputRef.current?.click()} className="h-8 px-3 text-xs"><ImagePlus className="mr-2 h-4 w-4" />{photoFile ? "Replace" : "Choose photo"}</Button>
                        {photoFile ? <Button type="button" variant="secondary" onClick={() => { setPhotoFile(null); setPhotoPreviewUrl(null); if (photoInputRef.current) photoInputRef.current.value = ""; }} className="h-8 px-3 text-xs">Remove</Button> : null}
                      </div>
                      <input ref={photoInputRef} type="file" accept="image/*" className="sr-only" onChange={(event) => {
                        const file = event.target.files?.[0];
                        if (!file) return;
                        if (!file.type.startsWith("image/")) {
                          setPhotoError("Choose an image file to upload.");
                          event.target.value = "";
                          return;
                        }
                        if (file.size > 5 * 1024 * 1024) {
                          setPhotoError("Photo must be 5 MB or smaller.");
                          event.target.value = "";
                          return;
                        }
                        setPhotoError("");
                        setPhotoPreviewUrl(URL.createObjectURL(file));
                        setPhotoFile(file);
                      }} />
                    </div>
                  </div>
                  {photoError ? <p role="alert" className="mt-1.5 text-xs text-destructive">{photoError}</p> : <p className="mt-1.5 text-xs text-muted-foreground">Image file, maximum 5 MB.</p>}
                </div>
              </div>
              <div>
                <label htmlFor="address" className="mb-1.5 block text-sm font-medium">Address</label>
                <textarea id="address" value={form.address} onChange={(event) => setForm((value) => ({ ...value, address: event.target.value }))} className="min-h-24 w-full rounded-md border bg-background px-3 py-2 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/30" />
              </div>
            </section>

            <section className="space-y-4 border-t pt-6">
              <div>
                <h2 className="text-base font-semibold">Class placement</h2>
                <p className="mt-1 text-sm text-muted-foreground">Choose the class first to load its sections.</p>
              </div>
              <div className="grid gap-4 md:grid-cols-2">
                <div>
                  <label htmlFor="classId" className="mb-1.5 block text-sm font-medium">Class</label>
                  <select id="classId" required value={form.classId} onChange={(event) => setForm((value) => ({ ...value, classId: event.target.value, sectionId: "" }))} className="w-full rounded-md border bg-background px-3 py-2 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/30">
                    <option value="">Select class</option>
                    {classes.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
                  </select>
                </div>
                <div>
                  <label htmlFor="sectionId" className="mb-1.5 block text-sm font-medium">Section</label>
                  <select id="sectionId" required value={form.sectionId} onChange={(event) => setForm((value) => ({ ...value, sectionId: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/30">
                    <option value="">Select section</option>
                    {availableSections.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
                  </select>
                </div>
              </div>
            </section>

            <section className="space-y-4 border-t pt-6">
              <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                <div>
                  <h2 className="text-base font-semibold">Guardian information</h2>
                  <p className="mt-1 text-sm text-muted-foreground">Select an existing guardian or create one here.</p>
                </div>
                <div className="flex flex-wrap gap-2">
                  <Button type="button" variant="outline" onClick={() => setForm((value) => ({ ...value, guardians: [...value.guardians, { guardianId: undefined, relationship: "Father" }] }))}><Plus className="mr-2 h-4 w-4" />Add guardian</Button>
                  {canCreateGuardian ? <Button type="button" variant="outline" onClick={() => { setGuardianError(""); setGuardianDialogOpen(true); }}><UserRoundPlus className="mr-2 h-4 w-4" />Create guardian</Button> : null}
                </div>
              </div>
              {form.guardians.length === 0 ? <p className="rounded-md border border-dashed p-4 text-sm text-muted-foreground">No guardians added.</p> : null}
              <div className="space-y-3">
                {form.guardians.map((guardian, index) => (
                  <div key={`${guardian.guardianId ?? "new"}-${index}`} className="grid gap-3 rounded-md border bg-muted/10 p-3 sm:grid-cols-[minmax(0,1fr)_minmax(0,1fr)_auto] sm:items-end">
                    <div>
                      <label htmlFor={`guardian-${index}`} className="mb-1.5 block text-sm font-medium">Guardian name</label>
                      <select id={`guardian-${index}`} value={guardian.guardianId ?? ""} onChange={(event) => {
                        const selectedId = event.target.value ? Number(event.target.value) : undefined;
                        const selectedGuardian = guardianOptions.find((item) => item.id === selectedId);
                        updateGuardian(index, { guardianId: selectedId, guardianName: selectedGuardian?.fullName });
                      }} className="w-full rounded-md border bg-background px-3 py-2 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/30">
                        <option value="">{isGuardiansPending ? "Loading guardians..." : "Select guardian"}</option>
                        {guardianOptions.map((item) => <option key={item.id} value={item.id}>{item.fullName}{item.phoneNumber ? ` · ${item.phoneNumber}` : ""}</option>)}
                      </select>
                    </div>
                    <div>
                      <label htmlFor={`relationship-${index}`} className="mb-1.5 block text-sm font-medium">Relationship</label>
                      <input id={`relationship-${index}`} value={guardian.relationship} onChange={(event) => updateGuardian(index, { relationship: event.target.value })} className="w-full rounded-md border bg-background px-3 py-2 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/30" />
                    </div>
                    <Button type="button" variant="outline" aria-label="Remove guardian" title="Remove guardian" onClick={() => setForm((value) => ({ ...value, guardians: value.guardians.filter((_, guardianIndex) => guardianIndex !== index) }))} className="h-10 w-10 p-0 text-muted-foreground hover:text-destructive"><Trash2 className="h-4 w-4" /></Button>
                  </div>
                ))}
              </div>
            </section>

            <div className="flex flex-col-reverse gap-2 border-t pt-5 sm:flex-row sm:justify-end">
              <Button type="button" variant="outline" onClick={() => navigate("/students")}>Cancel</Button>
              <Button type="submit" disabled={(isEditing && isStudentPending) || createStudentMutation.isPending || updateStudentMutation.isPending}>
                {createStudentMutation.isPending || updateStudentMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                {isEditing ? "Save changes" : "Create student"}
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>

      {guardianDialogOpen ? (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-foreground/40 p-4" onClick={() => !createGuardianMutation.isPending && setGuardianDialogOpen(false)}>
          <section role="dialog" aria-modal="true" aria-labelledby="create-guardian-title" onClick={(event) => event.stopPropagation()} className="w-full max-w-lg rounded-lg border bg-background shadow-xl">
            <div className="flex items-start justify-between border-b px-5 py-4">
              <div>
                <h2 id="create-guardian-title" className="font-display text-lg font-semibold">Create guardian</h2>
                <p className="mt-1 text-sm text-muted-foreground">The new guardian will be selected for this student.</p>
              </div>
              <Button type="button" variant="outline" aria-label="Close dialog" disabled={createGuardianMutation.isPending} onClick={() => setGuardianDialogOpen(false)} className="h-9 w-9 p-0"><X className="h-4 w-4" /></Button>
            </div>
            <form onSubmit={handleCreateGuardian} className="space-y-4 p-5">
              {guardianError ? <div role="alert" className="rounded-md border border-destructive/30 bg-destructive/5 p-3 text-sm text-destructive">{guardianError}</div> : null}
              <div>
                <label htmlFor="newGuardianName" className="mb-1.5 block text-sm font-medium">Full name</label>
                <input autoFocus id="newGuardianName" required value={newGuardian.fullName} onChange={(event) => setNewGuardian((value) => ({ ...value, fullName: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/30" />
              </div>
              <div>
                <label htmlFor="newGuardianPhone" className="mb-1.5 block text-sm font-medium">Phone number</label>
                <input id="newGuardianPhone" type="tel" required value={newGuardian.phoneNumber} onChange={(event) => setNewGuardian((value) => ({ ...value, phoneNumber: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/30" />
              </div>
              <div>
                <label htmlFor="newGuardianEmail" className="mb-1.5 block text-sm font-medium">Email <span className="font-normal text-muted-foreground">(optional)</span></label>
                <input id="newGuardianEmail" type="email" value={newGuardian.email} onChange={(event) => setNewGuardian((value) => ({ ...value, email: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2 text-sm outline-none focus-visible:ring-2 focus-visible:ring-ring/30" />
              </div>
              <div className="flex justify-end gap-2 border-t pt-4">
                <Button type="button" variant="outline" disabled={createGuardianMutation.isPending} onClick={() => setGuardianDialogOpen(false)}>Cancel</Button>
                <Button type="submit" disabled={createGuardianMutation.isPending}>
                  {createGuardianMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <UserRoundPlus className="mr-2 h-4 w-4" />}
                  Create and select
                </Button>
              </div>
            </form>
          </section>
        </div>
      ) : null}
    </div>
  );
}
