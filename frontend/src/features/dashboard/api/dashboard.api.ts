import { authApiClient } from "@/lib/api/auth-client";
import type {
  AttendanceSummaryDto,
  DashboardData,
  EmployeeListItem,
  InvoiceListItem,
  NoticeListItem,
  StudentListItem,
  TeacherListItem,
  UpcomingExamItem,
} from "../types/dashboard.types";

async function getJson<T>(path: string): Promise<T> {
  const { data } = await authApiClient.get<T>(path);
  return data;
}

export async function getDashboardData(): Promise<DashboardData> {
  const [students, teachers, employees, invoices, upcomingExams, notices, attendance] =
    await Promise.all([
      getJson<StudentListItem[]>("/Students").catch(() => []),
      getJson<TeacherListItem[]>("/Teachers").catch(() => []),
      getJson<EmployeeListItem[]>("/Employees").catch(() => []),
      getJson<InvoiceListItem[]>("/invoices").catch(() => []),
      getJson<UpcomingExamItem[]>("/Exam/upcoming?count=5").catch(() => []),
      getJson<NoticeListItem[]>("/Notice/recent?count=5").catch(() => []),
      getJson<AttendanceSummaryDto>("/AttendanceReport/admin-dashboard").catch(() => null),
    ]);

  return {
    students,
    teachers,
    employees,
    invoices,
    upcomingExams,
    notices,
    attendance,
  };
}
