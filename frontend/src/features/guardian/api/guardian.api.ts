import { authApiClient } from "@/lib/api/auth-client";
import type { CreateGuardianDto, GuardianDto, UpdateGuardianDto } from "../types/guardian.types";

async function getJson<T>(path: string): Promise<T> {
  const { data } = await authApiClient.get<T>(path);
  return data;
}

async function postJson<T>(path: string, body: unknown): Promise<T> {
  const { data } = await authApiClient.post<T>(path, body);
  return data;
}

async function putJson<T>(path: string, body: unknown): Promise<T> {
  const { data } = await authApiClient.put<T>(path, body);
  return data;
}

async function deleteJson(path: string): Promise<void> {
  await authApiClient.delete(path);
}

export async function getGuardians(): Promise<GuardianDto[]> {
  return getJson<GuardianDto[]>("/Guardians");
}

export async function searchGuardians(keyword: string): Promise<GuardianDto[]> {
  return getJson<GuardianDto[]>(`/Guardians/search?keyword=${encodeURIComponent(keyword)}`);
}

export async function getGuardianById(id: number): Promise<GuardianDto> {
  return getJson<GuardianDto>(`/Guardians/${id}`);
}

export async function createGuardian(payload: CreateGuardianDto): Promise<GuardianDto> {
  return postJson<GuardianDto>("/Guardians", payload);
}

export async function updateGuardian(id: number, payload: UpdateGuardianDto): Promise<GuardianDto> {
  return putJson<GuardianDto>(`/Guardians/${id}`, payload);
}

export async function deleteGuardian(id: number): Promise<void> {
  await deleteJson(`/Guardians/${id}`);
}
