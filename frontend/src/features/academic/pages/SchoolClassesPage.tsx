import { useEffect, useMemo, useRef, useState } from "react";
import { Link } from "react-router-dom";
import { toast } from "sonner";
import { AlertTriangle, Loader2, Pencil, Plus, Search, Trash2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { getApiErrorMessage } from "@/lib/api/error-message";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import {
  useCreateSchoolClass,
  useCreateSection,
  useDeleteSchoolClass,
  useDeleteSection,
  useSchoolClasses,
  useSections,
  useUpdateSchoolClass,
  useUpdateSection,
} from "../hooks/useAcademicData";
import type { SchoolClassDto, SectionDto } from "../types/academic.types";

type DeleteTarget =
  | { type: "class"; id: number; name: string }
  | { type: "section"; id: number; name: string };

const EMPTY_CLASSES: SchoolClassDto[] = [];
const EMPTY_SECTIONS: SectionDto[] = [];

export function SchoolClassesPage() {
  const { hasPermission } = usePermissions();
  const canViewClasses = hasPermission(Permission.SchoolClassView);
  const canCreateClass = hasPermission(Permission.SchoolClassCreate);
  const canEditClass = hasPermission(Permission.SchoolClassEdit);
  const canDeleteClass = hasPermission(Permission.SchoolClassDelete);
  const canViewSections = hasPermission(Permission.SectionView);
  const canCreateSection = hasPermission(Permission.SectionCreate);
  const canEditSection = hasPermission(Permission.SectionEdit);
  const canDeleteSection = hasPermission(Permission.SectionDelete);

  const classesQuery = useSchoolClasses(canViewClasses);
  const sectionsQuery = useSections(canViewSections);
  const createClass = useCreateSchoolClass();
  const updateClass = useUpdateSchoolClass();
  const deleteClass = useDeleteSchoolClass();
  const createSection = useCreateSection();
  const updateSection = useUpdateSection();
  const deleteSection = useDeleteSection();

  const [search, setSearch] = useState("");
  const [selectedClassId, setSelectedClassId] = useState("all");
  const [className, setClassName] = useState("");
  const [displayOrder, setDisplayOrder] = useState("1");
  const [editingClassId, setEditingClassId] = useState<number | null>(null);
  const [sectionName, setSectionName] = useState("");
  const [sectionClassId, setSectionClassId] = useState("");
  const [editingSectionId, setEditingSectionId] = useState<number | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<DeleteTarget | null>(null);
  const cancelDeleteRef = useRef<HTMLButtonElement>(null);
  const lastClassErrorAt = useRef(0);
  const lastSectionErrorAt = useRef(0);

  const classes = classesQuery.data ?? EMPTY_CLASSES;
  const sections = sectionsQuery.data ?? EMPTY_SECTIONS;
  const isClassMutationPending =
    createClass.isPending || updateClass.isPending || deleteClass.isPending;
  const isSectionMutationPending =
    createSection.isPending || updateSection.isPending || deleteSection.isPending;
  const anyMutationPending = isClassMutationPending || isSectionMutationPending;

  const classById = useMemo(
    () => new Map(classes.map((item) => [item.id, item])),
    [classes]
  );
  const sectionsByClassId = useMemo(() => {
    const grouped = new Map<number, SectionDto[]>();
    for (const section of sections) {
      const group = grouped.get(section.classId) ?? [];
      group.push(section);
      grouped.set(section.classId, group);
    }
    for (const group of grouped.values()) {
      group.sort((left, right) => left.name.localeCompare(right.name));
    }
    return grouped;
  }, [sections]);

  const classFilterOptions = useMemo(() => {
    const options = new Map(classes.map((item) => [item.id, item.name]));
    sections.forEach((section) => {
      if (!options.has(section.classId)) {
        options.set(section.classId, `Class #${section.classId}`);
      }
    });
    return [...options].sort((left, right) => {
      const leftClass = classById.get(left[0]);
      const rightClass = classById.get(right[0]);
      return (leftClass?.displayOrder ?? Number.MAX_SAFE_INTEGER) -
        (rightClass?.displayOrder ?? Number.MAX_SAFE_INTEGER) ||
        left[1].localeCompare(right[1]);
    });
  }, [classById, classes, sections]);

  const filteredClasses = useMemo(() => {
    const query = search.trim().toLocaleLowerCase();
    const classItems = canViewClasses
      ? [...classes].sort(
          (left, right) =>
            left.displayOrder - right.displayOrder ||
            left.name.localeCompare(right.name)
        )
      : [...sectionsByClassId.keys()].map(
          (id): SchoolClassDto => ({
            id,
            name: classById.get(id)?.name ?? `Class #${id}`,
            displayOrder: classById.get(id)?.displayOrder ?? Number.MAX_SAFE_INTEGER,
          })
        );

    return classItems
      .filter((item) => selectedClassId === "all" || item.id === Number(selectedClassId))
      .map((item) => {
        const classMatches = item.name.toLocaleLowerCase().includes(query);
        const classSections = canViewSections
          ? sectionsByClassId.get(item.id) ?? []
          : [];
        const matchingSections = classMatches
          ? classSections
          : classSections.filter((section) =>
              section.name.toLocaleLowerCase().includes(query)
            );
        return { ...item, sections: matchingSections, classMatches };
      })
      .filter(
        (item) =>
          !query ||
          item.classMatches ||
          item.sections.length > 0
      );
  }, [
    canViewClasses,
    canViewSections,
    classById,
    classes,
    search,
    sectionsByClassId,
    selectedClassId,
  ]);

  useEffect(() => {
    if (
      canViewClasses &&
      classesQuery.isError &&
      classesQuery.errorUpdatedAt > 0 &&
      lastClassErrorAt.current !== classesQuery.errorUpdatedAt
    ) {
      lastClassErrorAt.current = classesQuery.errorUpdatedAt;
      toast.error(
        getApiErrorMessage(
          classesQuery.error,
          "Unable to load classes. Please try again."
        )
      );
    }
  }, [
    canViewClasses,
    classesQuery.error,
    classesQuery.errorUpdatedAt,
    classesQuery.isError,
  ]);

  useEffect(() => {
    if (
      canViewSections &&
      sectionsQuery.isError &&
      sectionsQuery.errorUpdatedAt > 0 &&
      lastSectionErrorAt.current !== sectionsQuery.errorUpdatedAt
    ) {
      lastSectionErrorAt.current = sectionsQuery.errorUpdatedAt;
      toast.error(
        getApiErrorMessage(
          sectionsQuery.error,
          "Unable to load sections. Please try again."
        )
      );
    }
  }, [
    canViewSections,
    sectionsQuery.error,
    sectionsQuery.errorUpdatedAt,
    sectionsQuery.isError,
  ]);

  useEffect(() => {
    if (!deleteTarget) return;
    cancelDeleteRef.current?.focus();
    function closeOnEscape(event: KeyboardEvent) {
      if (event.key === "Escape") setDeleteTarget(null);
    }
    document.addEventListener("keydown", closeOnEscape);
    return () => document.removeEventListener("keydown", closeOnEscape);
  }, [deleteTarget]);

  function resetClassForm() {
    setClassName("");
    setDisplayOrder("1");
    setEditingClassId(null);
  }

  function resetSectionForm() {
    setSectionName("");
    setSectionClassId(selectedClassId === "all" ? "" : selectedClassId);
    setEditingSectionId(null);
  }

  function selectClassFilter(value: string) {
    setSelectedClassId(value);
    if (!editingSectionId) {
      setSectionClassId(value === "all" ? "" : value);
    }
  }

  async function handleClassSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const payload = { name: className.trim(), displayOrder: Number(displayOrder) };
    try {
      if (editingClassId !== null) {
        await updateClass.mutateAsync({
          id: editingClassId,
          payload: { ...payload, id: editingClassId },
        });
        toast.success("Class updated successfully.");
      } else {
        await createClass.mutateAsync(payload);
        toast.success("Class created successfully.");
      }
      resetClassForm();
    } catch (error) {
      toast.error(
        getApiErrorMessage(
          error,
          `Unable to ${editingClassId !== null ? "update" : "create"} class. Please try again.`
        )
      );
    }
  }

  async function handleSectionSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!sectionClassId) {
      toast.error("Select a class for this section.");
      return;
    }

    const payload = {
      name: sectionName.trim(),
      classId: Number(sectionClassId),
    };
    try {
      if (editingSectionId !== null) {
        await updateSection.mutateAsync({
          id: editingSectionId,
          payload: { ...payload, id: editingSectionId },
        });
        toast.success("Section updated successfully.");
      } else {
        await createSection.mutateAsync(payload);
        toast.success("Section created successfully.");
      }
      setSelectedClassId(sectionClassId);
      resetSectionForm();
    } catch (error) {
      toast.error(
        getApiErrorMessage(
          error,
          `Unable to ${editingSectionId !== null ? "update" : "create"} section. Please try again.`
        )
      );
    }
  }

  function startEditClass(item: SchoolClassDto) {
    setEditingClassId(item.id);
    setClassName(item.name);
    setDisplayOrder(String(item.displayOrder));
  }

  function startEditSection(item: SectionDto) {
    setEditingSectionId(item.id);
    setSectionName(item.name);
    setSectionClassId(String(item.classId));
    setSelectedClassId(String(item.classId));
  }

  async function confirmDelete() {
    if (!deleteTarget) return;
    const target = deleteTarget;
    try {
      if (target.type === "class") {
        await deleteClass.mutateAsync(target.id);
        toast.success("Class deleted successfully.");
        if (selectedClassId === String(target.id)) setSelectedClassId("all");
        if (editingClassId === target.id) resetClassForm();
      } else {
        await deleteSection.mutateAsync(target.id);
        toast.success("Section deleted successfully.");
        if (editingSectionId === target.id) resetSectionForm();
      }
      setDeleteTarget(null);
    } catch (error) {
      toast.error(
        getApiErrorMessage(
          error,
          `Unable to delete ${target.type}. Please try again.`
        )
      );
    }
  }

  const loading =
    (canViewClasses && classesQuery.isPending) ||
    (canViewSections && sectionsQuery.isPending);
  const hasLoadError =
    (canViewClasses && classesQuery.isError) ||
    (canViewSections && sectionsQuery.isError);

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">
            Classes &amp; Sections
          </h1>
          <p className="text-sm text-muted-foreground">
            Manage each class together with its sections.
          </p>
        </div>
        <Link
          to="/academic"
          className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent"
        >
          Back to overview
        </Link>
      </div>

      {(canCreateClass || canEditClass || canCreateSection || canEditSection) && (
        <div className="grid gap-6 lg:grid-cols-2">
          {(canCreateClass || canEditClass) && (
            <Card>
              <CardHeader>
                <CardTitle>
                  {editingClassId !== null ? "Edit class" : "Create class"}
                </CardTitle>
                <CardDescription>
                  {editingClassId !== null
                    ? "Update the class name and display order."
                    : "Create a class before adding its sections."}
                </CardDescription>
              </CardHeader>
              <CardContent>
                <form onSubmit={handleClassSubmit} className="space-y-4">
                  <div className="space-y-1.5">
                    <label htmlFor="class-name" className="text-sm font-medium">
                      Class name
                    </label>
                    <input
                      id="class-name"
                      required
                      maxLength={100}
                      value={className}
                      onChange={(event) => setClassName(event.target.value)}
                      className="w-full rounded-md border bg-background px-3 py-2"
                    />
                  </div>
                  <div className="space-y-1.5">
                    <label
                      htmlFor="class-order"
                      className="text-sm font-medium"
                    >
                      Display order
                    </label>
                    <input
                      id="class-order"
                      type="number"
                      min="1"
                      step="1"
                      required
                      value={displayOrder}
                      onChange={(event) => setDisplayOrder(event.target.value)}
                      className="w-full rounded-md border bg-background px-3 py-2"
                    />
                  </div>
                  <div className="flex flex-wrap gap-2">
                    <Button
                      type="submit"
                      disabled={
                        anyMutationPending ||
                        (editingClassId === null
                          ? !canCreateClass
                          : !canEditClass)
                      }
                    >
                      {isClassMutationPending ? (
                        <Loader2
                          aria-hidden="true"
                          className="mr-2 h-4 w-4 animate-spin"
                        />
                      ) : (
                        <Plus aria-hidden="true" className="mr-2 h-4 w-4" />
                      )}
                      {editingClassId !== null ? "Save changes" : "Create class"}
                    </Button>
                    {editingClassId !== null && (
                      <Button
                        type="button"
                        variant="outline"
                        onClick={resetClassForm}
                      >
                        Cancel
                      </Button>
                    )}
                  </div>
                </form>
              </CardContent>
            </Card>
          )}

          {(canCreateSection || canEditSection) && (
            <Card>
              <CardHeader>
                <CardTitle>
                  {editingSectionId !== null
                    ? "Edit section"
                    : "Add section to a class"}
                </CardTitle>
                <CardDescription>
                  Select a class to keep its sections grouped together.
                </CardDescription>
              </CardHeader>
              <CardContent>
                <form onSubmit={handleSectionSubmit} className="space-y-4">
                  <div className="space-y-1.5">
                    <label
                      htmlFor="section-class"
                      className="text-sm font-medium"
                    >
                      Class
                    </label>
                    <select
                      id="section-class"
                      required
                      value={sectionClassId}
                      onChange={(event) =>
                        setSectionClassId(event.target.value)
                      }
                      disabled={
                        !canViewClasses && editingSectionId === null
                      }
                      className="w-full rounded-md border bg-background px-3 py-2 disabled:opacity-60"
                    >
                      <option value="">Select class</option>
                      {sectionClassId &&
                        !classById.has(Number(sectionClassId)) && (
                          <option value={sectionClassId}>
                            {classById.get(Number(sectionClassId))?.name ??
                              `Class #${sectionClassId}`}
                          </option>
                        )}
                      {[...classes]
                        .sort(
                          (left, right) =>
                            left.displayOrder - right.displayOrder ||
                            left.name.localeCompare(right.name)
                        )
                        .map((item) => (
                          <option key={item.id} value={item.id}>
                            {item.name}
                          </option>
                        ))}
                    </select>
                    {!canViewClasses && editingSectionId === null && (
                      <p className="text-xs text-destructive">
                        Class view permission is required to assign a section.
                      </p>
                    )}
                  </div>
                  <div className="space-y-1.5">
                    <label
                      htmlFor="section-name"
                      className="text-sm font-medium"
                    >
                      Section name
                    </label>
                    <input
                      id="section-name"
                      required
                      maxLength={100}
                      value={sectionName}
                      onChange={(event) => setSectionName(event.target.value)}
                      className="w-full rounded-md border bg-background px-3 py-2"
                    />
                  </div>
                  <div className="flex flex-wrap gap-2">
                    <Button
                      type="submit"
                      disabled={
                        anyMutationPending ||
                        (!canViewClasses &&
                          (editingSectionId === null || !sectionClassId)) ||
                        (editingSectionId === null && classes.length === 0) ||
                        (editingSectionId === null
                          ? !canCreateSection
                          : !canEditSection)
                      }
                    >
                      {isSectionMutationPending ? (
                        <Loader2
                          aria-hidden="true"
                          className="mr-2 h-4 w-4 animate-spin"
                        />
                      ) : (
                        <Plus aria-hidden="true" className="mr-2 h-4 w-4" />
                      )}
                      {editingSectionId !== null
                        ? "Save changes"
                        : "Create section"}
                    </Button>
                    {editingSectionId !== null && (
                      <Button
                        type="button"
                        variant="outline"
                        onClick={resetSectionForm}
                      >
                        Cancel
                      </Button>
                    )}
                  </div>
                </form>
              </CardContent>
            </Card>
          )}
        </div>
      )}

      <Card>
        <CardHeader>
          <CardTitle>Class directory</CardTitle>
          <CardDescription>
            Filter by class to review its sections, then edit or remove each
            record.
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 sm:grid-cols-[minmax(0,1fr)_minmax(220px,0.7fr)]">
            <div className="relative">
              <Search
                aria-hidden="true"
                className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground"
              />
              <input
                value={search}
                onChange={(event) => setSearch(event.target.value)}
                placeholder="Search classes or sections"
                aria-label="Search classes or sections"
                className="w-full rounded-md border bg-background py-2 pl-9 pr-3"
              />
            </div>
            <select
              value={selectedClassId}
              onChange={(event) => selectClassFilter(event.target.value)}
              aria-label="Filter by class"
              className="w-full rounded-md border bg-background px-3 py-2"
            >
              <option value="all">All classes</option>
              {classFilterOptions.map(([id, name]) => (
                <option key={id} value={id}>
                  {name}
                </option>
              ))}
            </select>
          </div>

          {loading && (
            <div className="flex items-center justify-center gap-2 py-8 text-sm text-muted-foreground">
              <Loader2 aria-hidden="true" className="h-4 w-4 animate-spin" />
              Loading classes and sections…
            </div>
          )}

          {hasLoadError && (
            <div className="flex flex-col gap-3 rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive sm:flex-row sm:items-center sm:justify-between">
              <p>
                {canViewClasses && classesQuery.isError
                  ? getApiErrorMessage(
                      classesQuery.error,
                      "Unable to load classes. Please try again."
                    )
                  : getApiErrorMessage(
                      sectionsQuery.error,
                      "Unable to load sections. Please try again."
                    )}
              </p>
              <Button
                type="button"
                variant="outline"
                onClick={() => {
                  if (canViewClasses) void classesQuery.refetch();
                  if (canViewSections) void sectionsQuery.refetch();
                }}
              >
                Try again
              </Button>
            </div>
          )}

          {!loading && filteredClasses.length === 0 && !hasLoadError && (
            <div className="rounded-lg border border-dashed p-8 text-center">
              <p className="font-medium">No matching classes or sections</p>
              <p className="mt-1 text-sm text-muted-foreground">
                Change the search or class filter, or create a new class.
              </p>
            </div>
          )}

          {!loading && filteredClasses.length > 0 && (
            <div className="space-y-3">
              {filteredClasses.map((item) => (
                <section
                  key={item.id}
                  aria-labelledby={`class-heading-${item.id}`}
                  className="overflow-hidden rounded-lg border"
                >
                  <div className="flex flex-col gap-3 bg-muted/30 p-4 sm:flex-row sm:items-center sm:justify-between">
                    <div className="min-w-0">
                      <h2
                        id={`class-heading-${item.id}`}
                        className="truncate font-semibold"
                      >
                        {item.name}
                      </h2>
                      {canViewClasses && (
                        <p className="text-sm text-muted-foreground">
                          Display order: {item.displayOrder}
                        </p>
                      )}
                    </div>
                    <div className="flex shrink-0 items-center gap-2">
                      {canEditClass && canViewClasses && (
                        <Button
                          type="button"
                          variant="outline"
                          className="h-9 w-9 p-0"
                          aria-label={`Edit ${item.name}`}
                          onClick={() => startEditClass(item)}
                        >
                          <Pencil aria-hidden="true" className="h-4 w-4" />
                        </Button>
                      )}
                      {canDeleteClass && canViewClasses && (
                        <Button
                          type="button"
                          variant="outline"
                          className="h-9 w-9 p-0"
                          aria-label={`Delete ${item.name}`}
                          disabled={anyMutationPending}
                          onClick={() =>
                            setDeleteTarget({
                              type: "class",
                              id: item.id,
                              name: item.name,
                            })
                          }
                        >
                          <Trash2 aria-hidden="true" className="h-4 w-4" />
                        </Button>
                      )}
                    </div>
                  </div>
                  <div className="divide-y">
                    {item.sections.map((section) => (
                      <div
                        key={section.id}
                        className="flex flex-col gap-3 px-4 py-3 sm:flex-row sm:items-center sm:justify-between"
                      >
                        <div className="flex min-w-0 items-center gap-3">
                          <span
                            aria-hidden="true"
                            className="h-2 w-2 shrink-0 rounded-full bg-primary/70"
                          />
                          <span className="truncate text-sm font-medium">
                            {section.name}
                          </span>
                        </div>
                        <div className="flex shrink-0 items-center gap-2">
                          {canEditSection && (
                            <Button
                              type="button"
                              variant="outline"
                              className="h-9 w-9 p-0"
                              aria-label={`Edit ${section.name} section`}
                              onClick={() => startEditSection(section)}
                            >
                              <Pencil
                                aria-hidden="true"
                                className="h-4 w-4"
                              />
                            </Button>
                          )}
                          {canDeleteSection && (
                            <Button
                              type="button"
                              variant="outline"
                              className="h-9 w-9 p-0"
                              aria-label={`Delete ${section.name} section`}
                              disabled={anyMutationPending}
                              onClick={() =>
                                setDeleteTarget({
                                  type: "section",
                                  id: section.id,
                                  name: section.name,
                                })
                              }
                            >
                              <Trash2
                                aria-hidden="true"
                                className="h-4 w-4"
                              />
                            </Button>
                          )}
                        </div>
                      </div>
                    ))}
                    {item.sections.length === 0 &&
                      canViewSections &&
                      !sectionsQuery.isError && (
                      <p className="px-4 py-4 text-sm text-muted-foreground">
                        No sections are assigned to this class yet.
                      </p>
                    )}
                    {!canViewSections && (
                      <p className="px-4 py-4 text-sm text-muted-foreground">
                        You do not have permission to view this class&apos;s
                        sections.
                      </p>
                    )}
                  </div>
                </section>
              ))}
            </div>
          )}
        </CardContent>
      </Card>

      {deleteTarget && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4"
          onMouseDown={(event) => {
            if (event.target === event.currentTarget) setDeleteTarget(null);
          }}
        >
          <div
            role="alertdialog"
            aria-modal="true"
            aria-labelledby="delete-confirm-title"
            aria-describedby="delete-confirm-description"
            className="w-full max-w-md rounded-xl border bg-card p-6 text-card-foreground shadow-xl"
          >
            <div className="flex items-start gap-3">
              <span className="flex h-10 w-10 shrink-0 items-center justify-center rounded-full bg-destructive/10 text-destructive">
                <AlertTriangle aria-hidden="true" className="h-5 w-5" />
              </span>
              <div className="space-y-1">
                <h2 id="delete-confirm-title" className="font-semibold">
                  Delete {deleteTarget.type}?
                </h2>
                <p
                  id="delete-confirm-description"
                  className="text-sm text-muted-foreground"
                >
                  This will delete “{deleteTarget.name}”.
                  {deleteTarget.type === "class" &&
                    " The server may prevent deleting a class that is still in use."}
                </p>
              </div>
            </div>
            <div className="mt-6 flex justify-end gap-2">
              <Button
                ref={cancelDeleteRef}
                type="button"
                variant="outline"
                onClick={() => setDeleteTarget(null)}
              >
                Cancel
              </Button>
              <Button
                type="button"
                className="bg-destructive text-destructive-foreground hover:bg-destructive/90"
                disabled={anyMutationPending}
                onClick={() => void confirmDelete()}
              >
                {anyMutationPending && (
                  <Loader2
                    aria-hidden="true"
                    className="mr-2 h-4 w-4 animate-spin"
                  />
                )}
                Delete
              </Button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
