import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { createNotice, getNotices, publishNotice } from "../api/notice.api";
import type {
  CreateNoticePayload,
  NoticeFilters,
} from "../types/notice.types";

export function useNotices(filters: NoticeFilters, enabled = true) {
  return useQuery({
    queryKey: ["notice", "list", filters],
    queryFn: () => getNotices(filters),
    enabled,
    staleTime: 30 * 1000,
    retry: 1,
  });
}

function invalidateNoticeQueries(queryClient: ReturnType<typeof useQueryClient>) {
  void queryClient.invalidateQueries({ queryKey: ["notice"] });
  void queryClient.invalidateQueries({ queryKey: ["dashboard", "overview"] });
  void queryClient.invalidateQueries({ queryKey: ["public", "notices"] });
}

export function useCreateNotice() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (payload: CreateNoticePayload) => createNotice(payload),
    onSuccess: () => invalidateNoticeQueries(queryClient),
  });
}

export function usePublishNotice() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (noticeId: number) => publishNotice(noticeId),
    onSuccess: () => invalidateNoticeQueries(queryClient),
  });
}
