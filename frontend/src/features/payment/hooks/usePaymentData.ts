import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { collectPayment, getPaymentById, getPaymentsByInvoice, voidPayment } from "../api/payment.api";
import type { CreatePaymentDto, VoidPaymentDto } from "../types/payment.types";

export function useInvoicePayments(invoiceId: number | null, enabled = true) {
  return useQuery({
    queryKey: ["payment", "invoice", invoiceId],
    queryFn: () => getPaymentsByInvoice(invoiceId as number),
    enabled: enabled && invoiceId !== null,
    staleTime: 15_000,
  });
}

export function usePayment(id: number | null) {
  return useQuery({
    queryKey: ["payment", "detail", id],
    queryFn: () => getPaymentById(id as number),
    enabled: id !== null,
    staleTime: 15_000,
  });
}

function invalidatePaymentData(queryClient: ReturnType<typeof useQueryClient>) {
  void queryClient.invalidateQueries({ queryKey: ["payment"] });
  void queryClient.invalidateQueries({ queryKey: ["invoice"] });
}

export function useCollectPayment() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreatePaymentDto) => collectPayment(payload),
    onSuccess: () => invalidatePaymentData(queryClient),
    onError: () => invalidatePaymentData(queryClient),
  });
}

export function useVoidPayment() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: VoidPaymentDto }) => voidPayment(id, payload),
    onSuccess: () => invalidatePaymentData(queryClient),
    onError: () => invalidatePaymentData(queryClient),
  });
}