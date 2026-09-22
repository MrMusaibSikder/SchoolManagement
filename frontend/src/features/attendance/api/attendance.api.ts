import { authApiClient } from "@/lib/api/auth-client";
import type {
  AdminDashboardAttendanceDto,
  AttendanceTrendDto,
  BulkEmployeeAttendanceDto,
  BulkStudentAttendanceDto,
  ClassAttendanceSummaryDto,
  DashboardAttendanceDto,
  EmployeeAttendanceDto,
  StudentAttendanceDto,
} from "../types/attendance.types";

export async function getStudentAttendanceByClassSection(params: {
  classId: number;
  sectionId: number;
  attendanceDate: string;
}): Promise<StudentAttendanceDto[]> {
  const { data } = await authApiClient.get<StudentAttendanceDto[]>(
    "/StudentAttendance/class-section",
    { params }
  );
  return data;
}

export async function bulkSaveStudentAttendance(
  payload: BulkStudentAttendanceDto
): Promise<{ message: string }> {
  const { data } = await authApiClient.post<{ message: string }>(
    "/StudentAttendance/bulk",
    payload
  );
  return data;
}

export async function getEmployeeAttendanceByDate(
  attendanceDate: string
): Promise<EmployeeAttendanceDto[]> {
  const { data } = await authApiClient.get<EmployeeAttendanceDto[]>(
    "/EmployeeAttendance/by-date",
    { params: { attendanceDate } }
  );
  return data;
}

export async function bulkSaveEmployeeAttendance(
  payload: BulkEmployeeAttendanceDto
): Promise<{ message: string }> {
  const { data } = await authApiClient.post<{ message: string }>(
    "/EmployeeAttendance/bulk",
    payload
  );
  return data;
}

export async function getTodayAttendanceDashboard(): Promise<DashboardAttendanceDto> {
  const { data } = await authApiClient.get<DashboardAttendanceDto>(
    "/AttendanceReport/dashboard"
  );
  return data;
}

export async function getClassAttendanceSummary(params: {
  classId: number;
  sectionId: number;
  attendanceDate: string;
}): Promise<ClassAttendanceSummaryDto> {
  const { data } = await authApiClient.get<ClassAttendanceSummaryDto>(
    "/AttendanceReport/class-summary",
    { params }
  );
  return data;
}

export async function getAdminAttendanceDashboard(params: {
  fromDate: string;
  toDate: string;
}): Promise<AdminDashboardAttendanceDto> {
  const { data } = await authApiClient.get<AdminDashboardAttendanceDto>(
    "/AttendanceReport/admin-dashboard",
    { params }
  );
  return data;
}

export async function getAttendanceTrend(params: {
  fromDate: string;
  toDate: string;
}): Promise<AttendanceTrendDto[]> {
  const { data } = await authApiClient.get<AttendanceTrendDto[]>(
    "/AttendanceReport/trend",
    { params }
  );
  return data;
}
