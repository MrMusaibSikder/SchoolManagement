import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { downloadReceiptPdf, getReceiptById, getReceiptByPaymentId, voidReceipt } from "../api/receipt.api";
import type { VoidReceiptDto } from "../types/receipt.types";

export function useReceiptById(id: number | null) {
  return useQuery({
    queryKey: ["receipt", "detail", id],
    queryFn: () => getReceiptById(id as number),
    enabled: id !== null,
    staleTime: 30_000,
  });
}

export function useReceiptByPaymentId(paymentId: number | null) {
  return useQuery({
    queryKey: ["receipt", "payment", paymentId],
    queryFn: () => getReceiptByPaymentId(paymentId as number),
    enabled: paymentId !== null,
    staleTime: 30_000,
  });
}

export function useDownloadReceiptPdf() {
  return useMutation({ mutationFn: (id: number) => downloadReceiptPdf(id) });
}

export function useVoidReceipt() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: VoidReceiptDto }) => voidReceipt(id, payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["receipt"] });
    },
  });
}