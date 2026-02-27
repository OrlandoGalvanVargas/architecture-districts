const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

const shouldSimulateError = () => {
  return false;
};

let mockDistricts = [
  {
    id: 1,
    name: "Los Angeles Unified School District",
    code: "LAUSD-001",
    state: "CA",
    city: "Los Angeles",
    zipCode: "90012",
    address: "333 S Beaudry Ave",
    description:
      "The largest school district in California and the second largest in the United States. Serves over 600,000 students.",
    schoolCount: 15,
    createdAt: "2024-01-15T10:00:00Z",
    updatedAt: "2024-02-10T14:30:00Z",
  },
  {
    id: 2,
    name: "San Diego Unified School District",
    code: "SDUSD-002",
    state: "CA",
    city: "San Diego",
    zipCode: "92101",
    address: "4100 Normal St",
    description:
      "Second largest district in San Diego County, serving diverse communities.",
    schoolCount: 8,
    createdAt: "2024-01-20T09:00:00Z",
    updatedAt: "2024-02-05T11:20:00Z",
  },
  {
    id: 3,
    name: "Austin Independent School District",
    code: "AISD-003",
    state: "TX",
    city: "Austin",
    zipCode: "78701",
    address: "4000 S IH 35 Frontage Rd",
    description:
      "Serving the greater Austin area with innovative educational programs.",
    schoolCount: 12,
    createdAt: "2024-02-01T08:30:00Z",
    updatedAt: "2024-02-15T16:45:00Z",
  },
];

let nextId = 4;

export const mockDistrictsApi = {
  getAll: async (filters = {}) => {
    await delay(800);

    if (shouldSimulateError()) {
      throw {
        message: "Failed to fetch districts",
        status: 500,
      };
    }

    let result = [...mockDistricts];

    if (filters.search) {
      const searchLower = filters.search.toLowerCase();
      result = result.filter(
        (d) =>
          d.name.toLowerCase().includes(searchLower) ||
          d.code.toLowerCase().includes(searchLower),
      );
    }

    if (filters.state) {
      result = result.filter((d) => d.state === filters.state);
    }

    return result;
  },

  getById: async (id) => {
    await delay(500);

    if (shouldSimulateError()) {
      throw {
        message: "Failed to fetch district",
        status: 500,
      };
    }

    const district = mockDistricts.find((d) => d.id === parseInt(id));

    if (!district) {
      throw {
        message: "District not found",
        status: 404,
      };
    }

    return district;
  },

  create: async (data) => {
    await delay(1000);

    if (shouldSimulateError()) {
      throw {
        message: "Failed to create district",
        status: 500,
      };
    }

    if (!data.name || !data.code) {
      throw {
        message: "Name and code are required",
        status: 400,
      };
    }

    if (mockDistricts.some((d) => d.code === data.code)) {
      throw {
        message: "District code already exists",
        status: 409,
      };
    }

    const now = new Date().toISOString();
    const newDistrict = {
      ...data,
      id: nextId++,
      schoolCount: 0,
      createdAt: now,
      updatedAt: now,
    };

    mockDistricts.push(newDistrict);
    return newDistrict;
  },

  update: async (id, data) => {
    await delay(800);

    if (shouldSimulateError()) {
      throw {
        message: "Failed to update district",
        status: 500,
      };
    }

    const index = mockDistricts.findIndex((d) => d.id === parseInt(id));

    if (index === -1) {
      throw {
        message: "District not found",
        status: 404,
      };
    }

    if (data.code && data.code !== mockDistricts[index].code) {
      if (
        mockDistricts.some((d) => d.code === data.code && d.id !== parseInt(id))
      ) {
        throw {
          message: "District code already exists",
          status: 409,
        };
      }
    }

    mockDistricts[index] = {
      ...mockDistricts[index],
      ...data,
      updatedAt: new Date().toISOString(),
    };

    return mockDistricts[index];
  },

  delete: async (id) => {
    await delay(500);

    if (shouldSimulateError()) {
      throw {
        message: "Failed to delete district",
        status: 500,
      };
    }

    const index = mockDistricts.findIndex((d) => d.id === parseInt(id));

    if (index === -1) {
      throw {
        message: "District not found",
        status: 404,
      };
    }

    if (mockDistricts[index].schoolCount > 0) {
      throw {
        message: "Cannot delete district with assigned schools",
        status: 400,
      };
    }

    mockDistricts = mockDistricts.filter((d) => d.id !== parseInt(id));

    return {
      success: true,
      message: "District deleted successfully",
    };
  },
};
