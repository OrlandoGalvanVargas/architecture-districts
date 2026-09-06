import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "@/contexts/AuthContext";
import { LoadingSpinner } from "@/components/common/LoadingSpinner/LoadingSpinner";
import { ROUTES } from "./routes.config";
import { checkPermission } from "@/utils/permissions";

export const ProtectedRoute = ({ children, requiredPermission = null }) => {
  const { isAuthenticated, isLoading, user } = useAuth();
  const location = useLocation();

  if (isLoading) {
    return <LoadingSpinner description="Checking authentication..." fullScreen />;
  }

  if (!isAuthenticated) {
    return <Navigate to={ROUTES.AUTH.LOGIN} state={{ from: location.pathname }} replace />;
  }

  if (requiredPermission && !checkPermission(user, requiredPermission)) {
    return <Navigate to="/forbidden" replace />;
  }

  return children;
};
