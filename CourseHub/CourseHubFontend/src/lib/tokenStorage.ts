// Centralized in one file so it's easy to swap for a more XSS-resistant
// storage strategy later (e.g. an httpOnly cookie set by a backend-for-
// frontend) without touching every call site. localStorage is fine for
// local development / an internal tool, but is readable by any script
// running on the page — don't reuse this pattern as-is for a public,
// security-sensitive production app without reconsidering that trade-off.

const ACCESS_TOKEN_KEY = "coursehub.accessToken";
const REFRESH_TOKEN_KEY = "coursehub.refreshToken";

export const tokenStorage = {
  getAccessToken: () => localStorage.getItem(ACCESS_TOKEN_KEY),
  getRefreshToken: () => localStorage.getItem(REFRESH_TOKEN_KEY),
  setTokens: (accessToken: string, refreshToken: string) => {
    localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
  },
  clear: () => {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
  },
};
