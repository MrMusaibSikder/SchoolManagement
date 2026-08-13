import { authApiClient } from "@/lib/api/auth-client";
import type {
  CreateStudentDto,
  StudentAttendanceHistoryItem,
  StudentDto,
  StudentDocumentDto,
  UpdateStudentDto,
} from "../types/student.types";

async function getJson<T>(path: string): Promise<T> {
  const { data } = await authApiClient.get<T>(path);
  return data;
}

async function postForm<T>(path: string, body: FormData): Promise<T> {
  const { data } = await authApiClient.post<T>(path, body, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return data;
}

async function putForm<T>(path: string, body: FormData): Promise<T> {
  const { data } = await authApiClient.put<T>(path, body, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return data;
}

async function deleteJson(path: string): Promise<void> {
  await authApiClient.delete(path);
}

export async function getStudents(): Promise<StudentDto[]> {
  return getJson<StudentDto[]>("/Students");
}

export async function getStudentById(id: number): Promise<StudentDto> {
  return getJson<StudentDto>(`/Students/${id}`);
}

export async function createStudent(payload: CreateStudentDto, photoFile?: File | null): Promise<StudentDto> {
  const formData = new FormData();
  Object.entries(payload).forEach(([key, value]) => {
    if (value === undefined || value === null) return;
    if (Array.isArray(value)) {
      formData.append(key, JSON.stringify(value));
      return;
    }
    formData.append(key, String(value));
  });
  if (photoFile) {
    formData.append("PhotoFile", photoFile);
  }
  return postForm<StudentDto>("/Students", formData);
}

export async function updateStudent(id: number, payload: UpdateStudentDto, photoFile?: File | null): Promise<StudentDto> {
  const formData = new FormData();
  Object.entries(payload).forEach(([key, value]) => {
    if (value === undefined || value === null) return;
    if (Array.isArray(value)) {
      formData.append(key, JSON.stringify(value));
      return;
    }
    formData.append(key, String(value));
  });
  if (photoFile) {
    formData.append("PhotoFile", photoFile);
  }
  return putForm<StudentDto>(`/Students/${id}`, formData);
}

export async function deleteStudent(id: number): Promise<void> {
  await deleteJson(`/Students/${id}`);
}

export async function getStudentDocuments(studentId: number): Promise<StudentDocumentDto[]> {
  return getJson<StudentDocumentDto[]>(`/Students/${studentId}/documents`).catch(() => []);
}

export async function getStudentAttendanceHistory(studentId: number): Promise<StudentAttendanceHistoryItem[]> {
  return getJson<StudentAttendanceHistoryItem[]>(`/StudentAttendance/student/${studentId}/history`).catch(() => []);
}
