import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "sonner";
import { Eye, EyeOff, Loader2, Lock, User } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useLogin } from "../hooks/useLogin";
import { useAuth } from "../hooks/useAuth";
import { loginSchema, type LoginFormValues } from "../schemas/login.schema";
import { getAuthErrorMessage } from "../api/auth.api";

export function LoginPage() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const loginMutation = useLogin();
  const [showPassword, setShowPassword] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { usernameOrEmail: "", password: "" },
  });

  const onSubmit = handleSubmit(async (values) => {
    try {
      const response = await loginMutation.mutateAsync({
        usernameOrEmail: values.usernameOrEmail,
        password: values.password,
      });
      login(response);
      toast.success(`Welcome back, ${response.username || "there"}!`);
      // After login, land on the authenticated dashboard (inside the AppShell).
      navigate("/dashboard", { replace: true });
    } catch (error) {
      toast.error(getAuthErrorMessage(error));
    }
  });

  return (
    <main className="flex min-h-screen items-center justify-center bg-paper px-6 text-ink">
      <div className="w-full max-w-md rounded-lg border border-ink/10 bg-card p-8 shadow-[0_18px_40px_-20px_rgba(15,42,61,0.35)]">
        <div className="mb-8 text-center">
          <h1 className="font-display text-2xl font-semibold text-ink">
            School Login
          </h1>
          <p className="mt-2 text-sm text-ink-muted">
            Sign in to access the school dashboard.
          </p>
        </div>

        <form onSubmit={onSubmit} noValidate className="space-y-5">
          <div>
            <label
              htmlFor="usernameOrEmail"
              className="mb-1.5 block text-sm font-medium text-ink"
            >
              Username or email <span className="text-rust">*</span>
            </label>
            <div className="relative">
              <User
                aria-hidden="true"
                className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-ink-muted"
              />
              <input
                id="usernameOrEmail"
                type="text"
                autoComplete="username"
                autoFocus
                placeholder="Enter username or email"
                className={`w-full rounded-md border bg-background py-2.5 pl-10 pr-3 text-sm text-ink shadow-sm outline-none transition focus:ring-2 ${
                  errors.usernameOrEmail
                    ? "border-rust focus:ring-rust/30"
                    : "border-input focus:ring-gold/40"
                }`}
                {...register("usernameOrEmail")}
              />
            </div>
            {errors.usernameOrEmail && (
              <p className="mt-1.5 text-xs text-rust">
                {errors.usernameOrEmail.message}
              </p>
            )}
          </div>

          <div>
            <div className="mb-1.5 flex items-center justify-between">
              <label
                htmlFor="password"
                className="block text-sm font-medium text-ink"
              >
                Password <span className="text-rust">*</span>
              </label>
              <Link
                to="/forgot-password"
                className="text-xs font-medium text-gold hover:underline"
              >
                Forgot password?
              </Link>
            </div>
            <div className="relative">
              <Lock
                aria-hidden="true"
                className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-ink-muted"
              />
              <input
                id="password"
                type={showPassword ? "text" : "password"}
                autoComplete="current-password"
                placeholder="Enter password"
                className={`w-full rounded-md border bg-background py-2.5 pl-10 pr-10 text-sm text-ink shadow-sm outline-none transition focus:ring-2 ${
                  errors.password
                    ? "border-rust focus:ring-rust/30"
                    : "border-input focus:ring-gold/40"
                }`}
                {...register("password")}
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
            {errors.password && (
              <p className="mt-1.5 text-xs text-rust">
                {errors.password.message}
              </p>
            )}
          </div>

          <Button
            type="submit"
            disabled={loginMutation.isPending}
            className="w-full bg-gold text-ink hover:bg-gold/90 disabled:opacity-60"
          >
            {loginMutation.isPending ? (
              <>
                <Loader2
                  className="mr-2 h-4 w-4 animate-spin"
                  aria-hidden="true"
                />
                Signing in…
              </>
            ) : (
              "Sign in"
            )}
</Button>
        </form>

        <p className="mt-6 text-center text-sm text-ink-muted">
          Don&apos;t have an account?{" "}
          <Link to="/register" className="font-medium text-gold hover:underline">
            Create an account
          </Link>
        </p>
        <p className="mt-2 text-center text-xs text-ink-muted">
          <Link to="/" className="hover:underline">
            Back to home
          </Link>
        </p>
      </div>
    </main>
  );
}
