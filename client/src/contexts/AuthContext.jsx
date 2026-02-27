import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useState,
} from "react";
import { tokenManager } from "../utils/tokenManager";
import { authApi } from "../api/auth/auth.api";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);

  const login = useCallback(async (credentials) => {
    try {
      const response = await authApi.login(credentials);
      const { accessToken, refreshToken, user } = response;

      tokenManager.setToken(accessToken, refreshToken);
      tokenManager.setUser(user);

      setUser(user);
      setIsAuthenticated(true);

      return { success: true };
    } catch (error) {
      return {
        success: false,
        error: error?.message || "Login failed",
      };
    }
  }, []);

  const logout = useCallback(async () => {
    try {
      await authApi.logout();
    } catch (error) {
      console.log("Logout error: ", error);
    } finally {
      tokenManager.clearTokens();
      setUser(null);
      setIsAuthenticated(false);
    }
  }, []);

  const updateUser = useCallback((updateUser) => {
    setUser(updateUser);
    tokenManager.setUser(updateUser);
  }, []);

  useEffect(() => {
    const initAuth = async () => {
      const savedUser = tokenManager.getUser();
      const hastToken = tokenManager.hasValidSession();

      if (savedUser && hastToken) {
        setUser(savedUser);
        setIsAuthenticated(true);

        try {
          const currentUser = await authApi.getCurrentUser();
          setUser(currentUser);
          tokenManager.setUser(currentUser);
        } catch (error) {
          console.log("Invalid token, logging out: ", error);
          logout();
        }
      }

      setIsLoading(false);
    };

    initAuth();
  }, [logout]);

  const value = {
    user,
    isAuthenticated,
    isLoading,
    login,
    logout,
    updateUser,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useauth must be used within AuthProvider");
  return context;
};
