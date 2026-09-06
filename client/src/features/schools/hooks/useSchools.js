import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { schoolsApi } from "@/api/endpoints/school.api";
import { queryKeys } from "@/api/queryKeys";
import { useNotification } from "@/contexts/Notification";

export const useSchools = (params = {}) => {
  return useQuery({
    queryKey: queryKeys.schools.list(params),
    queryFn: () => schoolsApi.getAll(params),
    placeholderData: (previousData) => previousData,
  });
};

export const useSchool = (id) => {
  return useQuery({
    queryKey: queryKeys.schools.detail(id),
    queryFn: () => schoolsApi.getById(id),
    enabled: !!id,
  });
};

export const useCreateSchool = () => {
  const queryClient = useQueryClient();
  const { showSuccess, handleApiError } = useNotification();

  return useMutation({
    mutationFn: (data) => schoolsApi.create(data),
    onSuccess: () => {
      showSuccess("School created successfully");
      queryClient.invalidateQueries({ queryKey: queryKeys.schools.all() });
    },
    onError: (error) => {
      handleApiError(error, "Failed to create school");
    },
  });
};

export const useUpdateSchool = (id) => {
  const queryClient = useQueryClient();
  const { showSuccess, handleApiError } = useNotification();

  return useMutation({
    mutationFn: (data) => schoolsApi.update(id, data),
    onSuccess: () => {
      showSuccess("School updated successfully");
      queryClient.invalidateQueries({ queryKey: queryKeys.schools.all() });
    },
    onError: (error) => {
      handleApiError(error, "Failed to update school");
    },
  });
};

export const useDeleteSchool = () => {
  const queryClient = useQueryClient();
  const { showSuccess, handleApiError } = useNotification();

  return useMutation({
    mutationFn: (id) => schoolsApi.delete(id),
    onSuccess: () => {
      showSuccess("School deleted successfully");
      queryClient.invalidateQueries({ queryKey: queryKeys.schools.all() });
    },
    onError: (error) => {
      handleApiError(error, "Failed to delete school");
    },
  });
};
