import { BrowserRouter, Routes, Route } from "react-router-dom";
import { lazy, Suspense } from "react";
import { AuthProvider } from "@/contexts/AuthContext";
import { ProtectedRoute } from "./ProtectedRoute";
import { MainLayout } from "@/components/Layout/MainLayout/MainLayout";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { NotFoundPage } from "@/pages/NotFound/NotFoundPage";
import { ForbiddenPage } from "@/pages/Forbidden/ForbiddenPage";
import { ROUTES } from "./routes.config";
import { PERMISSIONS } from "@/utils/permissions";
import { HomePage } from "@/pages/Home/HomePage";

const AuthView = lazy(() =>
  import("@/features/auth/AuthView").then((module) => ({ default: module.AuthView }))
);
const DistrictView = lazy(() =>
  import("@/features/districts/DistrictView").then((module) => ({ default: module.DistrictView }))
);
const SchoolView = lazy(() =>
  import("@/features/schools/SchoolView").then((module) => ({ default: module.SchoolView }))
);
const UserView = lazy(() =>
  import("@/features/users/UserView").then((module) => ({ default: module.UserView }))
);
const BeaconView = lazy(() =>
  import("@/features/beacons/BeaconView").then((module) => ({ default: module.BeaconView }))
);
const FacultyView = lazy(() =>
  import("@/features/faculties/FacultyView").then((module) => ({ default: module.FacultyView }))
);

export const AppRouter = () => {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Suspense fallback={<LoadingSpinner description="Loading..." fullScreen />}>
          <Routes>
            <Route path={`${ROUTES.AUTH.BASE}/*`} element={<AuthView />} />

            <Route
              path={ROUTES.HOME}
              element={
                <ProtectedRoute>
                  <MainLayout />
                </ProtectedRoute>
              }
            >
              <Route index element={<HomePage />} />

              <Route
                path={`${ROUTES.DISTRICTS.LIST}/*`}
                element={
                  <ProtectedRoute requiredPermission={PERMISSIONS.DISTRICTS.VIEW_LIST}>
                    <DistrictView />
                  </ProtectedRoute>
                }
              />

              <Route
                path={`${ROUTES.SCHOOLS.LIST}/*`}
                element={
                  <ProtectedRoute requiredPermission={PERMISSIONS.SCHOOLS.VIEW_LIST}>
                    <SchoolView />
                  </ProtectedRoute>
                }
              />

              <Route
                path={`${ROUTES.USERS.LIST}/*`}
                element={
                  <ProtectedRoute requiredPermission={PERMISSIONS.USERS.VIEW_LIST}>
                    <UserView />
                  </ProtectedRoute>
                }
              />

              <Route
                path={`${ROUTES.BEACONS.LIST}/*`}
                element={
                  <ProtectedRoute requiredPermission={PERMISSIONS.BEACONS.VIEW_LIST}>
                    <BeaconView />
                  </ProtectedRoute>
                }
              />

              <Route
                path={`${ROUTES.FACULTIES.LIST}/*`}
                element={
                  <ProtectedRoute requiredPermission={PERMISSIONS.FACULTIES.VIEW_LIST}>
                    <FacultyView />
                  </ProtectedRoute>
                }
              />

              <Route path="*" element={<NotFoundPage />} />
            </Route>

            <Route path="/forbidden" element={<ForbiddenPage />} />
          </Routes>
        </Suspense>
      </AuthProvider>
    </BrowserRouter>
  );
};
