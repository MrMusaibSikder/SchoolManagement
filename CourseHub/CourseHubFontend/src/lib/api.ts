import { tokenStorage } from "./tokenStorage";
import type { ProblemDetails } from "./types";

const BASE_URL = import.meta.env.VITE_API_BASE_URL ?? "https://localhost:7164";
export class ApiError extends Error {
  status: number;
  problem: ProblemDetails | null;

  constructor(status: number, problem: ProblemDetails | null, fallbackMessage: string) {
    super(problem?.detail ?? problem?.title ?? fallbackMessage);
    this.status = status;
    this.problem = problem;
  }
}

let refreshPromise: Promise<boolean> | null = null;

/**
 * Every CourseHubBackend permission requires re-login/refresh to pick up
 * a role change (see the backend README's "Authorization & permissions"
 * section) — this refresh call is also how the frontend's JWT ever picks
 * up newly-assigned permissions, not just how it survives expiry.
 */
async function refreshAccessToken(): Promise<boolean> {
  const refreshToken = tokenStorage.getRefreshToken();
  if (!refreshToken) return false;

  // De-duplicate concurrent refresh attempts (e.g. several requests
  // firing 401 at once) into a single in-flight refresh call.
  if (!refreshPromise) {
    refreshPromise = fetch(`${BASE_URL}/api/auth/refresh`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ refreshToken }),
    })
      .then(async (res) => {
        if (!res.ok) return false;
        const data = await res.json();
        tokenStorage.setTokens(data.accessToken, data.refreshToken);
        return true;
      })
      .catch(() => false)
      .finally(() => {
        refreshPromise = null;
      });
  }

  return refreshPromise;
}

interface RequestOptions {
  method?: "GET" | "POST" | "PUT" | "DELETE";
  body?: unknown;
  query?: Record<string, string | number | boolean | undefined | null>;
  /** Internal — prevents infinite refresh-retry loops. */
  _retried?: boolean;
}

function buildUrl(path: string, query?: RequestOptions["query"]): string {
  const url = new URL(path.replace(/^\//, ""), BASE_URL + "/");
  if (query) {
    for (const [key, value] of Object.entries(query)) {
      if (value !== undefined && value !== null && value !== "") {
        url.searchParams.set(key, String(value));
      }
    }
  }
  return url.toString();
}

async function request<T>(path: string, options: RequestOptions = {}): Promise<T> {
  const { method = "GET", body, query, _retried } = options;

  const headers: Record<string, string> = { "Content-Type": "application/json" };
  const accessToken = tokenStorage.getAccessToken();
  if (accessToken) headers.Authorization = `Bearer ${accessToken}`;

  const res = await fetch(buildUrl(path, query), {
    method,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  });

  if (res.status === 401 && accessToken && !_retried) {
    const refreshed = await refreshAccessToken();
    if (refreshed) {
      return request<T>(path, { ...options, _retried: true });
    }
    tokenStorage.clear();
    window.location.href = "/login";
    throw new ApiError(401, null, "Session expired.");
  }

  if (res.status === 204) {
    return undefined as T;
  }

  const contentType = res.headers.get("content-type") ?? "";
  const data = contentType.includes("application/json") ? await res.json() : null;

  if (!res.ok) {
    throw new ApiError(res.status, data as ProblemDetails, `Request failed with status ${res.status}`);
  }

  return data as T;
}

/**
 * Multipart submit — used only by POST /me/assignments/{id}/submit, which
 * is [Consumes("multipart/form-data")] on the backend. Deliberately does
 * NOT set Content-Type itself: the browser needs to add the multipart
 * boundary, which it only does when it builds the header from the
 * FormData body.
 */
async function requestForm<T>(path: string, formData: FormData, _retried = false): Promise<T> {
  const headers: Record<string, string> = {};
  const accessToken = tokenStorage.getAccessToken();
  if (accessToken) headers.Authorization = `Bearer ${accessToken}`;

  const res = await fetch(buildUrl(path), { method: "POST", headers, body: formData });

  if (res.status === 401 && accessToken && !_retried) {
    const refreshed = await refreshAccessToken();
    if (refreshed) return requestForm<T>(path, formData, true);
    tokenStorage.clear();
    window.location.href = "/login";
    throw new ApiError(401, null, "Session expired.");
  }

  const contentType = res.headers.get("content-type") ?? "";
  const data = contentType.includes("application/json") ? await res.json() : null;

  if (!res.ok) {
    throw new ApiError(res.status, data as ProblemDetails, `Request failed with status ${res.status}`);
  }

  return data as T;
}

/**
 * Downloads a binary response (used by GET /admin/submissions/{id}/file)
 * and returns it as a Blob plus the filename the server suggested, so the
 * caller can trigger a normal browser "Save As" without ever navigating
 * away (a plain <a href> can't carry the Authorization header).
 */
async function downloadFile(path: string, _retried = false): Promise<{ blob: Blob; filename: string }> {
  const headers: Record<string, string> = {};
  const accessToken = tokenStorage.getAccessToken();
  if (accessToken) headers.Authorization = `Bearer ${accessToken}`;

  const res = await fetch(buildUrl(path), { headers });

  if (res.status === 401 && accessToken && !_retried) {
    const refreshed = await refreshAccessToken();
    if (refreshed) return downloadFile(path, true);
    tokenStorage.clear();
    window.location.href = "/login";
    throw new ApiError(401, null, "Session expired.");
  }

  if (!res.ok) {
    let problem: ProblemDetails | null = null;
    try {
      problem = await res.json();
    } catch {
      // Response wasn't JSON — fall through with problem left null.
    }
    throw new ApiError(res.status, problem, `Request failed with status ${res.status}`);
  }

  const disposition = res.headers.get("content-disposition") ?? "";
  const match = /filename\*?=(?:UTF-8'')?"?([^";]+)"?/i.exec(disposition);
  const filename = match ? decodeURIComponent(match[1]) : "download";

  return { blob: await res.blob(), filename };
}

export const api = {
  get: <T>(path: string, query?: RequestOptions["query"]) => request<T>(path, { method: "GET", query }),
  post: <T>(path: string, body?: unknown) => request<T>(path, { method: "POST", body }),
  put: <T>(path: string, body?: unknown) => request<T>(path, { method: "PUT", body }),
  del: <T>(path: string) => request<T>(path, { method: "DELETE" }),
  postForm: <T>(path: string, formData: FormData) => requestForm<T>(path, formData),
  downloadFile: (path: string) => downloadFile(path),
};
