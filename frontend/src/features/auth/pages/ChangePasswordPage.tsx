import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "sonner";
import { Eye, EyeOff, KeyRound, Loader2, Lock } from "lucide-react";
import { Button } from "@/components/ui/button";
import { useChangePassword } from "../hooks/useChangePassword";
import {
  changePasswordSchema,
  type ChangePasswordFormValues,
} from "../schemas/change-password.schema";
import { getAuthErrorMessage } from "../api/auth.api";

export function ChangePasswordPage() {
  const navigate = useNavigate();
  const changeMutation = useChangePassword();
  const [showCurrent, setShowCurrent] = useState(false);
  const [showNew, setShowNew] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<ChangePasswordFormValues>({
    resolver: zodResolver(changePasswordSchema),
    defaultValues: {
      currentPassword: "",
      newPassword: "",
      confirmNewPassword: "",
    },
  });

  const onSubmit = handleSubmit(async (values) => {
    try {
      await changeMutation.mutateAsync({
        currentPassword: values.currentPassword,
        newPassword: values.newPassword,
        confirmNewPassword: values.confirmNewPassword,
      });
      toast.success("Password updated successfully.");
      reset();
      navigate("/login", { replace: true });
    } catch (error) {
      toast.error(getAuthErrorMessage(error));
    }
  });

  const inputCls = (hasError: boolean) =>
    `w-full rounded-md border bg-background py-2.5 pl-10 text-sm text-ink shadow-sm outline-none transition focus:ring-2 ${
      hasError
        ? "border-rust focus:ring-rust/30"
        : "border-input focus:ring-gold/40"
    }`;

  const passwordField = (
    id: string,
    label: string,
    show: boolean,
    toggle: () => void,
    autoComplete: string,
    placeholder: string,
    errorMsg: string | undefined,
    registration: ReturnType<typeof register>
  ) => (
    <div>
      <label
        htmlFor={id}
        className="mb-1.5 block text-sm font-medium text-ink"
      >
        {label} <span className="text-rust">*</span>
      </label>
      <div className="relative">
        <Lock
          aria-hidden="true"
          className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-ink-muted"
        />
        <input
          id={id}
          type={show ? "text" : "password"}
          autoComplete={autoComplete}
          placeholder={placeholder}
          className={`${inputCls(!!errorMsg)} pr-10`}
          {...registration}
        />
        <button
          type="button"
          onClick={toggle}
          aria-label={show ? "Hide password" : "Show password"}
          className="absolute right-3 top-1/2 -translate-y-1/2 text-ink-muted transition hover:text-ink"
        >
          {show ? (
            <EyeOff aria-hidden="true" className="h-4 w-4" />
          ) : (
            <Eye aria-hidden="true" className="h-4 w-4" />
          )}
        </button>
      </div>
      {errorMsg && <p className="mt-1.5 text-xs text-rust">{errorMsg}</p>}
    </div>
  );

  return (
    <main className="flex min-h-screen items-center justify-center bg-paper px-6 py-10 text-ink">
      <div className="w-full max-w-md rounded-lg border border-ink/10 bg-card p-8 shadow-[0_18px_40px_-20px_rgba(15,42,61,0.35)]">
        <div className="mb-8 text-center">
          <div className="mx-auto mb-4 inline-flex h-12 w-12 items-center justify-center rounded-full bg-gold/15 text-gold">
            <KeyRound aria-hidden="true" className="h-6 w-6" />
          </div>
          <h1 className="font-display text-2xl font-semibold text-ink">
            Change password
          </h1>
          <p className="mt-2 text-sm text-ink-muted">
            Enter your current password and choose a new one.
          </p>
        </div>

        <form onSubmit={onSubmit} noValidate className="space-y-5">
          {passwordField(
            "currentPassword",
            "Current password",
            showCurrent,
            () => setShowCurrent((v) => !v),
            "current-password",
            "Enter your current password",
            errors.currentPassword?.message,
            register("currentPassword")
          )}
          {passwordField(
            "newPassword",
            "New password",
            showNew,
            () => setShowNew((v) => !v),
            "new-password",
            "At least 8 characters",
            errors.newPassword?.message,
            register("newPassword")
          )}
          {passwordField(
            "confirmNewPassword",
            "Confirm new password",
            showConfirm,
            () => setShowConfirm((v) => !v),
            "new-password",
            "Re-enter your new password",
            errors.confirmNewPassword?.message,
            register("confirmNewPassword")
          )}

          <Button
            type="submit"
            disabled={changeMutation.isPending || isSubmitting}
            className="w-full bg-gold text-ink hover:bg-gold/90 disabled:opacity-60"
          >
            {changeMutation.isPending ? (
              <>
                <Loader2
                  className="mr-2 h-4 w-4 animate-spin"
                  aria-hidden="true"
                />
                Updating…
              </>
            ) : (
              "Update password"
            )}
          </Button>
        </form>
      </div>
    </main>
  );
}
