import { Routes, Route, Navigate } from "react-router-dom";
import { LoginPage } from "./pages/LoginPage";
import { ROUTES_CONFIG } from "@/router/routes";

export const AuthView = () => {
  return (
    <Routes>
      <Route
        index
        path={ROUTES_CONFIG.auth.children.login.pattern}
        element={<LoginPage />}
      />
      <Route
        path="*"
        element={
          <Navigate
            to={`/${ROUTES_CONFIG.auth.children.login.pattern}`}
            replace
          />
        }
      />
    </Routes>
  );
};
