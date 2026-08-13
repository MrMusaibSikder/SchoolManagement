/**
 * Matches the backend's /api/public/* DTOs exactly.
 * See .ai/references/API_REFERENCE.md → "Public (Anonymous) Endpoints".
 */

export interface PublicSchoolInfoDto {
  name: string;
  address?: string | null;
  logoUrl?: string | null;
  phone?: string | null;
  email?: string | null;
}

export interface PublicStatsDto {
  totalStudents: number;
  totalTeachers: number;
  totalEmployees: number;
}

/**
 * `priority` is the backend's NoticePriority enum serialized as a string
 * (e.g. via .ToString()). The exact member names have not been confirmed
 * against NoticePriority.cs yet — treat this as an opaque label, not a
 * known closed set, until that's verified. See pinColorFor() in
 * NoticeBoard.tsx for how this is handled defensively.
 */
export interface PublicNoticeDto {
  id: number;
  title: string;
  summary?: string | null;
  publishDate: string; // ISO date string
  priority: string;
}
