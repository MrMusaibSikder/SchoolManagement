import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  cancelInvoice,
  createInvoice,
  applyLateFines,
  generateMonthlyInvoices,
  getInvoiceById,
  getInvoices,
} from "../api/invoice.api";
import type {
  CancelInvoiceDto,
  CreateInvoiceDto,
  GenerateMonthlyInvoicesDto,
  InvoiceFilters,
} from "../types/invoice.types";

export function useInvoices(filters: InvoiceFilters, enabled = true) {
  return useQuery({
    queryKey: ["invoice", "list", filters],
    queryFn: () => getInvoices(filters),
    enabled,
    staleTime: 15_000,
  });
}

export function useInvoice(id: number | null, enabled = true) {
  return useQuery({
    queryKey: ["invoice", "detail", id],
    queryFn: () => getInvoiceById(id as number),
    enabled: enabled && id !== null,
    staleTime: 15_000,
  });
}

function invalidateInvoices(queryClient: ReturnType<typeof useQueryClient>) {
  void queryClient.invalidateQueries({ queryKey: ["invoice"] });
}

export function useCreateInvoice() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateInvoiceDto) => createInvoice(payload),
    onSuccess: () => invalidateInvoices(queryClient),
  });
}

export function useCancelInvoice() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: CancelInvoiceDto }) => cancelInvoice(id, payload),
    onSuccess: () => invalidateInvoices(queryClient),
    onError: () => invalidateInvoices(queryClient),
  });
}

export function useGenerateMonthlyInvoices() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: GenerateMonthlyInvoicesDto) => generateMonthlyInvoices(payload),
    onSuccess: () => invalidateInvoices(queryClient),
  });
}

export function useApplyLateFines() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (asOfDate?: string) => applyLateFines(asOfDate),
    onSuccess: () => invalidateInvoices(queryClient),
  });
}