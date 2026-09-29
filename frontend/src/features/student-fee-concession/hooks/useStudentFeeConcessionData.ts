import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  approveStudentConcession,
  createStudentConcession,
  deleteStudentConcession,
  getPendingConcessions,
  getStudentConcessionById,
  getStudentConcessions,
  updateStudentConcession,
} from "../api/student-fee-concession.api";
import type {
  CreateStudentFeeConcessionDto,
  StudentFeeConcessionDto,
  UpdateStudentFeeConcessionDto,
} from "../types/student-fee-concession.types";

export function loadStudentConcession(id: number) {
  return getStudentConcessionById(id);
}

export function useStudentConcessions(studentId: number | null, enabled = true) {
  return useQuery({
    queryKey: ["student-fee-concession", "student", studentId],
    queryFn: () => getStudentConcessions(studentId as number),
    enabled: enabled && studentId !== null,
    staleTime: 30_000,
  });
}

export function usePendingConcessions(enabled = true) {
  return useQuery({
    queryKey: ["student-fee-concession", "pending"],
    queryFn: getPendingConcessions,
    enabled,
    staleTime: 15_000,
  });
}

function invalidateConcessions(queryClient: ReturnType<typeof useQueryClient>) {
  void queryClient.invalidateQueries({ queryKey: ["student-fee-concession"] });
}

export function useCreateStudentConcession() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateStudentFeeConcessionDto) => createStudentConcession(payload),
    onSuccess: () => invalidateConcessions(queryClient),
  });
}

export function useUpdateStudentConcession() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateStudentFeeConcessionDto }) => updateStudentConcession(id, payload),
    onSuccess: () => invalidateConcessions(queryClient),
  });
}

export function useApproveStudentConcession() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => approveStudentConcession(id),
    onSuccess: () => invalidateConcessions(queryClient),
  });
}

export function useDeleteStudentConcession() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteStudentConcession(id),
    onSuccess: () => invalidateConcessions(queryClient),
  });
}

export function useApplicableStudentConcessions(studentId: number | null, enabled = true) {
  return useQuery({
    queryKey: ["student-fee-concession", "applicable", studentId],
    queryFn: async (): Promise<StudentFeeConcessionDto[]> => {
      const summaries = await getStudentConcessions(studentId as number);
      const applicableSummaries = summaries.filter((item) => item.isApproved && item.isActive);
      return Promise.all(applicableSummaries.map((item) => getStudentFeeConcessionDetail(item.id)));
    },
    enabled: enabled && studentId !== null,
    staleTime: 30_000,
  });
}

async function getStudentFeeConcessionDetail(id: number) {
  const { getStudentConcessionById } = await import("../api/student-fee-concession.api");
  return getStudentConcessionById(id);
}