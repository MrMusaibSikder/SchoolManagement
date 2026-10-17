export type NoticePriority = 1 | 2 | 3;
export type NoticeAudience = 1 | 2 | 3 | 4 | 5;

export interface NoticeDto {
  id: number;
  title: string;
  description: string;
  publishDate: string;
  expiryDate: string | null;
  priority: NoticePriority;
  audience: NoticeAudience;
  isPublished: boolean;
  isArchived: boolean;
  sendSms: boolean;
  sendEmail: boolean;
  attachmentPath: string | null;
  isActive: boolean;
  isUpcoming: boolean;
  isExpired: boolean;
  createdAt: string;
}

export type RecentNoticeDto = NoticeDto;

export interface NoticeFilters {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string;
  priority?: NoticePriority;
  audience?: NoticeAudience;
  isPublished?: boolean;
  isArchived?: boolean;
}

export interface CreateNoticePayload {
  title: string;
  description: string;
  publishDate: string;
  expiryDate: string | null;
  priority: NoticePriority;
  audience: NoticeAudience;
}

export interface PagedNotices {
  items: NoticeDto[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
