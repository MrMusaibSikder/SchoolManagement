import { authApiClient } from "@/lib/api/auth-client";
import type { CreateFeeCategoryDto, FeeCategoryDto, UpdateFeeCategoryDto } from "../types/fee-category.types";

export async function getFeeCategories(): Promise<FeeCategoryDto[]> {
  const { data } = await authApiClient.get<FeeCategoryDto[]>("/FeeCategories");
  return data;
}

export async function createFeeCategory(payload: CreateFeeCategoryDto): Promise<FeeCategoryDto> {
  const { data } = await authApiClient.post<FeeCategoryDto>("/FeeCategories", payload);
  return data;
}

export async function updateFeeCategory(id: number, payload: UpdateFeeCategoryDto): Promise<FeeCategoryDto> {
  const { data } = await authApiClient.put<FeeCategoryDto>(`/FeeCategories/${id}`, payload);
  return data;
}

export async function deleteFeeCategory(id: number): Promise<void> {
  await authApiClient.delete(`/FeeCategories/${id}`);
}
