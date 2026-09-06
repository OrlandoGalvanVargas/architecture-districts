import { Routes, Route, Navigate } from "react-router-dom";
import { BeaconsPage } from "./pages/BeaconsPage";
import { BeaconCreatePage } from "./pages/BeaconCreatePage";
import { BeaconDetailPage } from "./pages/BeaconDetailPage";
import { BeaconEditPage } from "./pages/BeaconEditPage";
import { ROUTES } from "@/router/routes.config";

export const BeaconView = () => {
  return (
    <Routes>
      <Route index element={<BeaconsPage />} />
      <Route path="create" element={<BeaconCreatePage />} />
      <Route path=":id" element={<BeaconDetailPage />} />
      <Route path=":id/edit" element={<BeaconEditPage />} />
      <Route path="*" element={<Navigate to={ROUTES.BEACONS.LIST} replace />} />
    </Routes>
  );
};
