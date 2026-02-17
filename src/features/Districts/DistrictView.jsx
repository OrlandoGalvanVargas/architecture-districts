import { Routes, Route, Navigate } from "react-router-dom";
import { DistrictsPage } from "./pages/DistrictsPage";
import { DistrictCreatePage } from "./pages/DistrictCreatePage";

export const DistrictView = () => {
  const baseBreadcrumbs = [{ label: "Districts", path: "/districts" }];

  return (
    <Routes>
      <Route
        index
        element={<DistrictsPage baseBreadcrumbs={baseBreadcrumbs} />}
      />
      <Route
        path="create"
        element={<DistrictCreatePage baseBreadcrumbs={baseBreadcrumbs} />}
      />

      <Route path="*" element={<Navigate to="/districts" replace />} />
    </Routes>
  );
};
