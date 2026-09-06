export const BEACON_TYPES = {
  1: "Pendant",
  2: "Wristband",
  3: "Fixed",
  4: "Mobile",
};

export const BEACON_STATUSES = {
  1: "Available",
  2: "Assigned",
  3: "Maintenance",
  4: "Inactive",
};

export const getBeaconTypeLabel = (type) => BEACON_TYPES[type] || "Unknown";
export const getBeaconStatusLabel = (status) => BEACON_STATUSES[status] || "Unknown";
