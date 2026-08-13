import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "@/contexts/AuthContext";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { RoutePaths } from "./RoutePaths";

export const ProtectedRoute = ({ children }) => {
  const { isAuthenticated, isLoading } = useAuth();
  const location = useLocation();

  if (isLoading) {
    return <LoadingSpinner description="Checking authentication..." />;
  }

  if (!isAuthenticated) {
    return (
      <Navigate
        to={RoutePaths.auth.login()}
        state={{ from: location }}
        replace
      />
    );
  }

  return children;
};
