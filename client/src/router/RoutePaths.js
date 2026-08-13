import { ROUTES_CONFIG } from "./routes";

export const RoutePaths = {
  home: () => ROUTES_CONFIG.home.path,

  auth: {
    login: () =>
      `${ROUTES_CONFIG.auth.path}/${ROUTES_CONFIG.auth.children.login.buildPath()}`,
  },

  districts: {
    list: () => ROUTES_CONFIG.districts.path,

    create: () =>
      `${ROUTES_CONFIG.districts.path}/${ROUTES_CONFIG.districts.children.create.buildPath()}`,

    detail: (id) =>
      `${ROUTES_CONFIG.districts.path}/${ROUTES_CONFIG.districts.children.detail.buildPath(id)}`,

    edit: (id) =>
      `${ROUTES_CONFIG.districts.path}/${ROUTES_CONFIG.districts.children.edit.buildPath(id)}`,
  },

  schools: {
    list: () => ROUTES_CONFIG.schools.path,

    create: () =>
      `${ROUTES_CONFIG.schools.path}/${ROUTES_CONFIG.schools.children.create.buildPath()}`,

    detail: (id) =>
      `${ROUTES_CONFIG.schools.path}/${ROUTES_CONFIG.schools.children.detail.buildPath(id)}`,

    edit: (id) =>
      `${ROUTES_CONFIG.schools.path}/${ROUTES_CONFIG.schools.children.edit.buildPath(id)}`,
  },
};
