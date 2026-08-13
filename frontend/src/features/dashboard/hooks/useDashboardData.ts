import { useQuery } from "@tanstack/react-query";
import { getDashboardData } from "../api/dashboard.api";

export function useDashboardData() {
  return useQuery({
    queryKey: ["dashboard", "overview"],
    queryFn: getDashboardData,
    staleTime: 60 * 1000,
    retry: 1,
  });
}
