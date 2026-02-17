// Simular delay de red
const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

// Datos mock
let mockDistricts = [
  {
    id: 1,
    name: "Los Angeles Unified School District",
    code: "LAUSD-001",
    state: "CA",
    city: "Los Angeles",
    zipCode: "90012",
    address: "333 S Beaudry Ave",
    description: "Largest school district in California",
    schoolCount: 15,
  },
  {
    id: 2,
    name: "San Diego Unified School District",
    code: "SDUSD-002",
    state: "CA",
    city: "San Diego",
    zipCode: "92101",
    address: "4100 Normal St",
    description: "Second largest district in San Diego County",
    schoolCount: 8,
  },
];

let nextId = 3;

export const mockDistrictsApi = {
  getAll: async () => {
    await delay(800);
    return mockDistricts;
  },

  getById: async (id) => {
    await delay(500);
    const district = mockDistricts.find((d) => d.id === parseInt(id));
    if (!district) {
      throw { message: "District not found", status: 404 };
    }
    return district;
  },

  create: async (data) => {
    await delay(1000);
    const newDistrict = {
      ...data,
      id: nextId++,
      schoolCount: 0,
    };
    mockDistricts.push(newDistrict);
    return newDistrict;
  },

  update: async (id, data) => {
    await delay(800);
    const index = mockDistricts.findIndex((d) => d.id === parseInt(id));
    if (index === -1) {
      throw { message: "District not found", status: 404 };
    }
    mockDistricts[index] = { ...mockDistricts[index], ...data };
    return mockDistricts[index];
  },

  delete: async (id) => {
    await delay(500);
    const index = mockDistricts.findIndex((d) => d.id === parseInt(id));
    if (index === -1) {
      throw { message: "District not found", status: 404 };
    }
    mockDistricts = mockDistricts.filter((d) => d.id !== parseInt(id));
    return { success: true };
  },
};
