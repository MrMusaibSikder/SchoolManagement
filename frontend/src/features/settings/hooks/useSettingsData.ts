import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getSchool, updateSchool } from "../api/settings.api";

export function useSchoolSettings() {
  return useQuery({
    queryKey: ["settings", "school"],
    queryFn: getSchool,
    staleTime: 60_000,
  });
}

export function useUpdateSchool() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      payload,
      logoFile,
    }: {
      payload: Parameters<typeof updateSchool>[0];
      logoFile?: File | null;
    }) => updateSchool(payload, logoFile),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["settings", "school"] });
    },
  });
}
