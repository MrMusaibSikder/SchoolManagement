
import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "sonner";
import {
  Eye,
  EyeOff,
  Loader2,
  Lock,
  Mail,
  User,
} from "lucide-react";

import { Button } from "@/components/ui/button";
import { useRegister } from "../hooks/useRegister";
import {
  registerSchema,
  type RegisterFormValues,
} from "../schemas/register.schema";
import { getAuthErrorMessage } from "../api/auth.api";

export function RegisterPage() {
  const navigate = useNavigate();
  const registerMutation = useRegister();

  const [showPassword, setShowPassword] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);

  const {
  register,
  handleSubmit,
  formState: { errors },
} = useForm<RegisterFormValues>({
  resolver: zodResolver(registerSchema),
  defaultValues: {
    username: "",
    email: "",
    password: "",
    confirmPassword: "",
  },
});


const onSubmit = handleSubmit(
  async (values) => {
    console.log("✅ FORM SUBMITTED", values);

    try {
      const created = await registerMutation.mutateAsync({
        username: values.username,
        email: values.email,
        password: values.password,
      });

      console.log("✅ API SUCCESS:", created);
      toast.success(`Account created! Please sign in.`);
      navigate("/login", { replace: true });
    } catch (error) {
      console.error(" API ERROR:", error);
      toast.error(getAuthErrorMessage(error));
    }
  },
  (errors) => {
    console.error(" VALIDATION FAILED:", errors);
  }
);

  const inputCls = (hasError: boolean) =>
    `w-full rounded-md border bg-background py-2.5 pl-10 pr-3 text-sm text-ink shadow-sm outline-none transition focus:ring-2 ${
      hasError
        ? "border-rust focus:ring-rust/30"
        : "border-input focus:ring-gold/40"
    }`;

  return (
    <main className="flex min-h-screen items-center justify-center bg-paper px-6 py-10 text-ink">
      <div className="w-full max-w-md rounded-lg border border-ink/10 bg-card p-8 shadow-[0_18px_40px_-20px_rgba(15,42,61,0.35)]">
        <div className="mb-8 text-center">
          <h1 className="font-display text-2xl font-semibold text-ink">
            Create an account
          </h1>

          <p className="mt-2 text-sm text-ink-muted">
            Register to access the school portal.
          </p>
        </div>

        <form onSubmit={onSubmit} noValidate className="space-y-5">
          {/* Username */}
          <div>
            <label
              htmlFor="username"
              className="mb-1.5 block text-sm font-medium text-ink"
            >
              Username <span className="text-rust">*</span>
            </label>

            <div className="relative">
              <User
                aria-hidden="true"
                className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-ink-muted"
              />

              <input
                id="username"
                type="text"
                autoComplete="username"
                autoFocus
                placeholder="Enter a username"
                className={inputCls(!!errors.username)}
                {...register("username")}
              />
            </div>

            {errors.username && (
              <p className="mt-1.5 text-xs text-rust">
                {errors.username.message}
              </p>
            )}
          </div>

          {/* Email */}
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
                placeholder="you@example.com"
                className={inputCls(!!errors.email)}
                {...register("email")}
              />
            </div>

            {errors.email && (
              <p className="mt-1.5 text-xs text-rust">
                {errors.email.message}
              </p>
            )}
          </div>

          {/* Password */}
          <div>
            <label
              htmlFor="password"
              className="mb-1.5 block text-sm font-medium text-ink"
            >
              Password <span className="text-rust">*</span>
            </label>

            <div className="relative">
              <Lock
                aria-hidden="true"
                className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-ink-muted"
              />

              <input
                id="password"
                type={showPassword ? "text" : "password"}
                autoComplete="new-password"
                placeholder="At least 8 characters"
                className={`${inputCls(!!errors.password)} pr-10`}
                {...register("password")}
              />

              <button
                type="button"
                onClick={() => setShowPassword((value) => !value)}
                aria-label={
                  showPassword ? "Hide password" : "Show password"
                }
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

          {/* Confirm password */}
          <div>
            <label
              htmlFor="confirmPassword"
              className="mb-1.5 block text-sm font-medium text-ink"
            >
              Confirm password <span className="text-rust">*</span>
            </label>

            <div className="relative">
              <Lock
                aria-hidden="true"
                className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-ink-muted"
              />

              <input
                id="confirmPassword"
                type={showConfirm ? "text" : "password"}
                autoComplete="new-password"
                placeholder="Re-enter your password"
                className={`${inputCls(!!errors.confirmPassword)} pr-10`}
                {...register("confirmPassword")}
              />

              <button
                type="button"
                onClick={() => setShowConfirm((value) => !value)}
                aria-label={
                  showConfirm ? "Hide password" : "Show password"
                }
                className="absolute right-3 top-1/2 -translate-y-1/2 text-ink-muted transition hover:text-ink"
              >
                {showConfirm ? (
                  <EyeOff aria-hidden="true" className="h-4 w-4" />
                ) : (
                  <Eye aria-hidden="true" className="h-4 w-4" />
                )}
              </button>
            </div>

            {errors.confirmPassword && (
              <p className="mt-1.5 text-xs text-rust">
                {errors.confirmPassword.message}
              </p>
            )}
          </div>

          {/* Submit */}
         <Button
  type="submit"
  disabled={registerMutation.isPending}
  className="w-full bg-gold text-ink hover:bg-gold/90 disabled:opacity-60"
>
  {registerMutation.isPending ? (
    <>
      <Loader2
        className="mr-2 h-4 w-4 animate-spin"
        aria-hidden="true"
      />
      Creating account…
    </>
  ) : (
    "Create account"
  )}
</Button>
        </form>

        <p className="mt-6 text-center text-sm text-ink-muted">
          Already have an account?{" "}
          <Link
            to="/login"
            className="font-medium text-gold hover:underline"
          >
            Sign in
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

