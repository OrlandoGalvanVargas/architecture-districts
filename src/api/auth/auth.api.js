import { apiClient } from "../client";
import { mockAuthApi } from "../mock/auth.mock";

const ENDPOINT = "/auth";
const USE_MOCK = true;

export const authApi = USE_MOCK
  ? mockAuthApi
  : {
      login: async (credentials) => {
        const response = await apiClient.post(`${ENDPOINT}/login`, credentials);
        return response.data;
      },

      logout: async () => {
        const response = await apiClient.post(`${ENDPOINT}/logout`);
        return response.data;
      },

      refreshToken: async (refreshToken) => {
        const response = await apiClient.post(`${ENDPOINT}/refresh`, {
          refreshToken,
        });
        return response.data;
      },

      getCurrentUser: async () => {
        const response = await apiClient.get(`${ENDPOINT}/me`);
        return response.data;
      },
    };
