import { useCallback } from "react";
import { useApi } from "@/hooks/useApi";
import { districtService } from "../../../services/districtService";

export const useDistricts = (params = {}, options = {}) => {
  const apiCall = useCallback(() => {
    return districtService.getAll(params);
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
