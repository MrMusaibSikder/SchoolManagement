import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, Pencil, Plus, Search, Trash2, X } from "lucide-react";
import { toast } from "sonner";
import { useQueryClient } from "@tanstack/react-query";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useAcademicYears, useSchoolClasses, useSections } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { useFeeTypes } from "@/features/fee-type/hooks/useFeeTypeData";
import { Permission } from "@/lib/permissions";
import {
  useCreateFeeStructure,
  useDeleteFeeStructure,
  useFeeStructures,
  useUpdateFeeStructure,
  loadFeeStructure,
} from "../hooks/useFeeStructureData";
import type {
  FeeStructureDto,
  FeeStructureFilters,
  FeeStructureItemDto,
  FeeStructureListDto,
} from "../types/fee-structure.types";

interface ItemDraft {
  id?: number;
  feeTypeId: string;
  amount: string;
  isOptional: boolean;
  sortOrder: string;
  isDeleted: boolean;
}

interface StructureDraft {
  name: string;
  description: string;
  academicYearId: string;
  schoolClassId: string;
  sectionId: string;
  isTemplate: boolean;
  isActive: boolean;
  effectiveFrom: string;
  effectiveTo: string;
  items: ItemDraft[];
}

function emptyDraft(): StructureDraft {
  return {
    name: "",
    description: "",
    academicYearId: "",
    schoolClassId: "",
    sectionId: "",
    isTemplate: false,
    isActive: true,
    effectiveFrom: new Date().toISOString().slice(0, 10),
    effectiveTo: "",
    items: [],
  };
}

function itemDraft(item: FeeStructureItemDto): ItemDraft {
  return {
    id: item.id,
    feeTypeId: String(item.feeTypeId),
    amount: String(item.amount),
    isOptional: item.isOptional,
    sortOrder: String(item.sortOrder),
    isDeleted: false,
  };
}

function draftFromStructure(structure: FeeStructureDto): StructureDraft {
  return {
    name: structure.name,
    description: structure.description ?? "",
    academicYearId: String(structure.academicYearId),
    schoolClassId: String(structure.schoolClassId),
    sectionId: structure.sectionId ? String(structure.sectionId) : "",
    isTemplate: structure.isTemplate,
    isActive: structure.isActive,
    effectiveFrom: structure.effectiveFrom.slice(0, 10),
    effectiveTo: structure.effectiveTo?.slice(0, 10) ?? "",
    items: structure.items.map(itemDraft),
  };
}

function formatMoney(amount: number) {
  return new Intl.NumberFormat("en-BD", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(amount);
}

export function FeeStructuresPage() {
  const { hasPermission } = usePermissions();
  const { data: academicYears = [] } = useAcademicYears();
  const { data: schoolClasses = [] } = useSchoolClasses();
  const { data: sections = [] } = useSections();
  const { data: feeTypes = [] } = useFeeTypes();

  const [filters, setFilters] = useState<FeeStructureFilters>({});
  const { data: structures = [], isPending, isError } = useFeeStructures(filters);
  const [search, setSearch] = useState("");
  const [editingId, setEditingId] = useState<number | null>(null);
  const [isLoadingEdit, setIsLoadingEdit] = useState(false);
  const [draft, setDraft] = useState<StructureDraft>(emptyDraft);
  const queryClient = useQueryClient();

  const createStructure = useCreateFeeStructure();
  const updateStructure = useUpdateFeeStructure();
  const deleteStructure = useDeleteFeeStructure();
  const canCreate = hasPermission(Permission.FeeStructureCreate);
  const canEdit = hasPermission(Permission.FeeStructureEdit);
  const canDelete = hasPermission(Permission.FeeStructureDelete);

  const visibleSections = useMemo(
    () => sections.filter((section) => String(section.classId) === draft.schoolClassId),
    [sections, draft.schoolClassId]
  );
  const filteredStructures = useMemo(
    () =>
      structures.filter((item) =>
        `${item.name} ${item.schoolClassName} ${item.academicYearName} ${item.sectionName ?? ""}`
          .toLowerCase()
          .includes(search.trim().toLowerCase())
      ),
    [structures, search]
  );

  function resetForm() {
    setEditingId(null);
    setDraft(emptyDraft());
  }

  function addItem() {
    const usedFeeTypeIds = draft.items.filter((item) => !item.isDeleted).map((item) => item.feeTypeId);
    const firstAvailable = feeTypes.find((feeType) => !usedFeeTypeIds.includes(String(feeType.id)));
    setDraft((value) => ({
      ...value,
      items: [
        ...value.items,
        {
          feeTypeId: firstAvailable ? String(firstAvailable.id) : "",
          amount: "",
          isOptional: false,
          sortOrder: String(value.items.length + 1),
          isDeleted: false,
        },
      ],
    }));
  }

  function updateItem(index: number, changes: Partial<ItemDraft>) {
    setDraft((value) => ({
      ...value,
      items: value.items.map((item, itemIndex) => itemIndex === index ? { ...item, ...changes } : item),
    }));
  }

  function removeItem(index: number) {
    setDraft((value) => ({
      ...value,
      items: value.items.flatMap((item, itemIndex) => {
        if (itemIndex !== index) return [item];
        return item.id ? [{ ...item, isDeleted: true }] : [];
      }),
    }));
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const activeItems = draft.items.filter((item) => !item.isDeleted);
    if (!draft.name.trim() || !draft.academicYearId || !draft.schoolClassId || !draft.effectiveFrom) {
      toast.error("Complete the required fee structure details.");
      return;
    }
    if (!activeItems.length) {
      toast.error("Add at least one fee type to the structure.");
      return;
    }
    if (draft.effectiveTo && draft.effectiveTo < draft.effectiveFrom) {
      toast.error("Effective end date must be on or after the start date.");
      return;
    }

    const usedIds = activeItems.map((item) => item.feeTypeId);
    if (activeItems.some((item) => !item.feeTypeId || !Number.isFinite(Number(item.amount)) || Number(item.amount) <= 0)) {
      toast.error("Choose a fee type and enter an amount greater than zero for each item.");
      return;
    }
    if (new Set(usedIds).size !== usedIds.length) {
      toast.error("Each fee type can appear only once in a structure.");
      return;
    }

    try {
      const shared = {
        name: draft.name.trim(),
        description: draft.description.trim() || null,
        sectionId: draft.sectionId ? Number(draft.sectionId) : null,
        isTemplate: draft.isTemplate,
        effectiveFrom: draft.effectiveFrom,
        effectiveTo: draft.effectiveTo || null,
      };

      if (editingId) {
        await updateStructure.mutateAsync({
          id: editingId,
          payload: {
            id: editingId,
            ...shared,
            isActive: draft.isActive,
            items: draft.items.map((item) => ({
              id: item.id ?? null,
              feeTypeId: Number(item.feeTypeId),
              amount: Number(item.amount) || 0,
              isOptional: item.isOptional,
              sortOrder: Number(item.sortOrder) || 1,
              isDeleted: item.isDeleted,
            })),
          },
        });
        toast.success("Fee structure updated.");
      } else {
        await createStructure.mutateAsync({
          ...shared,
          academicYearId: Number(draft.academicYearId),
          schoolClassId: Number(draft.schoolClassId),
          items: activeItems.map((item) => ({
            feeTypeId: Number(item.feeTypeId),
            amount: Number(item.amount),
            isOptional: item.isOptional,
            sortOrder: Number(item.sortOrder) || 1,
          })),
        });
        toast.success("Fee structure created.");
      }
      resetForm();
    } catch (error) {
      toast.error((error as Error)?.message ?? "Unable to save the fee structure.");
    }
  }

  async function startEdit(item: FeeStructureListDto) {
    setDraft(emptyDraft());
    setEditingId(item.id);
    setIsLoadingEdit(true);
    try {
      const structure = await queryClient.fetchQuery({
        queryKey: ["fee-structure", "detail", item.id],
        queryFn: () => loadFeeStructure(item.id),
        staleTime: 30_000,
      });
      setDraft(draftFromStructure(structure));
    } catch (error) {
      setEditingId(null);
      toast.error((error as Error)?.message ?? "Unable to load fee structure details.");
    } finally {
      setIsLoadingEdit(false);
    }
  }

  async function handleDelete(id: number) {
    if (!window.confirm("Delete this fee structure? It cannot be deleted if invoices already reference it.")) return;
    try {
      await deleteStructure.mutateAsync(id);
      toast.success("Fee structure deleted.");
    } catch (error) {
      toast.error((error as Error)?.message ?? "Unable to delete the fee structure.");
    }
  }

  if (!hasPermission(Permission.FeeStructureView)) {
    return <Card className="mx-auto max-w-4xl"><CardContent className="p-6 text-sm text-destructive">You do not have permission to view fee structures.</CardContent></Card>;
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Fee structures</h1>
          <p className="text-sm text-muted-foreground">Set class and section fee schedules by academic year.</p>
        </div>
        <Link to="/fees/types" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">Fee types</Link>
      </div>

      <div className="grid gap-6 xl:grid-cols-[1fr_1.15fr]">
        {(canCreate || (canEdit && editingId !== null)) && (
          <Card>
            <CardHeader>
              <CardTitle>{editingId ? "Edit fee structure" : "Create fee structure"}</CardTitle>
              <CardDescription>Choose a class-wide structure or scope it to a specific section.</CardDescription>
            </CardHeader>
            <CardContent>
              {isLoadingEdit && editingId ? (
                <div className="flex items-center justify-center py-10 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading structure details…</div>
              ) : (
                <form onSubmit={(event) => void handleSubmit(event)} className="space-y-4">
                  <div>
                    <label htmlFor="structure-name" className="mb-1 block text-sm font-medium">Name</label>
                    <input id="structure-name" required maxLength={150} value={draft.name} onChange={(event) => setDraft((value) => ({ ...value, name: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
                  </div>
                  <div>
                    <label htmlFor="structure-description" className="mb-1 block text-sm font-medium">Description</label>
                    <textarea id="structure-description" value={draft.description} onChange={(event) => setDraft((value) => ({ ...value, description: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" rows={2} />
                  </div>
                  <div className="grid gap-4 sm:grid-cols-2">
                    <div>
                      <label htmlFor="structure-year" className="mb-1 block text-sm font-medium">Academic year</label>
                      <select id="structure-year" required disabled={editingId !== null} value={draft.academicYearId} onChange={(event) => setDraft((value) => ({ ...value, academicYearId: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2">
                        <option value="">Select year</option>
                        {academicYears.map((year) => <option key={year.id} value={year.id}>{year.name}{year.isCurrent ? " (Current)" : ""}</option>)}
                      </select>
                    </div>
                    <div>
                      <label htmlFor="structure-class" className="mb-1 block text-sm font-medium">Class</label>
                      <select id="structure-class" required disabled={editingId !== null} value={draft.schoolClassId} onChange={(event) => setDraft((value) => ({ ...value, schoolClassId: event.target.value, sectionId: "" }))} className="w-full rounded-md border bg-background px-3 py-2">
                        <option value="">Select class</option>
                        {schoolClasses.map((schoolClass) => <option key={schoolClass.id} value={schoolClass.id}>{schoolClass.name}</option>)}
                      </select>
                    </div>
                  </div>
                  <div className="grid gap-4 sm:grid-cols-2">
                    <div>
                      <label htmlFor="structure-section" className="mb-1 block text-sm font-medium">Section <span className="text-muted-foreground">(optional)</span></label>
                      <select id="structure-section" value={draft.sectionId} onChange={(event) => setDraft((value) => ({ ...value, sectionId: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2">
                        <option value="">All sections</option>
                        {visibleSections.map((section) => <option key={section.id} value={section.id}>{section.name}</option>)}
                      </select>
                    </div>
                    <div className="grid grid-cols-2 gap-3">
                      <div>
                        <label htmlFor="effective-from" className="mb-1 block text-sm font-medium">Effective from</label>
                        <input id="effective-from" type="date" required value={draft.effectiveFrom} onChange={(event) => setDraft((value) => ({ ...value, effectiveFrom: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
                      </div>
                      <div>
                        <label htmlFor="effective-to" className="mb-1 block text-sm font-medium">Effective to</label>
                        <input id="effective-to" type="date" min={draft.effectiveFrom} value={draft.effectiveTo} onChange={(event) => setDraft((value) => ({ ...value, effectiveTo: event.target.value }))} className="w-full rounded-md border bg-background px-3 py-2" />
                      </div>
                    </div>
                  </div>
                  <div className="flex flex-wrap gap-x-6 gap-y-2">
                    <label className="flex items-center gap-2 text-sm"><input type="checkbox" checked={draft.isTemplate} onChange={(event) => setDraft((value) => ({ ...value, isTemplate: event.target.checked }))} />Template</label>
                    {editingId !== null && <label className="flex items-center gap-2 text-sm"><input type="checkbox" checked={draft.isActive} onChange={(event) => setDraft((value) => ({ ...value, isActive: event.target.checked }))} />Active</label>}
                  </div>

                  <div className="space-y-3 border-t pt-4">
                    <div className="flex items-center justify-between gap-3">
                      <div><h3 className="text-sm font-semibold">Fee items</h3><p className="text-xs text-muted-foreground">Each fee type can be added once.</p></div>
                      <Button type="button" variant="outline" className="px-3 py-1.5" onClick={addItem} disabled={feeTypes.length === 0}><Plus className="mr-2 h-4 w-4" />Add item</Button>
                    </div>
                    {draft.items.map((item, index) => item.isDeleted ? (
                      <div key={item.id ?? index} className="flex items-center justify-between rounded-md border border-dashed p-3 text-sm text-muted-foreground">
                        <span>{feeTypes.find((type) => String(type.id) === item.feeTypeId)?.name ?? "Fee item"} will be removed</span>
                        <Button type="button" variant="outline" className="px-3 py-1.5" onClick={() => updateItem(index, { isDeleted: false })}>Restore</Button>
                      </div>
                    ) : (
                      <div key={item.id ?? `new-${index}`} className="grid gap-3 rounded-md border p-3 sm:grid-cols-[1.4fr_1fr_auto_auto] sm:items-end">
                        <div>
                          <label className="mb-1 block text-xs font-medium">Fee type</label>
                          <select required value={item.feeTypeId} onChange={(event) => updateItem(index, { feeTypeId: event.target.value })} className="w-full rounded-md border bg-background px-3 py-2 text-sm">
                            <option value="">Select fee type</option>
                            {feeTypes.filter((type) => type.isActive || type.id === Number(item.feeTypeId)).map((type) => <option key={type.id} value={type.id}>{type.name} ({type.code})</option>)}
                          </select>
                        </div>
                        <div>
                          <label className="mb-1 block text-xs font-medium">Amount</label>
                          <input required type="number" min="0.01" step="0.01" value={item.amount} onChange={(event) => updateItem(index, { amount: event.target.value })} className="w-full rounded-md border bg-background px-3 py-2 text-sm" />
                        </div>
                        <label className="flex items-center gap-2 pb-2 text-xs"><input type="checkbox" checked={item.isOptional} onChange={(event) => updateItem(index, { isOptional: event.target.checked })} />Optional</label>
                        <Button type="button" variant="outline" className="h-9 w-9 px-0 py-0" aria-label="Remove fee item" onClick={() => removeItem(index)}><X className="h-4 w-4" /></Button>
                      </div>
                    ))}
                    {draft.items.length === 0 && <p className="rounded-md border border-dashed p-4 text-sm text-muted-foreground">No fee items added yet.</p>}
                  </div>

                  <div className="flex flex-wrap gap-2">
                    <Button type="submit" disabled={createStructure.isPending || updateStructure.isPending || isLoadingEdit || feeTypes.length === 0}>
                      {(createStructure.isPending || updateStructure.isPending) && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                      {editingId ? "Save changes" : "Create structure"}
                    </Button>
                    {editingId !== null && <Button type="button" variant="outline" onClick={resetForm}>Cancel</Button>}
                    {feeTypes.length === 0 && <span className="self-center text-xs text-muted-foreground">Create a fee type before adding a structure.</span>}
                  </div>
                </form>
              )}
            </CardContent>
          </Card>
        )}

        <Card>
          <CardHeader>
            <CardTitle>Structures</CardTitle>
            <CardDescription>Filter by academic year, class, and active status.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid gap-3 sm:grid-cols-2">
              <select aria-label="Filter by academic year" value={filters.academicYearId ?? ""} onChange={(event) => setFilters((value) => ({ ...value, academicYearId: event.target.value ? Number(event.target.value) : undefined }))} className="w-full rounded-md border bg-background px-3 py-2 text-sm">
                <option value="">All academic years</option>
                {academicYears.map((year) => <option key={year.id} value={year.id}>{year.name}</option>)}
              </select>
              <select aria-label="Filter by class" value={filters.schoolClassId ?? ""} onChange={(event) => setFilters((value) => ({ ...value, schoolClassId: event.target.value ? Number(event.target.value) : undefined }))} className="w-full rounded-md border bg-background px-3 py-2 text-sm">
                <option value="">All classes</option>
                {schoolClasses.map((schoolClass) => <option key={schoolClass.id} value={schoolClass.id}>{schoolClass.name}</option>)}
              </select>
              <select aria-label="Filter by status" value={filters.isActive === undefined ? "" : String(filters.isActive)} onChange={(event) => setFilters((value) => ({ ...value, isActive: event.target.value === "" ? undefined : event.target.value === "true" }))} className="w-full rounded-md border bg-background px-3 py-2 text-sm">
                <option value="">All statuses</option><option value="true">Active</option><option value="false">Inactive</option>
              </select>
              <div className="relative">
                <Search aria-hidden="true" className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
                <input value={search} onChange={(event) => setSearch(event.target.value)} placeholder="Search results" className="w-full rounded-md border bg-background py-2 pl-9 pr-3 text-sm" />
              </div>
            </div>

            {isPending && <div className="flex items-center justify-center py-8 text-sm text-muted-foreground"><Loader2 className="mr-2 h-4 w-4 animate-spin" />Loading fee structures…</div>}
            {isError && <div className="rounded-md border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">Unable to load fee structures.</div>}
            {!isPending && !isError && filteredStructures.length === 0 && <div className="rounded-md border border-dashed p-6 text-sm text-muted-foreground">No fee structures match these filters.</div>}

            <div className="space-y-3">
              {filteredStructures.map((item) => (
                <div key={item.id} className="flex flex-col gap-3 rounded-md border p-4 sm:flex-row sm:items-center sm:justify-between">
                  <div className="min-w-0">
                    <div className="flex flex-wrap items-center gap-2">
                      <p className="font-medium">{item.name}</p>
                      <span className={`rounded-full px-2 py-0.5 text-xs font-medium ${item.isActive ? "bg-primary/10 text-primary" : "bg-muted text-muted-foreground"}`}>{item.isActive ? "Active" : "Inactive"}</span>
                      {item.isTemplate && <span className="rounded-full bg-secondary px-2 py-0.5 text-xs font-medium">Template</span>}
                    </div>
                    <p className="mt-1 text-sm text-muted-foreground">{item.academicYearName} · {item.schoolClassName}{item.sectionName ? ` · ${item.sectionName}` : " · All sections"}</p>
                    <p className="text-sm text-muted-foreground">{item.itemCount} items · Total {formatMoney(item.totalAmount)} · From {new Date(item.effectiveFrom).toLocaleDateString()}{item.effectiveTo ? ` to ${new Date(item.effectiveTo).toLocaleDateString()}` : ""}</p>
                  </div>
                  <div className="flex shrink-0 gap-2">
                    {canEdit && <Button type="button" variant="outline" className="h-9 w-9 px-0 py-0" aria-label={`Edit ${item.name}`} onClick={() => startEdit(item)}><Pencil className="h-4 w-4" /></Button>}
                    {canDelete && <Button type="button" variant="outline" className="h-9 w-9 px-0 py-0" aria-label={`Delete ${item.name}`} disabled={deleteStructure.isPending} onClick={() => void handleDelete(item.id)}><Trash2 className="h-4 w-4" /></Button>}
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}