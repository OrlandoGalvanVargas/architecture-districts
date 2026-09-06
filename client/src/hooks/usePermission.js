import { useCallback } from "react";
import { useAuth } from "@/contexts/AuthContext";
import { checkPermission } from "@/utils/permissions";

export const usePermission = () => {
  const { user } = useAuth();

  const can = useCallback(
    (permission) => {
      return checkPermission(user, permission);
    },
    [user]
  );

  const canAny = useCallback(
    (permissions) => {
      return permissions.some((p) => checkPermission(user, p));
    },
    [user]
  );

  const canAll = useCallback(
    (permissions) => {
      return permissions.every((p) => checkPermission(user, p));
    },
    [user]
  );

  const isAdmin = user?.role === "Admin";
  const isDistrictAdmin = user?.role === "DistrictAdmin";
  const isSchoolAdmin = user?.role === "SchoolAdmin";
  const isUser = user?.role === "User";

  return {
    can,
    canAny,
    canAll,
    isAdmin,
    isDistrictAdmin,
    isSchoolAdmin,
    isUser,
    userRole: user?.role,
    userEntityId: user?.entityId,
    userEntityType: user?.entityType,
  };
};
