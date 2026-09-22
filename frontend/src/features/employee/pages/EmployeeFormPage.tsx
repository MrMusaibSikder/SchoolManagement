import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Loader2, Plus } from "lucide-react";
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

  const canSubmit = isEditing
    ? hasPermission(Permission.EmployeeEdit)
    : hasPermission(Permission.EmployeeCreate);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<EmployeeFormValues>({
    resolver: zodResolver(employeeFormSchema),
    defaultValues: {
      employeeCode: "",
      fullName: "",
      phone: "",
      email: "",
      joiningDate: "",
      isActive: true,
      designationId: "",
      userId: "",
    },
  });

  useEffect(() => {
    if (!employee) return;
    reset({
      employeeCode: employee.employeeCode ?? "",
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
                  Employee code <span className="text-destructive">*</span>
                </label>
                <input
                  id="employeeCode"
                  {...register("employeeCode")}
                  className="w-full rounded-md border bg-background px-3 py-2"
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
                <input
                  id="joiningDate"
                  type="date"
                  {...register("joiningDate")}
                  className="w-full rounded-md border bg-background px-3 py-2"
                />
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
