import { create } from "zustand";
import { persist } from "zustand/middleware";

export const useUIStore = create(
  persist(
    (set) => ({
      sidebarCollapsed: false,
      toggleSidebar: () => set((state) => ({ sidebarCollapsed: !state.sidebarCollapsed })),

      theme: "light",
      setTheme: (theme) => set({ theme }),

      activeModal: null,
      openModal: (modal) => set({ activeModal: modal }),
      closeModal: () => set({ activeModal: null }),

      formDrafts: {},
      saveDraft: (key, data) =>
        set((state) => ({
          formDrafts: { ...state.formDrafts, [key]: data },
        })),
      clearDraft: (key) =>
        set((state) => {
          const { [key]: _, ...rest } = state.formDrafts;
          return { formDrafts: rest };
        }),
    }),
    {
      name: "facilityos-ui",
      partialize: (state) => ({
        theme: state.theme,
        sidebarCollapsed: state.sidebarCollapsed,
      }),
    }
  )
);
