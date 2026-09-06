import { apiClient } from "@/api/client";

const ENDPOINT = "/faculties";

export const facultiesApi = {
  getAll: async (params = {}) => {
    const response = await apiClient.get(ENDPOINT, { params });
    return response.data;
  },

  getById: async (id) => {
    const response = await apiClient.get(`${ENDPOINT}/${id}`);
    return response.data;
  },

  create: async (facultyData) => {
    const response = await apiClient.post(ENDPOINT, facultyData);
    return response.data;
  },

  update: async (id, facultyData) => {
    const response = await apiClient.put(`${ENDPOINT}/${id}`, facultyData);
    return response.data;
  },

  delete: async (id) => {
    await apiClient.delete(`${ENDPOINT}/${id}`);
  },
};
