import { useMemo } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Loader2, Pencil, UserRound } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useSchoolClasses, useSections } from "@/features/academic/hooks/useAcademicData";
import { useStudent, useStudentAttendanceHistory, useStudentDocuments } from "../hooks/useStudentData";

export function StudentDetailsPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const studentId = Number(id);
  const { data: student, isPending, isError } = useStudent(Number.isNaN(studentId) ? null : studentId);
  const { data: docs = [] } = useStudentDocuments(Number.isNaN(studentId) ? null : studentId);
  const { data: history = [] } = useStudentAttendanceHistory(Number.isNaN(studentId) ? null : studentId);
  const { data: classes = [] } = useSchoolClasses();
  const { data: sections = [] } = useSections();

  const className = useMemo(() => classes.find((item) => item.id === student?.classId)?.name ?? "—", [classes, student?.classId]);
  const sectionName = useMemo(() => sections.find((item) => item.id === student?.sectionId)?.name ?? "—", [sections, student?.sectionId]);

  if (isPending) {
    return <div className="mx-auto max-w-6xl py-8 text-center text-sm text-muted-foreground"><Loader2 className="mr-2 inline h-4 w-4 animate-spin" />Loading student profile…</div>;
  }

  if (isError || !student) {
    return <div className="mx-auto max-w-6xl rounded-lg border border-destructive/20 bg-destructive/5 p-6 text-sm text-destructive">Unable to load the student profile.</div>;
  }

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Student profile</h1>
          <p className="text-sm text-muted-foreground">View profile information, guardians, documents, and attendance history.</p>
        </div>
        <div className="flex gap-2">
          <Link to="/students" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">Back to list</Link>
          <Button onClick={() => navigate(`/students/${student.id}/edit`)}><Pencil className="mr-2 h-4 w-4" />Edit</Button>
        </div>
      </div>

      <Card>
        <CardContent className="grid gap-6 p-6 lg:grid-cols-[0.9fr_1.1fr]">
          <div className="flex flex-col items-center gap-4 rounded-lg border bg-muted/40 p-6 text-center">
            <div className="flex h-24 w-24 items-center justify-center rounded-full bg-background text-muted-foreground">
              {student.photo ? <img src={student.photo} alt={student.fullName} className="h-24 w-24 rounded-full object-cover" /> : <UserRound className="h-12 w-12" />}
            </div>
            <div>
              <p className="text-xl font-semibold">{student.fullName}</p>
              <p className="text-sm text-muted-foreground">Admission #{student.admissionNumber}</p>
            </div>
            <div className="rounded-full bg-primary/10 px-3 py-1 text-sm font-medium text-primary">{className} • {sectionName}</div>
          </div>

          <div className="space-y-4">
            <div className="grid gap-4 md:grid-cols-2">
              <div className="rounded-lg border p-4">
                <p className="text-sm font-medium text-muted-foreground">Date of birth</p>
                <p className="mt-1 font-medium">{student.dateOfBirth ? new Date(student.dateOfBirth).toLocaleDateString("en-BD") : "—"}</p>
              </div>
              <div className="rounded-lg border p-4">
                <p className="text-sm font-medium text-muted-foreground">Admission date</p>
                <p className="mt-1 font-medium">{student.admissionDate ? new Date(student.admissionDate).toLocaleDateString("en-BD") : "—"}</p>
              </div>
              <div className="rounded-lg border p-4">
                <p className="text-sm font-medium text-muted-foreground">Gender</p>
                <p className="mt-1 font-medium">{student.gender}</p>
              </div>
              <div className="rounded-lg border p-4">
                <p className="text-sm font-medium text-muted-foreground">Blood group</p>
                <p className="mt-1 font-medium">{student.bloodGroup ?? "—"}</p>
              </div>
            </div>
            <div className="rounded-lg border p-4">
              <p className="text-sm font-medium text-muted-foreground">Address</p>
              <p className="mt-1 text-sm">{student.address ?? "—"}</p>
            </div>
          </div>
        </CardContent>
      </Card>

      <div className="grid gap-6 lg:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Guardian information</CardTitle>
            <CardDescription>Link and review guardians connected to the student.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-3">
            {student.guardians?.length ? student.guardians.map((guardian, index) => (
              <div key={`${guardian.guardianId ?? index}`} className="rounded-lg border p-3">
                <p className="font-medium">{guardian.guardianName ?? `Guardian ${index + 1}`}</p>
                <p className="text-sm text-muted-foreground">Relationship: {guardian.relationship ?? "—"}</p>
              </div>
            )) : <div className="rounded-lg border border-dashed p-4 text-sm text-muted-foreground">No guardian information available.</div>}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Documents</CardTitle>
            <CardDescription>Uploaded student documents when provided by the backend.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-3">
            {docs.length === 0 ? <div className="rounded-lg border border-dashed p-4 text-sm text-muted-foreground">No documents available.</div> : docs.map((doc) => (
              <div key={doc.id ?? doc.fileName} className="flex items-center justify-between rounded-lg border p-3">
                <div>
                  <p className="font-medium">{doc.name ?? doc.fileName ?? "Document"}</p>
                  <p className="text-sm text-muted-foreground">{doc.fileName ?? "Uploaded file"}</p>
                </div>
                {doc.fileUrl ? <a href={doc.fileUrl} target="_blank" rel="noreferrer" className="text-sm font-medium text-primary">Open</a> : null}
              </div>
            ))}
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Attendance history</CardTitle>
          <CardDescription>Recent attendance entries for this student, when available.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-3">
          {history.length === 0 ? <div className="rounded-lg border border-dashed p-4 text-sm text-muted-foreground">No attendance history available.</div> : history.slice(0, 8).map((entry) => (
            <div key={entry.id} className="flex items-center justify-between rounded-lg border p-3">
              <div>
                <p className="font-medium">{new Date(entry.attendanceDate).toLocaleDateString("en-BD")}</p>
                <p className="text-sm text-muted-foreground">{entry.remarks ?? "No remarks"}</p>
              </div>
              <span className="rounded-full bg-muted px-2.5 py-1 text-xs font-medium">Status {entry.status}</span>
            </div>
          ))}
        </CardContent>
      </Card>
    </div>
  );
}
