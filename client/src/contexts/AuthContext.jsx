/* eslint-disable react-refresh/only-export-components */
import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import { tokenManager } from "@/utils/tokenManager";
import { authApi } from "@/api/endpoints/auth.api";
import { logger } from "@/services/logger.service";
import { useNotification } from "./Notification";
import { queryClient } from "@/config/queryClient";
import { checkPermission } from "@/utils/permissions";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(() => tokenManager.getUser());
  const [isAuthenticated, setIsAuthenticated] = useState(() => tokenManager.hasValidSession());
  const [isLoading, setIsLoading] = useState(true);
  const { showError } = useNotification();

  const login = useCallback(
    async (credentials) => {
      try {
        const response = await authApi.login(credentials);
        const { accessToken, user } = response;

        tokenManager.setSession(accessToken, user);
        setUser(user);
        setIsAuthenticated(true);

        return { success: true };
      } catch (error) {
        logger.error("Login failed", error);
        const msg = error.friendlyMessage || "Invalid credentials. Please try again.";
        showError(msg);
        return { success: false, error: msg };
      }
    },
    [showError]
  );

  const logout = useCallback(async () => {
    try {
      await authApi.logout();
    } catch (error) {
      logger.warn("Logout API failed, clearing local session anyway", error);
    } finally {
      tokenManager.clearSession();
      setUser(null);
      setIsAuthenticated(false);
      queryClient.clear();
    }
  }, []);

  const updateUser = useCallback((updatedUser) => {
    setUser(updatedUser);
    tokenManager.setUser(updatedUser);
  }, []);

  useEffect(() => {
    const verifySession = async () => {
      if (!tokenManager.hasValidSession()) {
        setIsLoading(false);
        return;
      }

      try {
        const currentUser = await authApi.getCurrentUser();
        setUser(currentUser);
        tokenManager.setUser(currentUser);
        setIsAuthenticated(true);
      } catch (error) {
        logger.warn("Session verification failed", error);
        tokenManager.clearSession();
        setUser(null);
        setIsAuthenticated(false);
      } finally {
        setIsLoading(false);
      }
    };

    verifySession();
  }, []);

  const hasRole = useCallback(
    (roles) => {
      if (!user) return false;
      if (Array.isArray(roles)) return roles.includes(user.role);
      return user.role === roles;
    },
    [user]
  );

  const hasPermission = useCallback(
    (permission) => {
      return checkPermission(user, permission);
    },
    [user]
  );

  const value = useMemo(
    () => ({
      user,
      isAuthenticated,
      isLoading,
      login,
      logout,
      updateUser,
      hasRole,
      hasPermission,
    }),
    [user, isAuthenticated, isLoading, login, logout, updateUser, hasRole, hasPermission]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }
  return context;
};
