import { Routes, Route, Navigate } from "react-router-dom";
import { DistrictListPage } from "./pages/DistrictsPage";
import { DistrictCreatePage } from "./pages/DistrictCreatePage";
import { DistrictDetailPage } from "./pages/DistrictDetailPage";
import { DistrictEditPage } from "./pages/DistrictEditPage";
import { ROUTES_CONFIG } from "@/router/routes";

export const DistrictView = () => {
  const routes = ROUTES_CONFIG.districts.children;

  return (
    <Routes>
      <Route index element={<DistrictListPage />} />
      <Route path={routes.create.pattern} element={<DistrictCreatePage />} />
      <Route path={routes.detail.pattern} element={<DistrictDetailPage />} />
      <Route path={routes.edit.pattern} element={<DistrictEditPage />} />

      <Route path="*" element={<Navigate to="/districts" replace />} />
    </Routes>
  );
};
