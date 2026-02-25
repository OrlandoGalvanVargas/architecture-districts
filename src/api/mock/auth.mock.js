const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

const MOCK_USERS = [
  {
    id: 1,
    email: "admin@livefree.com",
    password: "123456",
    name: "Admin User",
    role: "admin",
  },
  {
    id: 2,
    email: "teacher@livefree.com",
    password: "123456",
    name: "Teacher User",
    role: "teacher",
  },
];

const generateMockToken = () => {
  return `mock_token_${Date.now()}_${Math.random().toString(36).substr(2, 9)}`;
};

export const mockAuthApi = {
  login: async ({ email, password }) => {
    await delay(1000);

    const user = MOCK_USERS.find(
      (u) => u.email === email && u.password === password,
    );

    if (!user) {
      throw {
        message: "Invalid email or password",
        status: 401,
      };
    }

    const { password: _, ...userWithoutPassword } = user;

    return {
      accessToken: generateMockToken(),
      refreshToken: generateMockToken(),
      user: userWithoutPassword,
    };
  },

  logout: async () => {
    await delay(500);
    return { success: true };
  },

  refreshToken: async ({ refreshToken }) => {
    await delay(500);

    return {
      accessToken: generateMockToken(),
    };
  },

  getCurrentUser: async () => {
    await delay(500);

    const { password: _, ...userWithoutPassword } = MOCK_USERS[0];
    return userWithoutPassword;
  },
};
