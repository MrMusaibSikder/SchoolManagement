import { authApiClient } from "@/lib/api/auth-client";
import type { CreateFeeTypeDto, FeeTypeDto, FeeTypeListDto, UpdateFeeTypeDto } from "../types/fee-type.types";

export async function getFeeTypes(): Promise<FeeTypeListDto[]> {
  const { data } = await authApiClient.get<FeeTypeListDto[]>("/FeeTypes");
  return data;
}

export async function getFeeTypeById(id: number): Promise<FeeTypeDto> {
  const { data } = await authApiClient.get<FeeTypeDto>(`/FeeTypes/${id}`);
  return data;
}

export async function createFeeType(payload: CreateFeeTypeDto): Promise<FeeTypeDto> {
  const { data } = await authApiClient.post<FeeTypeDto>("/FeeTypes", payload);
  return data;
}

export async function updateFeeType(id: number, payload: UpdateFeeTypeDto): Promise<FeeTypeDto> {
  const { data } = await authApiClient.put<FeeTypeDto>(`/FeeTypes/${id}`, payload);
  return data;
}

export async function deleteFeeType(id: number): Promise<void> {
  await authApiClient.delete(`/FeeTypes/${id}`);
}
