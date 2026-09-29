import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createLateFineRule, deleteLateFineRule, getLateFineRules, updateLateFineRule } from "../api/late-fine-rule.api";
import type { CreateLateFineRuleDto, UpdateLateFineRuleDto } from "../types/late-fine-rule.types";

export function useLateFineRules(academicYearId: number | null, enabled = true) {
  return useQuery({
    queryKey: ["late-fine-rule", academicYearId],
    queryFn: () => getLateFineRules(academicYearId as number),
    enabled: enabled && academicYearId !== null,
    staleTime: 30_000,
  });
}

function invalidateRules(queryClient: ReturnType<typeof useQueryClient>) {
  void queryClient.invalidateQueries({ queryKey: ["late-fine-rule"] });
}

export function useCreateLateFineRule() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateLateFineRuleDto) => createLateFineRule(payload),
    onSuccess: () => invalidateRules(queryClient),
  });
}

export function useUpdateLateFineRule() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateLateFineRuleDto }) => updateLateFineRule(id, payload),
    onSuccess: () => invalidateRules(queryClient),
  });
}

export function useDeleteLateFineRule() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteLateFineRule(id),
    onSuccess: () => invalidateRules(queryClient),
  });
}