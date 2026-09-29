import { authApiClient } from "@/lib/api/auth-client";
import type { DefaulterReportDto, FeeCollectionSummaryDto } from "../types/fee-reports.types";

export async function getCollectionSummary(dateFrom: string, dateTo: string): Promise<FeeCollectionSummaryDto> {
  const { data } = await authApiClient.get<FeeCollectionSummaryDto>("/FeeReports/collection-summary", {
    params: { dateFrom, dateTo },
  });
  return data;
}

export async function getDefaulters(asOfDate?: string, schoolClassId?: number | null): Promise<DefaulterReportDto> {
  const { data } = await authApiClient.get<DefaulterReportDto>("/FeeReports/defaulters", {
    params: {
      ...(asOfDate ? { asOfDate } : {}),
      ...(schoolClassId ? { schoolClassId } : {}),
    },
  });
  return data;
}
