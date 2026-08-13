import { useMemo } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Loader2, Pencil, UserRound } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useStudents } from "@/features/academic/hooks/useAcademicData";
import { useGuardian } from "../hooks/useGuardianData";

export function GuardianDetailsPage() {
  const navigate = useNavigate();
  const { id } = useParams();
  const guardianId = Number(id);
  const { data: guardian, isPending, isError } = useGuardian(Number.isNaN(guardianId) ? null : guardianId);
  const { data: students = [] } = useStudents();

  const linkedStudents = useMemo(() => {
    return students.filter((student) => student.guardians?.some((relationship) => relationship.guardianId === guardianId));
  }, [guardianId, students]);

  if (isPending) {
    return <div className="mx-auto max-w-6xl py-8 text-center text-sm text-muted-foreground"><Loader2 className="mr-2 inline h-4 w-4 animate-spin" />Loading guardian profile…</div>;
  }

  if (isError || !guardian) {
    return <div className="mx-auto max-w-6xl rounded-lg border border-destructive/20 bg-destructive/5 p-6 text-sm text-destructive">Unable to load the guardian profile.</div>;
  }

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Guardian profile</h1>
          <p className="text-sm text-muted-foreground">Review the guardian’s contact details and linked students.</p>
        </div>
        <div className="flex gap-2">
          <Link to="/guardians" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">Back to list</Link>
          <Button onClick={() => navigate(`/guardians/${guardian.id}/edit`)}><Pencil className="mr-2 h-4 w-4" />Edit</Button>
        </div>
      </div>

      <Card>
        <CardContent className="grid gap-6 p-6 lg:grid-cols-[0.9fr_1.1fr]">
          <div className="flex flex-col items-center gap-4 rounded-lg border bg-muted/40 p-6 text-center">
            <div className="flex h-24 w-24 items-center justify-center rounded-full bg-background text-muted-foreground">
              <UserRound className="h-12 w-12" />
            </div>
            <div>
              <p className="text-xl font-semibold">{guardian.fullName}</p>
              <p className="text-sm text-muted-foreground">{guardian.phoneNumber}</p>
            </div>
          </div>

          <div className="space-y-4">
            <div className="grid gap-4 md:grid-cols-2">
              <div className="rounded-lg border p-4">
                <p className="text-sm font-medium text-muted-foreground">Email</p>
                <p className="mt-1 font-medium">{guardian.email ?? "—"}</p>
              </div>
              <div className="rounded-lg border p-4">
                <p className="text-sm font-medium text-muted-foreground">Occupation</p>
                <p className="mt-1 font-medium">{guardian.occupation ?? "—"}</p>
              </div>
            </div>
            <div className="rounded-lg border p-4">
              <p className="text-sm font-medium text-muted-foreground">Address</p>
              <p className="mt-1 text-sm">{guardian.address ?? "—"}</p>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Linked students</CardTitle>
          <CardDescription>Students connected to this guardian through the student management relationship.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-3">
          {linkedStudents.length === 0 ? <div className="rounded-lg border border-dashed p-4 text-sm text-muted-foreground">No students are linked to this guardian yet.</div> : linkedStudents.map((student) => (
            <div key={student.id} className="flex items-center justify-between rounded-lg border p-3">
              <div>
                <p className="font-medium">{student.fullName}</p>
                <p className="text-sm text-muted-foreground">Roll {student.rollNo} • Admission #{student.admissionNumber}</p>
              </div>
              <Button type="button" variant="outline" onClick={() => navigate(`/students/${student.id}`)}>View student</Button>
            </div>
          ))}
        </CardContent>
      </Card>
    </div>
  );
}
