import { authApiClient } from "@/lib/api/auth-client";
import type {
  NoticeFilters,
  PagedNotices,
  CreateNoticePayload,
  RecentNoticeDto,
} from "../types/notice.types";

export async function getNotices(filters: NoticeFilters): Promise<PagedNotices> {
  const { data } = await authApiClient.get<PagedNotices>("/Notice", {
    params: filters,
  });
  return data;
}

export async function getRecentNotices(count = 5): Promise<RecentNoticeDto[]> {
  const { data } = await authApiClient.get<RecentNoticeDto[]>("/Notice/recent", {
    params: { count },
  });
  return data;
}

export async function createNotice(
  payload: CreateNoticePayload
): Promise<RecentNoticeDto> {
  const formData = new FormData();
  formData.set("Title", payload.title);
  formData.set("Description", payload.description);
  formData.set("PublishDate", payload.publishDate);
  if (payload.expiryDate) formData.set("ExpiryDate", payload.expiryDate);
  formData.set("Priority", String(payload.priority));
  formData.set("Audience", String(payload.audience));

  const { data } = await authApiClient.post<RecentNoticeDto>("/Notice", formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return data;
}

export async function publishNotice(id: number): Promise<RecentNoticeDto> {
  const { data } = await authApiClient.post<RecentNoticeDto>(
    `/Notice/${id}/publish`
  );
  return data;
}
