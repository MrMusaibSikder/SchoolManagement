import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  bulkSaveEmployeeAttendance,
  bulkSaveStudentAttendance,
  getAdminAttendanceDashboard,
  getAttendanceTrend,
  getClassAttendanceSummary,
  getEmployeeAttendanceByDate,
  getStudentAttendanceByClassSection,
  getTodayAttendanceDashboard,
} from "../api/attendance.api";
import type {
  BulkEmployeeAttendanceDto,
  BulkStudentAttendanceDto,
} from "../types/attendance.types";

export function useStudentAttendanceSheet(params: {
  classId: number | null;
  sectionId: number | null;
  attendanceDate: string;
}) {
  const enabled = Boolean(params.classId && params.sectionId && params.attendanceDate);
  return useQuery({
    queryKey: [
      "attendance",
      "students",
      params.classId,
      params.sectionId,
      params.attendanceDate,
    ],
    queryFn: () =>
      getStudentAttendanceByClassSection({
        classId: params.classId as number,
        sectionId: params.sectionId as number,
        attendanceDate: params.attendanceDate,
      }),
    enabled,
    staleTime: 15_000,
  });
}

export function useBulkStudentAttendance() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: BulkStudentAttendanceDto) => bulkSaveStudentAttendance(payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["attendance"] });
      void queryClient.invalidateQueries({ queryKey: ["dashboard"] });
      void queryClient.invalidateQueries({ queryKey: ["student", "attendance-history"] });
    },
  });
}

export function useEmployeeAttendanceSheet(attendanceDate: string) {
  return useQuery({
    queryKey: ["attendance", "employees", attendanceDate],
    queryFn: () => getEmployeeAttendanceByDate(attendanceDate),
    enabled: Boolean(attendanceDate),
    staleTime: 15_000,
  });
}

export function useBulkEmployeeAttendance() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: BulkEmployeeAttendanceDto) => bulkSaveEmployeeAttendance(payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["attendance"] });
      void queryClient.invalidateQueries({ queryKey: ["dashboard"] });
    },
  });
}

export function useTodayAttendanceDashboard(enabled: boolean) {
  return useQuery({
    queryKey: ["attendance", "report", "today"],
    queryFn: getTodayAttendanceDashboard,
    enabled,
    staleTime: 30_000,
  });
}

export function useClassAttendanceSummary(
  params: { classId: number | null; sectionId: number | null; attendanceDate: string },
  enabled: boolean
) {
  return useQuery({
    queryKey: [
      "attendance",
      "report",
      "class",
      params.classId,
      params.sectionId,
      params.attendanceDate,
    ],
    queryFn: () =>
      getClassAttendanceSummary({
        classId: params.classId as number,
        sectionId: params.sectionId as number,
        attendanceDate: params.attendanceDate,
      }),
    enabled: enabled && Boolean(params.classId && params.sectionId && params.attendanceDate),
    staleTime: 15_000,
  });
}

export function useAdminAttendanceDashboard(
  params: { fromDate: string; toDate: string },
  enabled: boolean
) {
  return useQuery({
    queryKey: ["attendance", "report", "admin", params.fromDate, params.toDate],
    queryFn: () => getAdminAttendanceDashboard(params),
    enabled: enabled && Boolean(params.fromDate && params.toDate),
    staleTime: 30_000,
  });
}

export function useAttendanceTrend(
  params: { fromDate: string; toDate: string },
  enabled: boolean
) {
  return useQuery({
    queryKey: ["attendance", "report", "trend", params.fromDate, params.toDate],
    queryFn: () => getAttendanceTrend(params),
    enabled: enabled && Boolean(params.fromDate && params.toDate),
    staleTime: 30_000,
  });
}
