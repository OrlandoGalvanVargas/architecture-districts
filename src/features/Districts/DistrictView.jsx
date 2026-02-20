import { Routes, Route, Navigate } from "react-router-dom";
import { DistrictListPage } from "./pages/DistrictsPage";
import { DistrictCreatePage } from "./pages/DistrictCreatePage";
import { DistrictDetailPage } from "./pages/DistrictDetailPage";
import { DistrictEditPage } from "./pages/DistrictEditPage";

export const DistrictView = () => {
  return (
    <Routes>
      <Route index element={<DistrictListPage />} />
      <Route path="create" element={<DistrictCreatePage />} />
      <Route path=":id" element={<DistrictDetailPage />} />
      <Route path=":id/edit" element={<DistrictEditPage />} />

      <Route path="*" element={<Navigate to="/districts" replace />} />
    </Routes>
  );
};
