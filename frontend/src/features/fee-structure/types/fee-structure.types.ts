export interface FeeStructureListDto {
  id: number;
  name: string;
  description?: string | null;
  academicYearId: number;
  academicYearName: string;
  schoolClassId: number;
  schoolClassName: string;
  sectionId?: number | null;
  sectionName?: string | null;
  isTemplate: boolean;
  isActive: boolean;
  effectiveFrom: string;
  effectiveTo?: string | null;
  itemCount: number;
  totalAmount: number;
}

export interface FeeStructureItemDto {
  id: number;
  feeTypeId: number;
  feeTypeName: string;
  amount: number;
  isOptional: boolean;
  sortOrder: number;
}

export interface FeeStructureDto extends FeeStructureListDto {
  items: FeeStructureItemDto[];
}

export interface CreateFeeStructureItemDto {
  feeTypeId: number;
  amount: number;
  isOptional: boolean;
  sortOrder: number;
}

export interface UpdateFeeStructureItemDto extends CreateFeeStructureItemDto {
  id?: number | null;
  isDeleted: boolean;
}

export interface CreateFeeStructureDto {
  name: string;
  description?: string | null;
  academicYearId: number;
  schoolClassId: number;
  sectionId?: number | null;
  isTemplate: boolean;
  effectiveFrom: string;
  effectiveTo?: string | null;
  clonedFromId?: number | null;
  items: CreateFeeStructureItemDto[];
}

export interface UpdateFeeStructureDto {
  id: number;
  name: string;
  description?: string | null;
  isActive: boolean;
  sectionId?: number | null;
  isTemplate: boolean;
  effectiveFrom: string;
  effectiveTo?: string | null;
  items: UpdateFeeStructureItemDto[];
}

export interface FeeStructureFilters {
  academicYearId?: number;
  schoolClassId?: number;
  isActive?: boolean;
}