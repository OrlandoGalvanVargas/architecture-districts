export const ROUTES_CONFIG = {
  home: {
    path: "/",
    title: "Home",
  },

  districts: {
    path: "/districts",
    title: "Districts",
    singularTitle: "District",

    children: {
      list: {
        pattern: "",
        title: "Districts List",
        buildPath: () => "",
      },
      create: {
        pattern: "create",
        title: "Create District",
        buildPath: () => "create",
      },
      detail: {
        pattern: ":id",
        title: "District Details",
        singularTitle: "District",
        buildPath: (id) => `${id}`,
      },
      edit: {
        pattern: ":id/edit",
        title: "Edit District",
        buildPath: (id) => `${id}/edit`,
      },
    },
  },
};
