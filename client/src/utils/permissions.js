export const ROLES = {
  ADMIN: "Admin",
  DISTRICT_ADMIN: "DistrictAdmin",
  SCHOOL_ADMIN: "SchoolAdmin",
  USER: "User",
};

export const ENTITY_TYPES = {
  GLOBAL: 0,
  DISTRICT: 1,
  SCHOOL: 2,
};

export const PERMISSIONS = {
  DISTRICTS: {
    VIEW_LIST: "districts.view.list",
    VIEW_DETAIL: "districts.view.detail",
    CREATE: "districts.create",
    UPDATE: "districts.update",
    DELETE: "districts.delete",
  },

  SCHOOLS: {
    VIEW_LIST: "schools.view.list",
    VIEW_DETAIL: "schools.view.detail",
    CREATE: "schools.create",
    UPDATE: "schools.update",
    DELETE: "schools.delete",
  },

  USERS: {
    VIEW_LIST: "users.view.list",
    VIEW_DETAIL: "users.view.detail",
    CREATE: "users.create",
    UPDATE: "users.update",
    DELETE: "users.delete",
  },

  BEACONS: {
    VIEW_LIST: "beacons.view.list",
    VIEW_DETAIL: "beacons.view.detail",
    CREATE: "beacons.create",
    UPDATE: "beacons.update",
    DELETE: "beacons.delete",
  },

  FACULTIES: {
    VIEW_LIST: "faculties.view.list",
    VIEW_DETAIL: "faculties.view.detail",
    CREATE: "faculties.create",
    UPDATE: "faculties.update",
    DELETE: "faculties.delete",
  },
};

export const ROLE_PERMISSIONS = {
  [ROLES.ADMIN]: {
    [PERMISSIONS.DISTRICTS.VIEW_LIST]: true,
    [PERMISSIONS.DISTRICTS.VIEW_DETAIL]: true,
    [PERMISSIONS.DISTRICTS.CREATE]: true,
    [PERMISSIONS.DISTRICTS.UPDATE]: true,
    [PERMISSIONS.DISTRICTS.DELETE]: true,

    [PERMISSIONS.SCHOOLS.VIEW_LIST]: true,
    [PERMISSIONS.SCHOOLS.VIEW_DETAIL]: true,
    [PERMISSIONS.SCHOOLS.CREATE]: true,
    [PERMISSIONS.SCHOOLS.UPDATE]: true,
    [PERMISSIONS.SCHOOLS.DELETE]: true,

    [PERMISSIONS.USERS.VIEW_LIST]: true,
    [PERMISSIONS.USERS.VIEW_DETAIL]: true,
    [PERMISSIONS.USERS.CREATE]: true,
    [PERMISSIONS.USERS.UPDATE]: true,
    [PERMISSIONS.USERS.DELETE]: true,

    [PERMISSIONS.BEACONS.VIEW_LIST]: true,
    [PERMISSIONS.BEACONS.VIEW_DETAIL]: true,
    [PERMISSIONS.BEACONS.CREATE]: true,
    [PERMISSIONS.BEACONS.UPDATE]: true,
    [PERMISSIONS.BEACONS.DELETE]: true,

    [PERMISSIONS.FACULTIES.VIEW_LIST]: true,
    [PERMISSIONS.FACULTIES.VIEW_DETAIL]: true,
    [PERMISSIONS.FACULTIES.CREATE]: true,
    [PERMISSIONS.FACULTIES.UPDATE]: true,
    [PERMISSIONS.FACULTIES.DELETE]: true,
  },

  [ROLES.DISTRICT_ADMIN]: {
    [PERMISSIONS.DISTRICTS.VIEW_LIST]: true,
    [PERMISSIONS.DISTRICTS.VIEW_DETAIL]: true,
    [PERMISSIONS.DISTRICTS.CREATE]: false,
    [PERMISSIONS.DISTRICTS.UPDATE]: true,
    [PERMISSIONS.DISTRICTS.DELETE]: false,

    [PERMISSIONS.SCHOOLS.VIEW_LIST]: true,
    [PERMISSIONS.SCHOOLS.VIEW_DETAIL]: true,
    [PERMISSIONS.SCHOOLS.CREATE]: true,
    [PERMISSIONS.SCHOOLS.UPDATE]: true,
    [PERMISSIONS.SCHOOLS.DELETE]: false,

    [PERMISSIONS.USERS.VIEW_LIST]: true,
    [PERMISSIONS.USERS.VIEW_DETAIL]: true,
    [PERMISSIONS.USERS.CREATE]: true,
    [PERMISSIONS.USERS.UPDATE]: true,
    [PERMISSIONS.USERS.DELETE]: true,

    [PERMISSIONS.BEACONS.VIEW_LIST]: true,
    [PERMISSIONS.BEACONS.VIEW_DETAIL]: true,
    [PERMISSIONS.BEACONS.CREATE]: false,
    [PERMISSIONS.BEACONS.UPDATE]: false,
    [PERMISSIONS.BEACONS.DELETE]: false,

    [PERMISSIONS.FACULTIES.VIEW_LIST]: true,
    [PERMISSIONS.FACULTIES.VIEW_DETAIL]: true,
    [PERMISSIONS.FACULTIES.CREATE]: true,
    [PERMISSIONS.FACULTIES.UPDATE]: true,
    [PERMISSIONS.FACULTIES.DELETE]: true,
  },

  [ROLES.SCHOOL_ADMIN]: {
    [PERMISSIONS.DISTRICTS.VIEW_LIST]: true,
    [PERMISSIONS.DISTRICTS.VIEW_DETAIL]: true,
    [PERMISSIONS.DISTRICTS.CREATE]: false,
    [PERMISSIONS.DISTRICTS.UPDATE]: false,
    [PERMISSIONS.DISTRICTS.DELETE]: false,

    [PERMISSIONS.SCHOOLS.VIEW_LIST]: true,
    [PERMISSIONS.SCHOOLS.VIEW_DETAIL]: true,
    [PERMISSIONS.SCHOOLS.CREATE]: false,
    [PERMISSIONS.SCHOOLS.UPDATE]: true,
    [PERMISSIONS.SCHOOLS.DELETE]: false,

    [PERMISSIONS.USERS.VIEW_LIST]: true,
    [PERMISSIONS.USERS.VIEW_DETAIL]: true,
    [PERMISSIONS.USERS.CREATE]: true,
    [PERMISSIONS.USERS.UPDATE]: true,
    [PERMISSIONS.USERS.DELETE]: false,

    [PERMISSIONS.BEACONS.VIEW_LIST]: true,
    [PERMISSIONS.BEACONS.VIEW_DETAIL]: true,
    [PERMISSIONS.BEACONS.CREATE]: false,
    [PERMISSIONS.BEACONS.UPDATE]: false,
    [PERMISSIONS.BEACONS.DELETE]: false,

    [PERMISSIONS.FACULTIES.VIEW_LIST]: true,
    [PERMISSIONS.FACULTIES.VIEW_DETAIL]: true,
    [PERMISSIONS.FACULTIES.CREATE]: true,
    [PERMISSIONS.FACULTIES.UPDATE]: true,
    [PERMISSIONS.FACULTIES.DELETE]: false,
  },
};

export const checkPermission = (user, permission) => {
  if (!user) return false;

  if (user.role === ROLES.ADMIN) return true;

  const rolePermissions = ROLE_PERMISSIONS[user.role];
  if (!rolePermissions) return false;

  return rolePermissions[permission] || false;
};
