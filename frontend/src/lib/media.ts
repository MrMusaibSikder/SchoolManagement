/**
 * Resolves backend-relative upload paths (e.g. `/uploads/employees/x.png`)
 * so they load through the Vite `/uploads` proxy in development.
 */
export function resolveMediaUrl(path?: string | null): string | null {
  if (!path || path.trim().length === 0) return null;
  const value = path.trim();
  if (
    value.startsWith("blob:") ||
    value.startsWith("http://") ||
    value.startsWith("https://") ||
    value.startsWith("data:")
  ) {
    return value;
  }
  if (value.startsWith("/uploads/")) return value;
  if (value.startsWith("uploads/")) return `/${value}`;
  return value;
}
