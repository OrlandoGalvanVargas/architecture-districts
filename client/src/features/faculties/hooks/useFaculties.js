import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { facultiesApi } from "@/api/endpoints/faculties.api";
import { queryKeys } from "@/api/queryKeys";
import { useNotification } from "@/contexts/Notification";

export const useFaculties = (params = {}) => {
  return useQuery({
    queryKey: queryKeys.faculties.list(params),
    queryFn: () => facultiesApi.getAll(params),
    placeholderData: (previousData) => previousData,
  });
};

export const useFaculty = (id) => {
  return useQuery({
    queryKey: queryKeys.faculties.detail(id),
    queryFn: () => facultiesApi.getById(id),
    enabled: !!id,
  });
};

export const useCreateFaculty = () => {
  const queryClient = useQueryClient();
  const { showSuccess, handleApiError } = useNotification();

  return useMutation({
    mutationFn: (data) => facultiesApi.create(data),
    onSuccess: () => {
      showSuccess("Faculty created successfully");
      queryClient.invalidateQueries({ queryKey: queryKeys.faculties.all() });
      queryClient.invalidateQueries({ queryKey: queryKeys.beacons.all() });
    },
    onError: (error) => {
      handleApiError(error, "Failed to create faculty");
    },
  });
};

export const useUpdateFaculty = (id) => {
  const queryClient = useQueryClient();
  const { showSuccess, handleApiError } = useNotification();

  return useMutation({
    mutationFn: (data) => facultiesApi.update(id, data),
    onSuccess: () => {
      showSuccess("Faculty updated successfully");
      queryClient.invalidateQueries({ queryKey: queryKeys.faculties.all() });
      queryClient.invalidateQueries({ queryKey: queryKeys.beacons.all() });
    },
    onError: (error) => {
      handleApiError(error, "Failed to update faculty");
    },
  });
};

export const useDeleteFaculty = () => {
  const queryClient = useQueryClient();
  const { showSuccess, handleApiError } = useNotification();

  return useMutation({
    mutationFn: (id) => facultiesApi.delete(id),
    onSuccess: () => {
      showSuccess("Faculty deleted successfully");
      queryClient.invalidateQueries({ queryKey: queryKeys.faculties.all() });
      queryClient.invalidateQueries({ queryKey: queryKeys.beacons.all() });
    },
    onError: (error) => {
      handleApiError(error, "Failed to delete faculty");
    },
  });
};
