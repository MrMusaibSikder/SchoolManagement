import { authApiClient } from "@/lib/api/auth-client";
import type { CreateEmployeeDto, EmployeeDto, UpdateEmployeeDto } from "../types/employee.types";

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

export async function getEmployees(): Promise<EmployeeDto[]> {
  return getJson<EmployeeDto[]>("/Employees");
}

export async function getEmployeeById(id: number): Promise<EmployeeDto> {
  return getJson<EmployeeDto>(`/Employees/${id}`);
}

export async function createEmployee(payload: CreateEmployeeDto, photoFile?: File | null): Promise<EmployeeDto> {
  const formData = new FormData();
  Object.entries(payload).forEach(([key, value]) => {
    if (value === undefined || value === null) return;
    formData.append(key, String(value));
  });

  if (photoFile) {
    formData.append("EmployeePhotoFile", photoFile);
  }

  return postForm<EmployeeDto>("/Employees", formData);
}

export async function updateEmployee(id: number, payload: UpdateEmployeeDto, photoFile?: File | null): Promise<EmployeeDto> {
  const formData = new FormData();
  Object.entries(payload).forEach(([key, value]) => {
    if (value === undefined || value === null) return;
    formData.append(key, String(value));
  });

  if (photoFile) {
    formData.append("EmployeePhotoFile", photoFile);
  }

  return putForm<EmployeeDto>(`/Employees/${id}`, formData);
}

export async function deleteEmployee(id: number): Promise<void> {
  await deleteJson(`/Employees/${id}`);
}
