import { useQuery } from "@tanstack/react-query";
import { getSchoolInfo } from "../api/public.api";

export function useSchoolInfo() {
  return useQuery({
    queryKey: ["public", "school-info"],
    queryFn: getSchoolInfo,
    staleTime: 5 * 60 * 1000, // school profile rarely changes
  });
}
