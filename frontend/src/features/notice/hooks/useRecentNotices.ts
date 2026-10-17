import { useQuery } from "@tanstack/react-query";
import { getRecentNotices } from "../api/notice.api";

export function useRecentNotices(count = 5) {
  return useQuery({
    queryKey: ["notice", "recent", count],
    queryFn: () => getRecentNotices(count),
    staleTime: 60 * 1000,
    retry: 1,
  });
}
