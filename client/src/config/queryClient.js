import { QueryClient } from "@tanstack/react-query";
import { persistQueryClient } from "@tanstack/react-query-persist-client";
import { createAsyncStoragePersister } from "@tanstack/query-async-storage-persister";
import { get, set, del } from "idb-keyval";
import { handleApiError } from "@/utils/errorHandler";

const indexedDBStorage = {
  getItem: async (key) => {
    try {
      const value = await get(key);
      return value ?? null;
    } catch {
      return null;
    }
  },
  setItem: async (key, value) => {
    try {
      await set(key, value);
    } catch (error) {
      console.warn("Failed to persist query cache:", error);
    }
  },
  removeItem: async (key) => {
    try {
      await del(key);
    } catch (error) {
      console.warn("Failed to remove query cache entry:", error);
    }
  },
};

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000,
      gcTime: 30 * 60 * 1000,
      retry: (failureCount, error) => {
        if (error?.status >= 400 && error?.status < 500) {
          return false;
        }
        return failureCount < 2;
      },
      refetchOnWindowFocus: false,
      refetchOnReconnect: true,
    },
    mutations: {
      retry: 1,
      onError: (error) => {
        return handleApiError(error);
      },
    },
  },
});

const asyncPersister = createAsyncStoragePersister({
  storage: indexedDBStorage,
  key: "facilityos-query-cache",
  throttleTime: 1000,
});

persistQueryClient({
  queryClient,
  persister: asyncPersister,
  maxAge: 24 * 60 * 60 * 1000,
  buster: import.meta.env.VITE_APP_VERSION || "v1",
});
