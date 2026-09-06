import { apiClient } from "@/api/client";

const ENDPOINT = "/users";

export const usersApi = {
  getAll: async (params = {}) => {
    const response = await apiClient.get(ENDPOINT, { params });
    return response.data;
  },

  getById: async (id) => {
    const response = await apiClient.get(`${ENDPOINT}/${id}`);
    return response.data;
  },

  create: async (userData) => {
    const response = await apiClient.post(ENDPOINT, userData);
    return response.data;
  },

  update: async (id, userData) => {
    const response = await apiClient.put(`${ENDPOINT}/${id}`, userData);
    return response.data;
  },

  delete: async (id) => {
    await apiClient.delete(`${ENDPOINT}/${id}`);
  },
};
