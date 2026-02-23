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

    goToSchools: () => navigate(RoutePaths.schools.list()),
    goToSchoolDetail: (id) => navigate(RoutePaths.schools.detail(id)),
    goToSchoolEdit: (id) => navigate(RoutePaths.schools.edit(id)),
    goToSchoolCreate: () => navigate(RoutePaths.schools.create()),
  };
};
