import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createGuardian,
  deleteGuardian,
  getGuardianById,
  getGuardians,
  searchGuardians,
  updateGuardian,
} from "../api/guardian.api";
import type { CreateGuardianDto, UpdateGuardianDto } from "../types/guardian.types";

export function useGuardians() {
  return useQuery({ queryKey: ["guardian", "list"], queryFn: getGuardians, staleTime: 30_000 });
}

export function useGuardian(id: number | null) {
  return useQuery({
    queryKey: ["guardian", "detail", id],
    queryFn: () => getGuardianById(id as number),
    enabled: Boolean(id),
    staleTime: 30_000,
  });
}

export function useSearchGuardians(keyword: string) {
  return useQuery({
    queryKey: ["guardian", "search", keyword],
    queryFn: () => searchGuardians(keyword),
    enabled: keyword.trim().length > 0,
    staleTime: 30_000,
  });
}

export function useCreateGuardian() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (payload: CreateGuardianDto) => createGuardian(payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["guardian", "list"] });
    },
  });
}

export function useUpdateGuardian() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ id, payload }: { id: number; payload: UpdateGuardianDto }) => updateGuardian(id, payload),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["guardian", "list"] });
      void queryClient.invalidateQueries({ queryKey: ["guardian", "detail"] });
    },
  });
}

export function useDeleteGuardian() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteGuardian(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["guardian", "list"] });
    },
  });
}
