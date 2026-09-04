const KEYS = {
  ACCESS_TOKEN: "access_token",
  USER: "user",
};

export const tokenManager = {
  setAccessToken: (accessToken) => {
    localStorage.setItem(KEYS.ACCESS_TOKEN, accessToken);
  },

  getAccessToken: () => {
    return localStorage.getItem(KEYS.ACCESS_TOKEN);
  },

  setUser: (user) => {
    localStorage.setItem(KEYS.USER, JSON.stringify(user));
  },

  getUser: () => {
    const user = localStorage.getItem(KEYS.USER);
    return user ? JSON.parse(user) : null;
  },

  setSession: (accessToken, user) => {
    localStorage.setItem(KEYS.ACCESS_TOKEN, accessToken);
    if (user) {
      localStorage.setItem(KEYS.USER, JSON.stringify(user));
    }
  },

  clearSession: () => {
    localStorage.removeItem(KEYS.ACCESS_TOKEN);
    localStorage.removeItem(KEYS.USER);
  },

  hasValidSession: () => {
    return !!tokenManager.getAccessToken();
  },
};
