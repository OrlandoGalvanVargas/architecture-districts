import { ERROR_CODES, getErrorMessage } from "./errorCatalog";

export class ApiError extends Error {
  constructor(status, message, details = null) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.details = details;
    this.friendlyMessage = message;
  }
}

export const handleApiError = (error, context = "default") => {
  if (error instanceof ApiError) {
    return error;
  }

  const status = error?.response?.status || ERROR_CODES.INTERNAL_SERVER_ERROR;

  const serverDetail = error?.response?.data?.detail;
  const serverTitle = error?.response?.data?.title;

  if (error?.response?.data?.errors) {
    const validationErrors = error.response.data.errors;
    const firstError = Object.values(validationErrors)[0]?.[0];
    return new ApiError(status, firstError || "Validation failed", validationErrors);
  }

  const friendlyMessage = serverDetail || serverTitle || getErrorMessage(status, context);

  return new ApiError(status, friendlyMessage, error?.response?.data);
};
