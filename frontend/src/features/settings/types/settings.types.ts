export interface SchoolDto {
  id: number;
  name: string;
  eiin?: string | null;
  address?: string | null;
  phone?: string | null;
  email?: string | null;
  logo?: string | null;
}

export interface UpdateSchoolDto {
  id: number;
  name: string;
  eiin?: string | null;
  address?: string | null;
  phone?: string | null;
  email?: string | null;
}
