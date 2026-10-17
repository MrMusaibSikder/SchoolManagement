import { useCurrentUser } from "@/features/auth/hooks/useCurrentUser";
import { Avatar } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { CheckCircle2, Loader2, RefreshCw, ShieldCheck, UserRound } from "lucide-react";

export function ProfilePage() {
  const { data: profile, isPending, isError, isFetching, refetch } = useCurrentUser();

  if (isPending) {
    return (
      <div className="mx-auto max-w-5xl space-y-6" aria-label="Loading profile">
        <div className="h-20 animate-pulse rounded-xl bg-muted/60" />
        <div className="grid gap-6 lg:grid-cols-[0.8fr_1.2fr]">
          <div className="h-56 animate-pulse rounded-xl bg-muted/60" />
          <div className="h-56 animate-pulse rounded-xl bg-muted/60" />
        </div>
      </div>
    );
  }

  if (isError || !profile) {
    return (
      <div className="mx-auto max-w-5xl">
        <Card>
          <CardContent className="flex flex-col items-center p-8 text-center">
            <UserRound aria-hidden="true" className="h-9 w-9 text-muted-foreground" />
            <h1 className="mt-3 font-semibold">Profile unavailable</h1>
            <p className="mt-1 text-sm text-muted-foreground">
              We could not load your account details. Please try again.
            </p>
            <Button
              type="button"
              variant="outline"
              className="mt-4"
              onClick={() => void refetch()}
              disabled={isFetching}
            >
              {isFetching ? (
                <Loader2 aria-hidden="true" className="mr-2 h-4 w-4 animate-spin" />
              ) : (
                <RefreshCw aria-hidden="true" className="mr-2 h-4 w-4" />
              )}
              Try again
            </Button>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div>
        <p className="text-sm font-medium text-primary">Account</p>
        <h1 className="mt-1 font-display text-2xl font-semibold">My profile</h1>
        <p className="mt-1 text-sm text-muted-foreground">
          Review your account information, assigned roles, and effective access.
        </p>
      </div>

      <Card className="overflow-hidden border-primary/15">
        <div className="bg-gradient-to-br from-primary/10 via-primary/5 to-background p-6 sm:p-8">
          <div className="flex flex-col gap-5 sm:flex-row sm:items-center">
            <Avatar
              name={profile.username}
              className="h-16 w-16 rounded-2xl text-xl shadow-sm"
            />
            <div className="min-w-0 flex-1">
              <div className="flex flex-wrap items-center gap-2">
                <h2 className="break-words font-display text-2xl font-semibold">
                  {profile.username}
                </h2>
                <Badge
                  variant="secondary"
                  className={
                    profile.isActive
                      ? "bg-emerald-100 text-emerald-800"
                      : "bg-muted text-muted-foreground"
                  }
                >
                  <CheckCircle2 aria-hidden="true" className="mr-1.5 h-3.5 w-3.5" />
                  {profile.isActive ? "Active account" : "Inactive account"}
                </Badge>
              </div>
              <p className="mt-1 break-all text-sm text-muted-foreground">
                {profile.email}
              </p>
            </div>
          </div>
        </div>
        <CardContent className="grid gap-4 border-t p-6 sm:grid-cols-2">
          <div>
            <p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
              User ID
            </p>
            <p className="mt-1 font-medium">{profile.userId}</p>
          </div>
          <div>
            <p className="text-xs font-medium uppercase tracking-wide text-muted-foreground">
              Account status
            </p>
            <p className="mt-1 font-medium">
              {profile.isActive ? "Enabled" : "Disabled"}
            </p>
          </div>
        </CardContent>
      </Card>

      <div className="grid gap-6 lg:grid-cols-[0.8fr_1.2fr]">
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <UserRound aria-hidden="true" className="h-5 w-5 text-primary" />
              Assigned roles
            </CardTitle>
            <CardDescription>
              Roles assigned to your account.
            </CardDescription>
          </CardHeader>
          <CardContent>
            {profile.roles.length > 0 ? (
              <ul className="flex flex-wrap gap-2">
                {profile.roles.map((role) => (
                  <li key={role}>
                    <Badge variant="secondary">{role}</Badge>
                  </li>
                ))}
              </ul>
            ) : (
              <p className="rounded-lg border border-dashed p-4 text-sm text-muted-foreground">
                No roles are assigned to your account.
              </p>
            )}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <ShieldCheck aria-hidden="true" className="h-5 w-5 text-primary" />
              Effective permissions
            </CardTitle>
            <CardDescription>
              {profile.permissions.length.toLocaleString("en-BD")} permissions
              inherited from your assigned roles.
            </CardDescription>
          </CardHeader>
          <CardContent>
            {profile.permissions.length > 0 ? (
              <ul className="flex max-h-80 flex-wrap gap-2 overflow-y-auto pr-1">
                {profile.permissions.map((permission) => (
                  <li key={permission}>
                    <Badge variant="outline" className="bg-background">
                      {permission}
                    </Badge>
                  </li>
                ))}
              </ul>
            ) : (
              <p className="rounded-lg border border-dashed p-4 text-sm text-muted-foreground">
                No permissions are assigned to your account.
              </p>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
