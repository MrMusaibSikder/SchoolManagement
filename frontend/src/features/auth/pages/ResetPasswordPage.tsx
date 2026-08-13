import { useState } from "react";
import { Link, useNavigate, useSearchParams } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "sonner";
import { Eye, EyeOff, Loader2, Lock, ShieldCheck } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useResetPassword } from "../hooks/useResetPassword";
import {
  resetPasswordSchema,
  type ResetPasswordFormValues,
} from "../schemas/reset-password.schema";
import { getAuthErrorMessage } from "../api/auth.api";

export function ResetPasswordPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const resetMutation = useResetPassword();
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ResetPasswordFormValues>({
    resolver: zodResolver(resetPasswordSchema),
    defaultValues: {
      token: searchParams.get("token") ?? "",
      newPassword: "",
      confirmNewPassword: "",
    },
  });

  const onSubmit = handleSubmit(async (values) => {
    try {
      await resetMutation.mutateAsync({
        token: values.token,
        newPassword: values.newPassword,
        confirmNewPassword: values.confirmNewPassword,
      });
      toast.success("Your password has been reset. Please sign in.");
      navigate("/login", { replace: true });
    } catch (error) {
      toast.error(getAuthErrorMessage(error));
    }
  });

  const inputCls = (hasError: boolean, withToggle = false) =>
    `w-full rounded-md border bg-background py-2.5 pl-10 text-sm text-ink shadow-sm outline-none transition focus:ring-2 ${
      withToggle ? "pr-10" : "pr-3"
    } ${
      hasError
        ? "border-rust focus:ring-rust/30"
        : "border-input focus:ring-gold/40"
    }`;

  return (
    <main className="flex min-h-screen items-center justify-center bg-paper px-6 py-10 text-ink">
      <div className="w-full max-w-md rounded-lg border border-ink/10 bg-card p-8 shadow-[0_18px_40px_-20px_rgba(15,42,61,0.35)]">
        <div className="mb-8 text-center">
          <div className="mx-auto mb-4 inline-flex h-12 w-12 items-center justify-center rounded-full bg-gold/15 text-gold">
            <ShieldCheck aria-hidden="true" className="h-6 w-6" />
          </div>
          <h1 className="font-display text-2xl font-semibold text-ink">
            Set a new password
          </h1>
          <p className="mt-2 text-sm text-ink-muted">
            Choose a strong password to finish resetting your account.
          </p>
        </div>

        <form onSubmit={onSubmit} noValidate className="space-y-5">
          <div>
            <label
              htmlFor="newPassword"
              className="mb-1.5 block text-sm font-medium text-ink"
            >
              New password <span className="text-rust">*</span>
            </label>
            <div className="relative">
              <Lock
                aria-hidden="true"
                className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-ink-muted"
              />
              <input
                id="newPassword"
                type={showPassword ? "text" : "password"}
                autoComplete="new-password"
                autoFocus
                placeholder="At least 8 characters"
                className={inputCls(!!errors.newPassword, true)}
                {...register("newPassword")}
              />
              <button
                type="button"
                onClick={() => setShowPassword((v) => !v)}
                aria-label={showPassword ? "Hide password" : "Show password"}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-ink-muted transition hover:text-ink"
              >
                {showPassword ? (
                  <EyeOff aria-hidden="true" className="h-4 w-4" />
                ) : (
                  <Eye aria-hidden="true" className="h-4 w-4" />
                )}
              </button>
            </div>
            {errors.newPassword && (
              <p className="mt-1.5 text-xs text-rust">
                {errors.newPassword.message}
              </p>
            )}
          </div>

          <div>
            <label
              htmlFor="confirmNewPassword"
              className="mb-1.5 block text-sm font-medium text-ink"
            >
              Confirm new password <span className="text-rust">*</span>
            </label>
            <div className="relative">
              <Lock
                aria-hidden="true"
                className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-ink-muted"
              />
              <input
                id="confirmNewPassword"
                type={showConfirm ? "text" : "password"}
                autoComplete="new-password"
                placeholder="Re-enter your new password"
                className={inputCls(!!errors.confirmNewPassword, true)}
                {...register("confirmNewPassword")}
              />
              <button
                type="button"
                onClick={() => setShowConfirm((v) => !v)}
                aria-label={showConfirm ? "Hide password" : "Show password"}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-ink-muted transition hover:text-ink"
              >
                {showConfirm ? (
                  <EyeOff aria-hidden="true" className="h-4 w-4" />
                ) : (
                  <Eye aria-hidden="true" className="h-4 w-4" />
                )}
              </button>
            </div>
            {errors.confirmNewPassword && (
              <p className="mt-1.5 text-xs text-rust">
                {errors.confirmNewPassword.message}
              </p>
            )}
          </div>

          <Button
            type="submit"
            disabled={resetMutation.isPending}
            className="w-full bg-gold text-ink hover:bg-gold/90 disabled:opacity-60"
          >
            {resetMutation.isPending ? (
              <>
                <Loader2
                  className="mr-2 h-4 w-4 animate-spin"
                  aria-hidden="true"
                />
                Resetting…
              </>
            ) : (
              "Reset password"
            )}
          </Button>
        </form>

        <p className="mt-6 text-center text-sm text-ink-muted">
          <Link to="/login" className="font-medium text-gold hover:underline">
            Back to sign in
          </Link>
        </p>
      </div>
    </main>
  );
}
