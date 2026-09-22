import { useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { Loader2 } from "lucide-react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { useSchoolClasses } from "@/features/academic/hooks/useAcademicData";
import { useExamCalendar } from "../hooks/useExamData";

function monthRange(offset = 0) {
  const now = new Date();
  const start = new Date(now.getFullYear(), now.getMonth() + offset, 1);
  const end = new Date(now.getFullYear(), now.getMonth() + offset + 1, 0);
  const iso = (date: Date) =>
    `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, "0")}-${String(date.getDate()).padStart(2, "0")}`;
  return { fromDate: iso(start), toDate: iso(end) };
}

export function ExamCalendarPage() {
  const { data: classes = [] } = useSchoolClasses();
  const initial = useMemo(() => monthRange(0), []);
  const [fromDate, setFromDate] = useState(initial.fromDate);
  const [toDate, setToDate] = useState(initial.toDate);
  const [classId, setClassId] = useState("");
  const { data = [], isPending, isError } = useExamCalendar({
    fromDate,
    toDate,
    classId: classId ? Number(classId) : null,
  });

  return (
    <div className="mx-auto max-w-6xl space-y-6">
      <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
        <div>
          <h1 className="font-display text-2xl font-semibold">Exam calendar</h1>
          <p className="text-sm text-muted-foreground">Subject-wise papers in the selected date range.</p>
        </div>
        <Link to="/exams" className="inline-flex items-center rounded-md border px-4 py-2 text-sm font-medium hover:bg-accent">
          Back
        </Link>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Filters</CardTitle>
          <CardDescription>Defaults to the current month.</CardDescription>
        </CardHeader>
        <CardContent className="grid gap-3 md:grid-cols-3">
          <input type="date" value={fromDate} onChange={(event) => setFromDate(event.target.value)} className="rounded-md border bg-background px-3 py-2" />
          <input type="date" value={toDate} onChange={(event) => setToDate(event.target.value)} className="rounded-md border bg-background px-3 py-2" />
          <select value={classId} onChange={(event) => setClassId(event.target.value)} className="rounded-md border bg-background px-3 py-2">
            <option value="">All classes</option>
            {classes.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}
          </select>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Papers</CardTitle>
        </CardHeader>
        <CardContent className="space-y-2">
          {isPending ? (
            <div className="flex items-center text-sm text-muted-foreground">
              <Loader2 className="mr-2 h-4 w-4 animate-spin" /> Loading…
            </div>
          ) : null}
          {isError ? <p className="text-sm text-destructive">Unable to load the calendar.</p> : null}
          {!isPending && data.length === 0 ? <p className="text-sm text-muted-foreground">No papers in this range.</p> : null}
          {data.map((item) => (
            <Link
              key={item.scheduleId}
              to={`/exams/${item.examId}`}
              className="flex flex-col gap-1 rounded-lg border p-3 hover:bg-muted/40 md:flex-row md:items-center md:justify-between"
            >
              <div>
                <p className="font-medium">{item.examName} • {item.subjectName}</p>
                <p className="text-sm text-muted-foreground">{item.className}</p>
              </div>
              <p className="text-sm text-muted-foreground">{new Date(item.examDate).toLocaleString("en-BD")}</p>
            </Link>
          ))}
        </CardContent>
      </Card>
    </div>
  );
}
