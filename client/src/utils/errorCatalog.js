export const ERROR_CODES = {
  BAD_REQUEST: 400,
  UNAUTHORIZED: 401,
  FORBIDDEN: 403,
  NOT_FOUND: 404,
  CONFLICT: 409,
  VALIDATION_ERROR: 422,
  INTERNAL_SERVER_ERROR: 500,
  SERVICE_UNAVAILABLE: 503,
};

export const ERROR_MESSAGES = {
  [ERROR_CODES.BAD_REQUEST]: {
    default: "Invalid request. Please check your input.",
    login: "Invalid login request.",
    create: "Invalid data provided for creation.",
    update: "Invalid data provided for update.",
  },

  [ERROR_CODES.UNAUTHORIZED]: {
    default: "Your session has expired. Please sign in again.",
    login: "Invalid email or password. Please try again.",
    tokenExpired: "Your session has expired. Redirecting to login...",
    invalidCredentials: "Invalid credentials. Please check your email and password.",
    accountDisabled: "Your account has been disabled. Contact your administrator.",
  },

  [ERROR_CODES.FORBIDDEN]: {
    default: "You do not have permission to perform this action.",
    view: "You do not have permission to view this resource.",
    create: "You do not have permission to create this resource.",
    update: "You do not have permission to modify this resource.",
    delete: "You do not have permission to delete this resource.",
  },

  [ERROR_CODES.NOT_FOUND]: {
    default: "The requested resource was not found.",
    district: "District not found.",
    school: "School not found.",
    user: "User not found.",
    beacon: "Beacon not found.",
    faculty: "Faculty not found.",
  },

  [ERROR_CODES.CONFLICT]: {
    default: "A conflict occurred. Please try again.",
    duplicateEmail: "This email is already registered.",
    duplicateCode: "This code is already in use.",
    duplicateSerial: "This serial number is already in use.",
    beaconAssigned: "This beacon is already assigned.",
    beaconNotAvailable: "This beacon is not available for assignment.",
  },

  [ERROR_CODES.VALIDATION_ERROR]: {
    default: "Please check the form for errors.",
    required: "This field is required.",
    email: "Please enter a valid email address.",
    passwordLength: "Password must be at least 6 characters.",
    passwordComplexity: "Password must contain uppercase, lowercase, and numbers.",
  },

  [ERROR_CODES.INTERNAL_SERVER_ERROR]: {
    default: "An unexpected error occurred. Please try again later.",
  },

  [ERROR_CODES.SERVICE_UNAVAILABLE]: {
    default: "Service temporarily unavailable. Please try again later.",
  },
};

export const getErrorMessage = (status, context = "default") => {
  const messages = ERROR_MESSAGES[status];
  if (!messages) {
    return ERROR_MESSAGES[ERROR_CODES.INTERNAL_SERVER_ERROR].default;
  }
  return messages[context] || messages.default;
};
