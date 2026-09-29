export const FineType = {
  Fixed: 1,
  Percentage: 2,
  DailyAccrual: 3,
} as const;

export type FineTypeValue = (typeof FineType)[keyof typeof FineType];

export interface LateFineRuleDto {
  id: number;
  academicYearId: number;
  academicYearName: string;
  feeTypeId?: number | null;
  feeTypeName?: string | null;
  type: FineTypeValue;
  amount: number;
  gracePeriodDays: number;
  maxFineAmount?: number | null;
  isActive: boolean;
  createdAt: string;
}

export interface CreateLateFineRuleDto {
  academicYearId: number;
  feeTypeId?: number | null;
  type: FineTypeValue;
  amount: number;
  gracePeriodDays: number;
  maxFineAmount?: number | null;
  isActive: boolean;
}

export interface UpdateLateFineRuleDto {
  id: number;
  type: FineTypeValue;
  amount: number;
  gracePeriodDays: number;
  maxFineAmount?: number | null;
  isActive: boolean;
}