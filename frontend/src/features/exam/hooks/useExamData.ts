import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  activateExamWeightSetup,
  activateGradeSetup,
  addExamWeightItem,
  cancelExam,
  completeExam,
  createExam,
  createExamSchedule,
  createExamType,
  createExamWeightSetup,
  createGradeSetup,
  deactivateExamWeightSetup,
  deactivateGradeSetup,
  deleteExam,
  deleteExamSchedule,
  deleteExamType,
  deleteExamWeightItem,
  deleteExamWeightSetup,
  deleteGradeSetup,
  getExamCalendar,
  getExamDashboard,
  getExamDetails,
  getExams,
  getExamTypes,
  getExamWeightSetups,
  getGradeSetups,
  publishExam,
  reopenExam,
  updateExam,
  updateExamSchedule,
  updateExamType,
  updateExamWeightItem,
  updateExamWeightSetup,
  updateGradeSetup,
} from "../api/exam.api";
import type {
  CreateExamDto,
  CreateExamScheduleDto,
  CreateExamWeightItemDto,
  CreateExamWeightSetupDto,
  CreateGradeSetupDto,
  UpdateExamDto,
  UpdateExamScheduleDto,
  UpdateExamWeightItemDto,
  UpdateExamWeightSetupDto,
} from "../types/exam.types";

export function useExamTypes() {
  return useQuery({ queryKey: ["exam", "types"], queryFn: getExamTypes, staleTime: 30_000 });
}

export function useExams() {
  return useQuery({ queryKey: ["exam", "list"], queryFn: getExams, staleTime: 20_000 });
}

export function useExamDetails(id: number | null) {
  return useQuery({
    queryKey: ["exam", "details", id],
    queryFn: () => getExamDetails(id as number),
    enabled: Boolean(id),
    staleTime: 15_000,
  });
}

export function useExamDashboard(enabled = true) {
  return useQuery({
    queryKey: ["exam", "dashboard"],
    queryFn: getExamDashboard,
    enabled,
    staleTime: 30_000,
  });
}

export function useExamCalendar(params: {
  fromDate: string;
  toDate: string;
  classId?: number | null;
}) {
  return useQuery({
    queryKey: ["exam", "calendar", params.fromDate, params.toDate, params.classId],
    queryFn: () => getExamCalendar(params),
    enabled: Boolean(params.fromDate && params.toDate),
    staleTime: 20_000,
  });
}

export function useGradeSetups() {
  return useQuery({ queryKey: ["exam", "grades"], queryFn: getGradeSetups, staleTime: 30_000 });
}

export function useExamWeightSetups() {
  return useQuery({ queryKey: ["exam", "weights"], queryFn: getExamWeightSetups, staleTime: 30_000 });
}

function invalidateExams(queryClient: ReturnType<typeof useQueryClient>) {
  void queryClient.invalidateQueries({ queryKey: ["exam"] });
  void queryClient.invalidateQueries({ queryKey: ["dashboard"] });
}

export function useCreateExamType() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (name: string) => createExamType(name),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useUpdateExamType() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, name }: { id: number; name: string }) => updateExamType(id, name),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useDeleteExamType() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteExamType(id),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useCreateExam() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateExamDto) => createExam(payload),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useUpdateExam() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateExamDto }) =>
      updateExam(id, payload),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useDeleteExam() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteExam(id),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useExamLifecycle() {
  const queryClient = useQueryClient();
  return {
    publish: useMutation({
      mutationFn: publishExam,
      onSuccess: () => invalidateExams(queryClient),
    }),
    complete: useMutation({
      mutationFn: completeExam,
      onSuccess: () => invalidateExams(queryClient),
    }),
    cancel: useMutation({
      mutationFn: cancelExam,
      onSuccess: () => invalidateExams(queryClient),
    }),
    reopen: useMutation({
      mutationFn: reopenExam,
      onSuccess: () => invalidateExams(queryClient),
    }),
  };
}

export function useCreateExamSchedule() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateExamScheduleDto) => createExamSchedule(payload),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useUpdateExamSchedule() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateExamScheduleDto }) =>
      updateExamSchedule(id, payload),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useDeleteExamSchedule() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteExamSchedule(id),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useCreateGradeSetup() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateGradeSetupDto) => createGradeSetup(payload),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useUpdateGradeSetup() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      id,
      payload,
    }: {
      id: number;
      payload: Omit<CreateGradeSetupDto, "academicYearId"> & { id: number };
    }) => updateGradeSetup(id, payload),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useDeleteGradeSetup() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteGradeSetup(id),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useToggleGradeSetup() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, activate }: { id: number; activate: boolean }) =>
      activate ? activateGradeSetup(id) : deactivateGradeSetup(id),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useCreateExamWeightSetup() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateExamWeightSetupDto) => createExamWeightSetup(payload),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useUpdateExamWeightSetup() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateExamWeightSetupDto }) =>
      updateExamWeightSetup(id, payload),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useDeleteExamWeightSetup() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteExamWeightSetup(id),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useToggleExamWeightSetup() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, activate }: { id: number; activate: boolean }) =>
      activate ? activateExamWeightSetup(id) : deactivateExamWeightSetup(id),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useAddExamWeightItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateExamWeightItemDto) => addExamWeightItem(payload),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useUpdateExamWeightItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ itemId, payload }: { itemId: number; payload: UpdateExamWeightItemDto }) =>
      updateExamWeightItem(itemId, payload),
    onSuccess: () => invalidateExams(queryClient),
  });
}

export function useDeleteExamWeightItem() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (itemId: number) => deleteExamWeightItem(itemId),
    onSuccess: () => invalidateExams(queryClient),
  });
}
