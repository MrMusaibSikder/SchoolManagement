import { useEffect, useState } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { toast } from "sonner";
import {
  BellRing,
  ChevronLeft,
  ChevronRight,
  Loader2,
  Megaphone,
  Plus,
  RefreshCw,
  Search,
  Send,
  X,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { getApiErrorMessage } from "@/lib/api/error-message";
import { Permission } from "@/lib/permissions";
import { NoticeCards } from "../components/NoticeCards";
import { useCreateNotice, useNotices, usePublishNotice } from "../hooks/useNotices";
import {
  createNoticeSchema,
  type CreateNoticeFormValues,
} from "../schemas/notice.schema";
import type { NoticeAudience, NoticeFilters, NoticePriority } from "../types/notice.types";

const PAGE_SIZE = 12;
const PRIORITIES: { value: NoticePriority; label: string }[] = [
  { value: 3, label: "High priority" },
  { value: 2, label: "Medium priority" },
  { value: 1, label: "Low priority" },
];
const AUDIENCES: { value: NoticeAudience; label: string }[] = [
  { value: 1, label: "Students" },
  { value: 2, label: "Teachers" },
  { value: 3, label: "Employees" },
  { value: 4, label: "Guardians" },
  { value: 5, label: "Everyone" },
];

type NoticeStatus = "all" | "published" | "draft" | "archived";

export function NoticesPage() {
  const { hasPermission, isPending: permissionsPending } = usePermissions();
  const canView = hasPermission(Permission.NoticeView);
  const canCreate = hasPermission(Permission.NoticeCreate);
  const canPublish = hasPermission(Permission.NoticePublish);
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [search, setSearch] = useState("");
  const [searchTerm, setSearchTerm] = useState("");
  const [priority, setPriority] = useState("");
  const [audience, setAudience] = useState("");
  const [status, setStatus] = useState<NoticeStatus>("all");
  const [pageNumber, setPageNumber] = useState(1);

  useEffect(() => {
    const timeoutId = window.setTimeout(() => {
      setSearchTerm(search.trim());
      setPageNumber(1);
    }, 300);

    return () => window.clearTimeout(timeoutId);
  }, [search]);

  const filters: NoticeFilters = {
    pageNumber,
    pageSize: PAGE_SIZE,
    ...(searchTerm ? { searchTerm } : {}),
    ...(priority ? { priority: Number(priority) as NoticePriority } : {}),
    ...(audience ? { audience: Number(audience) as NoticeAudience } : {}),
    ...(status === "published"
      ? { isPublished: true, isArchived: false }
      : status === "draft"
        ? { isPublished: false, isArchived: false }
        : status === "archived"
          ? { isArchived: true }
          : {}),
  };
  const { data, isPending, isError, isFetching, refetch } = useNotices(
    filters,
    canView && !permissionsPending
  );
  const createMutation = useCreateNotice();
  const publishMutation = usePublishNotice();
  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<CreateNoticeFormValues>({
    resolver: zodResolver(createNoticeSchema),
    defaultValues: {
      title: "",
      description: "",
      publishDate: getLocalDateTime(),
      expiryDate: "",
      priority: "2",
      audience: "5",
    },
  });

  async function onCreateNotice(values: CreateNoticeFormValues) {
    try {
      await createMutation.mutateAsync({
        title: values.title.trim(),
        description: values.description.trim(),
        publishDate: new Date(values.publishDate).toISOString(),
        expiryDate: values.expiryDate
          ? new Date(values.expiryDate).toISOString()
          : null,
        priority: Number(values.priority) as NoticePriority,
        audience: Number(values.audience) as NoticeAudience,
      });
      toast.success("Notice saved as a draft.");
      reset({ ...values, title: "", description: "", expiryDate: "" });
      setStatus("draft");
      setPageNumber(1);
      setShowCreateForm(false);
    } catch (error) {
      toast.error(getApiErrorMessage(error, "Could not create the notice."));
    }
  }

  async function onPublishNotice(noticeId: number) {
    try {
      await publishMutation.mutateAsync(noticeId);
      toast.success("Notice published.");
    } catch (error) {
      toast.error(getApiErrorMessage(error, "Could not publish the notice."));
    }
  }

  function resetFilters() {
    setSearch("");
    setSearchTerm("");
    setPriority("");
    setAudience("");
    setStatus("all");
    setPageNumber(1);
  }

  function updateFilter(callback: () => void) {
    callback();
    setPageNumber(1);
  }

  if (permissionsPending) {
    return (
      <div className="flex min-h-48 items-center justify-center text-sm text-muted-foreground">
        <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
        Checking access…
      </div>
    );
  }

  if (!canView && !canCreate) {
    return (
      <Card>
        <CardContent className="p-8 text-center">
          <BellRing aria-hidden="true" className="mx-auto h-8 w-8 text-muted-foreground" />
          <h1 className="mt-3 font-semibold">Notice access required</h1>
          <p className="mt-1 text-sm text-muted-foreground">
            Your account does not have permission to view school notices.
          </p>
        </CardContent>
      </Card>
    );
  }

  const totalPages = Math.max(data?.totalPages ?? 0, 1);

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
        <div>
          <p className="text-sm font-medium text-primary">School communications</p>
          <h1 className="mt-1 font-display text-2xl font-semibold">Notice Board</h1>
          <p className="mt-1 text-sm text-muted-foreground">
            Browse school announcements and filter by priority, audience, or status.
          </p>
        </div>
        <div className="flex flex-wrap gap-2">
          {canCreate ? (
            <Button
              type="button"
              onClick={() => setShowCreateForm((visible) => !visible)}
            >
              {showCreateForm ? (
                <X aria-hidden="true" className="mr-2 h-4 w-4" />
              ) : (
                <Plus aria-hidden="true" className="mr-2 h-4 w-4" />
              )}
              {showCreateForm ? "Close form" : "Add notice"}
            </Button>
          ) : null}
          {canView ? (
            <Button
              type="button"
              variant="outline"
              onClick={() => void refetch()}
              disabled={isFetching}
            >
              {isFetching ? (
                <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
              ) : (
                <RefreshCw aria-hidden="true" className="mr-2 h-4 w-4" />
              )}
              Refresh
            </Button>
          ) : null}
        </div>
      </div>

      {showCreateForm ? (
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Megaphone aria-hidden="true" className="h-5 w-5 text-primary" />
              Create notice
            </CardTitle>
            <CardDescription>
              New notices are saved as drafts. Publish them from the notice list when ready.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <form
              noValidate
              onSubmit={handleSubmit(onCreateNotice)}
              className="space-y-5"
            >
              <div className="grid gap-5 md:grid-cols-2">
                <div className="space-y-2 md:col-span-2">
                  <Label htmlFor="notice-title">
                    Title <span className="text-destructive">*</span>
                  </Label>
                  <Input
                    id="notice-title"
                    maxLength={200}
                    aria-invalid={Boolean(errors.title)}
                    aria-describedby={errors.title ? "notice-title-error" : undefined}
                    {...register("title")}
                  />
                  {errors.title ? (
                    <p id="notice-title-error" className="text-sm text-destructive">
                      {errors.title.message}
                    </p>
                  ) : null}
                </div>

                <div className="space-y-2 md:col-span-2">
                  <Label htmlFor="notice-description">
                    Details <span className="text-destructive">*</span>
                  </Label>
                  <textarea
                    id="notice-description"
                    rows={5}
                    maxLength={2000}
                    aria-invalid={Boolean(errors.description)}
                    aria-describedby={
                      errors.description ? "notice-description-error" : undefined
                    }
                    className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
                    {...register("description")}
                  />
                  <div className="flex items-start justify-between gap-3">
                    {errors.description ? (
                      <p id="notice-description-error" className="text-sm text-destructive">
                        {errors.description.message}
                      </p>
                    ) : (
                      <span className="text-xs text-muted-foreground">
                        Maximum 2,000 characters.
                      </span>
                    )}
                  </div>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="notice-publish-date">
                    Publish date <span className="text-destructive">*</span>
                  </Label>
                  <Input
                    id="notice-publish-date"
                    type="datetime-local"
                    aria-invalid={Boolean(errors.publishDate)}
                    {...register("publishDate")}
                  />
                  {errors.publishDate ? (
                    <p className="text-sm text-destructive">
                      {errors.publishDate.message}
                    </p>
                  ) : null}
                </div>

                <div className="space-y-2">
                  <Label htmlFor="notice-expiry-date">Expiry date (optional)</Label>
                  <Input
                    id="notice-expiry-date"
                    type="datetime-local"
                    aria-invalid={Boolean(errors.expiryDate)}
                    {...register("expiryDate")}
                  />
                  {errors.expiryDate ? (
                    <p className="text-sm text-destructive">
                      {errors.expiryDate.message}
                    </p>
                  ) : null}
                </div>

                <div className="space-y-2">
                  <Label htmlFor="notice-priority">
                    Priority <span className="text-destructive">*</span>
                  </Label>
                  <select
                    id="notice-priority"
                    className="h-10 w-full rounded-md border border-input bg-background px-3 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
                    {...register("priority")}
                  >
                    <option value="1">Low</option>
                    <option value="2">Medium</option>
                    <option value="3">High</option>
                  </select>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="notice-audience">
                    Audience <span className="text-destructive">*</span>
                  </Label>
                  <select
                    id="notice-audience"
                    className="h-10 w-full rounded-md border border-input bg-background px-3 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
                    {...register("audience")}
                  >
                    {AUDIENCES.map((item) => (
                      <option key={item.value} value={item.value}>
                        {item.label}
                      </option>
                    ))}
                  </select>
                </div>
              </div>

              <div className="flex flex-col-reverse gap-2 border-t pt-4 sm:flex-row sm:justify-end">
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => {
                    reset();
                    setShowCreateForm(false);
                  }}
                  disabled={isSubmitting}
                >
                  Cancel
                </Button>
                <Button
                  type="submit"
                  disabled={isSubmitting || createMutation.isPending}
                >
                  {isSubmitting || createMutation.isPending ? (
                    <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
                  ) : (
                    <Plus aria-hidden="true" className="mr-2 h-4 w-4" />
                  )}
                  Save as draft
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      ) : null}

      {!canView ? (
        <Card>
          <CardContent className="p-6 text-sm text-muted-foreground">
            Notice list access requires the Notice.View permission. You can still
            create a draft notice using the Add notice button.
          </CardContent>
        </Card>
      ) : (
        <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <BellRing aria-hidden="true" className="h-5 w-5 text-primary" />
            All notices
          </CardTitle>
          <CardDescription>
            {typeof data?.totalCount === "number"
              ? `${data.totalCount.toLocaleString("en-BD")} notice${data.totalCount === 1 ? "" : "s"}`
              : "Search and filter the notice board."}
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-5">
          <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-4">
            <div className="relative sm:col-span-2 lg:col-span-1">
              <Search
                aria-hidden="true"
                className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground"
              />
              <Input
                value={search}
                onChange={(event) => setSearch(event.target.value)}
                placeholder="Search notices"
                aria-label="Search notices"
                className="pl-9 pr-9"
              />
              {search ? (
                <button
                  type="button"
                  aria-label="Clear notice search"
                  onClick={() => setSearch("")}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-muted-foreground hover:text-foreground"
                >
                  <X aria-hidden="true" className="h-4 w-4" />
                </button>
              ) : null}
            </div>
            <select
              aria-label="Filter notices by priority"
              value={priority}
              onChange={(event) =>
                updateFilter(() => setPriority(event.target.value))
              }
              className="h-10 rounded-md border border-input bg-background px-3 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
            >
              <option value="">All priorities</option>
              {PRIORITIES.map((item) => (
                <option key={item.value} value={item.value}>
                  {item.label}
                </option>
              ))}
            </select>
            <select
              aria-label="Filter notices by audience"
              value={audience}
              onChange={(event) =>
                updateFilter(() => setAudience(event.target.value))
              }
              className="h-10 rounded-md border border-input bg-background px-3 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
            >
              <option value="">All audiences</option>
              {AUDIENCES.map((item) => (
                <option key={item.value} value={item.value}>
                  {item.label}
                </option>
              ))}
            </select>
            <select
              aria-label="Filter notices by status"
              value={status}
              onChange={(event) =>
                updateFilter(() => setStatus(event.target.value as NoticeStatus))
              }
              className="h-10 rounded-md border border-input bg-background px-3 text-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
            >
              <option value="all">All statuses</option>
              <option value="published">Published</option>
              <option value="draft">Draft</option>
              <option value="archived">Archived</option>
            </select>
          </div>

          <div className="flex flex-wrap items-center justify-between gap-3">
            <p className="text-sm text-muted-foreground">
              {data ? `Page ${data.pageNumber} of ${totalPages}` : " "}
            </p>
            <Button
              type="button"
              variant="outline"
              onClick={resetFilters}
              disabled={!search && !priority && !audience && status === "all"}
            >
              Clear filters
            </Button>
          </div>

          {isPending ? (
            <div
              aria-label="Loading notices"
              className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3"
            >
              {Array.from({ length: 6 }, (_, index) => (
                <div
                  key={index}
                  aria-hidden="true"
                  className="h-44 animate-pulse rounded-xl border bg-muted/50"
                />
              ))}
            </div>
          ) : null}

          {isError ? (
            <div
              role="alert"
              className="flex flex-col gap-3 rounded-lg border border-destructive/20 bg-destructive/5 p-4 sm:flex-row sm:items-center sm:justify-between"
            >
              <p className="text-sm text-destructive">
                Notices could not be loaded. Please try again.
              </p>
              <Button
                type="button"
                variant="outline"
                onClick={() => void refetch()}
                disabled={isFetching}
              >
                <RefreshCw
                  aria-hidden="true"
                  className={`mr-2 h-4 w-4 ${isFetching ? "animate-spin" : ""}`}
                />
                Try again
              </Button>
            </div>
          ) : null}

          {!isPending && !isError && (data?.items.length ?? 0) === 0 ? (
            <div className="rounded-xl border border-dashed p-10 text-center">
              <BellRing
                aria-hidden="true"
                className="mx-auto h-9 w-9 text-muted-foreground/60"
              />
              <p className="mt-3 font-medium">No notices found</p>
              <p className="mt-1 text-sm text-muted-foreground">
                Try changing your search or filters.
              </p>
            </div>
          ) : null}

          {!isPending && !isError && (data?.items.length ?? 0) > 0 ? (
            <>
              <NoticeCards
                notices={data?.items ?? []}
                showStatus
                renderActions={
                  canPublish
                    ? (notice) =>
                        !notice.isPublished && !notice.isArchived ? (
                          <Button
                            type="button"
                            variant="outline"
                            className="border-primary/30 text-primary hover:bg-primary/10"
                            disabled={publishMutation.isPending}
                            onClick={() => void onPublishNotice(notice.id)}
                          >
                            {publishMutation.isPending ? (
                              <Loader2
                                aria-hidden="true"
                                className="mr-2 h-4 w-4 animate-spin"
                              />
                            ) : (
                              <Send aria-hidden="true" className="mr-2 h-4 w-4" />
                            )}
                            Publish
                          </Button>
                        ) : null
                    : undefined
                }
              />
              <div className="flex items-center justify-between gap-3 border-t pt-4">
                <p className="text-sm text-muted-foreground">
                  {data?.totalCount.toLocaleString("en-BD")} notices
                </p>
                <div className="flex gap-2">
                  <Button
                    type="button"
                    variant="outline"
                    className="h-9 w-9 px-0 py-0"
                    aria-label="Previous notice page"
                    disabled={!data?.hasPreviousPage || isFetching}
                    onClick={() => setPageNumber((value) => Math.max(1, value - 1))}
                  >
                    <ChevronLeft aria-hidden="true" className="h-4 w-4" />
                  </Button>
                  <Button
                    type="button"
                    variant="outline"
                    className="h-9 w-9 px-0 py-0"
                    aria-label="Next notice page"
                    disabled={!data?.hasNextPage || isFetching}
                    onClick={() => setPageNumber((value) => value + 1)}
                  >
                    <ChevronRight aria-hidden="true" className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            </>
          ) : null}
        </CardContent>
        </Card>
      )}
    </div>
  );
}

function getLocalDateTime(): string {
  const date = new Date();
  const localDate = new Date(date.getTime() - date.getTimezoneOffset() * 60_000);
  return localDate.toISOString().slice(0, 16);
}
