import { useMemo, useState } from "react";
import type { AxiosError } from "axios";
import { Link } from "react-router-dom";
import { Loader2, Pencil, Search, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useAcademicYears } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { useFeeTypes } from "@/features/fee-type/hooks/useFeeTypeData";
import { Permission } from "@/lib/permissions";
import { useCreateLateFineRule, useDeleteLateFineRule, useLateFineRules, useUpdateLateFineRule } from "../hooks/useLateFineRuleData";
import { FineType, type FineTypeValue, type LateFineRuleDto } from "../types/late-fine-rule.types";

const fineTypeOptions = [
  { value: FineType.Fixed, label: "Fixed amount" },
  { value: FineType.Percentage, label: "Percentage" },
  { value: FineType.DailyAccrual, label: "Daily accrual" },
] as const;

interface RuleDraft {
  type: FineTypeValue;
  amount: string;
  gracePeriodDays: string;
  maxFineAmount: string;
  isActive: boolean;
}

function initialDraft(): RuleDraft {
  return { type: FineType.Fixed, amount: "", gracePeriodDays: "0", maxFineAmount: "", isActive: true };
}

function errorMessage(error: unknown, fallback: string) {
  const response = (error as AxiosError<{ message?: string } | string>).response;
  if (typeof response?.data === "string" && response.data) return response.data;
  if (response?.data && typeof response.data === "object" && response.data.message) return response.data.message;
  return fallback;
}

function fineTypeLabel(type: number) {
  return fineTypeOptions.find((option) => option.value === type)?.label ?? "Unknown";
}

export function LateFineRulesPage() {
  const { hasPermission } = usePermissions();
  const canView = hasPermission(Permission.LateFineRuleView);
  const canManage = hasPermission(Permission.LateFineRuleManage);
  const { data: academicYears = [] } = useAcademicYears();
  const { data: feeTypes = [] } = useFeeTypes();
  const [academicYearId, setAcademicYearId] = useState("");
  const { data: rules = [], isPending, isError } = useLateFineRules(academicYearId ? Number(academicYearId) : null, canView);
  const [feeTypeScopeFilter, setFeeTypeScopeFilter] = useState("");
  const [draftFeeTypeScope, setDraftFeeTypeScope] = useState("");
  const [search, setSearch] = useState("");
  const [editingRule, setEditingRule] = useState<LateFineRuleDto | null>(null);
  const [draft, setDraft] = useState<RuleDraft>(initialDraft);
  const createRule = useCreateLateFineRule();
  const updateRule = useUpdateLateFineRule();
  const deleteRule = useDeleteLateFineRule();

  const filteredRules = useMemo(() => rules.filter((rule) => {
    const scopeMatch = feeTypeScopeFilter === "" || (feeTypeScopeFilter === "global" ? rule.feeTypeId == null : String(rule.feeTypeId) === feeTypeScopeFilter);
    return scopeMatch && `${rule.feeTypeName ?? "Global all fee types"} ${fineTypeLabel(rule.type)}`.toLowerCase().includes(search.toLowerCase());
  }), [rules, feeTypeScopeFilter, search]);

  function resetForm() {
    setEditingRule(null);
    setDraft(initialDraft());
    setDraftFeeTypeScope("");
  }

  function startEdit(rule: LateFineRuleDto) {
    setEditingRule(rule);
    setDraft({
      type: rule.type,
      amount: String(rule.amount),
      gracePeriodDays: String(rule.gracePeriodDays),
      maxFineAmount: rule.maxFineAmount == null ? "" : String(rule.maxFineAmount),
      isActive: rule.isActive,
    });
  }

  async function submitRule(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const amount = Number(draft.amount);
    const gracePeriodDays = Number(draft.gracePeriodDays);
    const maxFineAmount = draft.maxFineAmount === "" ? null : Number(draft.maxFineAmount);
    if (!editingRule && !academicYearId) return toast.error("Select an academic year first.");
    if (!Number.isFinite(amount) || amount <= 0) return toast.error("Fine amount must be greater than zero.");
    if (draft.type === FineType.Percentage && (amount < 0.01 || amount > 100)) return toast.error("Percentage must be between 0.01 and 100.");
    if (!Number.isInteger(gracePeriodDays) || gracePeriodDays < 0) return toast.error("Grace period must be a whole number of zero or more days.");
    if (maxFineAmount !== null && (!Number.isFinite(maxFineAmount) || maxFineAmount <= 0)) return toast.error("Maximum fine must be greater than zero or left blank.");

    try {
      if (editingRule) {
        await updateRule.mutateAsync({
          id: editingRule.id,
          payload: { id: editingRule.id, type: draft.type, amount, gracePeriodDays, maxFineAmount, isActive: draft.isActive },
        });
        toast.success("Late fine rule updated.");
      } else {
        await createRule.mutateAsync({
          academicYearId: Number(academicYearId),
          feeTypeId: draftFeeTypeScope === "global" ? null : Number(draftFeeTypeScope),
          type: draft.type,
          amount,
          gracePeriodDays,
          maxFineAmount,
          isActive: draft.isActive,
        });
        toast.success("Late fine rule created.");
      }
      resetForm();
    } catch (error) {
      toast.error(errorMessage(error, "Unable to save late fine rule."));
    }
  }

  async function removeRule(rule: LateFineRuleDto) {
    if (!window.confirm(`Remove the ${rule.feeTypeName ?? "global"} late fine rule?`)) return;
    try {
      await deleteRule.mutateAsync(rule.id);
      toast.success("Late fine rule removed.");
    } catch (error) {
      toast.error(errorMessage(error, "Unable to remove late fine rule."));
    }
  }

  if (!canView && !canManage) {
    return <Card className="mx-auto max-w-4xl"><CardContent className="p-6 text-sm text-destructive">You do not have permission to view or manage late fine rules.</CardContent></Card>;
  }

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
        <div><h1 className="font-display text-2xl font-semibold">Late fine rules</h1><p className="text-sm text-muted-foreground">Configure overdue fine behavior by academic year and fee type.</p></div>
        <Link to="/fees/invoices" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">Invoices</Link>
      </div>

      <Card>
        <CardHeader><CardTitle>Academic year</CardTitle><CardDescription>Each year can have a global rule and specific overrides per fee type.</CardDescription></CardHeader>
        <CardContent>
          <label htmlFor="late-fine-year" className="mb-1 block text-sm font-medium">Select academic year</label>
          <select id="late-fine-year" value={academicYearId} onChange={(event) => { setAcademicYearId(event.target.value); setFeeTypeScopeFilter(""); resetForm(); }} className="w-full max-w-md rounded-md border bg-background px-3 py-2">
            <option value="">Select academic year</option>
            {academicYears.map((year) => <option key={year.id} value={year.id}>{year.name}{year.isCurrent ? " · Current" : ""}</option>)}
          </select>
        </CardContent>
      </Card>

      {canManage && academicYearId && <Card>
        <CardHeader><CardTitle>{editingRule ? "Edit late fine rule" : "Create late fine rule"}</CardTitle><CardDescription>{editingRule ? "Academic year and fee type scope cannot be changed." : "A specific fee type rule takes priority over the global rule."}</CardDescription></CardHeader>
        <CardContent>
          <form onSubmit={(event) => void submitRule(event)} className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            <div><label htmlFor="fine-scope" className="mb-1 block text-sm font-medium">Applies to</label><select id="fine-scope" disabled={editingRule !== null} value={editingRule ? (editingRule.feeTypeId == null ? "global" : String(editingRule.feeTypeId)) : draftFeeTypeScope} onChange={(event) => setDraftFeeTypeScope(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2"><option value="">Choose scope</option><option value="global">All fee types (global)</option>{feeTypes.filter((type) => type.isActive).map((type) => <option key={type.id} value={type.id}>{type.name} · {type.code}</option>)}</select></div>
            <div><label htmlFor="fine-type" className="mb-1 block text-sm font-medium">Fine type</label><select id="fine-type" value={draft.type} onChange={(event) => setDraft((value) => ({ ...value, type: Number(event.target.value) as FineTypeValue }))} className="w-full rounded-md border bg-background px-3 py-2">{fineTypeOptions.map((option) => <option key={option.value} value={option.value}>{option.label}</option>)}</select></div>
            <div><label htmlFor="fine-amount" className="mb-1 block text-sm font-medium">{draft.type === FineType.Percentage ? "Percentage (%)" : "Amount (BDT)"}</label><input id="fine-amount" required type="number" min="0.01" max={draft.type === FineType.Percentage ? 100 : undefined} step="0.01" value={draft.amount} onChange={(event) => setDraft((value) => ({ ...value, amount: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" /></div>
            <div><label htmlFor="fine-grace" className="mb-1 block text-sm font-medium">Grace period (days)</label><input id="fine-grace" required type="number" min="0" step="1" value={draft.gracePeriodDays} onChange={(event) => setDraft((value) => ({ ...value, gracePeriodDays: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" /></div>
            <div><label htmlFor="fine-max" className="mb-1 block text-sm font-medium">Maximum fine <span className="text-muted-foreground">(optional)</span></label><input id="fine-max" type="number" min="0.01" step="0.01" value={draft.maxFineAmount} onChange={(event) => setDraft((value) => ({ ...value, maxFineAmount: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" /></div>
            {editingRule && <label className="flex items-center gap-2 self-end pb-2 text-sm"><input type="checkbox" checked={draft.isActive} onChange={(event) => setDraft((value) => ({ ...value, isActive: event.target.checked }))} />Active</label>}
            <div className="flex gap-2 sm:col-span-2 lg:col-span-3"><Button type="submit" disabled={createRule.isPending || updateRule.isPending || (!editingRule && !draftFeeTypeScope)}>{(createRule.isPending || updateRule.isPending) && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}{editingRule ? "Save changes" : "Create rule"}</Button>{editingRule && <Button type="button" variant="outline" onClick={resetForm}>Cancel</Button>}</div>
          </form>
        </CardContent>
      </Card>}

      {canView && <Card>
        <CardHeader><CardTitle>Configured rules</CardTitle><CardDescription>{academicYearId ? `${rules.length} rule${rules.length === 1 ? "" : "s"} for ${academicYears.find((year) => String(year.id) === academicYearId)?.name ?? "selected year"}` : "Select an academic year to view its rules."}</CardDescription></CardHeader>
        <CardContent className="space-y-4">
          {academicYearId && <div className="grid gap-3 sm:grid-cols-[1fr_1fr]"><select aria-label="Filter rules by scope" value={feeTypeScopeFilter} onChange={(event) => setFeeTypeScopeFilter(event.target.value)} className="w-full rounded-md border bg-background px-3 py-2 text-sm"><option value="">All scopes</option><option value="global">Global rules</option>{feeTypes.map((type) => <option key={type.id} value={type.id}>{type.name}</option>)}</select><div className="relative"><Search aria-hidden="true" className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" /><input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search scope or rule type" className="w-full rounded-md border bg-background py-2 pl-9 pr-3 text-sm" /></div></div>}
          {academicYearId && isPending && <p className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading rules…</p>}
          {academicYearId && isError && <p className="rounded-md border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load late fine rules.</p>}
          {academicYearId && !isPending && !isError && filteredRules.length === 0 && <p className="rounded-md border border-dashed p-6 text-sm text-muted-foreground">No late fine rules match this year and scope.</p>}
          <div className="space-y-2">{filteredRules.map((rule) => <div key={rule.id} className="flex flex-col gap-3 rounded-md border p-4 sm:flex-row sm:items-center sm:justify-between"><div><div className="flex flex-wrap items-center gap-2"><p className="font-medium">{rule.feeTypeName ?? "All fee types · Global"}</p><span className={`rounded-full px-2 py-0.5 text-xs font-medium ${rule.isActive ? "bg-primary/10 text-primary" : "bg-muted text-muted-foreground"}`}>{rule.isActive ? "Active" : "Inactive"}</span></div><p className="mt-1 text-sm text-muted-foreground">{fineTypeLabel(rule.type)} · {rule.type === FineType.Percentage ? `${rule.amount}%` : `BDT ${rule.amount.toFixed(2)}`} · {rule.gracePeriodDays} grace days{rule.maxFineAmount != null ? ` · Max BDT ${rule.maxFineAmount.toFixed(2)}` : ""}</p></div>{canManage && <div className="flex gap-2"><Button type="button" variant="outline" className="h-9 w-9 px-0 py-0" aria-label={`Edit rule for ${rule.feeTypeName ?? "all fee types"}`} onClick={() => startEdit(rule)}><Pencil className="h-4 w-4" /></Button><Button type="button" variant="outline" className="h-9 w-9 px-0 py-0" aria-label={`Remove rule for ${rule.feeTypeName ?? "all fee types"}`} disabled={deleteRule.isPending} onClick={() => void removeRule(rule)}><Trash2 className="h-4 w-4" /></Button></div>}</div>)}</div>
        </CardContent>
      </Card>}
    </div>
  );
}