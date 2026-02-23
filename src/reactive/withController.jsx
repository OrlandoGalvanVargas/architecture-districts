// reactive/withController.js
import { useState, useEffect, useCallback } from "react";
import services from "@/services";

export const withController = (Component, config) => {
  return (props) => {
    const [state, setState] = useState({});
    const [loadingStates, setLoadingStates] = useState({});
    const [errors, setErrors] = useState({});
    const [actions, setActions] = useState({});

    const executeService = useCallback(async (servicePath, options = {}) => {
      const { params = [], onSuccess, onError, immediate = false } = options;
      const [serviceName, actionName] = servicePath.split(".");
      const stateKey = options.stateKey || actionName;

      const execute = async (...runtimeParams) => {
        const finalParams = runtimeParams.length > 0 ? runtimeParams : params;

        setLoadingStates((prev) => ({ ...prev, [stateKey]: true }));
        setErrors((prev) => ({ ...prev, [stateKey]: null }));

        try {
          const result = await services[serviceName][actionName](
            ...finalParams,
          );

          setState((prev) => ({ ...prev, [stateKey]: result }));

          if (onSuccess) {
            onSuccess(result);
          }

          return result;
        } catch (error) {
          setErrors((prev) => ({ ...prev, [stateKey]: error }));

          if (onError) {
            onError(error);
          }

          throw error;
        } finally {
          setLoadingStates((prev) => ({ ...prev, [stateKey]: false }));
        }
      };

      if (immediate) {
        execute(...params);
      }

      return execute;
    }, []);

    useEffect(() => {
      const initServices = async () => {
        if (!config.services) return;

        const executors = {};

        for (const [key, serviceConfig] of Object.entries(config.services)) {
          executors[key] = await executeService(serviceConfig.path, {
            ...serviceConfig,
            stateKey: key,
          });
        }

        setActions(executors);

        if (config.init) {
          config.init({ services, props, actions: executors });
        }
      };

      initServices();
    }, [executeService, props]);

    const controllerProps = {
      data: state,
      loading: loadingStates,
      errors,
      actions: actions || {},
      services,
      ...props,
    };

    return <Component {...controllerProps} />;
  };
};
