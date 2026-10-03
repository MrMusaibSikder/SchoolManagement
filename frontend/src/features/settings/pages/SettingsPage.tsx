import { useState } from "react";
import { Loader2, Save } from "lucide-react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { ImageUploader } from "@/components/common/ImageUploader";
import { usePermissions } from "@/features/auth/hooks/usePermissions";
import { Permission } from "@/lib/permissions";
import { useSchoolSettings, useUpdateSchool } from "../hooks/useSettingsData";
import type { SchoolDto } from "../types/settings.types";

function createFormState(school: SchoolDto) {
  return {
    name: school.name ?? "",
    eiin: school.eiin ?? "",
    address: school.address ?? "",
    phone: school.phone ?? "",
    email: school.email ?? "",
    logoFile: null as File | null,
  };
}

function SchoolSettingsForm({
  school,
  canUpdate,
}: {
  school: SchoolDto;
  canUpdate: boolean;
}) {
  const [form, setForm] = useState(() => createFormState(school));
  const updateSchool = useUpdateSchool();

  async function handleSubmit(event: React.FormEvent) {
    event.preventDefault();

    const trimmedName = form.name.trim();
    if (!trimmedName) {
      toast.error("School name is required.");
      return;
    }

    try {
      await updateSchool.mutateAsync({
        payload: {
          id: school.id,
          name: trimmedName,
          eiin: form.eiin.trim() || null,
          address: form.address.trim() || null,
          phone: form.phone.trim() || null,
          email: form.email.trim() || null,
        },
        logoFile: form.logoFile,
      });
      toast.success("School settings updated.");
    } catch (error) {
      toast.error((error as Error)?.message ?? "Unable to update school settings.");
    }
  }

  return (
    <div className="mx-auto max-w-4xl space-y-6">
      <div>
        <h1 className="font-display text-2xl font-semibold">School settings</h1>
        <p className="text-sm text-muted-foreground">
          Update the school profile, contact information, and logo.
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>School profile</CardTitle>
          <CardDescription>This information appears across the app and public pages.</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={(event) => void handleSubmit(event)} className="space-y-5">
            <ImageUploader
              existingUrl={school.logo}
              file={form.logoFile}
              onFileChange={(file) => setForm((current) => ({ ...current, logoFile: file }))}
              disabled={!canUpdate || updateSchool.isPending}
            />

            <div className="grid gap-4 md:grid-cols-2">
              <div className="md:col-span-2">
                <label className="mb-1 block text-sm font-medium">School name</label>
                <input
                  value={form.name}
                  onChange={(event) => setForm((current) => ({ ...current, name: event.target.value }))}
                  className="w-full rounded-md border bg-background px-3 py-2"
                  placeholder="Badalpara High School And College"
                  disabled={!canUpdate}
                  required
                />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">EIIN</label>
                <input
                  value={form.eiin}
                  onChange={(event) => setForm((current) => ({ ...current, eiin: event.target.value }))}
                  className="w-full rounded-md border bg-background px-3 py-2"
                  placeholder="123456"
                  disabled={!canUpdate}
                />
              </div>

              <div>
                <label className="mb-1 block text-sm font-medium">Phone</label>
                <input
                  value={form.phone}
                  onChange={(event) => setForm((current) => ({ ...current, phone: event.target.value }))}
                  className="w-full rounded-md border bg-background px-3 py-2"
                  placeholder="01681439385"
                  disabled={!canUpdate}
                />
              </div>

              <div className="md:col-span-2">
                <label className="mb-1 block text-sm font-medium">Address</label>
                <textarea
                  value={form.address}
                  onChange={(event) => setForm((current) => ({ ...current, address: event.target.value }))}
                  rows={3}
                  className="w-full rounded-md border bg-background px-3 py-2"
                  placeholder="School address"
                  disabled={!canUpdate}
                />
              </div>

              <div className="md:col-span-2">
                <label className="mb-1 block text-sm font-medium">Email</label>
                <input
                  type="email"
                  value={form.email}
                  onChange={(event) => setForm((current) => ({ ...current, email: event.target.value }))}
                  className="w-full rounded-md border bg-background px-3 py-2"
                  placeholder="school@example.com"
                  disabled={!canUpdate}
                />
              </div>
            </div>

            {canUpdate && (
              <div className="flex justify-end">
                <Button type="submit" disabled={updateSchool.isPending}>
                  {updateSchool.isPending ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" /> Saving…
                    </>
                  ) : (
                    <>
                      <Save className="mr-2 h-4 w-4" /> Save changes
                    </>
                  )}
                </Button>
              </div>
            )}
          </form>
        </CardContent>
      </Card>
    </div>
  );
}

export function SettingsPage() {
  const { hasPermission } = usePermissions();
  const canUpdate = hasPermission(Permission.SchoolUpdate);
  const { data: school, isPending, isError } = useSchoolSettings();

  if (!hasPermission(Permission.SchoolView)) {
    return (
      <Card className="mx-auto max-w-4xl">
        <CardContent className="p-6 text-sm text-destructive">
          You do not have permission to manage the school settings.
        </CardContent>
      </Card>
    );
  }

  if (isPending) {
    return (
      <div className="flex items-center justify-center py-10 text-sm text-muted-foreground">
        <Loader2 className="mr-2 h-4 w-4 animate-spin" /> Loading school settings…
      </div>
    );
  }

  if (isError || !school) {
    return (
      <Card className="mx-auto max-w-4xl">
        <CardContent className="p-6 text-sm text-destructive">
          Unable to load school settings. Please refresh and try again.
        </CardContent>
      </Card>
    );
  }

  return (
    <SchoolSettingsForm
      key={school.id}
      school={school}
      canUpdate={canUpdate}
    />
  );
}
