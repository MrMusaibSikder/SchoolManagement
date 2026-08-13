import { useState } from "react";
import { Link } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "sonner";
import { KeyRound, Loader2, Mail, Send } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useForgotPassword } from "../hooks/useForgotPassword";
import {
  forgotPasswordSchema,
  type ForgotPasswordFormValues,
} from "../schemas/forgot-password.schema";
import { getAuthErrorMessage } from "../api/auth.api";

export function ForgotPasswordPage() {
  const forgotMutation = useForgotPassword();
  const [sent, setSent] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ForgotPasswordFormValues>({
    resolver: zodResolver(forgotPasswordSchema),
    defaultValues: { email: "" },
  });

  const onSubmit = handleSubmit(async (values) => {
    try {
      // Backend always returns 204 — it does not reveal whether the email
      // exists, so we show the same success message either way.
      await forgotMutation.mutateAsync({ email: values.email });
      setSent(true);
    } catch (error) {
      toast.error(getAuthErrorMessage(error));
    }
  });

  return (
    <main className="flex min-h-screen items-center justify-center bg-paper px-6 py-10 text-ink">
      <div className="w-full max-w-md rounded-lg border border-ink/10 bg-card p-8 shadow-[0_18px_40px_-20px_rgba(15,42,61,0.35)]">
        <div className="mb-8 text-center">
          <div className="mx-auto mb-4 inline-flex h-12 w-12 items-center justify-center rounded-full bg-gold/15 text-gold">
            <KeyRound aria-hidden="true" className="h-6 w-6" />
          </div>
          <h1 className="font-display text-2xl font-semibold text-ink">
            Forgot your password?
          </h1>
          <p className="mt-2 text-sm text-ink-muted">
            Enter your email and we'll send you a reset link.
          </p>
        </div>

        {sent ? (
          <div className="rounded-md border border-forest/30 bg-forest/5 p-5 text-center">
            <p className="text-sm leading-relaxed text-ink">
              If an account exists for that email, a password reset link is on
              its way. Check your inbox (and spam folder) and follow the link
              to choose a new password.
            </p>
            <Link
              to="/login"
              className="mt-4 inline-block text-sm font-medium text-gold hover:underline"
            >
              Back to sign in
            </Link>
          </div>
        ) : (
          <form onSubmit={onSubmit} noValidate className="space-y-5">
            <div>
              <label
                htmlFor="email"
                className="mb-1.5 block text-sm font-medium text-ink"
              >
                Email <span className="text-rust">*</span>
              </label>
              <div className="relative">
                <Mail
                  aria-hidden="true"
                  className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-ink-muted"
                />
                <input
                  id="email"
                  type="email"
                  autoComplete="email"
                  autoFocus
                  placeholder="you@example.com"
                  className={`w-full rounded-md border bg-background py-2.5 pl-10 pr-3 text-sm text-ink shadow-sm outline-none transition focus:ring-2 ${
                    errors.email
                      ? "border-rust focus:ring-rust/30"
                      : "border-input focus:ring-gold/40"
                  }`}
                  {...register("email")}
                />
              </div>
              {errors.email && (
                <p className="mt-1.5 text-xs text-rust">
                  {errors.email.message}
                </p>
              )}
            </div>

            <Button
              type="submit"
              disabled={forgotMutation.isPending}
              className="w-full bg-gold text-ink hover:bg-gold/90 disabled:opacity-60"
            >
              {forgotMutation.isPending ? (
                <>
                  <Loader2
                    className="mr-2 h-4 w-4 animate-spin"
                    aria-hidden="true"
                  />
                  Sending…
                </>
              ) : (
                <>
                  <Send aria-hidden="true" className="mr-2 h-4 w-4" />
                  Send reset link
                </>
              )}
            </Button>
          </form>
        )}

        {!sent && (
          <p className="mt-6 text-center text-sm text-ink-muted">
            Remembered it?{" "}
            <Link to="/login" className="font-medium text-gold hover:underline">
              Sign in
            </Link>
          </p>
        )}
      </div>
    </main>
  );
}
