import { Routes, Route, Navigate } from "react-router-dom";
import { DistrictsPage } from "./pages/DistrictsPage";
import { DistrictCreatePage } from "./pages/DistrictCreatePage";
import { DistrictDetailPage } from "./pages/DistrictDetailPage";
import { DistrictEditPage } from "./pages/DistrictEditPage";
import { ROUTES } from "@/router/routes.config";

export const DistrictView = () => {
  return (
    <Routes>
      <Route index element={<DistrictsPage />} />
      <Route path="create" element={<DistrictCreatePage />} />
      <Route path=":id" element={<DistrictDetailPage />} />
      <Route path=":id/edit" element={<DistrictEditPage />} />
      <Route path="*" element={<Navigate to={ROUTES.DISTRICTS.LIST} replace />} />
    </Routes>
  );
};
