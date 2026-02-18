import { Routes, Route, Navigate } from "react-router-dom";
import { DistrictListPage } from "./pages/DistrictsPage";
import { DistrictCreatePage } from "./pages/DistrictCreatePage";

export const DistrictView = () => {
  return (
    <Routes>
      <Route index element={<DistrictListPage />} />
      <Route path="create" element={<DistrictCreatePage />} />

      <Route path="*" element={<Navigate to="/districts" replace />} />
    </Routes>
  );
};
