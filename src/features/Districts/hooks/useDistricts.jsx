import { useCallback } from "react";
import { districtsApi } from "../../../api/districts.api";
import { useApi } from "../../../hooks/useApi";

export const useDistricts = (params = {}, options = {}) => {
  const apiCall = useCallback(() => {
    return districtsApi.getAll();
  }, [JSON.stringify(params)]);

  const { data, isLoading, error, execute } = useApi(apiCall, {
    inmediate: true,
    ...options,
  });

  return {
    districts: data || [],
    isLoading,
    error,
    refetch: execute,
  };
};
