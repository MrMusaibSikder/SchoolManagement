import { useState, type FormEvent } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../lib/auth";
import { ApiError } from "../lib/api";
import { Button, ErrorBanner, Input } from "../components/ui";

export default function RegisterPage() {
  const { register, homeRoute } = useAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    confirmPassword: "",
    requestedRole: "Student" as "Student" | "Teacher",
    superAdminCode: "",
  });
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  function update<K extends keyof typeof form>(key: K, value: (typeof form)[K]) {
    setForm((f) => ({ ...f, [key]: value }));
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError(null);
    setIsSubmitting(true);
    try {
      await register({
        email: form.email,
        password: form.password,
        confirmPassword: form.confirmPassword,
        firstName: form.firstName,
        lastName: form.lastName,
        requestedRole: form.requestedRole,
        superAdminCode: form.superAdminCode || undefined,
      });
      navigate(homeRoute());
    } catch (err) {
      setError(err instanceof ApiError ? err.message : "Something went wrong. Please try again.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <div className="relative flex min-h-screen items-center justify-center overflow-hidden bg-gradient-to-br from-brand-600 via-indigo-600 to-fuchsia-600 px-4 py-10">
      <div className="pointer-events-none absolute -left-24 -top-24 h-72 w-72 rounded-full bg-white/10 blur-3xl" />
      <div className="pointer-events-none absolute -bottom-32 -right-16 h-80 w-80 rounded-full bg-fuchsia-400/20 blur-3xl" />
      <div className="relative w-full max-w-sm">
        <div className="mb-8 text-center">
          <p className="font-display text-2xl font-bold text-white">CourseHub</p>
          <p className="mt-1 text-sm text-brand-50">Create your account</p>
        </div>
        <form onSubmit={handleSubmit} className="glass-surface space-y-4 rounded-xl p-6">
          <ErrorBanner message={error} />
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="mb-1 block text-xs font-medium text-slate-600">First name</label>
              <Input required value={form.firstName} onChange={(e) => update("firstName", e.target.value)} />
            </div>
            <div>
              <label className="mb-1 block text-xs font-medium text-slate-600">Last name</label>
              <Input required value={form.lastName} onChange={(e) => update("lastName", e.target.value)} />
            </div>
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-slate-600">Email</label>
            <Input type="email" required value={form.email} onChange={(e) => update("email", e.target.value)} />
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-slate-600">Password</label>
            <Input type="password" required value={form.password} onChange={(e) => update("password", e.target.value)} />
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-slate-600">Confirm password</label>
            <Input
              type="password"
              required
              value={form.confirmPassword}
              onChange={(e) => update("confirmPassword", e.target.value)}
            />
          </div>
          <div>
            <label className="mb-1 block text-xs font-medium text-slate-600">I am registering as</label>
            <select
              value={form.requestedRole}
              onChange={(e) => update("requestedRole", e.target.value as "Student" | "Teacher")}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm focus:border-brand-500 focus:outline-none focus:ring-2 focus:ring-brand-100"
            >
              <option value="Student">Student</option>
              <option value="Teacher">Teacher</option>
            </select>
            <p className="mt-1 text-xs text-slate-400">
              An Admin still has to promote you to a Teacher/Student profile afterwards — this only sets your role.
            </p>
          </div>
          <details className="rounded-lg border border-slate-200 px-3 py-2">
            <summary className="cursor-pointer text-xs font-medium text-slate-500">Have a SuperAdmin invite code?</summary>
            <div className="mt-2">
              <Input
                placeholder="SuperAdmin invite code"
                value={form.superAdminCode}
                onChange={(e) => update("superAdminCode", e.target.value)}
              />
            </div>
          </details>
          <Button type="submit" disabled={isSubmitting} className="w-full">
            {isSubmitting ? "Creating account…" : "Create account"}
          </Button>
        </form>
        <p className="mt-4 text-center text-sm text-brand-50">
          Already have an account?{" "}
          <Link to="/login" className="font-medium text-white underline underline-offset-2 hover:text-amber-200">
            Sign in
          </Link>
        </p>
        <p className="mt-2 text-center text-xs text-brand-100">
          <Link to="/" className="hover:text-white hover:underline">
            ← Back to home
          </Link>
        </p>
      </div>
    </div>
  );
}
