import { authApiClient } from "@/lib/api/auth-client";
import type { SchoolDto, UpdateSchoolDto } from "../types/settings.types";

async function putForm<T>(path: string, body: FormData): Promise<T> {
  const { data } = await authApiClient.put<T>(path, body, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return data;
}

function toFormData(payload: UpdateSchoolDto, logoFile?: File | null) {
  const formData = new FormData();
  formData.append("Id", String(payload.id));
  formData.append("Name", payload.name);
  if (payload.eiin) formData.append("EIIN", payload.eiin);
  if (payload.address) formData.append("Address", payload.address);
  if (payload.phone) formData.append("Phone", payload.phone);
  if (payload.email) formData.append("Email", payload.email);
  if (logoFile) formData.append("LogoFile", logoFile);
  return formData;
}

export async function getSchool(): Promise<SchoolDto> {
  const { data } = await authApiClient.get<SchoolDto[]>("/School");
  const school = data[0];

  if (!school) {
    throw new Error("No school profile is configured.");
  }

  return school;
}

export async function updateSchool(
  payload: UpdateSchoolDto,
  logoFile?: File | null
): Promise<SchoolDto> {
  return putForm<SchoolDto>(`/School/${payload.id}`, toFormData(payload, logoFile));
}
