import { Routes, Route } from "react-router-dom";
import { SchoolsPage } from "./pages/SchoolsPage";
import { SchoolCreatePage } from "./pages/SchoolCreatePage";
import { SchoolDetailPage } from "./pages/SchoolDetailPag";
import { SchoolEditPage } from "./pages/SchoolEditPage";

export const SchoolView = () => {
  return (
    <Routes>
      <Route index element={<SchoolsPage />} />
      <Route path="" element={<SchoolCreatePage />} />
      <Route path=":id" element={<SchoolDetailPage />} />
      <Route path=":id/edit" element={<SchoolEditPage />} />
    </Routes>
  );
};
