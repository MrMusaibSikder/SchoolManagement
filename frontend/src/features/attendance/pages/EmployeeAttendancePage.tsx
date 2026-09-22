import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2 } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { useEmployees } from "@/features/employee/hooks/useEmployeeData";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import {
  useBulkEmployeeAttendance,
  useEmployeeAttendanceSheet,
} from "../hooks/useAttendanceData";
import {
  ATTENDANCE_STATUS_OPTIONS,
  AttendanceStatus,
  todayIsoDate,
  type AttendanceStatusValue,
} from "../types/attendance.types";

interface RowState {
  employeeId: number;
  fullName: string;
  employeeCode: string;
  status: AttendanceStatusValue;
  checkIn: string;
  checkOut: string;
}

function timeFromIso(value?: string | null) {
  if (!value) return "";
  const match = /T(\d{2}:\d{2})/.exec(value);
  if (match) return match[1];
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "";
  return `${String(date.getHours()).padStart(2, "0")}:${String(date.getMinutes()).padStart(2, "0")}`;
}

function combineDateTime(date: string, time: string) {
  if (!time) return null;
  return `${date}T${time}:00`;
}

export function EmployeeAttendancePage() {
  const { hasPermission } = usePermissions();
  const canSave = hasPermission(Permission.EmployeeAttendanceCreate);
  const { data: employees = [], isPending: employeesPending } = useEmployees();
  const saveMutation = useBulkEmployeeAttendance();
  const [attendanceDate, setAttendanceDate] = useState(todayIsoDate());
  const [rows, setRows] = useState<RowState[]>([]);
  const sheet = useEmployeeAttendanceSheet(attendanceDate);

  useEffect(() => {
    const existing = new Map((sheet.data ?? []).map((item) => [item.employeeId, item]));
    setRows(
      employees.map((employee) => {
        const record = existing.get(employee.id);
        return {
          employeeId: employee.id,
          fullName: employee.fullName,
          employeeCode: employee.employeeCode,
          status: record?.status ?? AttendanceStatus.Present,
          checkIn: timeFromIso(record?.checkIn),
          checkOut: timeFromIso(record?.checkOut),
        };
      })
    );
  }, [employees, sheet.data]);

  function updateRow(employeeId: number, patch: Partial<RowState>) {
    setRows((current) =>
      current.map((row) => (row.employeeId === employeeId ? { ...row, ...patch } : row))
    );
  }

  async function handleSave() {
    if (rows.length === 0) return;
    try {
      await saveMutation.mutateAsync({
        attendanceDate,
        attendance: rows.map((row) => ({
          employeeId: row.employeeId,
          status: row.status,
          checkIn: combineDateTime(attendanceDate, row.checkIn),
          checkOut: combineDateTime(attendanceDate, row.checkOut),
        })),
      });
      toast.success("Employee attendance saved.");
    } catch {
      toast.error("Could not save attendance. Future dates and empty lists are not allowed.");
    }
  }

  const loading = employeesPending || sheet.isFetching;

  return (
    <div className="mx-auto max-w-7xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Employee attendance</h1>
          <p className="text-sm text-muted-foreground">
            Mark staff for a single date. Saving again updates the same day.
          </p>
        </div>
        <Link
          to="/attendance"
          className="inline-flex items-center justify-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent"
        >
          Back
        </Link>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Staff register</CardTitle>
          <CardDescription>Optional check-in and check-out times can be left blank.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex flex-col gap-3 sm:flex-row sm:items-center">
            <input
              type="date"
              max={todayIsoDate()}
              value={attendanceDate}
              onChange={(event) => setAttendanceDate(event.target.value)}
              className="rounded-md border bg-background px-3 py-2"
            />
            {canSave ? (
              <div className="flex gap-2">
                <Button
                  type="button"
                  variant="outline"
                  disabled={rows.length === 0}
                  onClick={() =>
                    setRows((current) =>
                      current.map((row) => ({ ...row, status: AttendanceStatus.Present }))
                    )
                  }
                >
                  All present
                </Button>
                <Button
                  type="button"
                  disabled={saveMutation.isPending || rows.length === 0}
                  onClick={() => void handleSave()}
                >
                  {saveMutation.isPending ? (
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  ) : null}
                  Save
                </Button>
              </div>
            ) : null}
          </div>

          {loading ? (
            <div className="flex items-center justify-center py-10 text-sm text-muted-foreground">
              <Loader2 className="mr-2 h-4 w-4 animate-spin" />
              Loading staff register…
            </div>
          ) : null}

          {sheet.isError ? (
            <div className="rounded-lg border border-destructive/20 bg-destructive/5 p-4 text-sm text-destructive">
              Unable to load existing staff attendance.
            </div>
          ) : null}

          {!loading && rows.length === 0 ? (
            <div className="rounded-lg border border-dashed p-6 text-sm text-muted-foreground">
              No employees found.
            </div>
          ) : null}

          {rows.length > 0 ? (
            <div className="overflow-x-auto rounded-lg border">
              <table className="w-full min-w-[720px] text-left text-sm">
                <thead className="border-b bg-muted/40">
                  <tr>
                    <th className="px-3 py-2 font-medium">Employee</th>
                    <th className="px-3 py-2 font-medium">Code</th>
                    <th className="px-3 py-2 font-medium">Status</th>
                    <th className="px-3 py-2 font-medium">Check in</th>
                    <th className="px-3 py-2 font-medium">Check out</th>
                  </tr>
                </thead>
                <tbody>
                  {rows.map((row) => (
                    <tr key={row.employeeId} className="border-b last:border-0">
                      <td className="px-3 py-2 font-medium">{row.fullName}</td>
                      <td className="px-3 py-2 text-muted-foreground">{row.employeeCode}</td>
                      <td className="px-3 py-2">
                        <select
                          value={row.status}
                          disabled={!canSave}
                          onChange={(event) =>
                            updateRow(row.employeeId, {
                              status: Number(event.target.value) as AttendanceStatusValue,
                            })
                          }
                          className="w-full rounded-md border bg-background px-2 py-1.5"
                        >
                          {ATTENDANCE_STATUS_OPTIONS.map((option) => (
                            <option key={option.value} value={option.value}>
                              {option.label}
                            </option>
                          ))}
                        </select>
                      </td>
                      <td className="px-3 py-2">
                        <input
                          type="time"
                          value={row.checkIn}
                          disabled={!canSave}
                          onChange={(event) =>
                            updateRow(row.employeeId, { checkIn: event.target.value })
                          }
                          className="rounded-md border bg-background px-2 py-1.5"
                        />
                      </td>
                      <td className="px-3 py-2">
                        <input
                          type="time"
                          value={row.checkOut}
                          disabled={!canSave}
                          onChange={(event) =>
                            updateRow(row.employeeId, { checkOut: event.target.value })
                          }
                          className="rounded-md border bg-background px-2 py-1.5"
                        />
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          ) : null}
        </CardContent>
      </Card>
    </div>
  );
}
