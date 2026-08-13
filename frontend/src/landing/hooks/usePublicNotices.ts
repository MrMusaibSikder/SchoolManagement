import { useQuery } from "@tanstack/react-query";
import { getPublicNotices } from "../api/public.api";

export function usePublicNotices(take = 5) {
  return useQuery({
    queryKey: ["public", "notices", take],
    queryFn: () => getPublicNotices(take),
    staleTime: 60 * 1000,
  });
}
