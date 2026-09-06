export const ROUTES = {
  AUTH: {
    BASE: "/auth",
    LOGIN: "/auth/login",
  },

  HOME: "/",

  DISTRICTS: {
    LIST: "/districts",
    CREATE: "/districts/create",
    DETAIL: (id) => `/districts/${id}`,
    EDIT: (id) => `/districts/${id}/edit`,
  },

  SCHOOLS: {
    LIST: "/schools",
    CREATE: "/schools/create",
    DETAIL: (id) => `/schools/${id}`,
    EDIT: (id) => `/schools/${id}/edit`,
  },

  USERS: {
    LIST: "/users",
    CREATE: "/users/create",
    DETAIL: (id) => `/users/${id}`,
    EDIT: (id) => `/users/${id}/edit`,
  },

  BEACONS: {
    LIST: "/beacons",
    CREATE: "/beacons/create",
    DETAIL: (id) => `/beacons/${id}`,
    EDIT: (id) => `/beacons/${id}/edit`,
  },

  FACULTIES: {
    LIST: "/faculties",
    CREATE: "/faculties/create",
    DETAIL: (id) => `/faculties/${id}`,
    EDIT: (id) => `/faculties/${id}/edit`,
  },
};

export const ROUTE_METADATA = {
  [ROUTES.HOME]: { title: "Home", breadcrumb: "Home" },

  [ROUTES.AUTH.LOGIN]: { title: "Login", breadcrumb: "Login" },

  [ROUTES.DISTRICTS.LIST]: { title: "Districts", breadcrumb: "Districts" },
  [ROUTES.DISTRICTS.CREATE]: { title: "Create District", breadcrumb: "Create" },
  [ROUTES.DISTRICTS.DETAIL(":id")]: { title: "District Details", breadcrumb: "Details" },
  [ROUTES.DISTRICTS.EDIT(":id")]: { title: "Edit District", breadcrumb: "Edit" },

  [ROUTES.SCHOOLS.LIST]: { title: "Schools", breadcrumb: "Schools" },
  [ROUTES.SCHOOLS.CREATE]: { title: "Create School", breadcrumb: "Create" },
  [ROUTES.SCHOOLS.DETAIL(":id")]: { title: "School Details", breadcrumb: "Details" },
  [ROUTES.SCHOOLS.EDIT(":id")]: { title: "Edit School", breadcrumb: "Edit" },

  [ROUTES.USERS.LIST]: { title: "Users", breadcrumb: "Users" },
  [ROUTES.USERS.CREATE]: { title: "Create User", breadcrumb: "Create" },
  [ROUTES.USERS.DETAIL(":id")]: { title: "User Details", breadcrumb: "Details" },
  [ROUTES.USERS.EDIT(":id")]: { title: "Edit User", breadcrumb: "Edit" },

  [ROUTES.BEACONS.LIST]: { title: "Beacons", breadcrumb: "Beacons" },
  [ROUTES.BEACONS.CREATE]: { title: "Create Beacon", breadcrumb: "Create" },
  [ROUTES.BEACONS.DETAIL(":id")]: { title: "Beacon Details", breadcrumb: "Details" },
  [ROUTES.BEACONS.EDIT(":id")]: { title: "Edit Beacon", breadcrumb: "Edit" },

  [ROUTES.FACULTIES.LIST]: { title: "Faculties", breadcrumb: "Faculties" },
  [ROUTES.FACULTIES.CREATE]: { title: "Create Faculty", breadcrumb: "Create" },
  [ROUTES.FACULTIES.DETAIL(":id")]: { title: "Faculty Details", breadcrumb: "Details" },
  [ROUTES.FACULTIES.EDIT(":id")]: { title: "Edit Faculty", breadcrumb: "Edit" },
};
