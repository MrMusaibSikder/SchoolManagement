export const ExamStatus = {
  Draft: 1,
  Published: 2,
  Completed: 3,
  Cancelled: 4,
} as const;

export type ExamStatusValue = (typeof ExamStatus)[keyof typeof ExamStatus];

export const EXAM_STATUS_LABEL: Record<number, string> = {
  [ExamStatus.Draft]: "Draft",
  [ExamStatus.Published]: "Published",
  [ExamStatus.Completed]: "Completed",
  [ExamStatus.Cancelled]: "Cancelled",
};

export function examStatusLabel(status: number | null | undefined) {
  return EXAM_STATUS_LABEL[status ?? 0] ?? "—";
}

export interface ExamTypeDto {
  id: number;
  name: string;
}

export interface ExamDto {
  id: number;
  name: string;
  examTypeId: number;
  examTypeName?: string | null;
  academicYearId: number;
  academicYearName?: string | null;
  status: ExamStatusValue;
}

export interface CreateExamDto {
  name: string;
  examTypeId: number;
  academicYearId: number;
}

export interface UpdateExamDto extends CreateExamDto {
  id: number;
}

export interface ExamScheduleDto {
  id: number;
  examId: number;
  examName?: string | null;
  classId: number;
  className?: string | null;
  subjectId: number;
  subjectName?: string | null;
  examDate: string;
  fullMarks: number;
  passMarks: number;
}

export interface CreateExamScheduleDto {
  examId: number;
  classId: number;
  subjectId: number;
  examDate: string;
  fullMarks: number;
  passMarks: number;
}

export interface UpdateExamScheduleDto extends CreateExamScheduleDto {
  id: number;
}

export interface ExamDetailsDto extends ExamDto {
  totalSchedules: number;
  schedules: ExamScheduleDto[];
}

export interface UpcomingExamDto {
  examId: number;
  examName: string;
  examTypeName: string;
  nextExamDate: string;
  daysRemaining: number;
  totalSchedules: number;
}

export interface ExamSummaryDto {
  id: number;
  name: string;
  examTypeName: string;
  academicYearName: string;
  status: ExamStatusValue;
  totalSchedules: number;
  firstExamDate?: string | null;
  lastExamDate?: string | null;
}

export interface ExamDashboardDto {
  totalExams: number;
  draftExams: number;
  publishedExams: number;
  completedExams: number;
  cancelledExams: number;
  upcomingExamsCount: number;
  upcomingExams: UpcomingExamDto[];
  recentExams: ExamSummaryDto[];
}

export interface ExamCalendarDto {
  scheduleId: number;
  examId: number;
  examName: string;
  subjectId: number;
  subjectName: string;
  classId: number;
  className: string;
  examDate: string;
  fullMarks: number;
  passMarks: number;
}

export interface GradeSetupDto {
  id: number;
  academicYearId: number;
  academicYearName: string;
  gradeName: string;
  gradePoint: number;
  minMarks: number;
  maxMarks: number;
  minPercentage: number;
  maxPercentage: number;
  isFail: boolean;
  displayOrder: number;
  isActive: boolean;
}

export interface CreateGradeSetupDto {
  academicYearId: number;
  gradeName: string;
  gradePoint: number;
  minMarks: number;
  maxMarks: number;
  minPercentage: number;
  maxPercentage: number;
  isFail: boolean;
  displayOrder: number;
}

export interface ExamWeightItemDto {
  id: number;
  examWeightSetupId: number;
  examId: number;
  examName?: string | null;
  weightPercentage: number;
}

export interface CreateExamWeightItemDto {
  examWeightSetupId: number;
  examId: number;
  weightPercentage: number;
}

export interface UpdateExamWeightItemDto {
  id: number;
  weightPercentage: number;
}

export interface ExamWeightSetupDto {
  id: number;
  academicYearId: number;
  academicYearName: string;
  name: string;
  isActive: boolean;
  totalWeight: number;
  items: ExamWeightItemDto[];
}

export interface CreateExamWeightSetupDto {
  academicYearId: number;
  name: string;
  items: CreateExamWeightItemDto[];
}

export interface UpdateExamWeightSetupDto {
  id: number;
  name: string;
}
