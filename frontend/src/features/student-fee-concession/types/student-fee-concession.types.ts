export const ConcessionType = {
  PercentageDiscount: 1,
  FixedAmountDiscount: 2,
  FullExemption: 3,
} as const;

export type ConcessionTypeValue = (typeof ConcessionType)[keyof typeof ConcessionType];

export interface StudentFeeConcessionListDto {
  id: number;
  studentName: string;
  feeTypeName: string;
  academicYearName: string;
  type: ConcessionTypeValue;
  value?: number | null;
  isApproved: boolean;
  isActive: boolean;
}

export interface StudentFeeConcessionDto extends StudentFeeConcessionListDto {
  studentId: number;
  feeTypeId: number;
  academicYearId: number;
  reason: string;
  requiresApproval: boolean;
  approvedByEmployeeId?: number | null;
  approvedByEmployeeName?: string | null;
  approvedAt?: string | null;
  validFrom?: string | null;
  validTo?: string | null;
  createdAt: string;
}

export interface CreateStudentFeeConcessionDto {
  studentId: number;
  feeTypeId: number;
  academicYearId: number;
  type: ConcessionTypeValue;
  value?: number | null;
  reason: string;
  requiresApproval: boolean;
  validFrom?: string | null;
  validTo?: string | null;
}

export interface UpdateStudentFeeConcessionDto {
  id: number;
  type: ConcessionTypeValue;
  value?: number | null;
  reason: string;
  validFrom?: string | null;
  validTo?: string | null;
  isActive: boolean;
}