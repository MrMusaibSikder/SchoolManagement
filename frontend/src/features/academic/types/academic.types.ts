export interface AcademicYearDto {
  id: number;
  name: string;
  startDate: string;
  endDate: string;
  isCurrent: boolean;
}

export interface AcademicSessionDto {
  id: number;
  name: string;
  startDate: string;
  endDate: string;
  isCurrent: boolean;
}

export interface CreateAcademicYearDto {
  name: string;
  startDate: string;
  endDate: string;
  isCurrent: boolean;
}

export interface UpdateAcademicYearDto extends CreateAcademicYearDto {
  id: number;
}

export interface CreateAcademicSessionDto {
  name: string;
  startDate: string;
  endDate: string;
  isCurrent: boolean;
}

export interface UpdateAcademicSessionDto extends CreateAcademicSessionDto {
  id: number;
}

export interface SchoolClassDto {
  id: number;
  name: string;
  displayOrder: number;
}

export interface CreateSchoolClassDto {
  name: string;
  displayOrder: number;
}

export interface UpdateSchoolClassDto extends CreateSchoolClassDto {
  id: number;
}

export interface SectionDto {
  id: number;
  name: string;
  classId: number;
}

export interface CreateSectionDto {
  name: string;
  classId: number;
}

export interface UpdateSectionDto extends CreateSectionDto {
  id: number;
}

export interface SubjectDto {
  id: number;
  name: string;
  code: string;
  fullMarks: number;
  passMarks: number;
}

export interface CreateSubjectDto {
  name: string;
  code: string;
  fullMarks: number;
  passMarks: number;
}

export interface UpdateSubjectDto extends CreateSubjectDto {
  id: number;
}

export interface TeacherDto {
  id: number;
  employeeId: number;
  qualification?: string | null;
  specialization?: string | null;
}

export interface CreateTeacherDto {
  employeeId: number;
  qualification?: string | null;
  specialization?: string | null;
}

export interface UpdateTeacherDto extends CreateTeacherDto {
  id: number;
}

export interface SubjectTeacherDto {
  subjectId: number;
  teacherId: number;
}

export interface StudentDto {
  id: number;
  admissionNumber: string;
  fullName: string;
  dateOfBirth: string;
  rollNo: string;
  admissionDate: string;
  gender: string;
  bloodGroup?: string | null;
  address?: string | null;
  photo?: string | null;
  classId: number;
  sectionId: number;
  guardians: Array<{ guardianId?: number; relationship?: string | null; guardianName?: string | null }>;
}

export interface RoutineDto {
  id?: number;
  dayOfWeek?: string | null;
  startTime?: string | null;
  endTime?: string | null;
  subjectName?: string | null;
  teacherName?: string | null;
  className?: string | null;
  sectionName?: string | null;
}
