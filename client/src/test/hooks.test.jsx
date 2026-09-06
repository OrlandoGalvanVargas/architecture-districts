import { act, renderHook } from "@testing-library/react";
import { describe, expect, it, vi } from "vitest";
import { useOnlineStatus } from "@/hooks/useOnlineStatus";
import { queryKeys } from "@/api/queryKeys";
import { useUIStore } from "@/store/ui.store";

describe("useOnlineStatus", () => {
  it("tracks browser online and offline events", () => {
    const { result, unmount } = renderHook(() => useOnlineStatus());

    expect(result.current).toBe(navigator.onLine);
    act(() => window.dispatchEvent(new Event("offline")));
    expect(result.current).toBe(false);
    act(() => window.dispatchEvent(new Event("online")));
    expect(result.current).toBe(true);
    unmount();
  });
});

describe("queryKeys", () => {
  it("creates stable list and detail keys for each resource", () => {
    expect(queryKeys.districts.list({ page: 1 })).toEqual(["districts", "list", { page: 1 }]);
    expect(queryKeys.schools.detail(4)).toEqual(["schools", "detail", 4]);
    expect(queryKeys.users.all()).toEqual(["users"]);
    expect(queryKeys.beacons.detail("b-1")).toEqual(["beacons", "detail", "b-1"]);
    expect(queryKeys.faculties.list()).toEqual(["faculties", "list", undefined]);
  });
});

describe("useUIStore", () => {
  it("updates transient UI state and keeps drafts isolated", () => {
    const initialState = useUIStore.getState();
    const storage = localStorage;
    storage.clear();

    act(() => {
      useUIStore.getState().toggleSidebar();
      useUIStore.getState().setTheme("dark");
      useUIStore.getState().openModal("profile");
      useUIStore.getState().saveDraft("district", { name: "North" });
    });

    expect(useUIStore.getState()).toMatchObject({
      sidebarCollapsed: !initialState.sidebarCollapsed,
      theme: "dark",
      activeModal: "profile",
      formDrafts: { district: { name: "North" } },
    });

    act(() => {
      useUIStore.getState().closeModal();
      useUIStore.getState().clearDraft("district");
    });
    expect(useUIStore.getState().activeModal).toBeNull();
    expect(useUIStore.getState().formDrafts).toEqual({});

    act(() => {
      useUIStore.setState(initialState);
    });
    vi.restoreAllMocks();
  });
});
