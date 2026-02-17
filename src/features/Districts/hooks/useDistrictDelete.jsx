import { districtsApi } from "@/api/districts/districts.api";
import { useApi } from "@/hooks/useApi";

export const useDistrictDelete = (options = {}) => {
  const { isLoading, error, execute } = useApi(districtsApi.delete, {
    inmediate: false,
    ...options,
  });

  return {
    deleteDistrict: execute,
    isDeleting: isLoading,
    error,
  };
};
