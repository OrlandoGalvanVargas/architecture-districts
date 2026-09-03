import axios from "axios";
import { tokenManager } from "@/utils/tokenManager";
import { handleApiError } from "@/utils/errorHandler";

export const apiClient = axios.create({
  baseURL: import.meta.env.PROD ? import.meta.env.VITE_API_URL : "/api",
  timeout: Number(import.meta.env.VITE_API_TIMEOUT) || 30000,
  headers: {
    "Content-Type": "application/json",
  },
  withCredentials: true,
});

apiClient.interceptors.request.use(
  (config) => {
    const token = tokenManager.getAccessToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach((prom) => {
    if (error) {
      prom.reject(error);
    } else {
      prom.resolve(token);
    }
  });
  failedQueue = [];
};

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (
      error.response?.status === 401 &&
      !originalRequest._retry &&
      !originalRequest.url?.includes("/auth/login") &&
      !originalRequest.url?.includes("/auth/refresh")
    ) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        })
          .then((token) => {
            originalRequest.headers.Authorization = `Bearer ${token}`;
            return apiClient(originalRequest);
          })
          .catch((err) => Promise.reject(err));
      }

      originalRequest._retry = true;
      isRefreshing = true;

      try {
        const response = await apiClient.post("/auth/refresh");
        const { accessToken, user } = response.data;

        tokenManager.setSession(accessToken, user);
        processQueue(null, accessToken);

        originalRequest.headers.Authorization = `Bearer ${accessToken}`;
        return apiClient(originalRequest);
      } catch (refreshError) {
        processQueue(refreshError, null);
        tokenManager.clearSession();

        if (window.location.pathname !== "/auth/login") {
          window.location.href = "/auth/login";
        }

        return Promise.reject(handleApiError(refreshError, "tokenExpired"));
      } finally {
        isRefreshing = false;
      }
    }

    return Promise.reject(handleApiError(error));
  }
);
