import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  calculateExamResults,
  calculateFinalResults,
  getExamResultDashboard,
  getExamResults,
  getFinalResults,
  getMarkEntriesBySchedule,
  getStudentFinalResult,
  lockMarkEntries,
  publishExamResults,
  publishFinalResults,
  saveBulkMarkEntries,
  submitMarkEntries,
  unpublishExamResults,
  unpublishFinalResults,
  unlockMarkEntries,
} from "../api/result.api";
import type { BulkMarkEntryDto } from "../types/result.types";

function invalidateResults(queryClient: ReturnType<typeof useQueryClient>) {
  void queryClient.invalidateQueries({ queryKey: ["result"] });
  void queryClient.invalidateQueries({ queryKey: ["exam"] });
}

export function useMarkEntries(examScheduleId: number | null) {
  return useQuery({
    queryKey: ["result", "marks", examScheduleId],
    queryFn: () => getMarkEntriesBySchedule(examScheduleId as number),
    enabled: Boolean(examScheduleId),
    staleTime: 10_000,
  });
}

export function useExamResults(examId: number | null, classId?: number | null) {
  return useQuery({
    queryKey: ["result", "exam", examId, classId],
    queryFn: () => getExamResults(examId as number, classId),
    enabled: Boolean(examId),
    staleTime: 15_000,
  });
}

export function useExamResultDashboard(examId: number | null) {
  return useQuery({
    queryKey: ["result", "dashboard", examId],
    queryFn: () => getExamResultDashboard(examId as number),
    enabled: Boolean(examId),
    staleTime: 15_000,
  });
}

export function useSaveBulkMarkEntries() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: BulkMarkEntryDto) => saveBulkMarkEntries(payload),
    onSuccess: () => invalidateResults(queryClient),
  });
}

export function useSubmitMarkEntries() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ examScheduleId, teacherId }: { examScheduleId: number; teacherId: number }) =>
      submitMarkEntries(examScheduleId, teacherId),
    onSuccess: () => invalidateResults(queryClient),
  });
}

export function useMarkEntryLock() {
  const queryClient = useQueryClient();
  return {
    lock: useMutation({ mutationFn: lockMarkEntries, onSuccess: () => invalidateResults(queryClient) }),
    unlock: useMutation({ mutationFn: unlockMarkEntries, onSuccess: () => invalidateResults(queryClient) }),
  };
}

export function useExamResultLifecycle() {
  const queryClient = useQueryClient();
  return {
    calculate: useMutation({ mutationFn: calculateExamResults, onSuccess: () => invalidateResults(queryClient) }),
    publish: useMutation({ mutationFn: publishExamResults, onSuccess: () => invalidateResults(queryClient) }),
    unpublish: useMutation({ mutationFn: unpublishExamResults, onSuccess: () => invalidateResults(queryClient) }),
  };
}

export function useFinalResults(academicYearId: number | null, classId?: number | null) {
  return useQuery({
    queryKey: ["result", "final", academicYearId, classId],
    queryFn: () => getFinalResults(academicYearId as number, classId),
    enabled: Boolean(academicYearId),
    staleTime: 15_000,
  });
}

export function useStudentFinalResult(studentId: number | null, academicYearId: number | null) {
  return useQuery({
    queryKey: ["result", "final", "student", studentId, academicYearId],
    queryFn: () => getStudentFinalResult(studentId as number, academicYearId as number),
    enabled: Boolean(studentId && academicYearId),
    staleTime: 15_000,
  });
}

export function useFinalResultLifecycle() {
  const queryClient = useQueryClient();
  return {
    calculate: useMutation({
      mutationFn: calculateFinalResults,
      onSuccess: () => invalidateResults(queryClient),
    }),
    publish: useMutation({
      mutationFn: publishFinalResults,
      onSuccess: () => invalidateResults(queryClient),
    }),
    unpublish: useMutation({
      mutationFn: unpublishFinalResults,
      onSuccess: () => invalidateResults(queryClient),
    }),
  };
}
