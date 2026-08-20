import { authApiClient } from "@/lib/api/auth-client";
import type { CurrentUserDto } from "../types/auth.types";

/** GET /api/CurrentUser/profile — roles + effective permissions for the JWT user. */
export async function getCurrentUserProfile(): Promise<CurrentUserDto> {
  const { data } = await authApiClient.get<CurrentUserDto>("/CurrentUser/profile");
  return data;
}
