import { useQuery } from "@tanstack/react-query";
import { getStudentProgressReport, getStudentTranscript } from "../api/transcript.api";

export function useStudentTranscript(studentId: number | null, academicYearId?: number | null) {
  return useQuery({
    queryKey: ["transcript", studentId, academicYearId],
    queryFn: () => getStudentTranscript(studentId as number, academicYearId),
    enabled: Boolean(studentId),
    staleTime: 30_000,
  });
}

export function useStudentProgressReport(studentId: number | null, academicYearId: number | null) {
  return useQuery({
    queryKey: ["progress-report", studentId, academicYearId],
    queryFn: () => getStudentProgressReport(studentId as number, academicYearId as number),
    enabled: Boolean(studentId && academicYearId),
    staleTime: 30_000,
  });
}
