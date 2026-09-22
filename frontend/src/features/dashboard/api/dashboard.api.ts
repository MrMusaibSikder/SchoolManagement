import { authApiClient } from "@/lib/api/auth-client";
import { publicApiClient } from "@/lib/api/public-client";
import type { DashboardAccess } from "../config/dashboard-access";
import type {
  AttendanceSummaryDto,
  DashboardData,
  InvoiceListItem,
  NoticeListItem,
  PagedResult,
  PublicStatsSummary,
  UpcomingExamItem,
} from "../types/dashboard.types";

interface DashboardAttendanceDto {
  totalStudents: number;
  presentToday: number;
  absentToday: number;
  lateToday: number;
  leaveToday: number;
  attendancePercentage: number;
}

async function getAuthJson<T>(
  path: string,
  config?: Parameters<typeof authApiClient.get>[1]
): Promise<T> {
  const { data } = await authApiClient.get<T>(path, config);
  return data;
}

async function getPublicJson<T>(path: string): Promise<T> {
  const { data } = await publicApiClient.get<T>(path);
  return data;
}

const EMPTY_STATS: PublicStatsSummary = {
  totalStudents: 0,
  totalTeachers: 0,
  totalEmployees: 0,
};

export async function getDashboardData(access: DashboardAccess): Promise<DashboardData> {
  const needsStats =
    access.canViewStudentStats ||
    access.canViewTeacherStats ||
    access.canViewEmployeeStats;

  const [stats, invoicePage, upcomingExams, notices, attendance] =
    await Promise.all([
      needsStats
        ? getPublicJson<PublicStatsSummary>("/public/stats").catch(() => EMPTY_STATS)
        : Promise.resolve(EMPTY_STATS),
      access.canViewFees
        ? getAuthJson<PagedResult<InvoiceListItem>>("/Invoices", {
            params: { pageNumber: 1, pageSize: 5 },
          }).catch(() => ({
            items: [],
            totalCount: 0,
            pageNumber: 1,
            pageSize: 5,
          }))
        : Promise.resolve({
            items: [],
            totalCount: 0,
            pageNumber: 1,
            pageSize: 5,
          }),
      access.canViewExams
        ? getAuthJson<UpcomingExamItem[]>("/Exam/upcoming", {
            params: { count: 5 },
          }).catch(() => [])
        : Promise.resolve([]),
      access.canViewNotices
        ? getAuthJson<NoticeListItem[]>("/Notice/recent", {
            params: { count: 5 },
          }).catch(() => [])
        : Promise.resolve([]),
      access.canViewAttendance
        ? getAuthJson<DashboardAttendanceDto>("/AttendanceReport/dashboard")
            .then((item): AttendanceSummaryDto => ({
              totalStudents: item.totalStudents,
              totalPresent: item.presentToday,
              totalAbsent: item.absentToday,
              totalLate: item.lateToday,
              totalLeave: item.leaveToday,
              attendancePercentage: item.attendancePercentage,
            }))
            .catch(() => null)
        : Promise.resolve(null),
    ]);

  return {
    stats,
    invoices: invoicePage.items ?? [],
    upcomingExams,
    notices,
    attendance,
  };
}
