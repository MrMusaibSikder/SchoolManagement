import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  createAcademicYear,
  createClassSubject,
  createSchoolClass,
  createSection,
  createSubject,
  createSubjectTeacher,
  createTeacher,
  deleteAcademicYear,
  deleteClassSubject,
  deleteSchoolClass,
  deleteSection,
  deleteSubject,
  deleteSubjectTeacher,
  deleteTeacher,
  getAcademicYears,
  getClassSubjects,
  getSchoolClasses,
  getSections,
  getStudents,
  getSubjects,
  getSubjectTeachers,
  getTeachers,
  updateAcademicYear,
  updateClassSubjectOptional,
  updateSchoolClass,
  updateSection,
  updateSubject,
  updateTeacher,
} from "../api/academic.api";
import type {
  CreateAcademicYearDto,
  ClassSubjectDto,
  CreateSchoolClassDto,
  CreateSectionDto,
  CreateSubjectDto,
  CreateTeacherDto,
  SubjectTeacherDto,
  UpdateAcademicYearDto,
  UpdateSchoolClassDto,
  UpdateSectionDto,
  UpdateSubjectDto,
  UpdateTeacherDto,
} from "../types/academic.types";

export function useAcademicYears() {
  return useQuery({ queryKey: ["academic", "years"], queryFn: getAcademicYears, staleTime: 30_000 });
}

export function useSchoolClasses() {
  return useQuery({ queryKey: ["academic", "classes"], queryFn: getSchoolClasses, staleTime: 30_000 });
}

export function useSections() {
  return useQuery({ queryKey: ["academic", "sections"], queryFn: getSections, staleTime: 30_000 });
}

export function useSubjects() {
  return useQuery({ queryKey: ["academic", "subjects"], queryFn: getSubjects, staleTime: 30_000 });
}

export function useTeachers() {
  return useQuery({ queryKey: ["academic", "teachers"], queryFn: getTeachers, staleTime: 30_000 });
}

export function useSubjectTeachers() {
  return useQuery({ queryKey: ["academic", "subject-teachers"], queryFn: getSubjectTeachers, staleTime: 30_000 });
}

export function useClassSubjects() {
  return useQuery({ queryKey: ["academic", "class-subjects"], queryFn: getClassSubjects, staleTime: 30_000 });
}

export function useStudents() {
  return useQuery({ queryKey: ["academic", "students"], queryFn: getStudents, staleTime: 30_000 });
}

export function useCreateAcademicYear() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateAcademicYearDto) => createAcademicYear(payload),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "years"] }); },
  });
}

export function useUpdateAcademicYear() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateAcademicYearDto }) => updateAcademicYear(id, payload),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "years"] }); },
  });
}

export function useDeleteAcademicYear() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteAcademicYear(id),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "years"] }); },
  });
}

export function useCreateSchoolClass() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateSchoolClassDto) => createSchoolClass(payload),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "classes"] }); },
  });
}

export function useUpdateSchoolClass() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateSchoolClassDto }) => updateSchoolClass(id, payload),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "classes"] }); },
  });
}

export function useDeleteSchoolClass() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteSchoolClass(id),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "classes"] }); },
  });
}

export function useCreateSection() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateSectionDto) => createSection(payload),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "sections"] }); },
  });
}

export function useUpdateSection() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateSectionDto }) => updateSection(id, payload),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "sections"] }); },
  });
}

export function useDeleteSection() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteSection(id),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "sections"] }); },
  });
}

export function useCreateSubject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateSubjectDto) => createSubject(payload),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "subjects"] }); },
  });
}

export function useUpdateSubject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateSubjectDto }) => updateSubject(id, payload),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "subjects"] }); },
  });
}

export function useDeleteSubject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteSubject(id),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "subjects"] }); },
  });
}

export function useCreateTeacher() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateTeacherDto) => createTeacher(payload),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "teachers"] }); },
  });
}

export function useUpdateTeacher() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateTeacherDto }) => updateTeacher(id, payload),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "teachers"] }); },
  });
}

export function useDeleteTeacher() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteTeacher(id),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "teachers"] }); },
  });
}

export function useCreateSubjectTeacher() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: SubjectTeacherDto) => createSubjectTeacher(payload),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "subject-teachers"] }); },
  });
}

export function useDeleteSubjectTeacher() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ subjectId, teacherId }: { subjectId: number; teacherId: number }) => deleteSubjectTeacher(subjectId, teacherId),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "subject-teachers"] }); },
  });
}

export function useCreateClassSubject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: ClassSubjectDto) => createClassSubject(payload),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "class-subjects"] }); },
  });
}

export function useDeleteClassSubject() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ classId, subjectId }: { classId: number; subjectId: number }) => deleteClassSubject(classId, subjectId),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "class-subjects"] }); },
  });
}

export function useUpdateClassSubjectOptional() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ classId, subjectId, isOptional }: { classId: number; subjectId: number; isOptional: boolean }) =>
      updateClassSubjectOptional(classId, subjectId, isOptional),
    onSuccess: () => { void queryClient.invalidateQueries({ queryKey: ["academic", "class-subjects"] }); },
  });
}
