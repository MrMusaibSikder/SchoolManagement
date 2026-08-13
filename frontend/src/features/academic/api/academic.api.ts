import { authApiClient } from "@/lib/api/auth-client";
import type {
  AcademicSessionDto,
  AcademicYearDto,
  CreateAcademicSessionDto,
  CreateAcademicYearDto,
  CreateSchoolClassDto,
  CreateSectionDto,
  CreateSubjectDto,
  CreateTeacherDto,
  SchoolClassDto,
  SectionDto,
  StudentDto,
  SubjectDto,
  SubjectTeacherDto,
  TeacherDto,
  UpdateAcademicSessionDto,
  UpdateAcademicYearDto,
  UpdateSchoolClassDto,
  UpdateSectionDto,
  UpdateSubjectDto,
  UpdateTeacherDto,
} from "../types/academic.types";

async function getJson<T>(path: string): Promise<T> {
  const { data } = await authApiClient.get<T>(path);
  return data;
}

async function postJson<T>(path: string, body?: unknown): Promise<T> {
  const { data } = await authApiClient.post<T>(path, body);
  return data;
}

async function putJson<T>(path: string, body?: unknown): Promise<T> {
  const { data } = await authApiClient.put<T>(path, body);
  return data;
}

async function deleteJson(path: string): Promise<void> {
  await authApiClient.delete(path);
}

export async function getAcademicYears(): Promise<AcademicYearDto[]> {
  return getJson<AcademicYearDto[]>("/AcademicYears");
}

export async function createAcademicYear(payload: CreateAcademicYearDto): Promise<AcademicYearDto> {
  return postJson<AcademicYearDto>("/AcademicYears", payload);
}

export async function updateAcademicYear(id: number, payload: UpdateAcademicYearDto): Promise<AcademicYearDto> {
  return putJson<AcademicYearDto>(`/AcademicYears/${id}`, payload);
}

export async function deleteAcademicYear(id: number): Promise<void> {
  await deleteJson(`/AcademicYears/${id}`);
}

export async function getAcademicSessions(): Promise<AcademicSessionDto[]> {
  return getJson<AcademicSessionDto[]>('/AcademicSessions');
}

export async function createAcademicSession(payload: CreateAcademicSessionDto): Promise<AcademicSessionDto> {
  return postJson<AcademicSessionDto>('/AcademicSessions', payload);
}

export async function updateAcademicSession(id: number, payload: UpdateAcademicSessionDto): Promise<AcademicSessionDto> {
  return putJson<AcademicSessionDto>(`/AcademicSessions/${id}`, payload);
}

export async function deleteAcademicSession(id: number): Promise<void> {
  await deleteJson(`/AcademicSessions/${id}`);
}

export async function getSchoolClasses(): Promise<SchoolClassDto[]> {
  return getJson<SchoolClassDto[]>("/SchoolClasses");
}

export async function createSchoolClass(payload: CreateSchoolClassDto): Promise<SchoolClassDto> {
  return postJson<SchoolClassDto>("/SchoolClasses", payload);
}

export async function updateSchoolClass(id: number, payload: UpdateSchoolClassDto): Promise<SchoolClassDto> {
  return putJson<SchoolClassDto>(`/SchoolClasses/${id}`, payload);
}

export async function deleteSchoolClass(id: number): Promise<void> {
  await deleteJson(`/SchoolClasses/${id}`);
}

export async function getSections(): Promise<SectionDto[]> {
  return getJson<SectionDto[]>("/Sections");
}

export async function createSection(payload: CreateSectionDto): Promise<SectionDto> {
  return postJson<SectionDto>("/Sections", payload);
}

export async function updateSection(id: number, payload: UpdateSectionDto): Promise<SectionDto> {
  return putJson<SectionDto>(`/Sections/${id}`, payload);
}

export async function deleteSection(id: number): Promise<void> {
  await deleteJson(`/Sections/${id}`);
}

export async function getSubjects(): Promise<SubjectDto[]> {
  return getJson<SubjectDto[]>("/Subjects");
}

export async function createSubject(payload: CreateSubjectDto): Promise<SubjectDto> {
  return postJson<SubjectDto>("/Subjects", payload);
}

export async function updateSubject(id: number, payload: UpdateSubjectDto): Promise<SubjectDto> {
  return putJson<SubjectDto>(`/Subjects/${id}`, payload);
}

export async function deleteSubject(id: number): Promise<void> {
  await deleteJson(`/Subjects/${id}`);
}

export async function getTeachers(): Promise<TeacherDto[]> {
  return getJson<TeacherDto[]>("/Teachers");
}

export async function createTeacher(payload: CreateTeacherDto): Promise<TeacherDto> {
  return postJson<TeacherDto>("/Teachers", payload);
}

export async function updateTeacher(id: number, payload: UpdateTeacherDto): Promise<TeacherDto> {
  return putJson<TeacherDto>(`/Teachers/${id}`, payload);
}

export async function deleteTeacher(id: number): Promise<void> {
  await deleteJson(`/Teachers/${id}`);
}

export async function getSubjectTeachers(): Promise<SubjectTeacherDto[]> {
  return getJson<SubjectTeacherDto[]>("/SubjectTeachers");
}

export async function createSubjectTeacher(payload: SubjectTeacherDto): Promise<SubjectTeacherDto> {
  return postJson<SubjectTeacherDto>("/SubjectTeachers", payload);
}

export async function deleteSubjectTeacher(subjectId: number, teacherId: number): Promise<void> {
  await deleteJson(`/SubjectTeachers/${subjectId}/${teacherId}`);
}

export async function getStudents(): Promise<StudentDto[]> {
  return getJson<StudentDto[]>("/Students");
}

export async function getStudentById(id: number): Promise<StudentDto> {
  return getJson<StudentDto>(`/Students/${id}`);
}

export async function createStudent(payload: FormData): Promise<StudentDto> {
  const { data } = await authApiClient.post<StudentDto>("/Students", payload, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return data;
}

export async function updateStudent(id: number, payload: FormData): Promise<StudentDto> {
  const { data } = await authApiClient.put<StudentDto>(`/Students/${id}`, payload, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return data;
}

export async function deleteStudent(id: number): Promise<void> {
  await deleteJson(`/Students/${id}`);
}

export async function getClassRoutines(): Promise<unknown[]> {
  return getJson<unknown[]>("/Exam/routine");
}
