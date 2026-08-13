import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createStudent,
  deleteStudent,
  getStudentAttendanceHistory,
  getStudentById,
  getStudentDocuments,
  getStudents,
  updateStudent,
} from "../api/student.api";
import type { CreateStudentDto, UpdateStudentDto } from "../types/student.types";

export function useStudents() {
  return useQuery({ queryKey: ["student", "list"], queryFn: getStudents, staleTime: 30_000 });
}

export function useStudent(id: number | null) {
  return useQuery({
    queryKey: ["student", "detail", id],
    queryFn: () => getStudentById(id as number),
    enabled: Boolean(id),
    staleTime: 30_000,
  });
}

export function useStudentDocuments(id: number | null) {
  return useQuery({
    queryKey: ["student", "documents", id],
    queryFn: () => getStudentDocuments(id as number),
    enabled: Boolean(id),
    staleTime: 30_000,
  });
}

export function useStudentAttendanceHistory(id: number | null) {
  return useQuery({
    queryKey: ["student", "attendance-history", id],
    queryFn: () => getStudentAttendanceHistory(id as number),
    enabled: Boolean(id),
    staleTime: 30_000,
  });
}

export function useCreateStudent() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ payload, photoFile }: { payload: CreateStudentDto; photoFile?: File | null }) => createStudent(payload, photoFile),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["student", "list"] });
    },
  });
}

export function useUpdateStudent() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload, photoFile }: { id: number; payload: UpdateStudentDto; photoFile?: File | null }) => updateStudent(id, payload, photoFile),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["student", "list"] });
      void queryClient.invalidateQueries({ queryKey: ["student", "detail"] });
    },
  });
}

export function useDeleteStudent() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteStudent(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["student", "list"] });
    },
  });
}
