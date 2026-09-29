import { authApiClient } from "@/lib/api/auth-client";
import type {
  CancelInvoiceDto,
  CreateInvoiceDto,
  GenerateMonthlyInvoicesDto,
  InvoiceDto,
  InvoiceFilters,
  InvoiceGenerationResultDto,
  InvoiceListDto,
  PagedResult,
} from "../types/invoice.types";

export async function getInvoices(filters: InvoiceFilters): Promise<PagedResult<InvoiceListDto>> {
  const { data } = await authApiClient.get<PagedResult<InvoiceListDto>>("/Invoices", { params: filters });
  return data;
}

export async function getInvoiceById(id: number): Promise<InvoiceDto> {
  const { data } = await authApiClient.get<InvoiceDto>(`/Invoices/${id}`);
  return data;
}

export async function createInvoice(payload: CreateInvoiceDto): Promise<InvoiceDto> {
  const { data } = await authApiClient.post<InvoiceDto>("/Invoices", payload);
  return data;
}

export async function cancelInvoice(id: number, payload: CancelInvoiceDto): Promise<InvoiceDto> {
  const { data } = await authApiClient.post<InvoiceDto>(`/Invoices/${id}/cancel`, payload);
  return data;
}

export async function generateMonthlyInvoices(payload: GenerateMonthlyInvoicesDto): Promise<InvoiceGenerationResultDto> {
  const { data } = await authApiClient.post<InvoiceGenerationResultDto>("/Invoices/generate-monthly", payload);
  return data;
}