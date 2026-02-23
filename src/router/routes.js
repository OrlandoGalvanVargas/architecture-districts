export const ROUTES_CONFIG = {
  home: {
    path: "/",
    title: "Home",
  },

  districts: {
    path: "/districts",
    title: "Districts",

    children: {
      list: {
        path: "",
        title: "Districts List",
      },
      create: {
        path: "create",
        title: "Create District",
      },
      detail: {
        path: ":id",
        title: "District Details",
        param: "id",
      },
      edit: {
        path: ":id/edit",
        title: "Edit District",
        param: "id",
      },
    },
  },

  schools: {
    path: "/schools",
    title: "Schools",

    children: {
      list: {
        path: "",
        title: "Schools List",
      },
      create: {
        path: "create",
        title: "Create School",
      },
      detail: {
        path: ":id",
        title: "School Details",
        param: "id",
      },
      edit: {
        path: ":id/edit",
        title: "Edit School",
        param: "id",
      },
    },
  },
};
