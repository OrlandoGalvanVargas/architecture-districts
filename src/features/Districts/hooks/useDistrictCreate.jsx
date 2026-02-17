import { districtsApi } from "@/api/districts/districts.api";
import { useApi } from "@/hooks/useApi";

export const useDistrictCreate = (options = {}) => {
  const { isLoading, error, execute } = useApi(districtsApi.create, {
    inmediate: false,
    ...options,
  });

  return {
    createDistrict: execute,
    isCreating: isLoading,
    error,
  };
};
