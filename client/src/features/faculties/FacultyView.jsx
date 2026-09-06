import { Routes, Route, Navigate } from "react-router-dom";
import { FacultiesPage } from "./pages/FacultiesPage";
import { FacultyCreatePage } from "./pages/FacultyCreatePage";
import { FacultyDetailPage } from "./pages/FacultyDetailPage";
import { FacultyEditPage } from "./pages/FacultyEditPage";
import { ROUTES } from "@/router/routes.config";

export const FacultyView = () => {
  return (
    <Routes>
      <Route index element={<FacultiesPage />} />
      <Route path="create" element={<FacultyCreatePage />} />
      <Route path=":id" element={<FacultyDetailPage />} />
      <Route path=":id/edit" element={<FacultyEditPage />} />
      <Route path="*" element={<Navigate to={ROUTES.FACULTIES.LIST} replace />} />
    </Routes>
  );
};
