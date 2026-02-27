import { useNavigate } from "react-router-dom";
import { useCallback, useMemo } from "react";
import { RoutePaths } from "../router/RoutePaths";

export const useAppNavigation = () => {
  const navigate = useNavigate();

  const goBack = useCallback(() => navigate(-1), [navigate]);
  const goHome = useCallback(() => navigate(RoutePaths.home()), [navigate]);

  const goToDistricts = useCallback(
    () => navigate(RoutePaths.districts.list()),
    [navigate],
  );
  const goToDistrictDetail = useCallback(
    (id) => navigate(RoutePaths.districts.detail(id)),
    [navigate],
  );
  const goToDistrictEdit = useCallback(
    (id) => navigate(RoutePaths.districts.edit(id)),
    [navigate],
  );
  const goToDistrictCreate = useCallback(
    () => navigate(RoutePaths.districts.create()),
    [navigate],
  );

  return useMemo(
    () => ({
      navigate,
      goBack,
      goHome,
      goToDistricts,
      goToDistrictDetail,
      goToDistrictEdit,
      goToDistrictCreate,
    }),
    [
      navigate,
      goBack,
      goHome,
      goToDistricts,
      goToDistrictDetail,
      goToDistrictEdit,
      goToDistrictCreate,
    ],
  );
};
