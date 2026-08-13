import { useState } from "react";
import { NavLink, Outlet, useNavigate } from "react-router-dom";
import {
  GraduationCap,
  LayoutDashboard,
  KeyRound,
  LogOut,
  Menu,
  School,
  Settings,
  Users,
  UserRound,
  X,
} from "lucide-react";
import { toast } from "sonner";
import { cn } from "@/lib/utils";
import { Avatar } from "@/components/ui/avatar";
import { Badge } from "@/components/ui/badge";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { useAuth } from "@/features/auth/hooks/useAuth";

const NAV_ITEMS = [
  { to: "/dashboard", label: "Dashboard", icon: LayoutDashboard },
  { to: "/academic", label: "Academic", icon: School },
  { to: "/students", label: "Students", icon: Users },
  { to: "/guardians", label: "Guardians", icon: UserRound },
];

export function AppShell() {
  const { session, logout } = useAuth();
  const navigate = useNavigate();
  const [mobileOpen, setMobileOpen] = useState(false);
  const [loggingOut, setLoggingOut] = useState(false);

  async function handleLogout() {
    if (loggingOut) return;
    setLoggingOut(true);
    try {
      await logout();
      toast.success("You have been signed out.");
    } catch {
      toast.error("Could not sign out. Please try again.");
    } finally {
      setLoggingOut(false);
    }
  }

  const sidebar = (
    <div className="flex h-full flex-col bg-sidebar">
      <div className="flex h-16 items-center gap-2 border-b px-6">
        <span className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary text-primary-foreground">
          <GraduationCap aria-hidden="true" className="h-5 w-5" />
        </span>
        <span className="font-display text-lg font-semibold text-foreground">
          SchoolERP
        </span>
      </div>

      <nav className="flex-1 space-y-1 overflow-y-auto p-4">
        <p className="px-2 pb-2 text-xs font-medium uppercase tracking-wider text-muted-foreground">
          Main
        </p>
        {NAV_ITEMS.map((item) => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) =>
              cn(
                "flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors",
                isActive
                  ? "bg-sidebar-accent text-sidebar-accent-foreground"
                  : "text-muted-foreground hover:bg-sidebar-accent/60 hover:text-foreground"
              )
            }
          >
            <item.icon aria-hidden="true" className="h-4 w-4" />
            {item.label}
          </NavLink>
        ))}

        <p className="px-2 pb-2 pt-6 text-xs font-medium uppercase tracking-wider text-muted-foreground">
          Account
        </p>
        <NavLink
          to="/change-password"
          className={({ isActive }) =>
            cn(
              "flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors",
              isActive
                ? "bg-sidebar-accent text-sidebar-accent-foreground"
                : "text-muted-foreground hover:bg-sidebar-accent/60 hover:text-foreground"
            )
          }
        >
          <KeyRound aria-hidden="true" className="h-4 w-4" />
          Change Password
        </NavLink>
      </nav>

      <div className="border-t p-4">
        <div className="flex items-center gap-3">
          <Avatar name={session?.username} />
          <div className="min-w-0 flex-1">
            <p className="truncate text-sm font-medium text-foreground">
              {session?.username}
            </p>
            <p className="truncate text-xs text-muted-foreground">
              {session?.roles?.[0] ?? "User"}
            </p>
          </div>
        </div>
        <button
          type="button"
          onClick={handleLogout}
          disabled={loggingOut}
          className="mt-3 flex w-full items-center justify-center gap-2 rounded-md border border-destructive/30 px-3 py-2 text-sm font-medium text-destructive transition hover:bg-destructive/10 disabled:opacity-60"
        >
          {loggingOut ? (
            <span className="h-4 w-4 animate-spin rounded-full border-2 border-destructive/30 border-t-destructive" />
          ) : (
            <LogOut aria-hidden="true" className="h-4 w-4" />
          )}
          {loggingOut ? "Signing out…" : "Logout"}
        </button>
      </div>
    </div>
  );

  return (
    <div className="min-h-screen bg-background">
      {/* Desktop sidebar */}
      <aside className="fixed inset-y-0 left-0 z-30 hidden w-64 border-r lg:block">
        {sidebar}
      </aside>

      {/* Mobile drawer */}
      {mobileOpen && (
        <div className="fixed inset-0 z-40 lg:hidden">
          <div
            className="absolute inset-0 bg-black/50"
            onClick={() => setMobileOpen(false)}
            aria-hidden="true"
          />
          <div className="absolute inset-y-0 left-0 w-64 bg-sidebar shadow-xl">
            <button
              type="button"
              onClick={() => setMobileOpen(false)}
              className="absolute right-3 top-4 text-muted-foreground hover:text-foreground"
              aria-label="Close menu"
            >
              <X aria-hidden="true" className="h-5 w-5" />
            </button>
            {sidebar}
          </div>
        </div>
      )}

      {/* Main column */}
      <div className="lg:pl-64">
        <header className="sticky top-0 z-20 flex h-16 items-center justify-between border-b bg-background/95 px-4 backdrop-blur sm:px-6">
          <div className="flex items-center gap-3">
            <button
              type="button"
              onClick={() => setMobileOpen(true)}
              className="rounded-md p-2 text-muted-foreground hover:bg-accent hover:text-foreground lg:hidden"
              aria-label="Open menu"
            >
              <Menu aria-hidden="true" className="h-5 w-5" />
            </button>
            <span className="text-sm font-medium text-muted-foreground">
              School Management System
            </span>
          </div>

          <DropdownMenu>
            <DropdownMenuTrigger
              asChild
              className="cursor-pointer rounded-full outline-none focus-visible:ring-2 focus-visible:ring-ring"
            >
              <button type="button" aria-label="Account menu">
                <Avatar name={session?.username} />
              </button>
            </DropdownMenuTrigger>
            <DropdownMenuContent className="w-60">
              <DropdownMenuLabel>
                <div className="flex flex-col">
                  <span className="text-sm font-semibold">
                    {session?.username}
                  </span>
                  <span className="text-xs font-normal text-muted-foreground">
                    {session?.email}
                  </span>
                </div>
              </DropdownMenuLabel>
              {session?.roles && session.roles.length > 0 && (
                <div className="px-2 pb-1">
                  {session.roles.map((r) => (
                    <Badge key={r} variant="secondary" className="mr-1">
                      {r}
                    </Badge>
                  ))}
                </div>
              )}
              <DropdownMenuSeparator />
              <DropdownMenuItem onSelect={() => navigate("/dashboard")}>
                <LayoutDashboard aria-hidden="true" className="h-4 w-4" />{" "}
                Dashboard
              </DropdownMenuItem>
              <DropdownMenuItem onSelect={() => navigate("/change-password")}>
                <KeyRound aria-hidden="true" className="h-4 w-4" /> Change
                Password
              </DropdownMenuItem>
              <DropdownMenuItem onSelect={() => navigate("/settings")}>
                <Settings aria-hidden="true" className="h-4 w-4" /> Settings
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                onSelect={handleLogout}
                className="text-destructive focus:text-destructive"
              >
                <LogOut aria-hidden="true" className="h-4 w-4" />
                {loggingOut ? "Signing out…" : "Logout"}
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </header>

        <main className="p-4 sm:p-6 lg:p-8">
          <Outlet />
        </main>
      </div>
    </div>
  );
}
