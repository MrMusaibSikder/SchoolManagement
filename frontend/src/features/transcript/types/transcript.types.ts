export interface TranscriptSummaryDto {
  totalExams: number;
  totalSubjects: number;
  finalGpa: number;
  finalGrade: string;
  cgpa?: number | null;
  attendancePercentage?: number | null;
  passCount?: number;
  failCount?: number;
  position?: number | null;
}

export interface TranscriptExamEntryDto {
  examId: number;
  examName: string;
  examTypeName?: string | null;
  totalMarks: number;
  percentage: number;
  gpa: number;
  grade: string;
  isPassed: boolean;
  position?: number | null;
  publishedAt?: string | null;
}

export interface TranscriptYearSummaryDto {
  academicYearId: number;
  academicYearName: string;
  finalGpa: number;
  finalGrade: string;
  totalMarks: number;
  percentage: number;
  position?: number | null;
}

export interface GpaHistoryPointDto {
  academicYearId: number;
  academicYearName: string;
  gpa: number;
}

export interface PositionHistoryPointDto {
  academicYearId: number;
  academicYearName: string;
  position?: number | null;
}

export interface TranscriptAttendanceSummaryDto {
  presentCount: number;
  totalCount: number;
  percentage: number;
}

export interface TranscriptDto {
  studentId: number;
  studentName: string;
  rollNo: string;
  className: string;
  sectionName: string;
  summary: TranscriptSummaryDto;
  examHistory: TranscriptExamEntryDto[];
  yearSummaries: TranscriptYearSummaryDto[];
  gpaHistory: GpaHistoryPointDto[];
  positionHistory: PositionHistoryPointDto[];
  attendanceSummary: TranscriptAttendanceSummaryDto;
  generatedAt: string;
}

export interface ProgressReportExamColumnDto {
  examId: number;
  examName: string;
  totalMarks: number;
  grade?: string | null;
  percentage?: number | null;
}

export interface ProgressReportSubjectRowDto {
  subjectId: number;
  subjectName: string;
  examMarks: Array<number | null>;
  average?: number | null;
  grade?: string | null;
}

export interface ProgressReportDto {
  studentId: number;
  studentName: string;
  rollNo: string;
  className: string;
  sectionName: string;
  academicYearId: number;
  academicYearName: string;
  exams: ProgressReportExamColumnDto[];
  subjects: ProgressReportSubjectRowDto[];
}
