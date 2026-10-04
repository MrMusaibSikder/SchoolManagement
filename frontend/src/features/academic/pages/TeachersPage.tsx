import { useEffect, useMemo, useRef, useState } from "react";
import { Link } from "react-router-dom";
import { toast } from "sonner";
import { Loader2, Pencil, Plus, Search, Trash2, UserRound, X } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { useEmployees } from "@/features/employee/hooks/useEmployeeData";
import { getApiErrorMessage } from "@/lib/api/error-message";
import { Permission } from "@/lib/permissions";
import {
  useCreateTeacher,
  useDeleteTeacher,
  useTeachers,
  useUpdateTeacher,
} from "../hooks/useAcademicData";
import type { TeacherDto } from "../types/academic.types";
import type { EmployeeDto } from "@/features/employee/types/employee.types";

const EMPTY_TEACHERS: TeacherDto[] = [];
const EMPTY_EMPLOYEES: EmployeeDto[] = [];

export function TeachersPage() {
  const { hasPermission } = usePermissions();
  const canViewTeachers = hasPermission(Permission.TeacherView);
  const canCreateTeacher = hasPermission(Permission.TeacherCreate);
  const canEditTeacher = hasPermission(Permission.TeacherEdit);
  const canDeleteTeacher = hasPermission(Permission.TeacherDelete);
  const canViewEmployees = hasPermission(Permission.EmployeeView);

  const teachersQuery = useTeachers();
  const employeesQuery = useEmployees(canViewEmployees);
  const createTeacherMutation = useCreateTeacher();
  const updateTeacherMutation = useUpdateTeacher();
  const deleteTeacherMutation = useDeleteTeacher();
  const [search, setSearch] = useState("");
  const [employeeSearch, setEmployeeSearch] = useState("");
  const [employeeId, setEmployeeId] = useState<number | null>(null);
  const [employeePickerOpen, setEmployeePickerOpen] = useState(false);
  const [qualification, setQualification] = useState("");
  const [specialization, setSpecialization] = useState("");
  const [editingId, setEditingId] = useState<number | null>(null);
  const employeeSearchRef = useRef<HTMLInputElement>(null);
  const lastTeachersErrorAt = useRef(0);
  const lastEmployeesErrorAt = useRef(0);

  const teachers = teachersQuery.data ?? EMPTY_TEACHERS;
  const employees = employeesQuery.data ?? EMPTY_EMPLOYEES;
  const employeeById = useMemo(
    () => new Map(employees.map((employee) => [employee.id, employee])),
    [employees]
  );

  const filteredEmployees = useMemo(() => {
    const query = employeeSearch.trim().toLocaleLowerCase();
    return [...employees]
      .filter((employee) => {
        return (
          !query ||
          [employee.fullName, employee.employeeCode, employee.phone, employee.email ?? ""]
            .join(" ")
            .toLocaleLowerCase()
            .includes(query)
        );
      })
      .sort((left, right) => left.fullName.localeCompare(right.fullName))
      .slice(0, 50);
  }, [employeeSearch, employees]);

  const filteredTeachers = useMemo(() => {
    const query = search.trim().toLocaleLowerCase();
    return teachers.filter((teacher) => {
      const employee = employeeById.get(teacher.employeeId);
      return [
        employee?.fullName ?? "",
        employee?.employeeCode ?? "",
        employee?.phone ?? "",
        teacher.employeeId,
        teacher.qualification ?? "",
        teacher.specialization ?? "",
      ]
        .join(" ")
        .toLocaleLowerCase()
        .includes(query);
    });
  }, [employeeById, search, teachers]);

  const selectedEmployee = employeeId === null ? undefined : employeeById.get(employeeId);
  const isSaving =
    createTeacherMutation.isPending || updateTeacherMutation.isPending;

  useEffect(() => {
    if (
      canViewTeachers &&
      teachersQuery.isError &&
      teachersQuery.errorUpdatedAt > 0 &&
      lastTeachersErrorAt.current !== teachersQuery.errorUpdatedAt
    ) {
      lastTeachersErrorAt.current = teachersQuery.errorUpdatedAt;
      toast.error(
        getApiErrorMessage(
          teachersQuery.error,
          "Unable to load teachers. Please try again."
        )
      );
    }
  }, [
    canViewTeachers,
    teachersQuery.error,
    teachersQuery.errorUpdatedAt,
    teachersQuery.isError,
  ]);

  useEffect(() => {
    if (
      canViewEmployees &&
      employeesQuery.isError &&
      employeesQuery.errorUpdatedAt > 0 &&
      lastEmployeesErrorAt.current !== employeesQuery.errorUpdatedAt
    ) {
      lastEmployeesErrorAt.current = employeesQuery.errorUpdatedAt;
      toast.error(
        getApiErrorMessage(
          employeesQuery.error,
          "Unable to load employee names. Please try again."
        )
      );
    }
  }, [
    canViewEmployees,
    employeesQuery.error,
    employeesQuery.errorUpdatedAt,
    employeesQuery.isError,
  ]);

  useEffect(() => {
    function closePicker(event: PointerEvent) {
      if (!employeeSearchRef.current?.parentElement?.contains(event.target as Node)) {
        setEmployeePickerOpen(false);
      }
    }
    document.addEventListener("pointerdown", closePicker);
    return () => document.removeEventListener("pointerdown", closePicker);
  }, []);

  function resetForm() {
    setEmployeeId(null);
    setEmployeeSearch("");
    setQualification("");
    setSpecialization("");
    setEditingId(null);
    setEmployeePickerOpen(false);
  }

  function selectEmployee(employee: EmployeeDto) {
    setEmployeeId(employee.id);
    setEmployeeSearch(employee.fullName);
    setEmployeePickerOpen(false);
  }

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!employeeId) {
      toast.error("Search for and select an employee before saving.");
      employeeSearchRef.current?.focus();
      return;
    }

    const payload = {
      employeeId,
      qualification: qualification.trim(),
      specialization: specialization.trim(),
    };
    try {
      if (editingId !== null) {
        await updateTeacherMutation.mutateAsync({
          id: editingId,
          payload: { ...payload, id: editingId },
        });
        toast.success("Teacher updated successfully.");
      } else {
        await createTeacherMutation.mutateAsync(payload);
        toast.success("Teacher created successfully.");
      }
      resetForm();
    } catch (error) {
      toast.error(
        getApiErrorMessage(
          error,
          `Unable to ${editingId !== null ? "update" : "create"} teacher. Please try again.`
        )
      );
    }
  }

  function startEdit(teacher: TeacherDto) {
    const employee = employeeById.get(teacher.employeeId);
    setEditingId(teacher.id);
    setEmployeeId(teacher.employeeId);
    setEmployeeSearch(employee?.fullName ?? "");
    setQualification(teacher.qualification ?? "");
    setSpecialization(teacher.specialization ?? "");
    setEmployeePickerOpen(false);
  }

  async function handleDelete(teacher: TeacherDto) {
    try {
      await deleteTeacherMutation.mutateAsync(teacher.id);
      toast.success("Teacher deleted successfully.");
      if (editingId === teacher.id) resetForm();
    } catch (error) {
      toast.error(
        getApiErrorMessage(error, "Unable to delete teacher. Please try again.")
      );
    }
  }

  function teacherName(teacher: TeacherDto) {
    const employee = employeeById.get(teacher.employeeId);
    return employee?.fullName ?? `Employee #${teacher.employeeId}`;
  }

  if (!canViewTeachers) {
    return (
      <Card>
        <CardContent className="p-6 text-sm text-destructive">
          You do not have permission to view teachers.
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Teachers</h1>
          <p className="text-sm text-muted-foreground">
            Find teachers by their name and manage their qualifications.
          </p>
        </div>
        <Link
          to="/academic"
          className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent"
        >
          Back to overview
        </Link>
      </div>

      <div className="grid gap-6 lg:grid-cols-[0.9fr_1.1fr]">
        {(canCreateTeacher || canEditTeacher) && (
          <Card>
            <CardHeader>
              <CardTitle>
                {editingId !== null ? "Edit teacher" : "Create teacher"}
              </CardTitle>
              <CardDescription>
                Search by employee name and select the matching employee.
              </CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={handleSubmit} className="space-y-4">
                <div className="space-y-1.5">
                  <label
                    htmlFor="teacher-employee-search"
                    className="text-sm font-medium"
                  >
                    Employee <span className="text-destructive">*</span>
                  </label>
                  {!canViewEmployees ? (
                    <div className="rounded-md border border-amber-500/30 bg-amber-500/5 p-3 text-sm text-muted-foreground">
                      Employee name search requires Employee.View access. Ask
                      your administrator to grant it.
                      {editingId !== null && employeeId !== null && (
                        <p className="mt-1 font-medium text-foreground">
                          Current employee ID: {employeeId}
                        </p>
                      )}
                    </div>
                  ) : employeesQuery.isPending ? (
                    <div className="flex items-center gap-2 rounded-md border px-3 py-2 text-sm text-muted-foreground">
                      <Loader2
                        aria-hidden="true"
                        className="h-4 w-4 animate-spin"
                      />
                      Loading employees…
                    </div>
                  ) : employeesQuery.isError ? (
                    <div className="flex flex-col gap-2 rounded-md border border-destructive/30 p-3 text-sm text-destructive">
                      <p>Employee names could not be loaded.</p>
                      <Button
                        type="button"
                        variant="outline"
                        className="self-start"
                        onClick={() => void employeesQuery.refetch()}
                      >
                        Try again
                      </Button>
                    </div>
                  ) : (
                    <div className="relative">
                      <Search
                        aria-hidden="true"
                        className="pointer-events-none absolute left-3 top-1/2 z-10 h-4 w-4 -translate-y-1/2 text-muted-foreground"
                      />
                      <input
                        id="teacher-employee-search"
                        ref={employeeSearchRef}
                        type="search"
                        autoComplete="off"
                        role="combobox"
                        aria-autocomplete="list"
                        aria-expanded={employeePickerOpen}
                        aria-controls="teacher-employee-options"
                        aria-label="Search employees by name"
                        placeholder="Type an employee name or code"
                        value={employeeSearch}
                        onFocus={() => setEmployeePickerOpen(true)}
                        onChange={(event) => {
                          setEmployeeSearch(event.target.value);
                          setEmployeeId(null);
                          setEmployeePickerOpen(true);
                        }}
                        onKeyDown={(event) => {
                          if (event.key === "Escape") {
                            setEmployeePickerOpen(false);
                          }
                        }}
                        className="w-full rounded-md border bg-background py-2 pl-9 pr-10"
                      />
                      {employeeSearch && (
                        <button
                          type="button"
                          aria-label="Clear employee selection"
                          onClick={() => {
                            setEmployeeId(null);
                            setEmployeeSearch("");
                            setEmployeePickerOpen(true);
                            employeeSearchRef.current?.focus();
                          }}
                          className="absolute right-2 top-1/2 z-10 -translate-y-1/2 rounded p-1 text-muted-foreground hover:bg-accent hover:text-foreground"
                        >
                          <X aria-hidden="true" className="h-4 w-4" />
                        </button>
                      )}
                      {employeePickerOpen && (
                        <ul
                          id="teacher-employee-options"
                          role="listbox"
                          className="absolute z-30 mt-1 max-h-64 w-full overflow-y-auto rounded-md border bg-popover p-1 text-popover-foreground shadow-lg"
                        >
                          {filteredEmployees.length === 0 ? (
                            <li className="px-3 py-2 text-sm text-muted-foreground">
                              {employees.length === 0
                                ? "No employees are available."
                                : "No matching employees found."}
                            </li>
                          ) : (
                            filteredEmployees.map((employee) => (
                              <li key={employee.id} role="presentation">
                                <button
                                  type="button"
                                  role="option"
                                  aria-selected={employee.id === employeeId}
                                  onClick={() => selectEmployee(employee)}
                                  className="flex w-full items-center gap-3 rounded px-3 py-2 text-left hover:bg-accent focus-visible:bg-accent focus-visible:outline-none"
                                >
                                  <UserRound
                                    aria-hidden="true"
                                    className="h-4 w-4 shrink-0 text-muted-foreground"
                                  />
                                  <span className="min-w-0 flex-1">
                                    <span className="block truncate text-sm font-medium">
                                      {employee.fullName}
                                    </span>
                                    <span className="block truncate text-xs text-muted-foreground">
                                      {employee.employeeCode} · {employee.phone}
                                      {!employee.isActive && " · Inactive"}
                                    </span>
                                  </span>
                                </button>
                              </li>
                            ))
                          )}
                        </ul>
                      )}
                    </div>
                  )}
                  {canViewEmployees && selectedEmployee && (
                    <p className="text-xs text-muted-foreground">
                      Selected employee ID: {selectedEmployee.id}
                    </p>
                  )}
                </div>

                <div className="space-y-1.5">
                  <label
                    htmlFor="teacher-qualification"
                    className="text-sm font-medium"
                  >
                    Qualification
                  </label>
                  <input
                    id="teacher-qualification"
                    value={qualification}
                    onChange={(event) => setQualification(event.target.value)}
                    maxLength={200}
                    className="w-full rounded-md border bg-background px-3 py-2"
                  />
                </div>
                <div className="space-y-1.5">
                  <label
                    htmlFor="teacher-specialization"
                    className="text-sm font-medium"
                  >
                    Specialization
                  </label>
                  <input
                    id="teacher-specialization"
                    value={specialization}
                    onChange={(event) => setSpecialization(event.target.value)}
                    maxLength={200}
                    className="w-full rounded-md border bg-background px-3 py-2"
                  />
                </div>

                <div className="flex flex-wrap gap-2">
                  <Button
                    type="submit"
                    disabled={
                      isSaving ||
                      (editingId === null
                        ? !canCreateTeacher || !canViewEmployees
                        : !canEditTeacher) ||
                      (canViewEmployees && employeesQuery.isPending) ||
                      (canViewEmployees && employeesQuery.isError)
                    }
                  >
                    {isSaving ? (
                      <Loader2
                        aria-hidden="true"
                        className="mr-2 h-4 w-4 animate-spin"
                      />
                    ) : (
                      <Plus aria-hidden="true" className="mr-2 h-4 w-4" />
                    )}
                    {editingId !== null ? "Save changes" : "Create teacher"}
                  </Button>
                  {editingId !== null && (
                    <Button
                      type="button"
                      variant="outline"
                      onClick={resetForm}
                    >
                      Cancel
                    </Button>
                  )}
                </div>
              </form>
            </CardContent>
          </Card>
        )}

        <Card>
          <CardHeader>
            <CardTitle>Teachers list</CardTitle>
            <CardDescription>
              Search and manage teachers by name.
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="relative">
              <Search
                aria-hidden="true"
                className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground"
              />
              <input
                value={search}
                onChange={(event) => setSearch(event.target.value)}
                placeholder="Search teachers by name or qualification"
                aria-label="Search teachers"
                className="w-full rounded-md border bg-background py-2 pl-9 pr-3"
              />
            </div>
            {teachersQuery.isPending && (
              <div className="flex items-center justify-center py-8 text-sm text-muted-foreground">
                <Loader2
                  aria-hidden="true"
                  className="mr-2 h-4 w-4 animate-spin"
                />
                Loading teachers…
              </div>
            )}
            {teachersQuery.isError && (
              <div className="flex flex-col gap-3 rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive sm:flex-row sm:items-center sm:justify-between">
                <p>
                  {getApiErrorMessage(
                    teachersQuery.error,
                    "Unable to load teachers. Please try again."
                  )}
                </p>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => void teachersQuery.refetch()}
                >
                  Try again
                </Button>
              </div>
            )}
            {canViewEmployees && employeesQuery.isError && (
              <div className="flex flex-col gap-3 rounded-lg border border-amber-500/30 bg-amber-500/5 p-4 text-sm sm:flex-row sm:items-center sm:justify-between">
                <p>
                  Employee names are unavailable. Grant Employee.View access or
                  retry to display teacher names.
                </p>
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => void employeesQuery.refetch()}
                >
                  Retry names
                </Button>
              </div>
            )}
            {!teachersQuery.isPending &&
              !teachersQuery.isError &&
              filteredTeachers.length === 0 && (
                <div className="rounded-lg border border-dashed p-6 text-center text-sm text-muted-foreground">
                  No teachers found.
                </div>
              )}
            <div className="space-y-2">
              {filteredTeachers.map((teacher) => (
                <div
                  key={teacher.id}
                  className="flex flex-col gap-3 rounded-lg border p-4 sm:flex-row sm:items-center sm:justify-between"
                >
                  <div className="min-w-0">
                    <p className="truncate font-medium">
                      {teacherName(teacher)}
                    </p>
                    <p className="text-xs text-muted-foreground">
                      Employee ID: {teacher.employeeId}
                    </p>
                    <p className="text-sm text-muted-foreground">
                      {teacher.qualification ?? "Qualification pending"} ·{" "}
                      {teacher.specialization ?? "Specialization pending"}
                    </p>
                  </div>
                  <div className="flex shrink-0 gap-2">
                    {canEditTeacher && (
                      <Button
                        type="button"
                        variant="outline"
                        className="h-9 w-9 p-0"
                        aria-label={`Edit ${teacherName(teacher)}`}
                        onClick={() => startEdit(teacher)}
                      >
                        <Pencil aria-hidden="true" className="h-4 w-4" />
                      </Button>
                    )}
                    {canDeleteTeacher && (
                      <Button
                        type="button"
                        variant="outline"
                        className="h-9 w-9 p-0"
                        aria-label={`Delete ${teacherName(teacher)}`}
                        disabled={deleteTeacherMutation.isPending}
                        onClick={() => void handleDelete(teacher)}
                      >
                        {deleteTeacherMutation.isPending &&
                        deleteTeacherMutation.variables === teacher.id ? (
                          <Loader2
                            aria-hidden="true"
                            className="h-4 w-4 animate-spin"
                          />
                        ) : (
                          <Trash2 aria-hidden="true" className="h-4 w-4" />
                        )}
                      </Button>
                    )}
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
