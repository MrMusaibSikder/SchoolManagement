import { authApiClient } from "@/lib/api/auth-client";
import type { CreateLateFineRuleDto, LateFineRuleDto, UpdateLateFineRuleDto } from "../types/late-fine-rule.types";

export async function getLateFineRules(academicYearId: number): Promise<LateFineRuleDto[]> {
  const { data } = await authApiClient.get<LateFineRuleDto[]>(`/LateFineRules/academic-year/${academicYearId}`);
  return data;
}

export async function createLateFineRule(payload: CreateLateFineRuleDto): Promise<LateFineRuleDto> {
  const { data } = await authApiClient.post<LateFineRuleDto>("/LateFineRules", payload);
  return data;
}

export async function updateLateFineRule(id: number, payload: UpdateLateFineRuleDto): Promise<LateFineRuleDto> {
  const { data } = await authApiClient.put<LateFineRuleDto>(`/LateFineRules/${id}`, payload);
  return data;
}

export async function deleteLateFineRule(id: number): Promise<void> {
  await authApiClient.delete(`/LateFineRules/${id}`);
}