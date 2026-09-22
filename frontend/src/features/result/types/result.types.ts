export const MarkAttendanceStatus = {
  Present: 1,
  Absent: 2,
  Medical: 3,
  Withheld: 4,
  Incomplete: 5,
  Excused: 6,
  Late: 7,
  Cheating: 8,
  Blocked: 9,
} as const;

export type MarkAttendanceStatusValue =
  (typeof MarkAttendanceStatus)[keyof typeof MarkAttendanceStatus];

export const MarkEntryStatus = { Draft: 1, Submitted: 2 } as const;

export interface ResultDto {
  id: number;
  studentId: number;
  studentName?: string | null;
  rollNo?: string | null;
  examScheduleId: number;
  examName?: string | null;
  subjectName?: string | null;
  fullMarks: number;
  passMarks: number;
  marksObtained: number;
  graceMarks: number;
  grade?: string | null;
  gpa?: number | null;
  isPassed: boolean;
  percentage?: number | null;
  attendanceStatus: MarkAttendanceStatusValue;
  entryStatus: 1 | 2;
  remarks?: string | null;
  isLocked: boolean;
  lockedAt?: string | null;
  enteredByTeacherId?: number | null;
  enteredByTeacherName?: string | null;
  createdAt: string;
  updatedAt?: string | null;
}

export interface CreateResultDto {
  studentId: number;
  examScheduleId: number;
  teacherId: number;
  marksObtained: number;
  graceMarks: number;
  attendanceStatus: MarkAttendanceStatusValue;
  remarks?: string | null;
}

export interface UpdateResultDto {
  id: number;
  teacherId: number;
  marksObtained: number;
  graceMarks: number;
  attendanceStatus: MarkAttendanceStatusValue;
  remarks?: string | null;
}

export interface MarkEntryItemDto {
  studentId: number;
  marksObtained: number;
  graceMarks: number;
  attendanceStatus: MarkAttendanceStatusValue;
  remarks?: string | null;
}

export interface BulkMarkEntryDto {
  examScheduleId: number;
  teacherId: number;
  entries: MarkEntryItemDto[];
}

export interface ExamResultDto {
  id: number;
  studentId: number;
  studentName: string;
  rollNo: string;
  className: string;
  sectionName: string;
  examId: number;
  examName: string;
  totalMarks: number;
  totalFullMarks: number;
  percentage: number;
  gpa: number;
  grade: string;
  isPassed: boolean;
  meritPosition?: number | null;
  classPosition?: number | null;
  sectionPosition?: number | null;
  isPublished: boolean;
  publishedAt?: string | null;
  teacherRemarks?: string | null;
  guardianRemarks?: string | null;
}

export interface ExamResultDashboardDto {
  examId: number;
  examName: string;
  totalStudents: number;
  appearedStudents: number;
  absentStudents: number;
  totalScheduleCount: number;
  fullySubmittedScheduleCount: number;
  completionPercentage: number;
  isResultPublished: boolean;
  publishedResultCount: number;
  pendingResultCount: number;
  subjectStatistics: SubjectStatisticsDto[];
}

export interface SubjectStatisticsDto {
  subjectId: number;
  subjectName: string;
  totalStudents: number;
  highestMarks: number;
  lowestMarks: number;
  averageMarks: number;
  passCount: number;
  failCount: number;
  passRate: number;
}

export const PromotionStatus = {
  Pending: 1,
  Promoted: 2,
  NotPromoted: 3,
} as const;

export type PromotionStatusValue = (typeof PromotionStatus)[keyof typeof PromotionStatus];

export interface FinalResultDetailDto {
  id: number;
  finalResultId: number;
  examId: number;
  examName: string;
  subjectId?: number | null;
  subjectName?: string | null;
  weightPercentage: number;
  marksObtained: number;
  fullMarks: number;
  percentage: number;
  gpa: number;
  grade: string;
  isPassed: boolean;
}

export interface FinalResultDto {
  id: number;
  studentId: number;
  studentName: string;
  rollNo: string;
  className: string;
  sectionName: string;
  academicYearId: number;
  academicYearName: string;
  examWeightSetupId: number;
  finalMarks: number;
  finalGpa: number;
  finalGrade: string;
  isPassed: boolean;
  promotionStatus: PromotionStatusValue;
  meritPosition?: number | null;
  classPosition?: number | null;
  sectionPosition?: number | null;
  isPublished: boolean;
  publishedAt?: string | null;
  teacherRemarks?: string | null;
  principalRemarks?: string | null;
  details: FinalResultDetailDto[];
}
