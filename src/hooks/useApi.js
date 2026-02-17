import { useCallback, useEffect, useState } from "react";

export const useApi = (apiFunction, options = {}) => {
  const { inmediate, onSuccess = null, onError = null } = options;

  const [data, setData] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);

  const execute = useCallback(
    async (...params) => {
      setIsLoading(true);
      setError(null);
      try {
        const result = await apiFunction(...params);
        setData(result);
        if (onSuccess) {
          onSuccess(result);
        }
        return result;
      } catch (error) {
        setError(error);
        if (onError) {
          onError(error);
        }
        throw error;
      } finally {
        setIsLoading(false);
      }
    },
    [apiFunction, onSuccess, onError],
  );

  const reset = useCallback(() => {
    setData(null);
    setIsLoading(false);
    setError(null);
  }, []);

  useEffect(() => {
    if (inmediate) {
      execute();
    }
  }, [inmediate, execute]);

  return {
    data,
    isLoading,
    error,
    execute,
    reset,
  };
};
