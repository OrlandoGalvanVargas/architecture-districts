import { apiClient } from "@/api/client";

const ENDPOINT = "/districts";

export const districtsApi = {
  getAll: async (params = {}) => {
    const response = await apiClient.get(ENDPOINT, { params });
    return response.data;
  },

  getById: async (id) => {
    const response = await apiClient.get(`${ENDPOINT}/${id}`);
    return response.data;
  },

  create: async (districtData) => {
    const response = await apiClient.post(ENDPOINT, districtData);
    return response.data;
  },

  update: async (id, districtData) => {
    const response = await apiClient.put(`${ENDPOINT}/${id}`, districtData);
    return response.data;
  },

  delete: async (id) => {
    await apiClient.delete(`${ENDPOINT}/${id}`);
  },
};
