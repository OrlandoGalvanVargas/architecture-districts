export const DISTRICT_BREADCRUMBS = {
  home: {
    label: "Home",
    path: "/",
  },
  list: {
    label: "Districts",
    path: "/districts",
  },
  create: {
    label: "Create",
    path: "/districts/create",
  },
  detail: {
    label: "Detail",
    path: "/districts/:id",
  },
  edit: {
    label: "Edit",
    path: "/districts/:id/edit",
  },
};

export const DISTRICT_BREADCRUMB_CHAINS = {
  list: ["home", "list"],
  create: ["home", "list", "create"],
  detail: ["home", "list", "detail"],
  edit: ["home", "list", "detail", "edit"],
};
