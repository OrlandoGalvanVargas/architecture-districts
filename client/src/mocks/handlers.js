import { http, HttpResponse } from "msw";
import { loadMockDB, saveMockDB } from "./store";

const delay = (ms) => new Promise((res) => setTimeout(res, ms));

const nextId = (arr) => (arr.length ? Math.max(...arr.map((item) => item.id)) + 1 : 1);

const paginate = (items, page = 1, pageSize = 10) => {
  const startIndex = (page - 1) * pageSize;
  const endIndex = startIndex + pageSize;
  const paginatedItems = items.slice(startIndex, endIndex);
  return {
    items: paginatedItems,
    page,
    pageSize,
    totalCount: items.length,
    totalPages: Math.ceil(items.length / pageSize),
    hasPrevious: page > 1,
    hasNext: endIndex < items.length,
  };
};

export const handlers = [
  http.post("/api/auth/login", async ({ request }) => {
    await delay(300);
    const { email, password } = await request.json();
    const db = await loadMockDB();
    const user = db.users.find((u) => u.email === email && u.password === password);
    if (!user) {
      return HttpResponse.json({ detail: "Invalid credentials" }, { status: 401 });
    }
    const accessToken = "mock-access-token";
    return HttpResponse.json(
      {
        accessToken,
        user: {
          id: user.id,
          name: user.name,
          email: user.email,
          role: user.role,
          entityId: user.entityId,
          entityType: user.entityType,
          isActive: user.isActive,
        },
      },
      { status: 200 }
    );
  }),

  http.post("/api/auth/refresh", async () => {
    const db = await loadMockDB();
    const admin = db.users.find((u) => u.role === "Admin");
    return HttpResponse.json({ accessToken: "mock-access-token", user: admin }, { status: 200 });
  }),

  http.get("/api/auth/me", async () => {
    await delay(200);
    const db = await loadMockDB();
    const admin = db.users.find((u) => u.role === "Admin");
    return HttpResponse.json(admin, { status: 200 });
  }),

  http.post("/api/auth/logout", async () => {
    return HttpResponse.json({}, { status: 200 });
  }),

  http.get("/api/districts", async ({ request }) => {
    await delay(300);
    const db = await loadMockDB();
    const params = Object.fromEntries(new URL(request.url).searchParams);
    let items = db.districts;

    if (params.search) {
      const search = params.search.toLowerCase();
      items = items.filter(
        (d) => d.name.toLowerCase().includes(search) || d.code.toLowerCase().includes(search)
      );
    }

    items = items.map((district) => ({
      ...district,
      schoolCount: db.schools.filter((s) => s.districtId === district.id).length,
      beaconCount: db.beacons.filter((b) => b.districtId === district.id).length,
      facultyCount: db.faculties.filter((f) => f.districtId === district.id).length,
    }));

    return HttpResponse.json(items, { status: 200 });
  }),

  http.get("/api/districts/:id", async ({ params }) => {
    await delay(200);
    const db = await loadMockDB();
    const id = Number(params.id);
    const district = db.districts.find((d) => d.id === id);
    if (!district) return HttpResponse.json({ detail: "District not found" }, { status: 404 });
    const schoolCount = db.schools.filter((s) => s.districtId === id).length;
    const beaconCount = db.beacons.filter((b) => b.districtId === id).length;
    const facultyCount = db.faculties.filter((f) => f.districtId === id).length;
    return HttpResponse.json(
      { ...district, schoolCount, beaconCount, facultyCount },
      { status: 200 }
    );
  }),

  http.post("/api/districts", async ({ request }) => {
    await delay(300);
    const db = await loadMockDB();
    const newDistrict = await request.json();
    const id = nextId(db.districts);
    const district = { id, ...newDistrict, createdAt: new Date().toISOString(), updatedAt: null };
    db.districts.push(district);
    await saveMockDB(db);
    return HttpResponse.json(district, { status: 201 });
  }),

  http.put("/api/districts/:id", async ({ request, params }) => {
    await delay(300);
    const db = await loadMockDB();
    const id = Number(params.id);
    const updates = await request.json();
    const index = db.districts.findIndex((d) => d.id === id);
    if (index === -1) return HttpResponse.json({ detail: "District not found" }, { status: 404 });
    db.districts[index] = {
      ...db.districts[index],
      ...updates,
      updatedAt: new Date().toISOString(),
    };
    await saveMockDB(db);
    return HttpResponse.json(db.districts[index], { status: 200 });
  }),

  http.delete("/api/districts/:id", async ({ params }) => {
    await delay(300);
    const db = await loadMockDB();
    const id = Number(params.id);
    db.districts = db.districts.filter((d) => d.id !== id);
    await saveMockDB(db);
    return new HttpResponse(null, { status: 204 });
  }),

  http.get("/api/schools", async ({ request }) => {
    await delay(300);
    const db = await loadMockDB();
    const params = Object.fromEntries(new URL(request.url).searchParams);
    const page = parseInt(params.page) || 1;
    const pageSize = parseInt(params.pageSize) || 10;
    let items = db.schools;
    if (params.search) {
      const search = params.search.toLowerCase();
      items = items.filter(
        (s) => s.name.toLowerCase().includes(search) || s.schoolCode.toLowerCase().includes(search)
      );
    }
    if (params.districtId) items = items.filter((s) => s.districtId === Number(params.districtId));
    if (params.level) items = items.filter((s) => s.level === Number(params.level));
    if (params.type) items = items.filter((s) => s.type === Number(params.type));
    if (params.isActive) items = items.filter((s) => s.isActive === (params.isActive === "true"));
    items = items.map((school) => ({
      ...school,
      beaconCount: db.beacons.filter((b) => b.schoolId === school.id).length,
      facultyCount: db.faculties.filter((f) => f.schoolId === school.id).length,
    }));
    const result = paginate(items, page, pageSize);
    return HttpResponse.json(result, { status: 200 });
  }),

  http.get("/api/schools/:id", async ({ params }) => {
    await delay(200);
    const db = await loadMockDB();
    const id = Number(params.id);
    const school = db.schools.find((s) => s.id === id);
    if (!school) return HttpResponse.json({ detail: "School not found" }, { status: 404 });
    const beaconCount = db.beacons.filter((b) => b.schoolId === id).length;
    const facultyCount = db.faculties.filter((f) => f.schoolId === id).length;
    return HttpResponse.json({ ...school, beaconCount, facultyCount }, { status: 200 });
  }),

  http.post("/api/schools", async ({ request }) => {
    await delay(300);
    const db = await loadMockDB();
    const newSchool = await request.json();
    const id = nextId(db.schools);
    const school = {
      id,
      ...newSchool,
      isActive: true,
      createdAt: new Date().toISOString(),
      updatedAt: null,
      districtName: db.districts.find((d) => d.id === newSchool.districtId)?.name || null,
    };
    db.schools.push(school);
    await saveMockDB(db);
    return HttpResponse.json(school, { status: 201 });
  }),

  http.put("/api/schools/:id", async ({ request, params }) => {
    await delay(300);
    const db = await loadMockDB();
    const id = Number(params.id);
    const updates = await request.json();
    const index = db.schools.findIndex((s) => s.id === id);
    if (index === -1) return HttpResponse.json({ detail: "School not found" }, { status: 404 });
    db.schools[index] = { ...db.schools[index], ...updates, updatedAt: new Date().toISOString() };
    await saveMockDB(db);
    return HttpResponse.json(db.schools[index], { status: 200 });
  }),

  http.delete("/api/schools/:id", async ({ params }) => {
    await delay(300);
    const db = await loadMockDB();
    const id = Number(params.id);
    db.schools = db.schools.filter((s) => s.id !== id);
    await saveMockDB(db);
    return new HttpResponse(null, { status: 204 });
  }),

  http.get("/api/users", async ({ request }) => {
    await delay(300);
    const db = await loadMockDB();
    const params = Object.fromEntries(new URL(request.url).searchParams);
    const page = parseInt(params.page) || 1;
    const pageSize = parseInt(params.pageSize) || 10;
    let items = db.users.map(({ password: _password, ...user }) => user);
    if (params.search) {
      const search = params.search.toLowerCase();
      items = items.filter(
        (u) => u.name.toLowerCase().includes(search) || u.email.toLowerCase().includes(search)
      );
    }
    if (params.role) items = items.filter((u) => u.role === params.role);
    if (params.entityType) items = items.filter((u) => u.entityType === Number(params.entityType));
    if (params.entityId) items = items.filter((u) => u.entityId === Number(params.entityId));
    if (params.isActive) items = items.filter((u) => u.isActive === (params.isActive === "true"));
    const result = paginate(items, page, pageSize);
    return HttpResponse.json(result, { status: 200 });
  }),

  http.get("/api/users/:id", async ({ params }) => {
    await delay(200);
    const db = await loadMockDB();
    const id = Number(params.id);
    const user = db.users.find((u) => u.id === id);
    if (!user) return HttpResponse.json({ detail: "User not found" }, { status: 404 });
    const { password: _password, ...userWithoutPassword } = user;
    return HttpResponse.json(userWithoutPassword, { status: 200 });
  }),

  http.post("/api/users", async ({ request }) => {
    await delay(300);
    const db = await loadMockDB();
    const newUser = await request.json();
    const id = nextId(db.users);
    const user = {
      id,
      ...newUser,
      isActive: true,
      createdAt: new Date().toISOString(),
      updatedAt: null,
    };
    db.users.push(user);
    await saveMockDB(db);
    const { password: _password, ...userWithoutPassword } = user;
    return HttpResponse.json(userWithoutPassword, { status: 201 });
  }),

  http.put("/api/users/:id", async ({ request, params }) => {
    await delay(300);
    const db = await loadMockDB();
    const id = Number(params.id);
    const updates = await request.json();
    const index = db.users.findIndex((u) => u.id === id);
    if (index === -1) return HttpResponse.json({ detail: "User not found" }, { status: 404 });
    if (updates.newPassword) {
      updates.password = updates.newPassword;
      delete updates.newPassword;
    }
    db.users[index] = { ...db.users[index], ...updates, updatedAt: new Date().toISOString() };
    await saveMockDB(db);
    const { password: _password, ...userWithoutPassword } = db.users[index];
    return HttpResponse.json(userWithoutPassword, { status: 200 });
  }),

  http.delete("/api/users/:id", async ({ params }) => {
    await delay(300);
    const db = await loadMockDB();
    const id = Number(params.id);
    db.users = db.users.filter((u) => u.id !== id);
    await saveMockDB(db);
    return new HttpResponse(null, { status: 204 });
  }),

  http.get("/api/beacons", async ({ request }) => {
    await delay(300);
    const db = await loadMockDB();
    const params = Object.fromEntries(new URL(request.url).searchParams);
    const page = parseInt(params.page) || 1;
    const pageSize = parseInt(params.pageSize) || 10;
    let items = db.beacons;
    if (params.search) {
      const search = params.search.toLowerCase();
      items = items.filter(
        (b) =>
          b.deviceName.toLowerCase().includes(search) ||
          b.serialNumber.toLowerCase().includes(search)
      );
    }
    if (params.type) items = items.filter((b) => b.type === Number(params.type));
    if (params.status) items = items.filter((b) => b.status === Number(params.status));
    if (params.districtId) items = items.filter((b) => b.districtId === Number(params.districtId));
    if (params.schoolId) items = items.filter((b) => b.schoolId === Number(params.schoolId));
    if (params.isAssigned)
      items = items.filter((b) => b.isAssigned === (params.isAssigned === "true"));
    items = items.map((beacon) => ({
      ...beacon,
      districtName: db.districts.find((d) => d.id === beacon.districtId)?.name || null,
      schoolName: db.schools.find((s) => s.id === beacon.schoolId)?.name || null,
      facultyName: db.faculties.find((f) => f.id === beacon.facultyId)?.fullName || null,
    }));
    const result = paginate(items, page, pageSize);
    return HttpResponse.json(result, { status: 200 });
  }),

  http.get("/api/beacons/:id", async ({ params }) => {
    await delay(200);
    const db = await loadMockDB();
    const id = Number(params.id);
    const beacon = db.beacons.find((b) => b.id === id);
    if (!beacon) return HttpResponse.json({ detail: "Beacon not found" }, { status: 404 });
    return HttpResponse.json(
      {
        ...beacon,
        districtName: db.districts.find((d) => d.id === beacon.districtId)?.name || null,
        schoolName: db.schools.find((s) => s.id === beacon.schoolId)?.name || null,
        facultyName: db.faculties.find((f) => f.id === beacon.facultyId)?.fullName || null,
      },
      { status: 200 }
    );
  }),

  http.post("/api/beacons", async ({ request }) => {
    await delay(300);
    const db = await loadMockDB();
    const newBeacon = await request.json();
    const id = nextId(db.beacons);
    const beacon = {
      id,
      ...newBeacon,
      status: 1,
      isAssigned: !!(newBeacon.districtId || newBeacon.schoolId),
      createdAt: new Date().toISOString(),
      updatedAt: null,
    };
    db.beacons.push(beacon);
    await saveMockDB(db);
    return HttpResponse.json(beacon, { status: 201 });
  }),

  http.put("/api/beacons/:id", async ({ request, params }) => {
    await delay(300);
    const db = await loadMockDB();
    const id = Number(params.id);
    const updates = await request.json();
    const index = db.beacons.findIndex((b) => b.id === id);
    if (index === -1) return HttpResponse.json({ detail: "Beacon not found" }, { status: 404 });
    db.beacons[index] = { ...db.beacons[index], ...updates, updatedAt: new Date().toISOString() };
    db.beacons[index].isAssigned = !!(updates.districtId || updates.schoolId);
    await saveMockDB(db);
    return HttpResponse.json(db.beacons[index], { status: 200 });
  }),

  http.delete("/api/beacons/:id", async ({ params }) => {
    await delay(300);
    const db = await loadMockDB();
    const id = Number(params.id);
    db.beacons = db.beacons.filter((b) => b.id !== id);
    await saveMockDB(db);
    return new HttpResponse(null, { status: 204 });
  }),

  http.get("/api/faculties", async ({ request }) => {
    await delay(300);
    const db = await loadMockDB();
    const params = Object.fromEntries(new URL(request.url).searchParams);
    const page = parseInt(params.page) || 1;
    const pageSize = parseInt(params.pageSize) || 10;
    let items = db.faculties;
    if (params.search) {
      const search = params.search.toLowerCase();
      items = items.filter(
        (f) =>
          f.firstName.toLowerCase().includes(search) ||
          f.lastName.toLowerCase().includes(search) ||
          f.fullName.toLowerCase().includes(search)
      );
    }
    if (params.districtId) items = items.filter((f) => f.districtId === Number(params.districtId));
    if (params.schoolId) items = items.filter((f) => f.schoolId === Number(params.schoolId));
    if (params.isActive) items = items.filter((f) => f.isActive === (params.isActive === "true"));
    if (params.hasBeacon)
      items = items.filter((f) =>
        params.hasBeacon === "true" ? f.beaconId !== null : f.beaconId === null
      );
    items = items.map((faculty) => ({
      ...faculty,
      districtName: db.districts.find((d) => d.id === faculty.districtId)?.name || null,
      schoolName: db.schools.find((s) => s.id === faculty.schoolId)?.name || null,
      beaconDeviceName: db.beacons.find((b) => b.id === faculty.beaconId)?.deviceName || null,
      beaconSerialNumber: db.beacons.find((b) => b.id === faculty.beaconId)?.serialNumber || null,
      beaconType: db.beacons.find((b) => b.id === faculty.beaconId)?.type || null,
    }));
    const result = paginate(items, page, pageSize);
    return HttpResponse.json(result, { status: 200 });
  }),

  http.get("/api/faculties/:id", async ({ params }) => {
    await delay(200);
    const db = await loadMockDB();
    const id = Number(params.id);
    const faculty = db.faculties.find((f) => f.id === id);
    if (!faculty) return HttpResponse.json({ detail: "Faculty not found" }, { status: 404 });
    return HttpResponse.json(
      {
        ...faculty,
        districtName: db.districts.find((d) => d.id === faculty.districtId)?.name || null,
        schoolName: db.schools.find((s) => s.id === faculty.schoolId)?.name || null,
        beaconDeviceName: db.beacons.find((b) => b.id === faculty.beaconId)?.deviceName || null,
        beaconSerialNumber: db.beacons.find((b) => b.id === faculty.beaconId)?.serialNumber || null,
        beaconType: db.beacons.find((b) => b.id === faculty.beaconId)?.type || null,
      },
      { status: 200 }
    );
  }),

  http.post("/api/faculties", async ({ request }) => {
    await delay(300);
    const db = await loadMockDB();
    const newFaculty = await request.json();
    const id = nextId(db.faculties);
    const fullName = `${newFaculty.firstName} ${newFaculty.lastName}`;
    const faculty = {
      id,
      ...newFaculty,
      fullName,
      isActive: true,
      createdAt: new Date().toISOString(),
      updatedAt: null,
    };
    db.faculties.push(faculty);
    await saveMockDB(db);
    return HttpResponse.json(faculty, { status: 201 });
  }),

  http.put("/api/faculties/:id", async ({ request, params }) => {
    await delay(300);
    const db = await loadMockDB();
    const id = Number(params.id);
    const updates = await request.json();
    const index = db.faculties.findIndex((f) => f.id === id);
    if (index === -1) return HttpResponse.json({ detail: "Faculty not found" }, { status: 404 });
    const fullName = `${updates.firstName || db.faculties[index].firstName} ${
      updates.lastName || db.faculties[index].lastName
    }`;
    db.faculties[index] = {
      ...db.faculties[index],
      ...updates,
      fullName,
      updatedAt: new Date().toISOString(),
    };
    await saveMockDB(db);
    return HttpResponse.json(db.faculties[index], { status: 200 });
  }),

  http.delete("/api/faculties/:id", async ({ params }) => {
    await delay(300);
    const db = await loadMockDB();
    const id = Number(params.id);
    db.faculties = db.faculties.filter((f) => f.id !== id);
    await saveMockDB(db);
    return new HttpResponse(null, { status: 204 });
  }),
];
