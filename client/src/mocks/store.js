import { get, set } from "idb-keyval";
import { seedData } from "./data/seed";

const DB_KEY = "facilityos-mock-db";

export const loadMockDB = async () => {
  const existing = await get(DB_KEY);
  if (existing) return existing;
  await set(DB_KEY, seedData);
  return seedData;
};

export const saveMockDB = async (db) => {
  await set(DB_KEY, db);
};
