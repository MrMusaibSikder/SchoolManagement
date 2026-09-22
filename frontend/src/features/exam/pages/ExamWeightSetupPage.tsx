import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2, Pencil, Star, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useAcademicYears } from "@/features/academic/hooks/useAcademicData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { getExamErrorMessage } from "../api/exam.api";
import {
  useAddExamWeightItem,
  useCreateExamWeightSetup,
  useDeleteExamWeightItem,
  useDeleteExamWeightSetup,
  useExamWeightSetups,
  useToggleExamWeightSetup,
  useUpdateExamWeightItem,
  useUpdateExamWeightSetup,
} from "../hooks/useExamData";

const emptySetupDraft = {
  academicYearId: "",
  name: "",
};

export function ExamWeightSetupPage() {
  const { hasPermission } = usePermissions();
  const canManage = hasPermission(Permission.WeightSetupManage);
  const { data: years = [] } = useAcademicYears();
  const { data: setups = [], isPending, isError } = useExamWeightSetups();
  const createSetup = useCreateExamWeightSetup();
  const updateSetup = useUpdateExamWeightSetup();
  const deleteSetup = useDeleteExamWeightSetup();
  const toggleSetup = useToggleExamWeightSetup();
  const addItem = useAddExamWeightItem();
  const updateItem = useUpdateExamWeightItem();
  const deleteItem = useDeleteExamWeightItem();

  const [yearFilter, setYearFilter] = useState("");
  const [editingId, setEditingId] = useState<number | null>(null);
  const [setupDraft, setSetupDraft] = useState(emptySetupDraft);
  const [itemForm, setItemForm] = useState<{
    setupId: number | null;
    itemId: number | null;
    examId: string;
    weightPercentage: string;
  }>({
    setupId: null,
    itemId: null,
    examId: "",
    weightPercentage: "",
  });

  const filtered = useMemo(
    () => (yearFilter ? setups.filter((item) => String(item.academicYearId) === yearFilter) : setups),
    [setups, yearFilter]
  );

  function resetSetupForm() {
    setEditingId(null);
    setSetupDraft(emptySetupDraft);
  }

  function startEditSetup(setup: { id: number; academicYearId: number; name: string }) {
    setEditingId(setup.id);
    setSetupDraft({
      academicYearId: String(setup.academicYearId),
      name: setup.name,
    });
  }

  async function handleSetupSubmit(event: React.FormEvent) {
    event.preventDefault();
    const academicYearId = Number(setupDraft.academicYearId);
    const name = setupDraft.name.trim();

    if (!academicYearId || !name) {
      toast.error("Academic year and setup name are required.");
      return;
    }

    try {
      if (editingId) {
        await updateSetup.mutateAsync({ id: editingId, payload: { id: editingId, name } });
        toast.success("Weight setup updated.");
      } else {
        await createSetup.mutateAsync({ academicYearId, name, items: [] });
        toast.success("Weight setup created.");
      }
      resetSetupForm();
    } catch (error) {
      toast.error(getExamErrorMessage(error));
    }
  }

  async function handleItemSubmit(setupId: number) {
    const examId = Number(itemForm.examId);
    const weightPercentage = Number(itemForm.weightPercentage);

    if (!examId || !Number.isFinite(weightPercentage) || weightPercentage <= 0) {
      toast.error("Select a valid exam and weight percentage.");
      return;
    }

    try {
      if (itemForm.itemId) {
        await updateItem.mutateAsync({
          itemId: itemForm.itemId,
          payload: { id: itemForm.itemId, weightPercentage },
        });
        toast.success("Weight item updated.");
      } else {
        await addItem.mutateAsync({
          examWeightSetupId: setupId,
          examId,
          weightPercentage,
        });
        toast.success("Weight item added.");
      }
      setItemForm({ setupId: null, itemId: null, examId: "", weightPercentage: "" });
    } catch (error) {
      toast.error(getExamErrorMessage(error));
    }
  }

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Exam weight setup</h1>
          <p className="text-sm text-muted-foreground">
            Define how each exam contributes to the final result for an academic year.
          </p>
        </div>
        <Link
          to="/exams"
          className="inline-flex items-center justify-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent"
        >
          Back
        </Link>
      </div>

      <div className="grid gap-6 lg:grid-cols-[0.9fr_1.1fr]">
        {canManage ? (
          <Card>
            <CardHeader>
              <CardTitle>{editingId ? "Edit setup" : "Create setup"}</CardTitle>
              <CardDescription>Each setup can aggregate several exams into a final weighted result.</CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={(event) => void handleSetupSubmit(event)} className="space-y-4">
                <select
                  required
                  value={setupDraft.academicYearId}
                  onChange={(event) => setSetupDraft((value) => ({ ...value, academicYearId: event.target.value }))}
                  className="w-full rounded-md border bg-background px-3 py-2"
                >
                  <option value="">Academic year</option>
                  {years.map((year) => (
                    <option key={year.id} value={year.id}>
                      {year.name}
                    </option>
                  ))}
                </select>

                <input
                  required
                  maxLength={120}
                  value={setupDraft.name}
                  onChange={(event) => setSetupDraft((value) => ({ ...value, name: event.target.value }))}
                  placeholder="Setup name (e.g. Annual Final Weight)"
                  className="w-full rounded-md border bg-background px-3 py-2"
                />

                <div className="flex gap-2">
                  <Button type="submit" disabled={createSetup.isPending || updateSetup.isPending}>
                    {(createSetup.isPending || updateSetup.isPending) && (
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    )}
                    {editingId ? "Save" : "Create"}
                  </Button>
                  {editingId ? (
                    <Button type="button" variant="outline" onClick={resetSetupForm}>
                      Cancel
                    </Button>
                  ) : null}
                </div>
              </form>
            </CardContent>
          </Card>
        ) : null}

        <Card>
          <CardHeader>
            <CardTitle>Weight setups</CardTitle>
            <CardDescription>
              <select
                value={yearFilter}
                onChange={(event) => setYearFilter(event.target.value)}
                className="mt-2 rounded-md border bg-background px-3 py-2"
              >
                <option value="">All academic years</option>
                {years.map((year) => (
                  <option key={year.id} value={String(year.id)}>
                    {year.name}
                  </option>
                ))}
              </select>
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            {isPending ? <p className="text-sm text-muted-foreground">Loading…</p> : null}
            {isError ? <p className="text-sm text-destructive">Unable to load weight setups.</p> : null}
            {!isPending && filtered.length === 0 ? (
              <p className="text-sm text-muted-foreground">No exam weight setup created yet.</p>
            ) : null}

            {filtered.map((setup) => {
              const totalWeight = setup.items.reduce((sum, item) => sum + item.weightPercentage, 0);

              return (
                <div key={setup.id} className="rounded-lg border p-4">
                  <div className="flex flex-col gap-3 md:flex-row md:items-start md:justify-between">
                    <div>
                      <div className="flex items-center gap-2">
                        <p className="font-semibold">{setup.name}</p>
                        {setup.isActive ? (
                          <span className="inline-flex items-center gap-1 rounded-full bg-emerald-500/15 px-2 py-0.5 text-xs font-medium text-emerald-700">
                            <Star className="h-3 w-3" /> Active
                          </span>
                        ) : (
                          <span className="inline-flex items-center rounded-full bg-muted px-2 py-0.5 text-xs font-medium text-muted-foreground">
                            Inactive
                          </span>
                        )}
                      </div>
                      <p className="text-sm text-muted-foreground">
                        {setup.academicYearName} · Total {totalWeight.toFixed(2)}%
                      </p>
                    </div>

                    {canManage ? (
                      <div className="flex flex-wrap gap-2">
                        <Button type="button" variant="outline" onClick={() => startEditSetup(setup)}>
                          <Pencil className="h-4 w-4" />
                        </Button>
                        <Button
                          type="button"
                          variant="outline"
                          onClick={() =>
                            void toggleSetup.mutateAsync({ id: setup.id, activate: !setup.isActive }).catch((error) => {
                              toast.error(getExamErrorMessage(error));
                            })
                          }
                        >
                          {setup.isActive ? "Deactivate" : "Activate"}
                        </Button>
                        <Button
                          type="button"
                          variant="outline"
                          onClick={() => {
                            if (!window.confirm("Delete this weight setup?")) return;
                            void deleteSetup
                              .mutateAsync(setup.id)
                              .then(() => toast.success("Weight setup deleted."))
                              .catch((error) => toast.error(getExamErrorMessage(error)));
                          }}
                        >
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </div>
                    ) : null}
                  </div>

                  <div className="mt-4 space-y-2">
                    {setup.items.length === 0 ? (
                      <p className="text-sm text-muted-foreground">No exam items added yet.</p>
                    ) : (
                      setup.items.map((item) => (
                        <div
                          key={item.id}
                          className="flex flex-col gap-2 rounded-md border bg-muted/20 p-3 sm:flex-row sm:items-center sm:justify-between"
                        >
                          <div>
                            <p className="font-medium">{item.examName ?? `Exam #${item.examId}`}</p>
                            <p className="text-sm text-muted-foreground">{item.weightPercentage}%</p>
                          </div>
                          {canManage ? (
                            <div className="flex gap-2">
                              <Button
                                type="button"
                                variant="outline"
                                onClick={() => {
                                  setItemForm({
                                    setupId: setup.id,
                                    itemId: item.id,
                                    examId: String(item.examId),
                                    weightPercentage: String(item.weightPercentage),
                                  });
                                }}
                              >
                                <Pencil className="h-4 w-4" />
                              </Button>
                              <Button
                                type="button"
                                variant="outline"
                                onClick={() => {
                                  if (!window.confirm("Remove this exam weight item?")) return;
                                  void deleteItem
                                    .mutateAsync(item.id)
                                    .then(() => toast.success("Weight item removed."))
                                    .catch((error) => toast.error(getExamErrorMessage(error)));
                                }}
                              >
                                <Trash2 className="h-4 w-4" />
                              </Button>
                            </div>
                          ) : null}
                        </div>
                      ))
                    )}
                  </div>

                  {canManage ? (
                    <div className="mt-4 rounded-md border bg-muted/10 p-3">
                      <div className="mb-2 flex items-center justify-between">
                        <p className="text-sm font-medium">{itemForm.setupId === setup.id && itemForm.itemId ? "Edit item" : "Add item"}</p>
                        {itemForm.setupId === setup.id ? (
                          <Button
                            type="button"
                            variant="outline"
                            onClick={() => setItemForm({ setupId: null, itemId: null, examId: "", weightPercentage: "" })}
                          >
                            Close
                          </Button>
                        ) : null}
                      </div>

                      <div className="grid gap-3 md:grid-cols-[1fr_140px_auto]">
                        <select
                          value={itemForm.setupId === setup.id ? itemForm.examId : ""}
                          onChange={(event) =>
                            setItemForm((value) => ({
                              ...value,
                              setupId: setup.id,
                              itemId: value.itemId,
                              examId: event.target.value,
                            }))
                          }
                          className="rounded-md border bg-background px-3 py-2"
                        >
                          <option value="">Select exam</option>
                          {setup.items.map((item) => (
                            <option key={item.examId} value={item.examId} disabled={Boolean(itemForm.itemId && item.examId === Number(itemForm.examId))}>
                              {item.examName ?? `Exam #${item.examId}`}
                            </option>
                          ))}
                        </select>

                        <input
                          type="number"
                          min="1"
                          step="0.01"
                          value={itemForm.setupId === setup.id ? itemForm.weightPercentage : ""}
                          onChange={(event) =>
                            setItemForm((value) => ({
                              ...value,
                              setupId: setup.id,
                              weightPercentage: event.target.value,
                            }))
                          }
                          placeholder="Weight %"
                          className="rounded-md border bg-background px-3 py-2"
                        />

                        <Button
                          type="button"
                          onClick={() => void handleItemSubmit(setup.id)}
                          disabled={addItem.isPending || updateItem.isPending}
                        >
                          {(addItem.isPending || updateItem.isPending) && (
                            <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                          )}
                          {itemForm.setupId === setup.id && itemForm.itemId ? "Save" : "Add"}
                        </Button>
                      </div>
                    </div>
                  ) : null}
                </div>
              );
            })}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
