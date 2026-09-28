import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, Pencil, Plus, Search, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { useFeeCategories } from "@/features/fee-category/hooks/useFeeCategoryData";
import { useCreateFeeType, useDeleteFeeType, useFeeTypes, useUpdateFeeType } from "../hooks/useFeeTypeData";
import type { FeeTypeListDto } from "../types/fee-type.types";

const frequencyOptions = [
  { label: "One Time", value: "1" },
  { label: "Monthly", value: "2" },
  { label: "Termly", value: "3" },
  { label: "Yearly", value: "4" },
] as const;

export function FeeTypesPage() {
  const { hasPermission } = usePermissions();
  const { data: categories = [] } = useFeeCategories();
  const { data = [], isPending, isError } = useFeeTypes();
  const createType = useCreateFeeType();
  const updateType = useUpdateFeeType();
  const deleteType = useDeleteFeeType();

  const [search, setSearch] = useState("");
  const [draft, setDraft] = useState({
    name: "",
    code: "",
    description: "",
    feeCategoryId: "",
    frequency: "2",
    isMandatory: true,
    isRefundable: false,
    defaultDueDayOfMonth: "",
    defaultGracePeriodDays: "",
    isActive: true,
  });
  const [editingId, setEditingId] = useState<number | null>(null);

  const canCreate = hasPermission(Permission.FeeTypeCreate);
  const canEdit = hasPermission(Permission.FeeTypeEdit);
  const canDelete = hasPermission(Permission.FeeTypeDelete);

  useEffect(() => {
    if (!draft.feeCategoryId && categories.length > 0) {
      setDraft((current) => ({ ...current, feeCategoryId: String(categories[0].id) }));
    }
  }, [categories, draft.feeCategoryId]);

  const filtered = useMemo(
    () =>
      data.filter((item) =>
        `${item.name} ${item.code} ${item.feeCategoryName}`.toLowerCase().includes(search.toLowerCase())
      ),
    [data, search]
  );

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();

    const name = draft.name.trim();
    const code = draft.code.trim();
    const feeCategoryId = Number(draft.feeCategoryId);

    if (!name || !code || !feeCategoryId) {
      toast.error("Please fill in the required fee type details.");
      return;
    }

    try {
      const frequency = Number(draft.frequency) as (typeof import("../types/fee-type.types").FeeFrequency)[keyof typeof import("../types/fee-type.types").FeeFrequency];

      const payload = {
        name,
        code,
        description: draft.description.trim() || null,
        feeCategoryId,
        frequency,
        isMandatory: draft.isMandatory,
        isRefundable: draft.isRefundable,
        defaultDueDayOfMonth: draft.frequency === "1" ? null : Number(draft.defaultDueDayOfMonth || 1),
        defaultGracePeriodDays: draft.defaultGracePeriodDays ? Number(draft.defaultGracePeriodDays) : null,
      };

      if (editingId) {
        await updateType.mutateAsync({
          id: editingId,
          payload: { id: editingId, ...payload, isActive: draft.isActive },
        });
        toast.success("Fee type updated.");
      } else {
        await createType.mutateAsync(payload);
        toast.success("Fee type created.");
      }

      setDraft({
        name: "",
        code: "",
        description: "",
        feeCategoryId: categories[0] ? String(categories[0].id) : "",
        frequency: "2",
        isMandatory: true,
        isRefundable: false,
        defaultDueDayOfMonth: "",
        defaultGracePeriodDays: "",
        isActive: true,
      });
      setEditingId(null);
    } catch (error) {
      toast.error((error as Error)?.message ?? "Unable to save fee type.");
    }
  }

  async function handleDelete(id: number) {
    if (!window.confirm("Delete this fee type? It may be in use by fee structures or invoices.")) return;

    try {
      await deleteType.mutateAsync(id);
      toast.success("Fee type deleted.");
    } catch (error) {
      toast.error((error as Error)?.message ?? "Unable to delete fee type.");
    }
  }

  function startEdit(item: FeeTypeListDto) {
    setEditingId(item.id);
    setDraft({
      name: item.name,
      code: item.code,
      description: "",
      feeCategoryId: String(item.feeCategoryId),
      frequency: String(item.frequency),
      isMandatory: item.isMandatory,
      isRefundable: item.isRefundable,
      defaultDueDayOfMonth: item.defaultDueDayOfMonth ? String(item.defaultDueDayOfMonth) : "",
      defaultGracePeriodDays: item.defaultGracePeriodDays ? String(item.defaultGracePeriodDays) : "",
      isActive: item.isActive,
    });
  }

  if (!hasPermission(Permission.FeeTypeView)) {
    return (
      <Card className="mx-auto max-w-4xl">
        <CardContent className="p-6 text-sm text-destructive">
          You do not have permission to view fee types.
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Fee types</h1>
          <p className="text-sm text-muted-foreground">Configure reusable fee definitions grouped by category.</p>
        </div>
        <Link to="/dashboard" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">
          Back to dashboard
        </Link>
      </div>

      <div className="grid gap-6 lg:grid-cols-[0.95fr_1.05fr]">
        {(canCreate || (canEdit && editingId)) && (
          <Card>
            <CardHeader>
              <CardTitle>{editingId ? "Edit fee type" : "Create fee type"}</CardTitle>
              <CardDescription>Frequency and due-day settings drive invoice generation and recurring fees.</CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={(event) => void handleSubmit(event)} className="space-y-4">
                <div className="grid gap-4 md:grid-cols-2">
                  <div>
                    <label className="mb-1 block text-sm font-medium">Name</label>
                    <input
                      required
                      value={draft.name}
                      onChange={(event) => setDraft((value) => ({ ...value, name: event.target.value }))}
                      className="w-full rounded-md border bg-background px-3 py-2"
                    />
                  </div>
                  <div>
                    <label className="mb-1 block text-sm font-medium">Code</label>
                    <input
                      required
                      value={draft.code}
                      onChange={(event) => setDraft((value) => ({ ...value, code: event.target.value }))}
                      className="w-full rounded-md border bg-background px-3 py-2"
                    />
                  </div>
                </div>

                <div>
                  <label className="mb-1 block text-sm font-medium">Category</label>
                  <select
                    required
                    value={draft.feeCategoryId}
                    onChange={(event) => setDraft((value) => ({ ...value, feeCategoryId: event.target.value }))}
                    className="w-full rounded-md border bg-background px-3 py-2"
                  >
                    {categories.map((category) => (
                      <option key={category.id} value={category.id}>
                        {category.name}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="mb-1 block text-sm font-medium">Description</label>
                  <textarea
                    value={draft.description}
                    onChange={(event) => setDraft((value) => ({ ...value, description: event.target.value }))}
                    className="w-full rounded-md border bg-background px-3 py-2"
                    rows={3}
                  />
                </div>

                <div className="grid gap-4 md:grid-cols-2">
                  <div>
                    <label className="mb-1 block text-sm font-medium">Frequency</label>
                    <select
                      value={draft.frequency}
                      onChange={(event) => setDraft((value) => ({ ...value, frequency: event.target.value }))}
                      className="w-full rounded-md border bg-background px-3 py-2"
                    >
                      {frequencyOptions.map((option) => (
                        <option key={option.value} value={option.value}>
                          {option.label}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div>
                    <label className="mb-1 block text-sm font-medium">Default due day</label>
                    <input
                      type="number"
                      min="1"
                      max="31"
                      value={draft.defaultDueDayOfMonth}
                      onChange={(event) => setDraft((value) => ({ ...value, defaultDueDayOfMonth: event.target.value }))}
                      className="w-full rounded-md border bg-background px-3 py-2"
                      disabled={draft.frequency === "1"}
                    />
                  </div>
                </div>

                <div className="grid gap-4 md:grid-cols-2">
                  <div>
                    <label className="mb-1 block text-sm font-medium">Grace period days</label>
                    <input
                      type="number"
                      min="0"
                      value={draft.defaultGracePeriodDays}
                      onChange={(event) => setDraft((value) => ({ ...value, defaultGracePeriodDays: event.target.value }))}
                      className="w-full rounded-md border bg-background px-3 py-2"
                    />
                  </div>
                  <div className="flex items-end">
                    <label className="flex items-center gap-2 text-sm">
                      <input
                        type="checkbox"
                        checked={draft.isActive}
                        onChange={(event) => setDraft((value) => ({ ...value, isActive: event.target.checked }))}
                      />
                      Active
                    </label>
                  </div>
                </div>

                <div className="grid gap-3 md:grid-cols-2">
                  <label className="flex items-center gap-2 text-sm">
                    <input
                      type="checkbox"
                      checked={draft.isMandatory}
                      onChange={(event) => setDraft((value) => ({ ...value, isMandatory: event.target.checked }))}
                    />
                    Mandatory
                  </label>
                  <label className="flex items-center gap-2 text-sm">
                    <input
                      type="checkbox"
                      checked={draft.isRefundable}
                      onChange={(event) => setDraft((value) => ({ ...value, isRefundable: event.target.checked }))}
                    />
                    Refundable
                  </label>
                </div>

                <div className="flex gap-2">
                  <Button type="submit" disabled={createType.isPending || updateType.isPending}>
                    {createType.isPending || updateType.isPending ? (
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    ) : (
                      <Plus className="mr-2 h-4 w-4" />
                    )}
                    {editingId ? "Save" : "Create"}
                  </Button>
                  {editingId ? (
                    <Button
                      type="button"
                      variant="outline"
                      onClick={() => {
                        setEditingId(null);
                        setDraft({
                          name: "",
                          code: "",
                          description: "",
                          feeCategoryId: categories[0] ? String(categories[0].id) : "",
                          frequency: "2",
                          isMandatory: true,
                          isRefundable: false,
                          defaultDueDayOfMonth: "",
                          defaultGracePeriodDays: "",
                          isActive: true,
                        });
                      }}
                    >
                      Cancel
                    </Button>
                  ) : null}
                </div>
              </form>
            </CardContent>
          </Card>
        )}

        <Card>
          <CardHeader>
            <CardTitle>Fee type list</CardTitle>
            <CardDescription>Search and manage all fee definitions.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="relative">
              <Search aria-hidden="true" className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <input
                value={search}
                onChange={(event) => setSearch(event.target.value)}
                placeholder="Search fee types"
                className="w-full rounded-md border bg-background py-2 pl-9 pr-3"
              />
            </div>

            {isPending ? (
              <div className="flex items-center justify-center py-8 text-sm text-muted-foreground">
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Loading fee types…
              </div>
            ) : null}

            {isError ? (
              <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">
                Unable to load fee types.
              </div>
            ) : null}

            {!isPending && !isError && filtered.length === 0 ? (
              <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">
                No fee types found.
              </div>
            ) : null}

            <div className="space-y-2">
              {filtered.map((item) => (
                <div key={item.id} className="flex items-center justify-between rounded-lg border p-4">
                  <div>
                    <p className="font-medium">{item.name}</p>
                    <p className="text-sm text-muted-foreground">
                      {item.code} • {item.feeCategoryName} • {frequencyOptions.find((option) => option.value === String(item.frequency))?.label ?? item.frequency}
                    </p>
                  </div>
                  <div className="flex items-center gap-2">
                    {item.isActive ? (
                      <span className="rounded-full bg-primary/10 px-2 py-1 text-xs font-medium text-primary">Active</span>
                    ) : (
                      <span className="rounded-full bg-muted px-2 py-1 text-xs font-medium text-muted-foreground">Inactive</span>
                    )}
                    {canEdit ? (
                      <Button type="button" variant="outline" className="h-9 w-9 p-0" onClick={() => startEdit(item)}>
                        <Pencil className="h-4 w-4" />
                      </Button>
                    ) : null}
                    {canDelete ? (
                      <Button type="button" variant="outline" className="h-9 w-9 p-0" onClick={() => void handleDelete(item.id)}>
                        <Trash2 className="h-4 w-4" />
                      </Button>
                    ) : null}
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
