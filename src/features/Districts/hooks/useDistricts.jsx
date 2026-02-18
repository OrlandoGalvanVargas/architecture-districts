import { useCallback } from "react";
import { districtsApi } from "@/api/district/districts.api";
import { useApi } from "@/hooks/useApi";

export const useDistricts = (params = {}, options = {}) => {
  const apiCall = useCallback(() => {
    return districtsApi.getAll(params);
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
