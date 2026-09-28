export const FeeFrequency = {
  OneTime: 1,
  Monthly: 2,
  Termly: 3,
  Yearly: 4,
} as const;

export type FeeFrequencyValue = (typeof FeeFrequency)[keyof typeof FeeFrequency];

export interface FeeTypeListDto {
  id: number;
  name: string;
  code: string;
  feeCategoryId: number;
  feeCategoryName: string;
  frequency: FeeFrequencyValue;
  isMandatory: boolean;
  isRefundable: boolean;
  isActive: boolean;
  defaultDueDayOfMonth?: number | null;
  defaultGracePeriodDays?: number | null;
}

export interface FeeTypeDto extends FeeTypeListDto {
  description?: string | null;
}

export interface CreateFeeTypeDto {
  name: string;
  code: string;
  description?: string | null;
  feeCategoryId: number;
  frequency: FeeFrequencyValue;
  isMandatory: boolean;
  isRefundable: boolean;
  defaultDueDayOfMonth?: number | null;
  defaultGracePeriodDays?: number | null;
}

export interface UpdateFeeTypeDto extends CreateFeeTypeDto {
  id: number;
  isActive: boolean;
}
