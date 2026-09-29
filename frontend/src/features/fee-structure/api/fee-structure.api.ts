import { authApiClient } from "@/lib/api/auth-client";
import type {
  CreateFeeStructureDto,
  FeeStructureDto,
  FeeStructureFilters,
  FeeStructureListDto,
  UpdateFeeStructureDto,
} from "../types/fee-structure.types";

export async function getFeeStructures(filters: FeeStructureFilters = {}): Promise<FeeStructureListDto[]> {
  const { data } = await authApiClient.get<FeeStructureListDto[]>("/FeeStructures", { params: filters });
  return data;
}

export async function getFeeStructureById(id: number): Promise<FeeStructureDto> {
  const { data } = await authApiClient.get<FeeStructureDto>(`/FeeStructures/${id}`);
  return data;
}

export async function createFeeStructure(payload: CreateFeeStructureDto): Promise<FeeStructureDto> {
  const { data } = await authApiClient.post<FeeStructureDto>("/FeeStructures", payload);
  return data;
}

export async function updateFeeStructure(id: number, payload: UpdateFeeStructureDto): Promise<FeeStructureDto> {
  const { data } = await authApiClient.put<FeeStructureDto>(`/FeeStructures/${id}`, payload);
  return data;
}

export async function deleteFeeStructure(id: number): Promise<void> {
  await authApiClient.delete(`/FeeStructures/${id}`);
}