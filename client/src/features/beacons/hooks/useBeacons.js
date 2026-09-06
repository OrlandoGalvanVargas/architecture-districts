import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { beaconsApi } from "@/api/endpoints/beacons.api";
import { queryKeys } from "@/api/queryKeys";
import { useNotification } from "@/contexts/Notification";

export const useBeacons = (params = {}) => {
  return useQuery({
    queryKey: queryKeys.beacons.list(params),
    queryFn: () => beaconsApi.getAll(params),
    placeholderData: (previousData) => previousData,
  });
};

export const useBeacon = (id) => {
  return useQuery({
    queryKey: queryKeys.beacons.detail(id),
    queryFn: () => beaconsApi.getById(id),
    enabled: !!id,
  });
};

export const useCreateBeacon = () => {
  const queryClient = useQueryClient();
  const { showSuccess, handleApiError } = useNotification();

  return useMutation({
    mutationFn: (data) => beaconsApi.create(data),
    onSuccess: () => {
      showSuccess("Beacon created successfully");
      queryClient.invalidateQueries({ queryKey: queryKeys.beacons.all() });
    },
    onError: (error) => {
      handleApiError(error, "Failed to create beacon");
    },
  });
};

export const useUpdateBeacon = (id) => {
  const queryClient = useQueryClient();
  const { showSuccess, handleApiError } = useNotification();

  return useMutation({
    mutationFn: (data) => beaconsApi.update(id, data),
    onSuccess: () => {
      showSuccess("Beacon updated successfully");
      queryClient.invalidateQueries({ queryKey: queryKeys.beacons.all() });
    },
    onError: (error) => {
      handleApiError(error, "Failed to update beacon");
    },
  });
};

export const useDeleteBeacon = () => {
  const queryClient = useQueryClient();
  const { showSuccess, handleApiError } = useNotification();

  return useMutation({
    mutationFn: (id) => beaconsApi.delete(id),
    onSuccess: () => {
      showSuccess("Beacon deleted successfully");
      queryClient.invalidateQueries({ queryKey: queryKeys.beacons.all() });
    },
    onError: (error) => {
      handleApiError(error, "Failed to delete beacon");
    },
  });
};
