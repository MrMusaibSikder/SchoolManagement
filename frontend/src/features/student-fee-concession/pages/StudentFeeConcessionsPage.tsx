import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Check, Loader2, Plus, Search, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useAcademicYears } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { useFeeTypes } from "@/features/fee-type/hooks/useFeeTypeData";
import { useStudents } from "@/features/student/hooks/useStudentData";
import { Permission } from "@/lib/permissions";
import {
  useApproveStudentConcession,
  useCreateStudentConcession,
  useDeleteStudentConcession,
  usePendingConcessions,
  useStudentConcessions,
} from "../hooks/useStudentFeeConcessionData";
import { ConcessionType, type ConcessionTypeValue } from "../types/student-fee-concession.types";

const typeOptions = [
  { value: ConcessionType.PercentageDiscount, label: "Percentage discount" },
  { value: ConcessionType.FixedAmountDiscount, label: "Fixed amount discount" },
  { value: ConcessionType.FullExemption, label: "Full exemption" },
] as const;

interface ConcessionDraft {
  studentId: string;
  feeTypeId: string;
  academicYearId: string;
  type: ConcessionTypeValue;
  value: string;
  reason: string;
  requiresApproval: boolean;
  validFrom: string;
  validTo: string;
}

function initialDraft(): ConcessionDraft {
  return {
    studentId: "",
    feeTypeId: "",
    academicYearId: "",
    type: ConcessionType.PercentageDiscount,
    value: "",
    reason: "",
    requiresApproval: true,
    validFrom: "",
    validTo: "",
  };
}

function typeLabel(type: number) {
  return typeOptions.find((option) => option.value === type)?.label ?? "Unknown concession";
}

export function StudentFeeConcessionsPage() {
  const { hasPermission } = usePermissions();
  const canView = hasPermission(Permission.ConcessionView);
  const canCreate = hasPermission(Permission.ConcessionCreate);
  const canApprove = hasPermission(Permission.ConcessionApprove);
  const canDelete = hasPermission(Permission.ConcessionDelete);

  const { data: students = [] } = useStudents();
  const { data: feeTypes = [] } = useFeeTypes();
  const { data: academicYears = [] } = useAcademicYears();
  const [studentFilter, setStudentFilter] = useState("");
  const [studentId, setStudentId] = useState("");
  const { data: studentConcessions = [], isPending: isStudentPending, isError: isStudentError } = useStudentConcessions(
    studentId ? Number(studentId) : null,
    canView
  );
  const { data: pendingConcessions = [], isPending: isPendingQueue, isError: isPendingError } = usePendingConcessions(canApprove);
  const createConcession = useCreateStudentConcession();
  const approveConcession = useApproveStudentConcession();
  const deleteConcession = useDeleteStudentConcession();
  const [draft, setDraft] = useState<ConcessionDraft>(initialDraft);

  const matchingStudents = useMemo(() => {
    const term = studentFilter.trim().toLowerCase();
    if (!term) return students;
    return students.filter((student) => `${student.fullName} ${student.admissionNumber}`.toLowerCase().includes(term));
  }, [studentFilter, students]);

  async function handleCreate(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const value = draft.value === "" ? null : Number(draft.value);
    if (!draft.studentId || !draft.feeTypeId || !draft.academicYearId || !draft.reason.trim()) {
      toast.error("Complete all required concession details.");
      return;
    }
    if (draft.type !== ConcessionType.FullExemption && value === null) {
      toast.error("Enter a concession value.");
      return;
    }
    if (draft.type === ConcessionType.PercentageDiscount && (value === null || value < 0.01 || value > 100)) {
      toast.error("Percentage must be between 0.01 and 100.");
      return;
    }
    if (draft.type === ConcessionType.FixedAmountDiscount && (value === null || value <= 0)) {
      toast.error("Fixed amount must be greater than zero.");
      return;
    }
    if (draft.reason.trim().length > 300) {
      toast.error("Reason must be 300 characters or fewer.");
      return;
    }
    if (draft.validFrom && draft.validTo && draft.validTo <= draft.validFrom) {
      toast.error("Valid-to date must be after valid-from date.");
      return;
    }

    try {
      await createConcession.mutateAsync({
        studentId: Number(draft.studentId),
        feeTypeId: Number(draft.feeTypeId),
        academicYearId: Number(draft.academicYearId),
        type: draft.type,
        value: draft.type === ConcessionType.FullExemption ? null : value,
        reason: draft.reason.trim(),
        requiresApproval: draft.requiresApproval,
        validFrom: draft.validFrom || null,
        validTo: draft.validTo || null,
      });
      toast.success(draft.requiresApproval ? "Concession submitted for approval." : "Concession created and activated.");
      setStudentId(draft.studentId);
      setDraft(initialDraft());
    } catch (error) {
      toast.error((error as Error)?.message ?? "Unable to create concession.");
    }
  }

  async function handleApprove(id: number) {
    if (!window.confirm("Approve this student fee concession?")) return;
    try {
      await approveConcession.mutateAsync(id);
      toast.success("Concession approved.");
    } catch (error) {
      toast.error((error as Error)?.message ?? "Unable to approve concession.");
    }
  }

  async function handleDelete(id: number) {
    if (!window.confirm("Remove this fee concession? It will no longer be available.")) return;
    try {
      await deleteConcession.mutateAsync(id);
      toast.success("Concession deleted.");
    } catch (error) {
      toast.error((error as Error)?.message ?? "Unable to delete concession.");
    }
  }

  if (!canView && !canCreate && !canApprove) {
    return <Card className="mx-auto max-w-4xl"><CardContent className="p-6 text-sm text-destructive">You do not have permission to manage student fee concessions.</CardContent></Card>;
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Student fee concessions</h1>
          <p className="text-sm text-muted-foreground">Manage student-specific discounts and exemptions.</p>
        </div>
        <Link to="/fees/structures" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">Fee structures</Link>
      </div>

      {canCreate && (
        <Card>
          <CardHeader>
            <CardTitle>Create concession</CardTitle>
            <CardDescription>One concession per student, fee type, and academic year.</CardDescription>
          </CardHeader>
          <CardContent>
            <form onSubmit={(event) => void handleCreate(event)} className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
              <div>
                <label htmlFor="concession-student-search" className="mb-1 block text-sm font-medium">Find student</label>
                <div className="relative mb-2">
                  <Search aria-hidden="true" className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                  <input id="concession-student-search" value={studentFilter} onChange={(event) => setStudentFilter(event.target.value)} placeholder="Name or admission number" className="w-full rounded-md border bg-background py-2 pl-9 pr-3" />
                </div>
                <select required value={draft.studentId} onChange={(event) => setDraft((current) => ({ ...current, studentId: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2">
                  <option value="">Select student</option>
                  {matchingStudents.map((student) => <option key={student.id} value={student.id}>{student.fullName} · {student.admissionNumber}</option>)}
                </select>
              </div>
              <div>
                <label htmlFor="concession-fee-type" className="mb-1 block text-sm font-medium">Fee type</label>
                <select required value={draft.feeTypeId} onChange={(event) => setDraft((current) => ({ ...current, feeTypeId: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2">
                  <option value="">Select fee type</option>
                  {feeTypes.filter((feeType) => feeType.isActive).map((feeType) => <option key={feeType.id} value={feeType.id}>{feeType.name} · {feeType.code}</option>)}
                </select>
              </div>
              <div>
                <label htmlFor="concession-academic-year" className="mb-1 block text-sm font-medium">Academic year</label>
                <select required value={draft.academicYearId} onChange={(event) => setDraft((current) => ({ ...current, academicYearId: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2">
                  <option value="">Select academic year</option>
                  {academicYears.map((year) => <option key={year.id} value={year.id}>{year.name}{year.isCurrent ? " · Current" : ""}</option>)}
                </select>
              </div>
              <div>
                <label htmlFor="concession-type" className="mb-1 block text-sm font-medium">Concession type</label>
                <select id="concession-type" value={draft.type} onChange={(event) => setDraft((current) => ({ ...current, type: Number(event.target.value) as ConcessionTypeValue, value: "" }))} className="w-full rounded-md border bg-background px-3 py-2">
                  {typeOptions.map((option) => <option key={option.value} value={option.value}>{option.label}</option>)}
                </select>
              </div>
              {draft.type !== ConcessionType.FullExemption && (
                <div>
                  <label htmlFor="concession-value" className="mb-1 block text-sm font-medium">{draft.type === ConcessionType.PercentageDiscount ? "Discount percentage" : "Discount amount"}</label>
                  <input id="concession-value" required type="number" min={draft.type === ConcessionType.PercentageDiscount ? "0.01" : "0.01"} max={draft.type === ConcessionType.PercentageDiscount ? "100" : undefined} step="0.01" value={draft.value} onChange={(event) => setDraft((current) => ({ ...current, value: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
                </div>
              )}
              <div className="md:col-span-2 xl:col-span-3">
                <label htmlFor="concession-reason" className="mb-1 block text-sm font-medium">Reason</label>
                <textarea id="concession-reason" required maxLength={300} rows={2} value={draft.reason} onChange={(event) => setDraft((current) => ({ ...current, reason: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
                <p className="mt-1 text-right text-xs text-muted-foreground">{draft.reason.length}/300</p>
              </div>
              <div>
                <label htmlFor="concession-valid-from" className="mb-1 block text-sm font-medium">Valid from <span className="text-muted-foreground">(optional)</span></label>
                <input id="concession-valid-from" type="date" value={draft.validFrom} onChange={(event) => setDraft((current) => ({ ...current, validFrom: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <div>
                <label htmlFor="concession-valid-to" className="mb-1 block text-sm font-medium">Valid to <span className="text-muted-foreground">(optional)</span></label>
                <input id="concession-valid-to" type="date" min={draft.validFrom || undefined} value={draft.validTo} onChange={(event) => setDraft((current) => ({ ...current, validTo: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
              </div>
              <label className="flex items-center gap-2 self-end pb-2 text-sm"><input type="checkbox" checked={draft.requiresApproval} onChange={(event) => setDraft((current) => ({ ...current, requiresApproval: event.target.checked }))} />Requires approval</label>
              <div className="md:col-span-2 xl:col-span-3">
                <Button type="submit" disabled={createConcession.isPending || students.length === 0 || feeTypes.length === 0 || academicYears.length === 0}>
                  {createConcession.isPending ? <Loader2 className="mr-2 h-4 w-4 animate-spin" /> : <Plus className="mr-2 h-4 w-4" />}
                  Create concession
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      )}

      <div className="grid gap-6 xl:grid-cols-2">
        {canView && (
          <Card>
            <CardHeader>
              <CardTitle>By student</CardTitle>
              <CardDescription>Choose a student to inspect their concession history.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="grid gap-2 sm:grid-cols-[1fr_1.4fr]">
                <label htmlFor="student-concession-search" className="sr-only">Search students</label>
                <input id="student-concession-search" value={studentFilter} onChange={(event) => setStudentFilter(event.target.value)} placeholder="Search name or admission no." className="w-full rounded-md border bg-background px-3 py-2 text-sm" />
                <select aria-label="Select student for concession history" value={studentId} onChange={(event) => setStudentId(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2 text-sm">
                  <option value="">Select student</option>
                  {matchingStudents.map((student) => <option key={student.id} value={student.id}>{student.fullName} · {student.admissionNumber}</option>)}
                </select>
              </div>
              {!studentId && <p className="rounded-md border border-dashed p-5 text-sm text-muted-foreground">Select a student to view concessions.</p>}
              {studentId && isStudentPending && <div className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading concessions…</div>}
              {studentId && isStudentError && <p className="rounded-md border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load this student’s concessions.</p>}
              {studentId && !isStudentPending && !isStudentError && studentConcessions.length === 0 && <p className="rounded-md border border-dashed p-5 text-sm text-muted-foreground">No concessions found for this student.</p>}
              <div className="space-y-2">
                {studentConcessions.map((concession) => (
                  <div key={concession.id} className="flex flex-col gap-3 rounded-md border p-4 sm:flex-row sm:items-center sm:justify-between">
                    <div>
                      <div className="flex flex-wrap items-center gap-2">
                        <p className="font-medium">{concession.feeTypeName}</p>
                        <span className={`rounded-full px-2 py-0.5 text-xs font-medium ${concession.isApproved ? "bg-primary/10 text-primary" : "bg-amber-500/10 text-amber-700"}`}>{concession.isApproved ? "Approved" : "Pending approval"}</span>
                        <span className={`rounded-full px-2 py-0.5 text-xs font-medium ${concession.isActive ? "bg-secondary text-secondary-foreground" : "bg-muted text-muted-foreground"}`}>{concession.isActive ? "Active" : "Inactive"}</span>
                      </div>
                      <p className="mt-1 text-sm text-muted-foreground">{concession.academicYearName} · {typeLabel(concession.type)}{concession.value !== null && concession.value !== undefined ? ` · ${concession.type === ConcessionType.PercentageDiscount ? `${concession.value}%` : `BDT ${concession.value.toFixed(2)}`}` : ""}</p>
                    </div>
                    {canDelete && <Button type="button" variant="outline" className="h-9 w-9 px-0 py-0 self-start sm:self-auto" aria-label={`Delete concession for ${concession.feeTypeName}`} disabled={deleteConcession.isPending} onClick={() => void handleDelete(concession.id)}><Trash2 className="h-4 w-4" /></Button>}
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        )}

        {canApprove && (
          <Card>
            <CardHeader>
              <CardTitle>Pending approvals</CardTitle>
              <CardDescription>Requests requiring an authorized approval.</CardDescription>
            </CardHeader>
            <CardContent className="space-y-3">
              {isPendingQueue && <div className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading approval queue…</div>}
              {isPendingError && <p className="rounded-md border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load pending approvals.</p>}
              {!isPendingQueue && !isPendingError && pendingConcessions.length === 0 && <p className="rounded-md border border-dashed p-5 text-sm text-muted-foreground">No concessions are awaiting approval.</p>}
              {pendingConcessions.map((concession) => (
                <div key={concession.id} className="flex flex-col gap-3 rounded-md border p-4 sm:flex-row sm:items-center sm:justify-between">
                  <div>
                    <p className="font-medium">{concession.studentName}</p>
                    <p className="text-sm text-muted-foreground">{concession.feeTypeName} · {concession.academicYearName}</p>
                    <p className="text-sm text-muted-foreground">{typeLabel(concession.type)}{concession.value !== null && concession.value !== undefined ? ` · ${concession.type === ConcessionType.PercentageDiscount ? `${concession.value}%` : `BDT ${concession.value.toFixed(2)}`}` : ""}</p>
                  </div>
                  <div className="flex gap-2">
                    <Button type="button" disabled={approveConcession.isPending} onClick={() => void handleApprove(concession.id)}><Check className="mr-2 h-4 w-4" />Approve</Button>
                    {canDelete && <Button type="button" variant="outline" className="h-10 w-10 px-0 py-0" aria-label={`Delete request for ${concession.studentName}`} disabled={deleteConcession.isPending} onClick={() => void handleDelete(concession.id)}><Trash2 className="h-4 w-4" /></Button>}
                  </div>
                </div>
              ))}
            </CardContent>
          </Card>
        )}
      </div>
      {!canView && canApprove && <p className="text-sm text-muted-foreground">Student history is hidden because you do not have concession view permission.</p>}
    </div>
  );
}