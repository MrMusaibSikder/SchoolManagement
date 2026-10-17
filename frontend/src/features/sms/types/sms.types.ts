export type SmsStatus = 1 | 2 | 3 | 4;

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface SmsLogDto {
  id: number;
  recipientNumber: string;
  message: string;
  status: SmsStatus;
  providerResponse: string | null;
  sentAt: string | null;
  provider: string | null;
  studentId: number | null;
  studentName: string | null;
  createdAt: string;
}

export interface SmsLogFilters {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string;
  status?: SmsStatus;
  recipientNumber?: string;
  fromDate?: string;
  toDate?: string;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface SmsDashboardStatsDto {
  totalSms: number;
  todaySms: number;
  weeklySms: number;
  monthlySms: number;
  successCount: number;
  failedCount: number;
  pendingCount: number;
  successRate: number;
}

export interface SmsTemplateDto {
  id: number;
  name: string;
  message: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface SmsTemplateFilters {
  pageNumber: number;
  pageSize: number;
  searchTerm?: string;
  isActive?: boolean;
  sortBy?: string;
  sortDescending?: boolean;
}

export interface CreateSmsTemplateDto {
  name: string;
  message: string;
  isActive: boolean;
}

export interface UpdateSmsTemplateDto extends CreateSmsTemplateDto {
  id: number;
}

export interface PlaceholderDataDto {
  studentName?: string;
  guardianName?: string;
  teacherName?: string;
  employeeName?: string;
  class?: string;
  section?: string;
  roll?: string;
  attendanceStatus?: string;
  date?: string;
  time?: string;
  schoolName?: string;
}

export interface RenderedSmsTemplateDto {
  templateId: number;
  templateName: string;
  rawMessage: string;
  renderedMessage: string;
}
