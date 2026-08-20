export interface PublicStatsSummary {
  totalStudents: number;
  totalTeachers: number;
  totalEmployees: number;
}

export interface InvoiceListItem {
  id?: number;
  invoiceNumber?: string | null;
  studentName?: string | null;
  dueDate?: string | null;
  totalAmount?: number | null;
  balanceDue?: number | null;
  status?: string | null;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface UpcomingExamItem {
  examId?: number;
  examName?: string | null;
  examTypeName?: string | null;
  nextExamDate?: string | null;
  daysRemaining?: number | null;
  totalSchedules?: number | null;
}

export interface NoticeListItem {
  id?: number;
  title?: string | null;
  description?: string | null;
  publishDate?: string | null;
  expiryDate?: string | null;
  priority?: string | null;
}

export interface AttendanceSummaryDto {
  totalStudents?: number;
  totalPresent?: number;
  totalAbsent?: number;
  totalLate?: number;
  totalLeave?: number;
  attendancePercentage?: number;
}

export interface DashboardData {
  stats: PublicStatsSummary;
  invoices: InvoiceListItem[];
  upcomingExams: UpcomingExamItem[];
  notices: NoticeListItem[];
  attendance: AttendanceSummaryDto | null;
}
