import { useNavigate } from "react-router-dom";
import { RoutePaths } from "../router/RoutePaths";
export const useAppNavigation = () => {
  const navigate = useNavigate();

  return {
    navigate,
    goBack: () => navigate(-1),
    goHome: () => navigate(RoutePaths.home()),

    goToDistricts: () => navigate(RoutePaths.districts.list()),
    goToDistrictDetail: (id) => navigate(RoutePaths.districts.detail(id)),
    goToDistrictEdit: (id) => navigate(RoutePaths.districts.edit(id)),
    goToDistrictCreate: () => navigate(RoutePaths.districts.create()),
  };
};
