import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm, useWatch } from "react-hook-form";
import {
  Check,
  Eye,
  FileText,
  Loader2,
  Pencil,
  Plus,
  Search,
  Trash2,
  X,
} from "lucide-react";
import { toast } from "sonner";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import {
  useCreateSmsTemplate,
  useDeleteSmsTemplate,
  usePreviewSmsTemplate,
  useSetSmsTemplateActive,
  useSmsTemplates,
  useUpdateSmsTemplate,
} from "../hooks/useSmsData";
import {
  smsPlaceholderSchema,
  smsTemplateSchema,
  type SmsPlaceholderFormValues,
  type SmsTemplateFormValues,
} from "../schemas/sms-template.schema";
import type { SmsTemplateDto } from "../types/sms.types";

const PAGE_SIZE = 10;
const PLACEHOLDERS = [
  ["studentName", "Student name", "StudentName"],
  ["guardianName", "Guardian name", "GuardianName"],
  ["teacherName", "Teacher name", "TeacherName"],
  ["employeeName", "Employee name", "EmployeeName"],
  ["class", "Class", "Class"],
  ["section", "Section", "Section"],
  ["roll", "Roll", "Roll"],
  ["attendanceStatus", "Attendance status", "AttendanceStatus"],
  ["date", "Date", "Date"],
  ["time", "Time", "Time"],
  ["schoolName", "School name", "SchoolName"],
] as const;

function formatDate(value: string | null) {
  if (!value) return "—";
  const date = new Date(value);
  return Number.isNaN(date.getTime())
    ? "—"
    : new Intl.DateTimeFormat(undefined, { dateStyle: "medium" }).format(date);
}

export function SmsTemplatesPage({ embedded = false }: { embedded?: boolean }) {
  const { hasPermission } = usePermissions();
  const canView = hasPermission(Permission.SmsTemplateView);
  const canCreate = hasPermission(Permission.SmsTemplateCreate);
  const canEdit = hasPermission(Permission.SmsTemplateEdit);
  const canDelete = hasPermission(Permission.SmsTemplateDelete);
  const [search, setSearch] = useState("");
  const [activeFilter, setActiveFilter] = useState("");
  const [page, setPage] = useState(1);
  const [formOpen, setFormOpen] = useState(canCreate && !canView);
  const [editingTemplate, setEditingTemplate] = useState<SmsTemplateDto | null>(
    null
  );
  const [previewTemplate, setPreviewTemplate] = useState<SmsTemplateDto | null>(
    null
  );
  const [previewResult, setPreviewResult] = useState<string | null>(null);
  const filters = useMemo(
    () => ({
      pageNumber: page,
      pageSize: PAGE_SIZE,
      searchTerm: search.trim() || undefined,
      isActive: activeFilter === "" ? undefined : activeFilter === "active",
      sortBy: "Name",
      sortDescending: false,
    }),
    [activeFilter, page, search]
  );
  const templatesQuery = useSmsTemplates(filters, canView);
  const createTemplate = useCreateSmsTemplate();
  const updateTemplate = useUpdateSmsTemplate();
  const setActive = useSetSmsTemplateActive();
  const deleteTemplate = useDeleteSmsTemplate();
  const preview = usePreviewSmsTemplate();

  const templateForm = useForm<SmsTemplateFormValues>({
    resolver: zodResolver(smsTemplateSchema),
    defaultValues: { name: "", message: "", isActive: true },
  });
  const placeholderForm = useForm<SmsPlaceholderFormValues>({
    resolver: zodResolver(smsPlaceholderSchema),
    defaultValues: {},
  });
  const messageValue = useWatch({
    control: templateForm.control,
    name: "message",
    defaultValue: "",
  });

  function openCreateForm() {
    setEditingTemplate(null);
    templateForm.reset({ name: "", message: "", isActive: true });
    setFormOpen(true);
  }

  function openEditForm(item: SmsTemplateDto) {
    setEditingTemplate(item);
    templateForm.reset({
      name: item.name,
      message: item.message,
      isActive: item.isActive,
    });
    setFormOpen(true);
  }

  async function saveTemplate(values: SmsTemplateFormValues) {
    try {
      if (editingTemplate) {
        await updateTemplate.mutateAsync({
          id: editingTemplate.id,
          payload: { id: editingTemplate.id, ...values },
        });
        toast.success("SMS template updated.");
      } else {
        await createTemplate.mutateAsync(values);
        toast.success("SMS template created.");
      }
      setFormOpen(false);
      setEditingTemplate(null);
    } catch (error) {
      toast.error(
        error instanceof Error ? error.message : "Unable to save SMS template."
      );
    }
  }

  async function toggleActive(item: SmsTemplateDto) {
    try {
      await setActive.mutateAsync({ id: item.id, isActive: !item.isActive });
      toast.success(item.isActive ? "Template deactivated." : "Template activated.");
    } catch (error) {
      toast.error(
        error instanceof Error ? error.message : "Unable to update template."
      );
    }
  }

  async function removeTemplate(item: SmsTemplateDto) {
    if (!window.confirm(`Delete the "${item.name}" template?`)) return;
    try {
      await deleteTemplate.mutateAsync(item.id);
      toast.success("SMS template deleted.");
    } catch (error) {
      toast.error(
        error instanceof Error ? error.message : "Unable to delete template."
      );
    }
  }

  async function renderPreview(values: SmsPlaceholderFormValues) {
    if (!previewTemplate) return;
    try {
      const result = await preview.mutateAsync({
        id: previewTemplate.id,
        values,
      });
      setPreviewResult(result.renderedMessage);
    } catch (error) {
      toast.error(
        error instanceof Error ? error.message : "Unable to preview template."
      );
    }
  }

  function closePreview() {
    setPreviewTemplate(null);
    setPreviewResult(null);
    placeholderForm.reset({});
  }

  if (!canView && !canCreate) {
    return (
      <Card className="mx-auto max-w-4xl">
        <CardContent className="p-6 text-sm text-destructive">
          You do not have permission to view SMS templates.
        </CardContent>
      </Card>
    );
  }

  const templates = templatesQuery.data?.items ?? [];

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <header className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        {!embedded && (
          <div>
            <h1 className="font-display text-2xl font-semibold">SMS templates</h1>
            <p className="mt-1 text-sm text-muted-foreground">
              Create reusable message templates and preview supported placeholders.
            </p>
          </div>
        )}
        <div className="flex flex-wrap gap-2">
          {!embedded && (
            <Link
              to="/sms/logs"
              className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent"
            >
              SMS logs
            </Link>
          )}
          {canCreate && (
            <Button onClick={openCreateForm}>
              <Plus aria-hidden="true" className="mr-2 h-4 w-4" />
              New template
            </Button>
          )}
        </div>
      </header>

      {formOpen && (
        <Card>
          <CardHeader>
            <CardTitle>
              {editingTemplate ? "Edit SMS template" : "Create SMS template"}
            </CardTitle>
            <CardDescription>
              Message text supports the placeholder tokens shown below. Maximum
              length is 500 characters.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form
              onSubmit={templateForm.handleSubmit(saveTemplate)}
              className="space-y-4"
            >
              <div>
                <label htmlFor="sms-template-name" className="mb-1 block text-sm font-medium">
                  Template name <span className="text-destructive">*</span>
                </label>
                <input
                  id="sms-template-name"
                  {...templateForm.register("name")}
                  maxLength={100}
                  aria-invalid={Boolean(templateForm.formState.errors.name)}
                  className="w-full rounded-md border bg-background px-3 py-2"
                  placeholder="e.g. Attendance alert"
                />
                {templateForm.formState.errors.name && (
                  <p className="mt-1 text-sm text-destructive">
                    {templateForm.formState.errors.name.message}
                  </p>
                )}
              </div>

              <div>
                <label htmlFor="sms-template-message" className="mb-1 block text-sm font-medium">
                  Message <span className="text-destructive">*</span>
                </label>
                <textarea
                  id="sms-template-message"
                  {...templateForm.register("message")}
                  maxLength={500}
                  rows={5}
                  aria-invalid={Boolean(templateForm.formState.errors.message)}
                  className="w-full resize-y rounded-md border bg-background px-3 py-2"
                  placeholder="Dear {{GuardianName}}, {{StudentName}} was marked {{AttendanceStatus}} today."
                />
                <div className="mt-1 flex justify-between gap-2 text-xs text-muted-foreground">
                  {templateForm.formState.errors.message ? (
                    <span className="text-destructive">
                      {templateForm.formState.errors.message.message}
                    </span>
                  ) : (
                    <span>Use exact placeholder spelling, including braces.</span>
                  )}
                  <span>{messageValue.length}/500</span>
                </div>
              </div>

              <div className="rounded-lg border bg-muted/30 p-3">
                <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-muted-foreground">
                  Supported placeholders
                </p>
                <div className="flex flex-wrap gap-2">
                  {PLACEHOLDERS.map(([, , token]) => (
                    <code key={token} className="rounded bg-background px-2 py-1 text-xs">
                      {`{{${token}}}`}
                    </code>
                  ))}
                </div>
              </div>

              <label className="flex w-fit items-center gap-2 text-sm">
                <input
                  type="checkbox"
                  {...templateForm.register("isActive")}
                  className="h-4 w-4 rounded border-input"
                />
                Active and available for use
              </label>

              <div className="flex flex-wrap gap-2">
                <Button
                  type="submit"
                  disabled={createTemplate.isPending || updateTemplate.isPending}
                >
                  {createTemplate.isPending || updateTemplate.isPending ? (
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  ) : (
                    <Check className="mr-2 h-4 w-4" />
                  )}
                  {editingTemplate ? "Save changes" : "Create template"}
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => {
                    setFormOpen(false);
                    setEditingTemplate(null);
                  }}
                >
                  <X aria-hidden="true" className="mr-2 h-4 w-4" />
                  Cancel
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      )}

      {canView && (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <FileText aria-hidden="true" className="h-5 w-5 text-primary" />
            Template library
          </CardTitle>
          <CardDescription>
            {templatesQuery.data
              ? `${templatesQuery.data.totalCount} template${templatesQuery.data.totalCount === 1 ? "" : "s"}`
              : "Search and manage message templates."}
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 sm:grid-cols-[1fr_13rem]">
            <label className="relative">
              <span className="sr-only">Search templates</span>
              <Search aria-hidden="true" className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <input
                value={search}
                onChange={(event) => {
                  setSearch(event.target.value);
                  setPage(1);
                }}
                className="w-full rounded-md border bg-background py-2 pl-9 pr-3"
                placeholder="Search by template name or message"
              />
            </label>
            <label>
              <span className="sr-only">Filter by active status</span>
              <select
                value={activeFilter}
                onChange={(event) => {
                  setActiveFilter(event.target.value);
                  setPage(1);
                }}
                className="w-full rounded-md border bg-background px-3 py-2"
              >
                <option value="">All statuses</option>
                <option value="active">Active</option>
                <option value="inactive">Inactive</option>
              </select>
            </label>
          </div>

          {templatesQuery.isPending ? (
            <div className="flex items-center justify-center gap-2 py-12 text-sm text-muted-foreground">
              <Loader2 aria-hidden="true" className="h-5 w-5 animate-spin" />
              Loading SMS templates…
            </div>
          ) : templatesQuery.isError ? (
            <div className="rounded-lg border border-destructive/30 bg-destructive/5 p-5 text-sm">
              <p className="font-medium text-destructive">Templates could not be loaded.</p>
              <p className="mt-1 text-muted-foreground">
                {templatesQuery.error instanceof Error
                  ? templatesQuery.error.message
                  : "Please try again."}
              </p>
              <Button className="mt-3" variant="outline" onClick={() => void templatesQuery.refetch()}>
                Try again
              </Button>
            </div>
          ) : templates.length === 0 ? (
            <div className="rounded-lg border border-dashed px-5 py-12 text-center">
              <FileText aria-hidden="true" className="mx-auto h-8 w-8 text-muted-foreground" />
              <p className="mt-3 font-medium">No SMS templates found</p>
              <p className="mt-1 text-sm text-muted-foreground">
                {search || activeFilter
                  ? "Try changing the search or status filter."
                  : "Create a template to standardize school messages."}
              </p>
              {canCreate && !search && !activeFilter && (
                <Button className="mt-4" onClick={openCreateForm}>
                  <Plus aria-hidden="true" className="mr-2 h-4 w-4" />
                  Create first template
                </Button>
              )}
            </div>
          ) : (
            <div className="space-y-3">
              {templates.map((item) => (
                <article key={item.id} className="rounded-lg border p-4 transition-colors hover:bg-muted/20">
                  <div className="flex flex-col gap-3 sm:flex-row sm:items-start sm:justify-between">
                    <div className="min-w-0 flex-1">
                      <div className="flex flex-wrap items-center gap-2">
                        <h3 className="font-semibold">{item.name}</h3>
                        <Badge
                          variant="outline"
                          className={
                            item.isActive
                              ? "border-emerald-200 bg-emerald-50 text-emerald-700"
                              : "border-slate-200 bg-slate-50 text-slate-600"
                          }
                        >
                          {item.isActive ? "Active" : "Inactive"}
                        </Badge>
                      </div>
                      <p className="mt-2 whitespace-pre-wrap break-words text-sm text-muted-foreground">
                        {item.message}
                      </p>
                      <p className="mt-2 text-xs text-muted-foreground">
                        Updated {formatDate(item.updatedAt ?? item.createdAt)}
                      </p>
                    </div>
                    <div className="flex flex-wrap gap-2 sm:justify-end">
                      <Button
                        variant="outline"
                        onClick={() => {
                          setPreviewTemplate(item);
                          setPreviewResult(null);
                          placeholderForm.reset({});
                        }}
                      >
                        <Eye aria-hidden="true" className="mr-2 h-4 w-4" />
                        Preview
                      </Button>
                      {canEdit && (
                        <>
                          <Button variant="outline" onClick={() => openEditForm(item)}>
                            <Pencil aria-hidden="true" className="mr-2 h-4 w-4" />
                            Edit
                          </Button>
                          <Button
                            variant="outline"
                            disabled={setActive.isPending}
                            onClick={() => void toggleActive(item)}
                          >
                            {item.isActive ? "Deactivate" : "Activate"}
                          </Button>
                        </>
                      )}
                      {canDelete && (
                        <Button
                          variant="outline"
                          className="text-destructive hover:bg-destructive/10"
                          onClick={() => void removeTemplate(item)}
                          disabled={deleteTemplate.isPending}
                          aria-label={`Delete ${item.name}`}
                        >
                          <Trash2 aria-hidden="true" className="h-4 w-4" />
                        </Button>
                      )}
                    </div>
                  </div>
                </article>
              ))}
            </div>
          )}

          {!templatesQuery.isError && templatesQuery.data && templatesQuery.data.totalPages > 1 && (
            <div className="flex items-center justify-between border-t pt-4">
              <p className="text-sm text-muted-foreground">
                Page {templatesQuery.data.pageNumber} of {templatesQuery.data.totalPages}
              </p>
              <div className="flex gap-2">
                <Button
                  variant="outline"
                  disabled={!templatesQuery.data.hasPreviousPage}
                  onClick={() => setPage((value) => Math.max(1, value - 1))}
                >
                  Previous
                </Button>
                <Button
                  variant="outline"
                  disabled={!templatesQuery.data.hasNextPage}
                  onClick={() => setPage((value) => value + 1)}
                >
                  Next
                </Button>
              </div>
            </div>
          )}
        </CardContent>
      </Card>
      )}

      {previewTemplate && (
        <div
          className="fixed inset-0 z-50 flex items-end justify-center bg-black/50 p-0 sm:items-center sm:p-4"
          role="presentation"
          onMouseDown={(event) => {
            if (event.target === event.currentTarget) closePreview();
          }}
        >
          <section
            role="dialog"
            aria-modal="true"
            aria-labelledby="sms-preview-title"
            className="max-h-[92vh] w-full max-w-3xl overflow-y-auto rounded-t-xl border bg-background shadow-xl sm:rounded-xl"
          >
            <div className="flex items-start justify-between gap-4 border-b p-5">
              <div>
                <h2 id="sms-preview-title" className="text-lg font-semibold">
                  Preview: {previewTemplate.name}
                </h2>
                <p className="mt-1 text-sm text-muted-foreground">
                  Supply sample values to test the backend placeholder rendering.
                </p>
              </div>
              <Button variant="outline" onClick={closePreview} aria-label="Close preview">
                <X aria-hidden="true" className="h-4 w-4" />
              </Button>
            </div>
            <div className="space-y-5 p-5">
              <form
                onSubmit={placeholderForm.handleSubmit(renderPreview)}
                className="space-y-4"
              >
                <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
                  {PLACEHOLDERS.map(([name, label]) => (
                    <div key={name}>
                      <label htmlFor={`preview-${name}`} className="mb-1 block text-sm font-medium">
                        {label}
                      </label>
                      <input
                        id={`preview-${name}`}
                        {...placeholderForm.register(name)}
                        className="w-full rounded-md border bg-background px-3 py-2"
                      />
                    </div>
                  ))}
                </div>
                <Button type="submit" disabled={preview.isPending}>
                  {preview.isPending && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                  Render preview
                </Button>
              </form>
              {previewResult !== null && (
                <div className="rounded-lg border border-primary/20 bg-primary/5 p-4">
                  <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-primary">
                    Rendered message
                  </p>
                  <p className="whitespace-pre-wrap break-words text-sm">{previewResult}</p>
                </div>
              )}
            </div>
          </section>
        </div>
      )}
    </div>
  );
}
