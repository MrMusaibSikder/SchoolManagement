export interface EmployeeDto {
  id: number;
  employeeCode: string;
  fullName: string;
  phone: string;
  email?: string | null;
  joiningDate: string;
  isActive: boolean;
  employeePhoto?: string | null;
  designationId: number;
  userId?: number | null;
}

export interface CreateEmployeeDto {
  employeeCode: string;
  fullName: string;
  phone: string;
  email?: string | null;
  joiningDate: string;
  isActive: boolean;
  designationId: number;
  userId?: number | null;
}

export interface UpdateEmployeeDto extends CreateEmployeeDto {
  id: number;
}
