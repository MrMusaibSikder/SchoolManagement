import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createFeeCategory, deleteFeeCategory, getFeeCategories, updateFeeCategory } from "../api/fee-category.api";
import type { CreateFeeCategoryDto, UpdateFeeCategoryDto } from "../types/fee-category.types";

export function useFeeCategories() {
  return useQuery({
    queryKey: ["fee-category", "list"],
    queryFn: getFeeCategories,
    staleTime: 30_000,
  });
}

export function useCreateFeeCategory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateFeeCategoryDto) => createFeeCategory(payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["fee-category", "list"] });
    },
  });
}

export function useUpdateFeeCategory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateFeeCategoryDto }) => updateFeeCategory(id, payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["fee-category", "list"] });
    },
  });
}

export function useDeleteFeeCategory() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteFeeCategory(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["fee-category", "list"] });
    },
  });
}
