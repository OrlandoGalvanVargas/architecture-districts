import { Routes, Route, Navigate } from "react-router-dom";
import { SchoolsPage } from "./pages/SchoolsPage";
import { SchoolCreatePage } from "./pages/SchoolCreatePage";
import { SchoolDetailPage } from "./pages/SchoolDetailPage";
import { SchoolEditPage } from "./pages/SchoolEditPage";
import { ROUTES_CONFIG } from "@/router/routes";

export const SchoolView = () => {
  const routes = ROUTES_CONFIG.schools.children;

  return (
    <Routes>
      <Route index element={<SchoolsPage />} />
      <Route path={routes.create.pattern} element={<SchoolCreatePage />} />
      <Route path={routes.detail.pattern} element={<SchoolDetailPage />} />
      <Route path={routes.edit.pattern} element={<SchoolEditPage />} />

      <Route
        path="*"
        element={<Navigate to={ROUTES_CONFIG.schools.path} replace />}
      />
    </Routes>
  );
};
