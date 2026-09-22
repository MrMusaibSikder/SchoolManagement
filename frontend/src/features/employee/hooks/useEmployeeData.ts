import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  createEmployee,
  deleteEmployee,
  getDesignations,
  getEmployeeById,
  getEmployees,
  getUsers,
  updateEmployee,
} from "../api/employee.api";
import type { CreateEmployeeDto, UpdateEmployeeDto } from "../types/employee.types";

export function useEmployees() {
  return useQuery({
    queryKey: ["employee", "list"],
    queryFn: getEmployees,
    staleTime: 30_000,
  });
}

export function useEmployee(id: number | null) {
  return useQuery({
    queryKey: ["employee", "detail", id],
    queryFn: () => getEmployeeById(id as number),
    enabled: Boolean(id),
    staleTime: 30_000,
  });
}

export function useDesignations() {
  return useQuery({
    queryKey: ["employee", "designations"],
    queryFn: getDesignations,
    staleTime: 60_000,
  });
}

export function useUsersLookup() {
  return useQuery({
    queryKey: ["employee", "users"],
    queryFn: () => getUsers().catch(() => []),
    staleTime: 60_000,
  });
}

export function useCreateEmployee() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      payload,
      photoFile,
    }: {
      payload: CreateEmployeeDto;
      photoFile?: File | null;
    }) => createEmployee(payload, photoFile),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["employee", "list"] });
    },
  });
}

export function useUpdateEmployee() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({
      id,
      payload,
      photoFile,
    }: {
      id: number;
      payload: UpdateEmployeeDto;
      photoFile?: File | null;
    }) => updateEmployee(id, payload, photoFile),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["employee", "list"] });
      void queryClient.invalidateQueries({ queryKey: ["employee", "detail"] });
    },
  });
}

export function useDeleteEmployee() {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: number) => deleteEmployee(id),
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: ["employee", "list"] });
    },
  });
}
