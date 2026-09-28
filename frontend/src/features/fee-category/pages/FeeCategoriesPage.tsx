import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, Pencil, Plus, Search, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { useCreateFeeCategory, useDeleteFeeCategory, useFeeCategories, useUpdateFeeCategory } from "../hooks/useFeeCategoryData";
import type { FeeCategoryDto } from "../types/fee-category.types";

export function FeeCategoriesPage() {
  const { hasPermission } = usePermissions();
  const { data = [], isPending, isError } = useFeeCategories();
  const createCategory = useCreateFeeCategory();
  const updateCategory = useUpdateFeeCategory();
  const deleteCategory = useDeleteFeeCategory();
  const [search, setSearch] = useState("");
  const [draft, setDraft] = useState({ name: "", description: "", displayOrder: "1", isActive: true });
  const [editingId, setEditingId] = useState<number | null>(null);

  const canCreate = hasPermission(Permission.FeeCategoryCreate);
  const canEdit = hasPermission(Permission.FeeCategoryEdit);
  const canDelete = hasPermission(Permission.FeeCategoryDelete);

  const filtered = useMemo(
    () =>
      data.filter((item) =>
        `${item.name} ${item.description ?? ""} ${item.displayOrder}`
          .toLowerCase()
          .includes(search.toLowerCase())
      ),
    [data, search]
  );

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();

    const trimmedName = draft.name.trim();
    if (!trimmedName) return;

    try {
      const payload = {
        name: trimmedName,
        description: draft.description.trim() || null,
        displayOrder: Number(draft.displayOrder) || 1,
        isActive: draft.isActive,
      };

      if (editingId) {
        await updateCategory.mutateAsync({
          id: editingId,
          payload: { id: editingId, ...payload },
        });
        toast.success("Fee category updated.");
      } else {
        await createCategory.mutateAsync(payload);
        toast.success("Fee category created.");
      }

      setDraft({ name: "", description: "", displayOrder: "1", isActive: true });
      setEditingId(null);
    } catch (error) {
      toast.error((error as Error)?.message ?? "Unable to save fee category.");
    }
  }

  async function handleDelete(id: number) {
    if (!window.confirm("Delete this fee category? It cannot be deleted if fee types still use it.")) return;

    try {
      await deleteCategory.mutateAsync(id);
      toast.success("Fee category deleted.");
    } catch (error) {
      toast.error((error as Error)?.message ?? "Unable to delete fee category.");
    }
  }

  function startEdit(item: FeeCategoryDto) {
    setEditingId(item.id);
    setDraft({
      name: item.name,
      description: item.description ?? "",
      displayOrder: String(item.displayOrder),
      isActive: item.isActive,
    });
  }

  if (!hasPermission(Permission.FeeCategoryView)) {
    return (
      <Card className="mx-auto max-w-4xl">
        <CardContent className="p-6 text-sm text-destructive">
          You do not have permission to view fee categories.
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Fee categories</h1>
          <p className="text-sm text-muted-foreground">Group fee types such as academic, transport, and lab charges.</p>
        </div>
        <Link to="/dashboard" className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent">
          Back to dashboard
        </Link>
      </div>

      <div className="grid gap-6 lg:grid-cols-[0.9fr_1.1fr]">
        {(canCreate || (canEdit && editingId)) && (
          <Card>
            <CardHeader>
              <CardTitle>{editingId ? "Edit fee category" : "Create fee category"}</CardTitle>
              <CardDescription>Display order controls the list sequence.</CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={(event) => void handleSubmit(event)} className="space-y-4">
                <div>
                  <label className="mb-1 block text-sm font-medium">Name</label>
                  <input
                    required
                    value={draft.name}
                    onChange={(event) => setDraft((value) => ({ ...value, name: event.target.value }))}
                    className="w-full rounded-md border bg-background px-3 py-2"
                    placeholder="Academic"
                  />
                </div>

                <div>
                  <label className="mb-1 block text-sm font-medium">Description</label>
                  <textarea
                    value={draft.description}
                    onChange={(event) => setDraft((value) => ({ ...value, description: event.target.value }))}
                    className="w-full rounded-md border bg-background px-3 py-2"
                    rows={3}
                    placeholder="Optional description"
                  />
                </div>

                <div className="grid gap-4 md:grid-cols-2">
                  <div>
                    <label className="mb-1 block text-sm font-medium">Display order</label>
                    <input
                      type="number"
                      min="1"
                      value={draft.displayOrder}
                      onChange={(event) => setDraft((value) => ({ ...value, displayOrder: event.target.value }))}
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

                <div className="flex gap-2">
                  <Button type="submit" disabled={createCategory.isPending || updateCategory.isPending}>
                    {createCategory.isPending || updateCategory.isPending ? (
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
                        setDraft({ name: "", description: "", displayOrder: "1", isActive: true });
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
            <CardTitle>Categories</CardTitle>
            <CardDescription>Search and manage available fee groups.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="relative">
              <Search aria-hidden="true" className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <input
                value={search}
                onChange={(event) => setSearch(event.target.value)}
                placeholder="Search categories"
                className="w-full rounded-md border bg-background py-2 pl-9 pr-3"
              />
            </div>

            {isPending ? (
              <div className="flex items-center justify-center py-8 text-sm text-muted-foreground">
                <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                Loading categories…
              </div>
            ) : null}

            {isError ? (
              <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">
                Unable to load fee categories.
              </div>
            ) : null}

            {!isPending && !isError && filtered.length === 0 ? (
              <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">
                No fee categories found.
              </div>
            ) : null}

            <div className="space-y-2">
              {filtered.map((item) => (
                <div key={item.id} className="flex items-center justify-between rounded-lg border p-4">
                  <div>
                    <p className="font-medium">{item.name}</p>
                    <p className="text-sm text-muted-foreground">
                      {item.description || "No description"} • Display order: {item.displayOrder}
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
