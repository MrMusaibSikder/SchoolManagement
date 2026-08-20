import { useNavigate, useParams } from "react-router-dom";
import { Loader2, Pencil, CalendarCheck, FileText } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useEmployee } from "../hooks/useEmployeeData";

export function EmployeeDetailsPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { data: employee, isPending } = useEmployee(id ? Number(id) : null);

  if (isPending) {
    return (
      <div className="mx-auto max-w-4xl py-12 text-center text-sm text-muted-foreground">
        <Loader2 className="mr-2 h-4 w-4 animate-spin inline-block" /> Loading employee…
      </div>
    );
  }

  if (!employee) {
    return (
      <div className="mx-auto max-w-4xl py-12 text-center text-sm text-muted-foreground">Employee not found.</div>
    );
  }

  return (
    <div className="mx-auto max-w-4xl space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">{employee.fullName}</h1>
          <p className="text-sm text-muted-foreground">Employee profile and quick actions.</p>
        </div>
        <div className="flex gap-2">
          <Button onClick={() => navigate(`/employees/${employee.id}/edit`)}>
            <Pencil className="mr-2 h-4 w-4" />Edit
          </Button>
        </div>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Profile</CardTitle>
          <CardDescription>Employee contact and employment details.</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex gap-6">
            <div className="flex h-32 w-32 items-center justify-center overflow-hidden rounded-full border bg-background text-muted-foreground">
              {employee.employeePhoto ? (
                <img src={employee.employeePhoto} alt={employee.fullName} className="h-full w-full object-cover" />
              ) : (
                <div className="text-muted-foreground">No photo</div>
              )}
            </div>

            <div className="flex-1">
              <div className="grid gap-3 md:grid-cols-2">
                <div>
                  <p className="text-xs text-muted-foreground">Employee code</p>
                  <p className="font-medium">{employee.employeeCode}</p>
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
                  <p className="font-medium">{employee.joiningDate?.slice(0, 10) ?? "—"}</p>
                </div>
              </div>

              <div className="mt-4 flex gap-2">
                <Button onClick={() => navigate(`/employees/${employee.id}/attendance`)}>
                  <CalendarCheck className="mr-2 h-4 w-4" /> Attendance
                </Button>
                <Button onClick={() => navigate(`/employees/${employee.id}/payslips`)}>
                  <FileText className="mr-2 h-4 w-4" /> Payslips
                </Button>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Notes</CardTitle>
          <CardDescription>Placeholder for future sections: attendance history, salary records, documents.</CardDescription>
        </CardHeader>
        <CardContent>
          <p className="text-sm text-muted-foreground">Implement attendance and payslip viewers on demand; these links are placeholders if those pages are not yet implemented.</p>
        </CardContent>
      </Card>
    </div>
  );
}
