import { useEffect, useRef, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Controller, useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { CalendarDays, Loader2, Plus } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { ImageUploader } from "@/components/common/ImageUploader";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import {
  useCreateEmployee,
  useDesignations,
  useEmployee,
  useUpdateEmployee,
  useUsersLookup,
} from "../hooks/useEmployeeData";
import {
  employeeFormSchema,
  type EmployeeFormValues,
} from "../schemas/employee.schema";

function getLocalDateValue(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}

function withEmployeeCodePrefix(employeeCode: string | null | undefined): string {
  const code = employeeCode?.trim() ?? "";
  return `EMP-${code.replace(/^EMP-/i, "")}`;
}

export function EmployeeFormPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const isEditing = Boolean(id);
  const employeeId = isEditing ? Number(id) : null;
  const { hasPermission } = usePermissions();
  const { data: employee, isPending: isEmployeePending } = useEmployee(employeeId);
  const { data: designations = [], isPending: designationsPending } = useDesignations();
  const { data: users = [] } = useUsersLookup();
  const createEmployeeMutation = useCreateEmployee();
  const updateEmployeeMutation = useUpdateEmployee();
  const [photoFile, setPhotoFile] = useState<File | null>(null);
  const [photoError, setPhotoError] = useState<string | undefined>();
  const joiningDateInputRef = useRef<HTMLInputElement | null>(null);

  const canSubmit = isEditing
    ? hasPermission(Permission.EmployeeEdit)
    : hasPermission(Permission.EmployeeCreate);

  const {
    control,
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<EmployeeFormValues>({
    resolver: zodResolver(employeeFormSchema),
    defaultValues: {
      employeeCode: "EMP-",
      fullName: "",
      phone: "",
      email: "",
      joiningDate: getLocalDateValue(new Date()),
      isActive: true,
      designationId: "",
      userId: "",
    },
  });
  const joiningDateField = register("joiningDate");

  useEffect(() => {
    if (!employee) return;
    reset({
      employeeCode: withEmployeeCodePrefix(employee.employeeCode),
      fullName: employee.fullName ?? "",
      phone: employee.phone ?? "",
      email: employee.email ?? "",
      joiningDate: employee.joiningDate ? employee.joiningDate.slice(0, 10) : "",
      isActive: employee.isActive ?? true,
      designationId: employee.designationId ? String(employee.designationId) : "",
      userId: employee.userId ? String(employee.userId) : "",
    });
    setPhotoFile(null);
  }, [employee, reset]);

  const onSubmit = handleSubmit(async (values) => {
    if (!canSubmit) return;
    setPhotoError(undefined);

    const payload = {
      employeeCode: values.employeeCode,
      fullName: values.fullName,
      phone: values.phone,
      email: values.email?.trim() ? values.email.trim() : null,
      joiningDate: values.joiningDate,
      isActive: values.isActive,
      designationId: Number(values.designationId),
      userId: values.userId ? Number(values.userId) : null,
    };

    try {
      if (isEditing && employeeId) {
        await updateEmployeeMutation.mutateAsync({
          id: employeeId,
          payload: { ...payload, id: employeeId },
          photoFile,
        });
        toast.success("Employee updated.");
      } else {
        await createEmployeeMutation.mutateAsync({ payload, photoFile });
        toast.success("Employee created.");
      }
      navigate("/employees");
    } catch {
      toast.error("Could not save the employee. Please check the details and try again.");
    }
  });

  function openJoiningDatePicker() {
    const input = joiningDateInputRef.current;
    if (!input) return;

    if (typeof input.showPicker === "function") {
      input.showPicker();
    } else {
      input.focus();
      input.click();
    }
  }

  const saving = createEmployeeMutation.isPending || updateEmployeeMutation.isPending;

  if (isEditing && isEmployeePending) {
    return (
      <div className="flex items-center justify-center py-16 text-sm text-muted-foreground">
        <Loader2 className="mr-2 h-4 w-4 animate-spin" />
        Loading employee details…
      </div>
    );
  }

  if (isEditing && !employee && !isEmployeePending) {
    return (
      <div className="mx-auto max-w-5xl rounded-lg border border-dashed p-8 text-center text-sm text-muted-foreground">
        Employee not found.{" "}
        <Link to="/employees" className="text-primary underline">
          Back to list
        </Link>
      </div>
    );
  }

  if (!canSubmit) {
    return (
      <div className="mx-auto max-w-5xl rounded-lg border p-8 text-center text-sm text-muted-foreground">
        You do not have permission to {isEditing ? "edit" : "create"} employees.
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">
            {isEditing ? "Edit employee" : "Add employee"}
          </h1>
          <p className="text-sm text-muted-foreground">
            Capture employment details and a JPEG/PNG profile photo.
          </p>
        </div>
        <Link
          to="/employees"
          className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent"
        >
          Back to list
        </Link>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Employee information</CardTitle>
          <CardDescription>
            Existing photos stay until you upload a replacement. Leave the photo empty to keep the current file.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={onSubmit} className="space-y-6" noValidate>
            <ImageUploader
              existingUrl={employee?.employeePhoto}
              file={photoFile}
              onFileChange={(next) => {
                setPhotoFile(next);
                setPhotoError(undefined);
              }}
              error={photoError}
              disabled={saving}
            />

            <div className="grid gap-4 md:grid-cols-2">
              <div>
                <label className="mb-1 block text-sm font-medium" htmlFor="employeeCode">
                  Employee ID <span className="text-destructive">*</span>
                </label>
                <Controller
                  control={control}
                  name="employeeCode"
                  render={({ field }) => {
                    const employeeCode = field.value ?? "EMP-";
                    const codeSuffix = employeeCode.toUpperCase().startsWith("EMP-")
                      ? employeeCode.slice(4)
                      : employeeCode;

                    return (
                      <div className="flex overflow-hidden rounded-md border bg-background focus-within:ring-2 focus-within:ring-ring">
                        <span
                          aria-hidden="true"
                          className="inline-flex items-center border-r bg-muted px-3 text-sm font-semibold text-muted-foreground"
                        >
                          EMP-
                        </span>
                        <input
                          id="employeeCode"
                          ref={field.ref}
                          name={field.name}
                          type="text"
                          value={codeSuffix}
                          onChange={(event) =>
                            field.onChange(`EMP-${event.target.value}`)
                          }
                          onBlur={field.onBlur}
                          placeholder="Enter employee ID"
                          autoComplete="off"
                          maxLength={46}
                          aria-label="Employee ID suffix"
                          className="min-w-0 flex-1 px-3 py-2 outline-none"
                        />
                      </div>
                    );
                  }}
                />
                {errors.employeeCode ? (
                  <p className="mt-1 text-sm text-destructive">{errors.employeeCode.message}</p>
                ) : null}
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium" htmlFor="fullName">
                  Full name <span className="text-destructive">*</span>
                </label>
                <input
                  id="fullName"
                  {...register("fullName")}
                  className="w-full rounded-md border bg-background px-3 py-2"
                />
                {errors.fullName ? (
                  <p className="mt-1 text-sm text-destructive">{errors.fullName.message}</p>
                ) : null}
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium" htmlFor="phone">
                  Phone <span className="text-destructive">*</span>
                </label>
                <input
                  id="phone"
                  {...register("phone")}
                  placeholder="01XXXXXXXXX"
                  className="w-full rounded-md border bg-background px-3 py-2"
                />
                {errors.phone ? (
                  <p className="mt-1 text-sm text-destructive">{errors.phone.message}</p>
                ) : null}
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium" htmlFor="email">
                  Email
                </label>
                <input
                  id="email"
                  type="email"
                  {...register("email")}
                  className="w-full rounded-md border bg-background px-3 py-2"
                />
                {errors.email ? (
                  <p className="mt-1 text-sm text-destructive">{errors.email.message}</p>
                ) : null}
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium" htmlFor="joiningDate">
                  Joining date <span className="text-destructive">*</span>
                </label>
                <div className="relative">
                  <input
                    id="joiningDate"
                    type="date"
                    {...joiningDateField}
                    ref={(element) => {
                      joiningDateField.ref(element);
                      joiningDateInputRef.current = element;
                    }}
                    className="w-full rounded-md border bg-background px-3 py-2 pr-11"
                  />
                  <button
                    type="button"
                    aria-label="Choose joining date"
                    onClick={openJoiningDatePicker}
                    className="absolute inset-y-0 right-0 flex w-10 items-center justify-center rounded-r-md text-muted-foreground transition-colors hover:bg-accent hover:text-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
                  >
                    <CalendarDays aria-hidden="true" className="h-4 w-4" />
                  </button>
                </div>
                {errors.joiningDate ? (
                  <p className="mt-1 text-sm text-destructive">{errors.joiningDate.message}</p>
                ) : null}
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium" htmlFor="designationId">
                  Designation <span className="text-destructive">*</span>
                </label>
                <select
                  id="designationId"
                  {...register("designationId")}
                  className="w-full rounded-md border bg-background px-3 py-2"
                  disabled={designationsPending}
                >
                  <option value="">Select designation</option>
                  {designations.map((item) => (
                    <option key={item.id} value={item.id}>
                      {item.name}
                    </option>
                  ))}
                </select>
                {errors.designationId ? (
                  <p className="mt-1 text-sm text-destructive">{errors.designationId.message}</p>
                ) : null}
                {!designationsPending && designations.length === 0 ? (
                  <p className="mt-1 text-sm text-muted-foreground">
                    No designations found. Create one in master data first.
                  </p>
                ) : null}
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium" htmlFor="userId">
                  Linked user account
                </label>
                <select
                  id="userId"
                  {...register("userId")}
                  className="w-full rounded-md border bg-background px-3 py-2"
                >
                  <option value="">None</option>
                  {users.map((item) => (
                    <option key={item.id} value={item.id}>
                      {item.username} ({item.email})
                    </option>
                  ))}
                </select>
              </div>

              <div className="flex items-center pt-7">
                <label className="flex items-center gap-2 text-sm font-medium">
                  <input type="checkbox" {...register("isActive")} className="h-4 w-4" />
                  Active employee
                </label>
              </div>
            </div>

            <div className="flex gap-2">
              <Button type="submit" disabled={saving}>
                {saving ? (
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                ) : (
                  <Plus className="mr-2 h-4 w-4" />
                )}
                {isEditing ? "Save changes" : "Create employee"}
              </Button>
              <Button type="button" variant="outline" onClick={() => navigate("/employees")}>
                Cancel
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
