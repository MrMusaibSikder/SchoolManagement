export interface FeeCategoryDto {
  id: number;
  name: string;
  description?: string | null;
  displayOrder: number;
  isActive: boolean;
}

export interface CreateFeeCategoryDto {
  name: string;
  description?: string | null;
  displayOrder: number;
  isActive: boolean;
}

export interface UpdateFeeCategoryDto {
  id: number;
  name: string;
  description?: string | null;
  displayOrder: number;
  isActive: boolean;
}
