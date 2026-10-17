import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createSmsTemplate,
  deleteSmsLog,
  deleteSmsTemplate,
  getSmsDashboardStats,
  getSmsLogs,
  getSmsTemplates,
  previewSmsTemplate,
  setSmsTemplateActive,
  updateSmsTemplate,
} from "../api/sms.api";
import type {
  CreateSmsTemplateDto,
  PlaceholderDataDto,
  SmsLogFilters,
  SmsTemplateFilters,
  UpdateSmsTemplateDto,
} from "../types/sms.types";

export function useSmsLogs(filters: SmsLogFilters, enabled = true) {
  return useQuery({
    queryKey: ["sms", "logs", filters],
    queryFn: () => getSmsLogs(filters),
    enabled,
    staleTime: 30_000,
    retry: 1,
  });
}

export function useSmsDashboardStats(enabled = true) {
  return useQuery({
    queryKey: ["sms", "dashboard"],
    queryFn: getSmsDashboardStats,
    enabled,
    staleTime: 60_000,
    retry: 1,
  });
}

export function useSmsTemplates(filters: SmsTemplateFilters, enabled = true) {
  return useQuery({
    queryKey: ["sms", "templates", filters],
    queryFn: () => getSmsTemplates(filters),
    enabled,
    staleTime: 30_000,
    retry: 1,
  });
}

function invalidateSmsQueries(queryClient: ReturnType<typeof useQueryClient>) {
  void queryClient.invalidateQueries({ queryKey: ["sms"] });
}

export function useCreateSmsTemplate() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateSmsTemplateDto) => createSmsTemplate(payload),
    onSuccess: () => invalidateSmsQueries(queryClient),
  });
}

export function useUpdateSmsTemplate() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateSmsTemplateDto }) =>
      updateSmsTemplate(id, payload),
    onSuccess: () => invalidateSmsQueries(queryClient),
  });
}

export function useSetSmsTemplateActive() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, isActive }: { id: number; isActive: boolean }) =>
      setSmsTemplateActive(id, isActive),
    onSuccess: () => invalidateSmsQueries(queryClient),
  });
}

export function useDeleteSmsTemplate() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteSmsTemplate(id),
    onSuccess: () => invalidateSmsQueries(queryClient),
  });
}

export function usePreviewSmsTemplate() {
  return useMutation({
    mutationFn: ({
      id,
      values,
    }: {
      id: number;
      values: PlaceholderDataDto;
    }) => previewSmsTemplate(id, values),
  });
}

export function useDeleteSmsLog() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteSmsLog(id),
    onSuccess: () => invalidateSmsQueries(queryClient),
  });
}
