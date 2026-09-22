import { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Loader2, Pencil, Plus, Search, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { MediaImage } from "@/components/common/MediaImage";
import { PermissionGuard } from "@/features/auth/components/PermissionGuard";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import {
  useDeleteEmployee,
  useDesignations,
  useEmployees,
} from "../hooks/useEmployeeData";

export function EmployeesPage() {
  const navigate = useNavigate();
  const { hasPermission } = usePermissions();
  const { data = [], isPending, isError, refetch } = useEmployees();
  const { data: designations = [] } = useDesignations();
  const deleteEmployeeMutation = useDeleteEmployee();
  const [search, setSearch] = useState("");
  const [status, setStatus] = useState("all");
  const [page, setPage] = useState(1);
  const pageSize = 8;

  const designationName = useMemo(() => {
    const map = new Map(designations.map((item) => [item.id, item.name]));
    return (id: number) => map.get(id) ?? `Designation #${id}`;
  }, [designations]);

  const filtered = useMemo(() => {
    const query = search.trim().toLowerCase();
    return data.filter((item) => {
      const matchesSearch =
        !query ||
        [item.fullName, item.employeeCode, item.phone, item.email ?? ""]
          .join(" ")
          .toLowerCase()
          .includes(query);
      const matchesStatus =
        status === "all"
          ? true
          : status === "active"
            ? item.isActive
            : !item.isActive;
      return matchesSearch && matchesStatus;
    });
  }, [data, search, status]);

  const totalPages = Math.max(1, Math.ceil(filtered.length / pageSize));
  const paged = filtered.slice((page - 1) * pageSize, page * pageSize);

  async function handleDelete(id: number, name: string) {
    if (!window.confirm(`Delete employee “${name}”? This cannot be undone.`)) {
      return;
    }
    try {
      await deleteEmployeeMutation.mutateAsync(id);
      toast.success("Employee deleted.");
    } catch {
      toast.error("Could not delete this employee.");
    }
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Employees</h1>
          <p className="text-sm text-muted-foreground">
            Manage staff profiles, designations, and photo records.
          </p>
        </div>
        <PermissionGuard permission={Permission.EmployeeCreate}>
          <Button onClick={() => navigate("/employees/new")}>
            <Plus className="mr-2 h-4 w-4" />
            Add Employee
          </Button>
        </PermissionGuard>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Employee directory</CardTitle>
          <CardDescription>
            Search by name, code, phone, or email. Photos load from the school file store.
          </CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid gap-3 md:grid-cols-[1.4fr_0.6fr]">
            <div className="relative">
              <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
              <input
                value={search}
                onChange={(event) => {
                  setSearch(event.target.value);
                  setPage(1);
                }}
                placeholder="Search employees"
                className="w-full rounded-md border bg-background py-2 pl-9 pr-3"
              />
            </div>
            <select
              value={status}
              onChange={(event) => {
                setStatus(event.target.value);
                setPage(1);
              }}
              className="rounded-md border bg-background px-3 py-2"
            >
              <option value="all">All status</option>
              <option value="active">Active</option>
              <option value="inactive">Inactive</option>
            </select>
          </div>

          {isPending ? (
            <div className="flex items-center justify-center py-8 text-sm text-muted-foreground">
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              Loading employees…
            </div>
          ) : null}

          {isError ? (
            <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">
              Unable to load employees.{" "}
              <button type="button" className="underline" onClick={() => void refetch()}>
                Try again
              </button>
            </div>
          ) : null}

          {!isPending && !isError && filtered.length === 0 ? (
            <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">
              No employees match the current search.
            </div>
          ) : null}

          <div className="space-y-2">
            {paged.map((employee) => (
              <div
                key={employee.id}
                className="flex flex-col gap-3 rounded-lg border p-4 md:flex-row md:items-center md:justify-between"
              >
                <div className="flex items-center gap-3">
                  <div className="flex h-12 w-12 overflow-hidden rounded-full border bg-muted">
                    <MediaImage src={employee.employeePhoto} alt={employee.fullName} />
                  </div>
                  <div>
                    <p className="font-medium">{employee.fullName}</p>
                    <p className="text-sm text-muted-foreground">
                      {employee.employeeCode} • {designationName(employee.designationId)} • {employee.phone}
                    </p>
                  </div>
                </div>

                <div className="flex flex-wrap items-center gap-2">
                  <span
                    className={`rounded-full px-2.5 py-1 text-xs font-medium ${
                      employee.isActive
                        ? "bg-emerald-100 text-emerald-700 dark:bg-emerald-500/15 dark:text-emerald-300"
                        : "bg-muted text-muted-foreground"
                    }`}
                  >
                    {employee.isActive ? "Active" : "Inactive"}
                  </span>
                  <Button
                    type="button"
                    variant="outline"
                    onClick={() => navigate(`/employees/${employee.id}`)}
                  >
                    View
                  </Button>
                  {hasPermission(Permission.EmployeeEdit) ? (
                    <Button
                      type="button"
                      variant="outline"
                      aria-label={`Edit ${employee.fullName}`}
                      onClick={() => navigate(`/employees/${employee.id}/edit`)}
                    >
                      <Pencil className="h-4 w-4" />
                    </Button>
                  ) : null}
                  {hasPermission(Permission.EmployeeDelete) ? (
                    <Button
                      type="button"
                      variant="outline"
                      aria-label={`Delete ${employee.fullName}`}
                      disabled={deleteEmployeeMutation.isPending}
                      onClick={() => void handleDelete(employee.id, employee.fullName)}
                    >
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  ) : null}
                </div>
              </div>
            ))}
          </div>

          {filtered.length > 0 ? (
            <div className="flex items-center justify-between gap-3 pt-2">
              <p className="text-sm text-muted-foreground">
                Page {page} of {totalPages} • {filtered.length} records
              </p>
              <div className="flex gap-2">
                <Button
                  type="button"
                  variant="outline"
                  disabled={page === 1}
                  onClick={() => setPage((value) => Math.max(1, value - 1))}
                >
                  Previous
                </Button>
                <Button
                  type="button"
                  variant="outline"
                  disabled={page === totalPages}
                  onClick={() => setPage((value) => Math.min(totalPages, value + 1))}
                >
                  Next
                </Button>
              </div>
            </div>
          ) : null}
        </CardContent>
      </Card>
    </div>
  );
}
