import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createFeeType, deleteFeeType, getFeeTypeById, getFeeTypes, updateFeeType } from "../api/fee-type.api";
import type { CreateFeeTypeDto, UpdateFeeTypeDto } from "../types/fee-type.types";

export function useFeeTypes() {
  return useQuery({
    queryKey: ["fee-type", "list"],
    queryFn: getFeeTypes,
    staleTime: 30_000,
  });
}

export function useFeeType(id: number) {
  return useQuery({
    queryKey: ["fee-type", id],
    queryFn: () => getFeeTypeById(id),
    enabled: Boolean(id),
    staleTime: 30_000,
  });
}

export function useCreateFeeType() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateFeeTypeDto) => createFeeType(payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["fee-type", "list"] });
    },
  });
}

export function useUpdateFeeType() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateFeeTypeDto }) => updateFeeType(id, payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["fee-type", "list"] });
      void queryClient.invalidateQueries({ queryKey: ["fee-type"] });
    },
  });
}

export function useDeleteFeeType() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteFeeType(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["fee-type", "list"] });
    },
  });
}
