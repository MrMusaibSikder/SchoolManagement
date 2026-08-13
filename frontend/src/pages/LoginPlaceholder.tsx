import { Link } from "react-router-dom";

/**
 * Placeholder for the `/login` route.
 *
 * Authentication is intentionally NOT implemented yet (see CURRENT_PROGRESS.md —
 * the Landing Page is the current in-progress task, Authentication is next).
 * This minimal screen exists only so the Landing Page's "Login" call-to-action
 * has a real destination to route to.
 */
export function LoginPlaceholder() {
  return (
    <main className="flex min-h-screen items-center justify-center bg-paper px-6 text-ink">
      <div className="w-full max-w-md rounded-lg border border-ink/10 bg-paper p-8 text-center shadow-[0_18px_40px_-20px_rgba(15,42,61,0.35)]">
        <h1 className="font-display text-2xl font-semibold text-ink">
          Login
        </h1>
        <p className="mt-3 text-sm leading-relaxed text-ink-muted">
          Authentication is not built yet. This page is a placeholder while the
          Landing Page is completed.
        </p>
        <Link
          to="/"
          className="mt-6 inline-flex items-center justify-center rounded-md bg-gold px-5 py-2.5 text-sm font-semibold text-ink transition hover:bg-gold/90"
        >
          Back to home
        </Link>
      </div>
    </main>
  );
}
