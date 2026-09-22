import { authApiClient } from "@/lib/api/auth-client";
import type {
  CreateEmployeeDto,
  DesignationDto,
  EmployeeDto,
  UpdateEmployeeDto,
  UserLookupDto,
} from "../types/employee.types";

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

function toFormData(payload: CreateEmployeeDto | UpdateEmployeeDto, photoFile?: File | null) {
  const formData = new FormData();
  formData.append("EmployeeCode", payload.employeeCode);
  formData.append("FullName", payload.fullName);
  formData.append("Phone", payload.phone);
  if (payload.email) formData.append("Email", payload.email);
  formData.append("JoiningDate", payload.joiningDate);
  formData.append("IsActive", String(payload.isActive));
  formData.append("DesignationId", String(payload.designationId));
  if (payload.userId != null) formData.append("UserId", String(payload.userId));
  if ("id" in payload) formData.append("Id", String(payload.id));
  if (photoFile) formData.append("EmployeePhotoFile", photoFile);
  return formData;
}

export async function getEmployees(): Promise<EmployeeDto[]> {
  return getJson<EmployeeDto[]>("/Employees");
}

export async function getEmployeeById(id: number): Promise<EmployeeDto> {
  return getJson<EmployeeDto>(`/Employees/${id}`);
}

export async function createEmployee(
  payload: CreateEmployeeDto,
  photoFile?: File | null
): Promise<EmployeeDto> {
  return postForm<EmployeeDto>("/Employees", toFormData(payload, photoFile));
}

export async function updateEmployee(
  id: number,
  payload: UpdateEmployeeDto,
  photoFile?: File | null
): Promise<EmployeeDto> {
  return putForm<EmployeeDto>(`/Employees/${id}`, toFormData(payload, photoFile));
}

export async function deleteEmployee(id: number): Promise<void> {
  await authApiClient.delete(`/Employees/${id}`);
}

export async function getDesignations(): Promise<DesignationDto[]> {
  return getJson<DesignationDto[]>("/Designations");
}

export async function getUsers(): Promise<UserLookupDto[]> {
  return getJson<UserLookupDto[]>("/User");
}
