import { authApiClient } from "@/lib/api/auth-client";
import type {
  CreateStudentFeeConcessionDto,
  StudentFeeConcessionDto,
  StudentFeeConcessionListDto,
  UpdateStudentFeeConcessionDto,
} from "../types/student-fee-concession.types";

export async function getStudentConcessions(studentId: number): Promise<StudentFeeConcessionListDto[]> {
  const { data } = await authApiClient.get<StudentFeeConcessionListDto[]>(`/StudentFeeConcessions/student/${studentId}`);
  return data;
}

export async function getPendingConcessions(): Promise<StudentFeeConcessionListDto[]> {
  const { data } = await authApiClient.get<StudentFeeConcessionListDto[]>("/StudentFeeConcessions/pending-approvals");
  return data;
}

export async function getStudentConcessionById(id: number): Promise<StudentFeeConcessionDto> {
  const { data } = await authApiClient.get<StudentFeeConcessionDto>(`/StudentFeeConcessions/${id}`);
  return data;
}

export async function createStudentConcession(payload: CreateStudentFeeConcessionDto): Promise<StudentFeeConcessionDto> {
  const { data } = await authApiClient.post<StudentFeeConcessionDto>("/StudentFeeConcessions", payload);
  return data;
}

export async function updateStudentConcession(id: number, payload: UpdateStudentFeeConcessionDto): Promise<StudentFeeConcessionDto> {
  const { data } = await authApiClient.put<StudentFeeConcessionDto>(`/StudentFeeConcessions/${id}`, payload);
  return data;
}

export async function approveStudentConcession(id: number): Promise<StudentFeeConcessionDto> {
  const { data } = await authApiClient.post<StudentFeeConcessionDto>(`/StudentFeeConcessions/${id}/approve`);
  return data;
}

export async function deleteStudentConcession(id: number): Promise<void> {
  await authApiClient.delete(`/StudentFeeConcessions/${id}`);
}