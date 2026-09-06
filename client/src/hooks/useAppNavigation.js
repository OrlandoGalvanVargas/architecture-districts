import { useNavigate } from "react-router-dom";
import { useCallback, useMemo } from "react";
import { ROUTES } from "@/router/routes.config";

export const useAppNavigation = () => {
  const navigate = useNavigate();

  const goTo = useCallback((path) => navigate(path), [navigate]);
  const goBack = useCallback(() => navigate(-1), [navigate]);
  const goHome = useCallback(() => navigate(ROUTES.HOME), [navigate]);

  const createModuleNavigation = useCallback(
    (moduleRoutes) => ({
      list: () => navigate(moduleRoutes.LIST),
      create: () => navigate(moduleRoutes.CREATE),
      detail: (id) => navigate(moduleRoutes.DETAIL(id)),
      edit: (id) => navigate(moduleRoutes.EDIT(id)),
    }),
    [navigate]
  );

  const navigation = useMemo(
    () => ({
      navigate,
      goTo,
      goBack,
      goHome,

      districts: createModuleNavigation(ROUTES.DISTRICTS),
      schools: createModuleNavigation(ROUTES.SCHOOLS),
      users: createModuleNavigation(ROUTES.USERS),
      beacons: createModuleNavigation(ROUTES.BEACONS),
      faculties: createModuleNavigation(ROUTES.FACULTIES),
    }),
    [navigate, goTo, goBack, goHome, createModuleNavigation]
  );

  return navigation;
};
