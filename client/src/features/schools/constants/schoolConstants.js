export const SCHOOL_LEVELS = {
  0: "Elementary",
  1: "Middle School",
  2: "High School",
  3: "K-12",
  4: "Pre-K",
};

export const SCHOOL_TYPES = {
  0: "Public",
  1: "Charter",
  2: "Magnet",
  3: "Alternative",
};

export const US_STATES = [
  { value: "CA", label: "California" },
  { value: "TX", label: "Texas" },
  { value: "NY", label: "New York" },
  { value: "FL", label: "Florida" },
  { value: "AZ", label: "Arizona" },
  { value: "NV", label: "Nevada" },
  { value: "WA", label: "Washington" },
  { value: "OR", label: "Oregon" },
  { value: "CO", label: "Colorado" },
  { value: "IL", label: "Illinois" },
];

export const getSchoolLevelLabel = (level) => SCHOOL_LEVELS[level] || "Unknown";
export const getSchoolTypeLabel = (type) => SCHOOL_TYPES[type] || "Unknown";
