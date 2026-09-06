import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { districtsApi } from "@/api/endpoints/districts.api";
import { queryKeys } from "@/api/queryKeys";
import { useNotification } from "@/contexts/Notification";

export const useDistricts = (params = {}) => {
  return useQuery({
    queryKey: queryKeys.districts.list(params),
    queryFn: () => districtsApi.getAll(params),
    select: (data) => data,
    placeholderData: (previousData) => previousData,
  });
};

export const useDistrict = (id) => {
  return useQuery({
    queryKey: queryKeys.districts.detail(id),
    queryFn: () => districtsApi.getById(id),
    enabled: !!id,
  });
};

export const useCreateDistrict = () => {
  const queryClient = useQueryClient();
  const { showSuccess, handleApiError } = useNotification();

  return useMutation({
    mutationFn: (data) => districtsApi.create(data),
    onSuccess: () => {
      showSuccess("District created successfully");
      queryClient.invalidateQueries({ queryKey: queryKeys.districts.all() });
    },
    onError: (error) => {
      handleApiError(error, "Failed to create district");
    },
  });
};

export const useUpdateDistrict = (id) => {
  const queryClient = useQueryClient();
  const { showSuccess, handleApiError } = useNotification();

  return useMutation({
    mutationFn: (data) => districtsApi.update(id, data),
    onSuccess: () => {
      showSuccess("District updated successfully");
      queryClient.invalidateQueries({ queryKey: queryKeys.districts.all() });
    },
    onError: (error) => {
      handleApiError(error, "Failed to update district");
    },
  });
};

export const useDeleteDistrict = () => {
  const queryClient = useQueryClient();
  const { showSuccess, handleApiError } = useNotification();

  return useMutation({
    mutationFn: (id) => districtsApi.delete(id),
    onSuccess: () => {
      showSuccess("District deleted successfully");
      queryClient.invalidateQueries({ queryKey: queryKeys.districts.all() });
    },
    onError: (error) => {
      handleApiError(error, "Failed to delete district");
    },
  });
};
