import { ROUTES_CONFIG } from "./routes";

export const RoutePaths = {
  home: () => ROUTES_CONFIG.home.path,

  districts: {
    list: () => ROUTES_CONFIG.districts.path,

    create: () =>
      `${ROUTES_CONFIG.districts.path}/${ROUTES_CONFIG.districts.children.create.path}`,

    detail: (id) => `${ROUTES_CONFIG.districts.path}/${id}`,

    edit: (id) => `${ROUTES_CONFIG.districts.path}/${id}/edit`,
  },

  schools: {
    list: () => ROUTES_CONFIG.schools.path,
    create: () =>
      `${ROUTES_CONFIG.schools.path}/${ROUTES_CONFIG.schools.children.create.path}`,
    detail: (id) => `${ROUTES_CONFIG.schools.path}/${id}`,
    edit: (id) => `${ROUTES_CONFIG.schools.path}/${id}/edit`,
  },
};
