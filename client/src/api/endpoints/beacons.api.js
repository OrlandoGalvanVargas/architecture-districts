import { apiClient } from "@/api/client";

const ENDPOINT = "/beacons";

export const beaconsApi = {
  getAll: async (params = {}) => {
    const response = await apiClient.get(ENDPOINT, { params });
    return response.data;
  },

  getById: async (id) => {
    const response = await apiClient.get(`${ENDPOINT}/${id}`);
    return response.data;
  },

  create: async (beaconData) => {
    const response = await apiClient.post(ENDPOINT, beaconData);
    return response.data;
  },

  update: async (id, beaconData) => {
    const response = await apiClient.put(`${ENDPOINT}/${id}`, beaconData);
    return response.data;
  },

  delete: async (id) => {
    await apiClient.delete(`${ENDPOINT}/${id}`);
  },
};
