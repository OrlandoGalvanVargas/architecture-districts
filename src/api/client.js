import axios from "axios";

export const apiClient = axios.create({
  baseURL: "https://api-backend/schools",
  timeout: 5000,
  headers: {
    "Content-Type": "application/json",
  },
});

apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("auth_token");
    if (token) config.headers.Authorization = `Bearer ${token}`;
    return config;
  },
  (error) => {
    return Promise.reject(error);
  },
);

apiClient.interceptors.response.use(
  (response) => {
    return response;
  },
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401) {
      originalRequest._retry = true;

      try {
        const response = await apiClient.get(
          `${originalRequest.baseURL}/auth/refresh`,
        );
        const newToken = response.data.token;
        localStorage.setItem("auth_token", newToken);

        originalRequest.headers.Authorization = `Bearer ${newToken}`;
        return apiClient(originalRequest);
      } catch (errorRefresh) {
        localStorage.removeItem("auth_token");
        window.location.href = "/login";
        return Promise.reject(errorRefresh);
      }
    }

    const errorMessage = error.response?.message || "An error occurred";
    return Promise.reject(errorMessage);
  },
);
