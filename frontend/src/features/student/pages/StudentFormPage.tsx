import { useEffect, useMemo, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Loader2, Plus, Upload } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useSchoolClasses, useSections } from "@/features/academic/hooks/useAcademicData";
import { useCreateStudent, useStudent, useUpdateStudent } from "../hooks/useStudentData";
import type { CreateStudentDto } from "../types/student.types";

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
  guardians: Array<{ guardianId?: number; relationship: string }>;
};

const initialForm: StudentFormState = {
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

export function StudentFormPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditing = Boolean(id);
  const { data: student, isPending: isStudentPending } = useStudent(isEditing ? Number(id) : null);
  const createStudentMutation = useCreateStudent();
  const updateStudentMutation = useUpdateStudent();
  const { data: classes = [] } = useSchoolClasses();
  const { data: sections = [] } = useSections();
  const [form, setForm] = useState(initialForm);
  const [photoFile, setPhotoFile] = useState<File | null>(null);
  const [errorMessage, setErrorMessage] = useState("");

  useEffect(() => {
    if (student) {
      setForm({
        admissionNumber: student.admissionNumber ?? "",
        fullName: student.fullName ?? "",
        dateOfBirth: student.dateOfBirth ? student.dateOfBirth.slice(0, 10) : "",
        rollNo: student.rollNo ?? "",
        admissionDate: student.admissionDate ? student.admissionDate.slice(0, 10) : "",
        gender: student.gender ?? "Male",
        bloodGroup: student.bloodGroup ?? "",
        address: student.address ?? "",
        classId: String(student.classId),
        sectionId: String(student.sectionId),
        guardians: student.guardians?.length ? student.guardians.map((guardian) => ({ guardianId: guardian.guardianId, relationship: guardian.relationship ?? "Father" })) : [{ guardianId: undefined, relationship: "Father" }],
      });
    }
  }, [student]);

  const availableSections = useMemo(() => sections.filter((section) => String(section.classId) === form.classId), [sections, form.classId]);

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    setErrorMessage("");
    const payload: CreateStudentDto = {
      admissionNumber: form.admissionNumber,
      fullName: form.fullName,
      dateOfBirth: form.dateOfBirth,
      rollNo: form.rollNo,
      admissionDate: form.admissionDate,
      gender: form.gender,
      bloodGroup: form.bloodGroup || null,
      address: form.address || null,
      classId: Number(form.classId),
      sectionId: Number(form.sectionId),
      guardians: form.guardians.filter((guardian) => guardian.guardianId || guardian.relationship).map((guardian) => ({ guardianId: guardian.guardianId, relationship: guardian.relationship ?? null })),
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
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">{isEditing ? "Edit student" : "Add student"}</h1>
          <p className="text-sm text-muted-foreground">Capture the student profile and enrollment information in one place.</p>
        </div>
        <Link to="/students" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">
          Back to list
        </Link>
      </div>

      {isStudentPending && isEditing ? <div className="flex items-center justify-center py-12 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading student details…</div> : null}

      <Card>
        <CardHeader>
          <CardTitle>Student information</CardTitle>
          <CardDescription>Fill in the core profile, guardians, class placement, and optional photo upload.</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-6">
            {errorMessage ? <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-3 text-sm text-destructive">{errorMessage}</div> : null}
            <div className="grid gap-4 md:grid-cols-2">
              <div>
                <label className="mb-1 block text-sm font-medium">Admission number</label>
                <input required value={form.admissionNumber} onChange={(event) => setForm((value) => ({ ...value, admissionNumber: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Full name</label>
                <input required value={form.fullName} onChange={(event) => setForm((value) => ({ ...value, fullName: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Date of birth</label>
                <input type="date" required value={form.dateOfBirth} onChange={(event) => setForm((value) => ({ ...value, dateOfBirth: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Roll number</label>
                <input required value={form.rollNo} onChange={(event) => setForm((value) => ({ ...value, rollNo: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Admission date</label>
                <input type="date" required value={form.admissionDate} onChange={(event) => setForm((value) => ({ ...value, admissionDate: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Gender</label>
                <select value={form.gender} onChange={(event) => setForm((value) => ({ ...value, gender: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2">
                  <option value="Male">Male</option>
                  <option value="Female">Female</option>
                  <option value="Other">Other</option>
                </select>
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Blood group</label>
                <input value={form.bloodGroup} onChange={(event) => setForm((value) => ({ ...value, bloodGroup: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Photo</label>
                <label className="flex cursor-pointer items-center gap-2 rounded-md border border-dashed bg-background px-3 py-2 text-sm text-muted-foreground">
                  <Upload className="h-4 w-4" />
                  <span>{photoFile ? photoFile.name : "Choose photo"}</span>
                  <input type="file" accept="image/*" className="hidden" onChange={(event) => setPhotoFile(event.target.files?.[0] ?? null)} />
                </label>
              </div>
            </div>

            <div>
              <label className="mb-1 block text-sm font-medium">Address</label>
              <textarea value={form.address} onChange={(event) => setForm((value) => ({ ...value, address: event.target.value }))} className="min-h-24 w-full rounded-md border bg-background px-3 py-2" />
            </div>

            <div className="grid gap-4 md:grid-cols-2">
              <div>
                <label className="mb-1 block text-sm font-medium">Class</label>
                <select required value={form.classId} onChange={(event) => setForm((value) => ({ ...value, classId: event.target.value, sectionId: "" }))} className="w-full rounded-md border bg-background px-3 py-2">
                  <option value="">Select class</option>
                  {classes.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
                </select>
              </div>
              <div>
                <label className="mb-1 block text-sm font-medium">Section</label>
                <select required value={form.sectionId} onChange={(event) => setForm((value) => ({ ...value, sectionId: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2">
                  <option value="">Select section</option>
                  {availableSections.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
                </select>
              </div>
            </div>

            <div className="space-y-3">
              <div className="flex items-center justify-between">
                <h2 className="text-lg font-semibold">Guardian information</h2>
                <Button type="button" variant="outline" onClick={() => setForm((value) => ({ ...value, guardians: [...value.guardians, { guardianId: undefined, relationship: "Father" }] }))}><Plus className="mr-2 h-4 w-4" />Add guardian</Button>
              </div>
              {form.guardians.map((guardian, index) => (
                <div key={`${guardian.relationship}-${index}`} className="grid gap-3 rounded-lg border p-4 md:grid-cols-2">
                  <div>
                    <label className="mb-1 block text-sm font-medium">Relationship</label>
                    <input value={guardian.relationship ?? ""} onChange={(event) => setForm((value) => ({ ...value, guardians: value.guardians.map((item, itemIndex) => itemIndex === index ? { ...item, relationship: event.target.value } : item) }))} className="w-full rounded-md border bg-background px-3 py-2" />
                  </div>
                  <div>
                    <label className="mb-1 block text-sm font-medium">Guardian ID</label>
                    <input type="number" value={guardian.guardianId ?? ""} onChange={(event) => setForm((value) => ({ ...value, guardians: value.guardians.map((item, itemIndex) => itemIndex === index ? { ...item, guardianId: event.target.value ? Number(event.target.value) : undefined } : item) }))} className="w-full rounded-md border bg-background px-3 py-2" />
                  </div>
                </div>
              ))}
            </div>

            <div className="flex gap-2">
              <Button type="submit" disabled={createStudentMutation.isPending || updateStudentMutation.isPending}>
                {createStudentMutation.isPending || updateStudentMutation.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                {isEditing ? "Save changes" : "Create student"}
              </Button>
              <Button type="button" variant="outline" onClick={() => navigate("/students")}>Cancel</Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
