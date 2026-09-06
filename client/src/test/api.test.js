import { beforeEach, describe, expect, it, vi } from "vitest";

const { apiClient } = vi.hoisted(() => ({
  apiClient: {
    get: vi.fn(),
    post: vi.fn(),
    put: vi.fn(),
    delete: vi.fn(),
  },
}));

vi.mock("@/api/client", () => ({ apiClient }));

import { authApi } from "@/api/endpoints/auth.api";
import { beaconsApi } from "@/api/endpoints/beacons.api";
import { districtsApi } from "@/api/endpoints/districts.api";
import { facultiesApi } from "@/api/endpoints/faculties.api";
import { schoolsApi } from "@/api/endpoints/school.api";
import { usersApi } from "@/api/endpoints/users.api";

const resources = [
  ["districts", districtsApi],
  ["schools", schoolsApi],
  ["users", usersApi],
  ["beacons", beaconsApi],
  ["faculties", facultiesApi],
];

describe("resource API adapters", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    apiClient.get.mockResolvedValue({ data: [{ id: 1 }] });
    apiClient.post.mockResolvedValue({ data: { id: 1 } });
    apiClient.put.mockResolvedValue({ data: { id: 1 } });
    apiClient.delete.mockResolvedValue({});
  });

  it.each(resources)("supports the %s CRUD contract", async (resource, api) => {
    const payload = { name: "Example" };

    await expect(api.getAll({ page: 1 })).resolves.toEqual([{ id: 1 }]);
    expect(apiClient.get).toHaveBeenCalledWith(`/${resource}`, { params: { page: 1 } });

    await expect(api.getById(7)).resolves.toEqual([{ id: 1 }]);
    expect(apiClient.get).toHaveBeenCalledWith(`/${resource}/7`);

    await expect(api.create(payload)).resolves.toEqual({ id: 1 });
    expect(apiClient.post).toHaveBeenCalledWith(`/${resource}`, payload);

    await expect(api.update(7, payload)).resolves.toEqual({ id: 1 });
    expect(apiClient.put).toHaveBeenCalledWith(`/${resource}/7`, payload);

    await expect(api.delete(7)).resolves.toBeUndefined();
    expect(apiClient.delete).toHaveBeenCalledWith(`/${resource}/7`);
  });

  it("defaults list parameters to an empty object", async () => {
    await districtsApi.getAll();

    expect(apiClient.get).toHaveBeenCalledWith("/districts", { params: {} });
  });
});

describe("auth API adapter", () => {
  beforeEach(() => {
    vi.clearAllMocks();
    apiClient.post.mockResolvedValue({ data: { accessToken: "token" } });
    apiClient.get.mockResolvedValue({ data: { id: 1 } });
  });

  it("maps authentication operations to their endpoints and returns response data", async () => {
    await expect(authApi.login({ email: "user@example.com", password: "secret" })).resolves.toEqual(
      {
        accessToken: "token",
      }
    );
    await expect(authApi.logout()).resolves.toEqual({ accessToken: "token" });
    await expect(authApi.refreshToken()).resolves.toEqual({ accessToken: "token" });
    await expect(authApi.getCurrentUser()).resolves.toEqual({ id: 1 });

    expect(apiClient.post).toHaveBeenNthCalledWith(1, "/auth/login", {
      email: "user@example.com",
      password: "secret",
    });
    expect(apiClient.post).toHaveBeenNthCalledWith(2, "/auth/logout");
    expect(apiClient.post).toHaveBeenNthCalledWith(3, "/auth/refresh");
    expect(apiClient.get).toHaveBeenCalledWith("/auth/me");
  });
});
