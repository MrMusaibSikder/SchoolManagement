import { Link, useNavigate, useParams } from "react-router-dom";
import { Loader2, Pencil } from "lucide-react";
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
import { Permission } from "@/lib/permissions";
import { useDesignations, useEmployee } from "../hooks/useEmployeeData";

export function EmployeeDetailsPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const employeeId = Number(id);
  const { data: employee, isPending, isError } = useEmployee(
    Number.isNaN(employeeId) ? null : employeeId
  );
  const { data: designations = [] } = useDesignations();
  const designationName =
    designations.find((item) => item.id === employee?.designationId)?.name ??
    (employee?.designationId ? `#${employee.designationId}` : "—");

  if (isPending) {
    return (
      <div className="mx-auto max-w-4xl py-12 text-center text-sm text-muted-foreground">
        <Loader2 className="mr-2 inline-block h-4 w-4 animate-spin" /> Loading employee…
      </div>
    );
  }

  if (isError || !employee) {
    return (
      <div className="mx-auto max-w-4xl rounded-lg border border-destructive/20 bg-destructive/5 p-6 text-sm text-destructive">
        Unable to load this employee.{" "}
        <Link to="/employees" className="underline">
          Back to list
        </Link>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-4xl space-y-6">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">{employee.fullName}</h1>
          <p className="text-sm text-muted-foreground">Employee profile and photo.</p>
        </div>
        <div className="flex gap-2">
          <Link
            to="/employees"
            className="inline-flex items-center justify-center rounded-md border border-input bg-background px-4 py-2 text-sm font-medium transition hover:bg-accent"
          >
            Back to list
          </Link>
          <PermissionGuard permission={Permission.EmployeeEdit}>
            <Button onClick={() => navigate(`/employees/${employee.id}/edit`)}>
              <Pencil className="mr-2 h-4 w-4" />
              Edit
            </Button>
          </PermissionGuard>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Profile</CardTitle>
          <CardDescription>Contact details, designation, and stored photo.</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col gap-6 sm:flex-row">
            <div className="flex h-32 w-32 shrink-0 overflow-hidden rounded-full border bg-muted">
              <MediaImage
                src={employee.employeePhoto}
                alt={employee.fullName}
                fallbackClassName="h-32 w-32"
              />
            </div>

            <div className="grid flex-1 gap-4 md:grid-cols-2">
              <div>
                <p className="text-xs text-muted-foreground">Employee code</p>
                <p className="font-medium">{employee.employeeCode}</p>
              </div>
              <div>
                <p className="text-xs text-muted-foreground">Designation</p>
                <p className="font-medium">{designationName}</p>
              </div>
              <div>
                <p className="text-xs text-muted-foreground">Phone</p>
                <p className="font-medium">{employee.phone}</p>
              </div>
              <div>
                <p className="text-xs text-muted-foreground">Email</p>
                <p className="font-medium">{employee.email ?? "—"}</p>
              </div>
              <div>
                <p className="text-xs text-muted-foreground">Joining date</p>
                <p className="font-medium">
                  {employee.joiningDate
                    ? new Date(employee.joiningDate).toLocaleDateString("en-BD")
                    : "—"}
                </p>
              </div>
              <div>
                <p className="text-xs text-muted-foreground">Status</p>
                <p className="font-medium">{employee.isActive ? "Active" : "Inactive"}</p>
              </div>
              <div>
                <p className="text-xs text-muted-foreground">Linked user</p>
                <p className="font-medium">{employee.userId ?? "—"}</p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
