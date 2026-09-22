import type { AxiosError } from "axios";
import { authApiClient } from "@/lib/api/auth-client";
import type {
  BulkMarkEntryDto,
  ExamResultDashboardDto,
  ExamResultDto,
  FinalResultDto,
  ResultDto,
} from "../types/result.types";

export function getResultErrorMessage(error: unknown) {
  const err = error as AxiosError<{ message?: string }>;
  return err.response?.data?.message ?? "Could not complete this result action. Please try again.";
}

export async function getMarkEntriesBySchedule(examScheduleId: number): Promise<ResultDto[]> {
  const { data } = await authApiClient.get<ResultDto[]>(`/MarkEntry/exam-schedule/${examScheduleId}`);
  return data;
}

export async function saveBulkMarkEntries(payload: BulkMarkEntryDto): Promise<ResultDto[]> {
  const { data } = await authApiClient.post<ResultDto[]>("/MarkEntry/bulk", payload);
  return data;
}

export async function submitMarkEntries(examScheduleId: number, teacherId: number): Promise<ResultDto[]> {
  const { data } = await authApiClient.post<ResultDto[]>(
    `/MarkEntry/exam-schedule/${examScheduleId}/submit`,
    null,
    { params: { teacherId } }
  );
  return data;
}

export async function lockMarkEntries(examScheduleId: number): Promise<void> {
  await authApiClient.post(`/MarkEntry/exam-schedule/${examScheduleId}/lock`);
}

export async function unlockMarkEntries(examScheduleId: number): Promise<void> {
  await authApiClient.post(`/MarkEntry/exam-schedule/${examScheduleId}/unlock`);
}

export async function getExamResults(examId: number, classId?: number | null): Promise<ExamResultDto[]> {
  const { data } = await authApiClient.get<ExamResultDto[]>(`/ExamResult/exam/${examId}`, {
    params: { classId: classId ?? undefined },
  });
  return data;
}

export async function getExamResultDashboard(examId: number): Promise<ExamResultDashboardDto> {
  const { data } = await authApiClient.get<ExamResultDashboardDto>(`/ExamResult/exam/${examId}/dashboard`);
  return data;
}

export async function calculateExamResults(examId: number): Promise<ExamResultDto[]> {
  const { data } = await authApiClient.post<ExamResultDto[]>(`/ExamResult/exam/${examId}/calculate`);
  return data;
}

export async function publishExamResults(examId: number): Promise<ExamResultDto[]> {
  const { data } = await authApiClient.post<ExamResultDto[]>(`/ExamResult/exam/${examId}/publish`);
  return data;
}

export async function unpublishExamResults(examId: number): Promise<void> {
  await authApiClient.post(`/ExamResult/exam/${examId}/unpublish`);
}

export async function getFinalResults(
  academicYearId: number,
  classId?: number | null
): Promise<FinalResultDto[]> {
  const { data } = await authApiClient.get<FinalResultDto[]>(`/FinalResult/academic-year/${academicYearId}`, {
    params: { classId: classId ?? undefined },
  });
  return data;
}

export async function getStudentFinalResult(
  studentId: number,
  academicYearId: number
): Promise<FinalResultDto> {
  const { data } = await authApiClient.get<FinalResultDto>(
    `/FinalResult/student/${studentId}/academic-year/${academicYearId}`
  );
  return data;
}

export async function calculateFinalResults(academicYearId: number): Promise<FinalResultDto[]> {
  const { data } = await authApiClient.post<FinalResultDto[]>(
    `/FinalResult/academic-year/${academicYearId}/calculate`
  );
  return data;
}

export async function publishFinalResults(academicYearId: number): Promise<FinalResultDto[]> {
  const { data } = await authApiClient.post<FinalResultDto[]>(
    `/FinalResult/academic-year/${academicYearId}/publish`
  );
  return data;
}

export async function unpublishFinalResults(academicYearId: number): Promise<void> {
  await authApiClient.post(`/FinalResult/academic-year/${academicYearId}/unpublish`);
}

export async function updateFinalResultRemarks(
  studentId: number,
  academicYearId: number,
  teacherRemarks?: string | null,
  principalRemarks?: string | null
): Promise<FinalResultDto> {
  const { data } = await authApiClient.patch<FinalResultDto>(
    `/FinalResult/student/${studentId}/academic-year/${academicYearId}/remarks`,
    null,
    {
      params: {
        teacherRemarks: teacherRemarks ?? undefined,
        principalRemarks: principalRemarks ?? undefined,
      },
    }
  );
  return data;
}
