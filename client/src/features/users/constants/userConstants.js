export const USER_ROLES = {
  Admin: "Admin",
  DistrictAdmin: "DistrictAdmin",
  SchoolAdmin: "SchoolAdmin",
  User: "User",
};

export const ENTITY_TYPES = {
  0: "Global",
  1: "District",
  2: "School",
};

export const getRoleColor = (role) => {
  switch (role) {
    case "Admin":
      return "gold";
    case "DistrictAdmin":
      return "blue";
    case "SchoolAdmin":
      return "green";
    default:
      return "default";
  }
};

export const getEntityTypeLabel = (type) => ENTITY_TYPES[type] || "Unknown";
