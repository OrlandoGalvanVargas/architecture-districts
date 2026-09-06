export const queryKeys = {
  auth: {
    me: () => ["auth", "me"],
  },

  districts: {
    all: () => ["districts"],
    list: (params) => ["districts", "list", params],
    detail: (id) => ["districts", "detail", id],
  },

  schools: {
    all: () => ["schools"],
    list: (params) => ["schools", "list", params],
    detail: (id) => ["schools", "detail", id],
  },

  users: {
    all: () => ["users"],
    list: (params) => ["users", "list", params],
    detail: (id) => ["users", "detail", id],
  },

  beacons: {
    all: () => ["beacons"],
    list: (params) => ["beacons", "list", params],
    detail: (id) => ["beacons", "detail", id],
  },

  faculties: {
    all: () => ["faculties"],
    list: (params) => ["faculties", "list", params],
    detail: (id) => ["faculties", "detail", id],
  },
};
