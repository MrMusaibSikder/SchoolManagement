export interface TranscriptSummaryDto {
  totalExamsIncluded: number;
  totalAcademicYearsIncluded: number;
  cgpa: number;
  highestYearGpa: number;
  lowestYearGpa: number;
  overallPassed: boolean;
  latestPromotionStatus: PromotionStatus | null;
}

export type PromotionStatus = 1 | 2 | 3;

export interface TranscriptSubjectDto {
  subjectId: number;
  subjectName: string;
  marksObtained: number;
  grade: string;
  gpa: number;
  isPassed: boolean;
  isOptional: boolean;
}

export interface TranscriptExamEntryDto {
  examId: number;
  examName: string;
  examTypeName: string;
  academicYearId: number;
  academicYearName: string;
  gpa: number;
  grade: string;
  isPassed: boolean;
  publishedAt?: string | null;
  subjects: TranscriptSubjectDto[];
}

export interface TranscriptYearSummaryDto {
  academicYearId: number;
  academicYearName: string;
  finalGpa: number;
  finalGrade: string;
  isPassed: boolean;
  promotionStatus: PromotionStatus;
  classPosition: number | null;
  sectionPosition: number | null;
  meritPosition: number | null;
  publishedAt: string | null;
  subjects: TranscriptSubjectDto[];
  teacherRemarks: string | null;
  principalRemarks: string | null;
}

export interface GpaHistoryPointDto {
  label: string;
  gpa: number;
}

export interface PositionHistoryPointDto {
  academicYearName: string;
  classPosition: number | null;
  sectionPosition: number | null;
  meritPosition: number | null;
}

export interface TranscriptAttendanceSummaryDto {
  totalDays: number;
  presentDays: number;
  absentDays: number;
  lateDays: number;
  leaveDays: number;
  attendancePercentage: number;
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
