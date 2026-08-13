import { publicApiClient } from "@/lib/api/public-client";
import type {
  PublicNoticeDto,
  PublicSchoolInfoDto,
  PublicStatsDto,
} from "../types/public.types";

export async function getSchoolInfo(): Promise<PublicSchoolInfoDto> {
  const { data } = await publicApiClient.get<PublicSchoolInfoDto>(
    "/public/school-info"
  );
  return data;
}

export async function getPublicStats(): Promise<PublicStatsDto> {
  const { data } = await publicApiClient.get<PublicStatsDto>("/public/stats");
  return data;
}

export async function getPublicNotices(
  take = 5
): Promise<PublicNoticeDto[]> {
  const { data } = await publicApiClient.get<PublicNoticeDto[]>(
    "/public/notices",
    { params: { take } }
  );
  return data;
}
