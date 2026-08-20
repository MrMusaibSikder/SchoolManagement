import { useMemo, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Loader2, Pencil, Plus, Search, Trash2, UserRound } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useDeleteEmployee, useEmployees } from "../hooks/useEmployeeData";

export function EmployeesPage() {
  const navigate = useNavigate();
  const { data = [], isPending, isError } = useEmployees();
  const deleteEmployeeMutation = useDeleteEmployee();
  const [search, setSearch] = useState("");

  const filtered = useMemo(() => {
    const query = search.trim().toLowerCase();
    if (!query) return data;

    return data.filter((item) =>
      [item.fullName, item.employeeCode, item.phone, item.email ?? ""].join(" ").toLowerCase().includes(query)
    );
  }, [data, search]);

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Employees</h1>
          <p className="text-sm text-muted-foreground">Manage employee profiles, contact information, and photo uploads.</p>
        </div>
        <Button onClick={() => navigate("/employees/new")}>
          <Plus className="mr-2 h-4 w-4" />
          Add Employee
        </Button>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Employee directory</CardTitle>
          <CardDescription>Use the search field below to quickly locate employee records.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="relative">
            <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <input
              value={search}
              onChange={(event) => setSearch(event.target.value)}
              placeholder="Search by name, code, phone, or email"
              className="w-full rounded-md border bg-background py-2 pl-9 pr-3"
            />
          </div>

          {isPending ? (
            <div className="flex items-center justify-center py-8 text-sm text-muted-foreground">
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              Loading employees…
            </div>
          ) : null}

          {isError ? (
            <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">
              Unable to load employees.
            </div>
          ) : null}

          {!isPending && !isError && filtered.length === 0 ? (
            <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">
              No employees match the current search.
            </div>
          ) : null}

          <div className="space-y-2">
            {filtered.map((employee) => (
              <div key={employee.id} className="flex flex-col gap-3 rounded-lg border p-4 md:flex-row md:items-center md:justify-between">
                <div className="flex items-center gap-3">
                  <div className="flex h-11 w-11 items-center justify-center overflow-hidden rounded-full bg-muted text-muted-foreground">
                    {employee.employeePhoto ? (
                      <img src={employee.employeePhoto} alt={employee.fullName} className="h-full w-full object-cover" />
                    ) : (
                      <UserRound className="h-5 w-5" />
                    )}
                  </div>
                  <div>
                    <p className="font-medium">{employee.fullName}</p>
                    <p className="text-sm text-muted-foreground">
                      {employee.employeeCode} • {employee.phone}
                    </p>
                  </div>
                </div>

                <div className="flex flex-wrap items-center gap-2">
                  <span className={`rounded-full px-2.5 py-1 text-xs font-medium ${employee.isActive ? "bg-emerald-100 text-emerald-700" : "bg-muted text-muted-foreground"}`}>
                    {employee.isActive ? "Active" : "Inactive"}
                  </span>
                  <Button type="button" variant="outline" onClick={() => navigate(`/employees/${employee.id}`)}>
                   View
                  </Button>
                  <Button type="button" variant="outline" onClick={() => navigate(`/employees/${employee.id}/edit`)}>
                   <Pencil className="h-4 w-4" />
                  </Button>
                  <Button type="button" variant="outline" onClick={() => deleteEmployeeMutation.mutate(employee.id)}>
                   <Trash2 className="h-4 w-4" />
                  </Button>
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
