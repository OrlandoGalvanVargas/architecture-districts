import { apiClient } from "../client";

const ENDPOINT = "/schools";

export const schoolsApi = {
  getAll: async (params = {}) => {
    const response = await apiClient.get(ENDPOINT, { params });
    return response.data.items;
  },
  getById: async (id) => {
    const response = await apiClient.get(`${ENDPOINT}/${id}`);
    return response.data;
  },
  create: async (data) => {
    const response = await apiClient.post(ENDPOINT, data);
    return response.data;
  },
  update: async (id, data) => {
    const response = await apiClient.put(`${ENDPOINT}/${id}`, data);
    return response.data;
  },
  delete: async (id) => {
    await apiClient.delete(`${ENDPOINT}/${id}`);
  },
};
