import { authApiClient } from "@/lib/api/auth-client";
import type { ProgressReportDto, TranscriptDto } from "../types/transcript.types";

export async function getStudentTranscript(studentId: number, academicYearId?: number | null): Promise<TranscriptDto> {
  const url = academicYearId
    ? `/Transcript/student/${studentId}/academic-year/${academicYearId}`
    : `/Transcript/student/${studentId}`;

  const { data } = await authApiClient.get<TranscriptDto>(url);
  return data;
}

export async function getStudentProgressReport(studentId: number, academicYearId: number): Promise<ProgressReportDto> {
  const { data } = await authApiClient.get<ProgressReportDto>(
    `/ProgressReport/student/${studentId}/academic-year/${academicYearId}`
  );
  return data;
}
