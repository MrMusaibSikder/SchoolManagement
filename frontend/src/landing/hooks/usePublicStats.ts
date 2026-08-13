import { useQuery } from "@tanstack/react-query";
import { getPublicStats } from "../api/public.api";

export function usePublicStats() {
  return useQuery({
    queryKey: ["public", "stats"],
    queryFn: getPublicStats,
    staleTime: 60 * 1000,
  });
}
