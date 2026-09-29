import { authApiClient } from "@/lib/api/auth-client";
import type { CreatePaymentDto, PaymentDto, PaymentListDto, VoidPaymentDto } from "../types/payment.types";

export async function getPaymentById(id: number): Promise<PaymentDto> {
  const { data } = await authApiClient.get<PaymentDto>(`/Payments/${id}`);
  return data;
}

export async function getPaymentsByInvoice(invoiceId: number): Promise<PaymentListDto[]> {
  const { data } = await authApiClient.get<PaymentListDto[]>(`/Payments/invoice/${invoiceId}`);
  return data;
}

export async function collectPayment(payload: CreatePaymentDto): Promise<PaymentDto> {
  const { data } = await authApiClient.post<PaymentDto>("/Payments", payload);
  return data;
}

export async function voidPayment(id: number, payload: VoidPaymentDto): Promise<void> {
  await authApiClient.post(`/Payments/${id}/void`, payload);
}