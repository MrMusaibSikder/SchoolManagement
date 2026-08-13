export interface StudentGuardianDto {
  guardianId?: number;
  relationship?: string | null;
  guardianName?: string | null;
}

export interface CreateStudentGuardianDto {
  guardianId?: number;
  relationship?: string | null;
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
  guardians: StudentGuardianDto[];
}

export interface CreateStudentDto {
  admissionNumber: string;
  fullName: string;
  dateOfBirth: string;
  rollNo: string;
  admissionDate: string;
  gender: string;
  bloodGroup?: string | null;
  address?: string | null;
  classId: number;
  sectionId: number;
  guardians: CreateStudentGuardianDto[];
}

export interface UpdateStudentDto extends CreateStudentDto {
  id: number;
}

export interface StudentDocumentDto {
  id?: number;
  name?: string | null;
  fileName?: string | null;
  fileUrl?: string | null;
}

export interface StudentAttendanceHistoryItem {
  id: number;
  studentId: number;
  studentName: string;
  attendanceDate: string;
  status: number;
  remarks?: string | null;
}

export interface StudentListFilterState {
  search: string;
  classId: string;
  sectionId: string;
  status: string;
}
