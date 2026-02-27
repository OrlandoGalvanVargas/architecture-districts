import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { MainLayout } from "@/components/Layout/MainLayout/MainLayout";
import { DistrictView } from "@/features/districts/DistrictView";
import { AuthProvider } from "../contexts/AuthContext";
import { ProtectedRoute } from "./ProtectedRoute";
import { AuthView } from "../features/auth/AuthView";

export const AppRouter = () => {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path="/auth/*" element={<AuthView />} />

          <Route
            path="/"
            element={
              <ProtectedRoute>
                <MainLayout />
              </ProtectedRoute>
            }
          >
            <Route index element={<Navigate to="/districts" replace />} />
            <Route path="districts/*" element={<DistrictView />} />

            <Route path="*" element={<div>404 - Not Found</div>} />
          </Route>
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
};
