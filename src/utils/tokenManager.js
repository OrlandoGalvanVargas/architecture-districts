const KEYS = {
  ACCESS_TOKEN: "auth-token",
  REFRESH_TOKEN: "refresh-token",
  USER: "user",
};

export const tokenManager = {
  setToken: (accessToken, refreshToken) => {
    localStorage.setItem(KEYS.ACCESS_TOKEN, accessToken);
    localStorage.setItem(KEYS.REFRESH_TOKEN, refreshToken);
  },

  getAccessToken: () => {
    return localStorage.getItem(KEYS.ACCESS_TOKEN);
  },

  getRefreshToken: () => {
    return localStorage.getItem(KEYS.REFRESH_TOKEN);
  },

  setAccessToken: (accessToken) => {
    localStorage.setItem(KEYS.ACCESS_TOKEN, accessToken);
  },

  setUser: (user) => {
    localStorage.setItem(KEYS.USER, JSON.stringify(user));
  },

  getUserL: () => {
    const user = localStorage.getItem(KEYS.USER);
    return user ? JSON.parse(user) : null;
  },

  clearTokens: () => {
    localStorage.removeItem(KEYS.ACCESS_TOKEN);
    localStorage.removeItem(KEYS.REFRESH_TOKEN);
    localStorage.removeItem(KEYS.USER);
  },

  hasValidSession: () => {
    return !!tokenManager.getAccessToken();
  },
};
