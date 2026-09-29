import { useQuery } from "@tanstack/react-query";
import { getCollectionSummary, getDefaulters } from "../api/fee-reports.api";

export function useCollectionSummaryReport(dateFrom: string, dateTo: string, enabled = true) {
  return useQuery({
    queryKey: ["fee-report", "collection-summary", dateFrom, dateTo],
    queryFn: () => getCollectionSummary(dateFrom, dateTo),
    enabled: enabled && Boolean(dateFrom) && Boolean(dateTo),
    staleTime: 30_000,
  });
}

export function useDefaulterReport(asOfDate: string, schoolClassId: number | null, enabled = true) {
  return useQuery({
    queryKey: ["fee-report", "defaulters", asOfDate, schoolClassId],
    queryFn: () => getDefaulters(asOfDate, schoolClassId),
    enabled: enabled && Boolean(asOfDate),
    staleTime: 30_000,
  });
}
