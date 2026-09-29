import { authApiClient } from "@/lib/api/auth-client";
import type { ReceiptDto, VoidReceiptDto } from "../types/receipt.types";

export async function getReceiptById(id: number): Promise<ReceiptDto> {
  const { data } = await authApiClient.get<ReceiptDto>(`/Receipts/${id}`);
  return data;
}

export async function getReceiptByPaymentId(paymentId: number): Promise<ReceiptDto> {
  const { data } = await authApiClient.get<ReceiptDto>(`/Receipts/payment/${paymentId}`);
  return data;
}

export async function downloadReceiptPdf(id: number): Promise<Blob> {
  const { data } = await authApiClient.get<Blob>(`/Receipts/${id}/pdf`, { responseType: "blob" });
  return data;
}

export async function voidReceipt(id: number, payload: VoidReceiptDto): Promise<void> {
  await authApiClient.post(`/Receipts/${id}/void`, payload);
}