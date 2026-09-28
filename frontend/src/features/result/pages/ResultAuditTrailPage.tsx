import { useMemo, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { Loader2 } from "lucide-react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { useResultAuditLogs } from "../hooks/useResultData";

const ACTION_LABELS: Record<number, string> = {
  1: "Calculated",
  2: "Recalculated",
  3: "Verified",
  4: "Published",
  5: "Unpublished",
  6: "Locked",
  7: "Unlocked",
  8: "Archived",
  9: "Rolled back",
  10: "Mark updated",
};

export function ResultAuditTrailPage() {
  const { entityType, entityId } = useParams();
  const { hasPermission } = usePermissions();
  const [selectedEntityType, setSelectedEntityType] = useState(entityType ?? "ExamResult");
  const [selectedEntityId, setSelectedEntityId] = useState(entityId ?? "");

  const auditLogsQuery = useResultAuditLogs(selectedEntityType || null, selectedEntityId ? Number(selectedEntityId) : null);

  const auditRows = useMemo(() => auditLogsQuery.data ?? [], [auditLogsQuery.data]);

  if (!hasPermission(Permission.ResultAuditView)) {
    return (
      <Card>
        <CardContent className="p-6 text-sm text-destructive">
          You do not have permission to view the result audit trail.
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Result audit trail</h1>
          <p className="text-sm text-muted-foreground">
            Read-only action history for result calculations, publications, and mark edits.
          </p>
        </div>
        <Link to="/exams" className="inline-flex items-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent">
          Back to exams
        </Link>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Entity lookup</CardTitle>
          <CardDescription>Use the entity type and ID to inspect audit records.</CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-3 md:flex-row">
          <select
            value={selectedEntityType}
            onChange={(event) => setSelectedEntityType(event.target.value)}
            className="rounded-md border bg-background px-3 py-2 md:max-w-xs"
          >
            <option value="ExamResult">ExamResult</option>
            <option value="FinalResult">FinalResult</option>
            <option value="MarkEntry">MarkEntry</option>
          </select>

          <input
            type="number"
            min="1"
            value={selectedEntityId}
            onChange={(event) => setSelectedEntityId(event.target.value)}
            placeholder="Entity ID"
            className="rounded-md border bg-background px-3 py-2 md:max-w-xs"
          />
        </CardContent>
      </Card>

      {auditLogsQuery.isPending ? (
        <p className="text-sm text-muted-foreground">
          <Loader2 className="mr-2 inline h-4 w-4 animate-spin" />
          Loading audit log…
        </p>
      ) : null}

      {!auditLogsQuery.isPending && !auditLogsQuery.isError && auditRows.length === 0 ? (
        <Card>
          <CardContent className="p-6 text-sm text-muted-foreground">
            No audit history found for this entity.
          </CardContent>
        </Card>
      ) : null}

      {auditRows.length > 0 ? (
        <Card>
          <CardContent className="overflow-x-auto p-0">
            <table className="w-full min-w-[780px] text-sm">
              <thead>
                <tr className="border-b text-left">
                  <th className="p-3">Action</th>
                  <th className="p-3">Performed by</th>
                  <th className="p-3">Notes</th>
                  <th className="p-3">Time</th>
                </tr>
              </thead>
              <tbody>
                {auditRows.map((item) => (
                  <tr key={item.id} className="border-b">
                    <td className="p-3 font-medium">{ACTION_LABELS[item.action] ?? `Action ${item.action}`}</td>
                    <td className="p-3">{item.performedByName ?? item.performedBy ?? "System"}</td>
                    <td className="p-3 text-muted-foreground">{item.notes ?? "—"}</td>
                    <td className="p-3 whitespace-nowrap">{new Date(item.performedAt).toLocaleString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </CardContent>
        </Card>
      ) : null}
    </div>
  );
}
