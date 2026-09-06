import { Routes, Route, Navigate } from "react-router-dom";
import { SchoolsPage } from "./pages/SchoolsPage";
import { SchoolCreatePage } from "./pages/SchoolCreatePage";
import { SchoolDetailPage } from "./pages/SchoolDetailPage";
import { SchoolEditPage } from "./pages/SchoolEditPage";
import { ROUTES } from "@/router/routes.config";

export const SchoolView = () => {
  return (
    <Routes>
      <Route index element={<SchoolsPage />} />
      <Route path="create" element={<SchoolCreatePage />} />
      <Route path=":id" element={<SchoolDetailPage />} />
      <Route path=":id/edit" element={<SchoolEditPage />} />
      <Route path="*" element={<Navigate to={ROUTES.SCHOOLS.LIST} replace />} />
    </Routes>
  );
};
