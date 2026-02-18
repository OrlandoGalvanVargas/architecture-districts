import { apiClient } from "../client";
import { mockDistrictsApi } from "../mock/districts.mock";

const ENDPOINT = "/districts";
const USE_MOCK = true;

export const districtsApi = USE_MOCK
  ? mockDistrictsApi
  : {
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
        return response;
      },
      update: async (id, districtData) => {
        const response = await apiClient.put(`${ENDPOINT}/${id}`, districtData);
        return response;
      },
      delete: async (id) => {
        const response = await apiClient.delete(`${ENDPOINT}/${id}`);
        return response;
      },
    };
