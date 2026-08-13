export interface GuardianDto {
  id: number;
  fullName: string;
  phoneNumber: string;
  email?: string | null;
  address?: string | null;
  occupation?: string | null;
}

export interface CreateGuardianDto {
  fullName: string;
  phoneNumber: string;
  email?: string | null;
  address?: string | null;
  occupation?: string | null;
}

export interface UpdateGuardianDto extends CreateGuardianDto {
  id: number;
}

export interface GuardianLinkSelection {
  studentId: number;
  relationship: string;
}

export interface GuardianFormState {
  fullName: string;
  phoneNumber: string;
  email: string;
  address: string;
  occupation: string;
  linkedStudents: GuardianLinkSelection[];
}
