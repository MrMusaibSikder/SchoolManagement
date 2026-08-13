/**
 * Centralized frontend configuration.
 *
 * Values are read from Vite environment variables (`.env`, `.env.development`,
 * etc.) with safe development fallbacks so the app can run without a
 * hand-authored env file. Never hardcode an environment-specific value in a
 * feature file — import from here instead.
 *
* Variable                | Default (dev)         | Purpose
 * ------------------------|-----------------------|--------------------------------
 * VITE_API_BASE_URL       | https://localhost:7083 | Absolute base URL for the API.
 *                                                     Used directly by the public
 *                                                     client; the authenticated
 *                                                     client will reuse it later.
 *
 * NOTE: The backend ASP.NET Core API is configured with UseHttpsRedirection(),
 * so plain HTTP on port 5053 returns a 307 redirect to HTTPS on port 7083.
 * Pointing the client at the HTTPS URL directly avoids that redirect (and the
 * cross-origin cert handshake it would trigger in the browser). The ASP.NET
 * development certificate is trusted on this machine (see `dotnet dev-certs`).
 */

function readEnv(key: string, fallback: string): string {
  const value = (import.meta.env?.[key] as string | undefined)?.trim();
  return value && value.length > 0 ? value : fallback;
}

/**
 * Base URL of the ASP.NET Core Web API backend.
 *
 * Defaults to a *relative* `/api` path so the browser always talks to the
 * same origin as the app. In development the Vite proxy forwards `/api` to
 * the real backend (see `vite.config.ts`), which avoids CORS, mixed-content,
 * and self-signed cert failures entirely. An absolute URL (e.g. for a
 * production API host) can still be supplied via `VITE_API_BASE_URL`.
 */
export const API_BASE_URL = readEnv("VITE_API_BASE_URL", "/api");

/** Request timeout used by API clients (milliseconds). */
export const API_TIMEOUT_MS = 15_000;
