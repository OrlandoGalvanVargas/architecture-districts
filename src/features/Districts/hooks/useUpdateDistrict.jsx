import { useApi } from "../../../hooks/useApi";
import { districtsApi } from "../../../api/district/districts.api";

export const useUpdateDistrict = (options = {}) => {
  const { isLoading, error, execute } = useApi(districtsApi.update, {
    inmediate: false,
    ...options,
  });

  return {
    updateDistrict: execute,
    isUpdating: isLoading,
    error,
  };
};
