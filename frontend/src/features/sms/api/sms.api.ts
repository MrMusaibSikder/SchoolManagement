import { authApiClient } from "@/lib/api/auth-client";
import type {
  CreateSmsTemplateDto,
  PagedResult,
  PlaceholderDataDto,
  RenderedSmsTemplateDto,
  SmsDashboardStatsDto,
  SmsLogDto,
  SmsLogFilters,
  SmsTemplateDto,
  SmsTemplateFilters,
  UpdateSmsTemplateDto,
} from "../types/sms.types";

export async function getSmsLogs(
  filters: SmsLogFilters
): Promise<PagedResult<SmsLogDto>> {
  const { data } = await authApiClient.get<PagedResult<SmsLogDto>>("/SmsLog", {
    params: filters,
  });
  return data;
}

export async function getSmsDashboardStats(): Promise<SmsDashboardStatsDto> {
  const { data } = await authApiClient.get<SmsDashboardStatsDto>(
    "/SmsLog/dashboard"
  );
  return data;
}

export async function deleteSmsLog(id: number): Promise<void> {
  await authApiClient.delete(`/SmsLog/${id}`);
}

export async function getSmsTemplates(
  filters: SmsTemplateFilters
): Promise<PagedResult<SmsTemplateDto>> {
  const { data } = await authApiClient.get<PagedResult<SmsTemplateDto>>(
    "/SmsTemplate",
    { params: filters }
  );
  return data;
}

export async function createSmsTemplate(
  payload: CreateSmsTemplateDto
): Promise<SmsTemplateDto> {
  const { data } = await authApiClient.post<SmsTemplateDto>(
    "/SmsTemplate",
    payload
  );
  return data;
}

export async function updateSmsTemplate(
  id: number,
  payload: UpdateSmsTemplateDto
): Promise<SmsTemplateDto> {
  const { data } = await authApiClient.put<SmsTemplateDto>(
    `/SmsTemplate/${id}`,
    payload
  );
  return data;
}

export async function setSmsTemplateActive(
  id: number,
  isActive: boolean
): Promise<SmsTemplateDto> {
  const action = isActive ? "activate" : "deactivate";
  const { data } = await authApiClient.patch<SmsTemplateDto>(
    `/SmsTemplate/${id}/${action}`
  );
  return data;
}

export async function deleteSmsTemplate(id: number): Promise<void> {
  await authApiClient.delete(`/SmsTemplate/${id}`);
}

export async function previewSmsTemplate(
  id: number,
  values: PlaceholderDataDto
): Promise<RenderedSmsTemplateDto> {
  const { data } = await authApiClient.post<RenderedSmsTemplateDto>(
    `/SmsTemplate/${id}/preview`,
    { data: values }
  );
  return data;
}
