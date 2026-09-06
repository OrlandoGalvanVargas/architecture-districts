import { apiClient } from "@/api/client";

const ENDPOINT = "/auth";

export const authApi = {
  login: async (credentials) => {
    const response = await apiClient.post(`${ENDPOINT}/login`, credentials);
    return response.data;
  },

  logout: async () => {
    const response = await apiClient.post(`${ENDPOINT}/logout`);
    return response.data;
  },

  refreshToken: async () => {
    const response = await apiClient.post(`${ENDPOINT}/refresh`);
    return response.data;
  },

  getCurrentUser: async () => {
    const response = await apiClient.get(`${ENDPOINT}/me`);
    return response.data;
  },
};
