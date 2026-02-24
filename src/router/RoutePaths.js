import { ROUTES_CONFIG } from "./routes";

export const RoutePaths = {
  home: () => ROUTES_CONFIG.home.path,

  districts: {
    list: () => ROUTES_CONFIG.districts.path,

    create: () =>
      `${ROUTES_CONFIG.districts.path}/${ROUTES_CONFIG.districts.children.create.buildPath()}`,

    detail: (id) =>
      `${ROUTES_CONFIG.districts.path}/${ROUTES_CONFIG.districts.children.detail.buildPath(id)}`,

    edit: (id) =>
      `${ROUTES_CONFIG.districts.path}/${ROUTES_CONFIG.districts.children.edit.buildPath(id)}`,
  },
};
