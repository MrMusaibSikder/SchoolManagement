import { useQuery } from "@tanstack/react-query";
import { useAuth } from "./useAuth";
import { getCurrentUserProfile } from "../api/current-user.api";

export function useCurrentUser() {
  const { isAuthenticated, session } = useAuth();

  return useQuery({
    queryKey: ["current-user", "profile", session?.userId],
    queryFn: getCurrentUserProfile,
    enabled: isAuthenticated,
    staleTime: 5 * 60 * 1000,
    retry: 1,
  });
}
