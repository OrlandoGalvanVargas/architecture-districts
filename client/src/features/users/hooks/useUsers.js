import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { usersApi } from "@/api/endpoints/users.api";
import { queryKeys } from "@/api/queryKeys";
import { useNotification } from "@/contexts/Notification";

export const useUsers = (params = {}) => {
  return useQuery({
    queryKey: queryKeys.users.list(params),
    queryFn: () => usersApi.getAll(params),
    placeholderData: (previousData) => previousData,
  });
};

export const useUser = (id) => {
  return useQuery({
    queryKey: queryKeys.users.detail(id),
    queryFn: () => usersApi.getById(id),
    enabled: !!id,
  });
};

export const useCreateUser = () => {
  const queryClient = useQueryClient();
  const { showSuccess, handleApiError } = useNotification();

  return useMutation({
    mutationFn: (data) => usersApi.create(data),
    onSuccess: () => {
      showSuccess("User created successfully");
      queryClient.invalidateQueries({ queryKey: queryKeys.users.all() });
    },
    onError: (error) => {
      handleApiError(error, "Failed to create user");
    },
  });
};

export const useUpdateUser = (id) => {
  const queryClient = useQueryClient();
  const { showSuccess, handleApiError } = useNotification();

  return useMutation({
    mutationFn: (data) => usersApi.update(id, data),
    onSuccess: () => {
      showSuccess("User updated successfully");
      queryClient.invalidateQueries({ queryKey: queryKeys.users.all() });
    },
    onError: (error) => {
      handleApiError(error, "Failed to update user");
    },
  });
};

export const useDeleteUser = () => {
  const queryClient = useQueryClient();
  const { showSuccess, handleApiError } = useNotification();

  return useMutation({
    mutationFn: (id) => usersApi.delete(id),
    onSuccess: () => {
      showSuccess("User deleted successfully");
      queryClient.invalidateQueries({ queryKey: queryKeys.users.all() });
    },
    onError: (error) => {
      handleApiError(error, "Failed to delete user");
    },
  });
};
