import { districtsApi } from "../../../api/districts.api";
import { useApi } from "../../../hooks/useApi";

export const useDistrictUpdate = (options = {}) => {
  const { isLoading, error, execute } = useApi(
    (id, data) => districtsApi.update(id, data),
    {
      inmediate: false,
      ...options,
    },
  );

  return {
    updateDistrict: execute,
    isUpdating: isLoading,
    error,
  };
};
