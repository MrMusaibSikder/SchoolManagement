import axios from "axios";
import { API_BASE_URL, API_TIMEOUT_MS } from "@/config/env";

/**
 * Axios instance for the *public / anonymous* endpoints used only by the
 * pre-login Landing Page (`/api/public/*`).
 *
 * IMPORTANT (see .ai/standards & DECISIONS.md #8):
 *  - This client must NEVER attach an `Authorization: Bearer` header. A
 *    visitor viewing the Landing Page is not logged in, and the backend's
 *    anonymous `PublicController` has no auth at all.
 *  - Do not reuse this client for authenticated requests — those will use a
 *    separate (future) authenticated client that sets the JWT automatically.
 */
export const publicApiClient = axios.create({
  baseURL: API_BASE_URL,
  timeout: API_TIMEOUT_MS,
  headers: {
    "Content-Type": "application/json",
  },
});
