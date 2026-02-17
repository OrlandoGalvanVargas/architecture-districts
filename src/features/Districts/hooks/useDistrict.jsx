import { useCallback } from "react";
import { districtsApi } from "../../../api/districts.api";
import { useApi } from "../../../hooks/useApi";

export const useDistrict = (id, options = {}) => {
  const apiCall = useCallback(() => {
    return districtsApi.getById(id);
  }, [id]);

  const { data, isLoading, error, execute } = useApi(apiCall, {
    inmediate: true,
    ...options,
  });

  return {
    district: data,
    isLoading,
    error,
    refetch: execute,
  };
};
