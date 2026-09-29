import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createFeeStructure,
  deleteFeeStructure,
  getFeeStructureById,
  getFeeStructures,
  updateFeeStructure,
} from "../api/fee-structure.api";
import type {
  CreateFeeStructureDto,
  FeeStructureFilters,
  UpdateFeeStructureDto,
} from "../types/fee-structure.types";

export function loadFeeStructure(id: number) {
  return getFeeStructureById(id);
}

export function useFeeStructures(filters: FeeStructureFilters = {}) {
  return useQuery({
    queryKey: ["fee-structure", "list", filters],
    queryFn: () => getFeeStructures(filters),
    staleTime: 30_000,
  });
}

export function useFeeStructure(id: number | null) {
  return useQuery({
    queryKey: ["fee-structure", "detail", id],
    queryFn: () => getFeeStructureById(id as number),
    enabled: id !== null,
    staleTime: 30_000,
  });
}

export function useCreateFeeStructure() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateFeeStructureDto) => createFeeStructure(payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["fee-structure", "list"] });
    },
  });
}

export function useUpdateFeeStructure() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateFeeStructureDto }) => updateFeeStructure(id, payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["fee-structure"] });
    },
  });
}

export function useDeleteFeeStructure() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteFeeStructure(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["fee-structure", "list"] });
    },
  });
}