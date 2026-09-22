import type { AxiosError } from "axios";
import { authApiClient } from "@/lib/api/auth-client";
import type {
  CreateExamDto,
  CreateExamScheduleDto,
  CreateGradeSetupDto,
  ExamCalendarDto,
  ExamDashboardDto,
  ExamDetailsDto,
  ExamDto,
  ExamScheduleDto,
  ExamTypeDto,
  GradeSetupDto,
  UpdateExamDto,
  UpdateExamScheduleDto,
} from "../types/exam.types";

export function getExamErrorMessage(error: unknown) {
  const err = error as AxiosError<{ message?: string }>;
  return err.response?.data?.message ?? "Could not complete this action. Please try again.";
}

export async function getExamTypes(): Promise<ExamTypeDto[]> {
  const { data } = await authApiClient.get<ExamTypeDto[]>("/ExamType");
  return data;
}

export async function createExamType(name: string): Promise<ExamTypeDto> {
  const { data } = await authApiClient.post<ExamTypeDto>("/ExamType", { name });
  return data;
}

export async function updateExamType(id: number, name: string): Promise<ExamTypeDto> {
  const { data } = await authApiClient.put<ExamTypeDto>(`/ExamType/${id}`, { id, name });
  return data;
}

export async function deleteExamType(id: number): Promise<void> {
  await authApiClient.delete(`/ExamType/${id}`);
}

export async function getExams(): Promise<ExamDto[]> {
  const { data } = await authApiClient.get<ExamDto[]>("/Exam");
  return data;
}

export async function getExamById(id: number): Promise<ExamDto> {
  const { data } = await authApiClient.get<ExamDto>(`/Exam/${id}`);
  return data;
}

export async function getExamDetails(id: number): Promise<ExamDetailsDto> {
  const { data } = await authApiClient.get<ExamDetailsDto>(`/Exam/${id}/details`);
  return data;
}

export async function createExam(payload: CreateExamDto): Promise<ExamDto> {
  const { data } = await authApiClient.post<ExamDto>("/Exam", payload);
  return data;
}

export async function updateExam(id: number, payload: UpdateExamDto): Promise<ExamDto> {
  const { data } = await authApiClient.put<ExamDto>(`/Exam/${id}`, payload);
  return data;
}

export async function deleteExam(id: number): Promise<void> {
  await authApiClient.delete(`/Exam/${id}`);
}

export async function publishExam(id: number): Promise<ExamDto> {
  const { data } = await authApiClient.post<ExamDto>(`/Exam/${id}/publish`);
  return data;
}

export async function completeExam(id: number): Promise<ExamDto> {
  const { data } = await authApiClient.post<ExamDto>(`/Exam/${id}/complete`);
  return data;
}

export async function cancelExam(id: number): Promise<ExamDto> {
  const { data } = await authApiClient.post<ExamDto>(`/Exam/${id}/cancel`);
  return data;
}

export async function reopenExam(id: number): Promise<ExamDto> {
  const { data } = await authApiClient.post<ExamDto>(`/Exam/${id}/reopen`);
  return data;
}

export async function getExamDashboard(): Promise<ExamDashboardDto> {
  const { data } = await authApiClient.get<ExamDashboardDto>("/Exam/dashboard");
  return data;
}

export async function getExamCalendar(params: {
  fromDate: string;
  toDate: string;
  classId?: number | null;
}): Promise<ExamCalendarDto[]> {
  const { data } = await authApiClient.get<ExamCalendarDto[]>("/Exam/calendar", {
    params: {
      fromDate: params.fromDate,
      toDate: params.toDate,
      classId: params.classId ?? undefined,
    },
  });
  return data;
}

export async function getSchedulesByExam(examId: number): Promise<ExamScheduleDto[]> {
  const { data } = await authApiClient.get<ExamScheduleDto[]>(`/ExamSchedule/exam/${examId}`);
  return data;
}

export async function createExamSchedule(payload: CreateExamScheduleDto): Promise<ExamScheduleDto> {
  const { data } = await authApiClient.post<ExamScheduleDto>("/ExamSchedule", payload);
  return data;
}

export async function updateExamSchedule(
  id: number,
  payload: UpdateExamScheduleDto
): Promise<ExamScheduleDto> {
  const { data } = await authApiClient.put<ExamScheduleDto>(`/ExamSchedule/${id}`, payload);
  return data;
}

export async function deleteExamSchedule(id: number): Promise<void> {
  await authApiClient.delete(`/ExamSchedule/${id}`);
}

export async function getGradeSetups(): Promise<GradeSetupDto[]> {
  const { data } = await authApiClient.get<GradeSetupDto[]>("/GradeSetup");
  return data;
}

export async function createGradeSetup(payload: CreateGradeSetupDto): Promise<GradeSetupDto> {
  const { data } = await authApiClient.post<GradeSetupDto>("/GradeSetup", payload);
  return data;
}

export async function updateGradeSetup(
  id: number,
  payload: Omit<CreateGradeSetupDto, "academicYearId"> & { id: number }
): Promise<GradeSetupDto> {
  const { data } = await authApiClient.put<GradeSetupDto>(`/GradeSetup/${id}`, payload);
  return data;
}

export async function deleteGradeSetup(id: number): Promise<void> {
  await authApiClient.delete(`/GradeSetup/${id}`);
}

export async function activateGradeSetup(id: number): Promise<GradeSetupDto> {
  const { data } = await authApiClient.post<GradeSetupDto>(`/GradeSetup/${id}/activate`);
  return data;
}

export async function deactivateGradeSetup(id: number): Promise<GradeSetupDto> {
  const { data } = await authApiClient.post<GradeSetupDto>(`/GradeSetup/${id}/deactivate`);
  return data;
}
