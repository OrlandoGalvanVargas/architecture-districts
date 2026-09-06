import { describe, expect, it, beforeEach } from "vitest";
import { tokenManager } from "@/utils/tokenManager";
import { checkPermission, PERMISSIONS, ROLE_PERMISSIONS, ROLES } from "@/utils/permissions";
import { ApiError, handleApiError } from "@/utils/errorHandler";
import { ERROR_CODES, getErrorMessage } from "@/utils/errorCatalog";
import {
  getBeaconStatusLabel,
  getBeaconTypeLabel,
} from "@/features/beacons/constants/beaconConstants";

describe("tokenManager", () => {
  beforeEach(() => {
    localStorage.clear();
  });

  it("stores and retrieves an access token and user", () => {
    const user = { id: 1, role: ROLES.ADMIN };

    tokenManager.setAccessToken("token");
    tokenManager.setUser(user);

    expect(tokenManager.getAccessToken()).toBe("token");
    expect(tokenManager.getUser()).toEqual(user);
    expect(tokenManager.hasValidSession()).toBe(true);
  });

  it("creates and clears a complete session", () => {
    const user = { id: 2, role: ROLES.USER };

    tokenManager.setSession("session-token", user);
    expect(tokenManager.getUser()).toEqual(user);

    tokenManager.clearSession();
    expect(tokenManager.getAccessToken()).toBeNull();
    expect(tokenManager.getUser()).toBeNull();
    expect(tokenManager.hasValidSession()).toBe(false);
  });

  it("does not replace the existing user when a session has no user", () => {
    tokenManager.setUser({ id: 3 });

    tokenManager.setSession("token-without-user");

    expect(tokenManager.getUser()).toEqual({ id: 3 });
  });
});

describe("permissions", () => {
  it("grants administrators every permission", () => {
    expect(checkPermission({ role: ROLES.ADMIN }, PERMISSIONS.USERS.DELETE)).toBe(true);
  });

  it("applies role-specific permissions", () => {
    expect(checkPermission({ role: ROLES.DISTRICT_ADMIN }, PERMISSIONS.DISTRICTS.CREATE)).toBe(
      false
    );
    expect(checkPermission({ role: ROLES.DISTRICT_ADMIN }, PERMISSIONS.SCHOOLS.CREATE)).toBe(true);
    expect(checkPermission({ role: ROLES.SCHOOL_ADMIN }, PERMISSIONS.SCHOOLS.UPDATE)).toBe(true);
  });

  it("denies anonymous and unknown roles", () => {
    expect(checkPermission(null, PERMISSIONS.DISTRICTS.VIEW_LIST)).toBe(false);
    expect(checkPermission({ role: "Unknown" }, PERMISSIONS.DISTRICTS.VIEW_LIST)).toBe(false);
  });

  it("keeps every declared role permission boolean", () => {
    Object.values(ROLE_PERMISSIONS).forEach((permissions) => {
      Object.values(permissions).forEach((allowed) => expect(typeof allowed).toBe("boolean"));
    });
  });
});

describe("error handling", () => {
  it("preserves an existing ApiError", () => {
    const error = new ApiError(400, "Already normalized");

    expect(handleApiError(error)).toBe(error);
  });

  it("uses the first server validation error", () => {
    const error = handleApiError({
      response: {
        status: ERROR_CODES.VALIDATION_ERROR,
        data: { errors: { email: ["Email is invalid"], password: ["Password is required"] } },
      },
    });

    expect(error).toMatchObject({
      name: "ApiError",
      status: 422,
      message: "Email is invalid",
      details: { email: ["Email is invalid"], password: ["Password is required"] },
    });
  });

  it("prefers server detail, then title, then a contextual catalog message", () => {
    expect(
      handleApiError(
        { response: { status: 404, data: { detail: "Missing district" } } },
        "district"
      )
    ).toMatchObject({ friendlyMessage: "Missing district" });
    expect(
      handleApiError({ response: { status: 404, data: { title: "Not found" } } }, "district")
    ).toMatchObject({ friendlyMessage: "Not found" });
    expect(handleApiError({ response: { status: 404 } }, "district")).toMatchObject({
      friendlyMessage: "District not found.",
    });
  });

  it("falls back safely for unknown status codes", () => {
    expect(getErrorMessage(999)).toBe("An unexpected error occurred. Please try again later.");
    expect(handleApiError({ response: { status: 999 } }).status).toBe(999);
  });
});

describe("beacon constants", () => {
  it("returns labels and an explicit fallback for unknown values", () => {
    expect(getBeaconTypeLabel(1)).toBe("Pendant");
    expect(getBeaconStatusLabel(2)).toBe("Assigned");
    expect(getBeaconTypeLabel(99)).toBe("Unknown");
    expect(getBeaconStatusLabel(99)).toBe("Unknown");
  });
});
