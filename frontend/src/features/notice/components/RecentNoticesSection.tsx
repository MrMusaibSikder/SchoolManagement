import { BellRing, Loader2, RefreshCw } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { NoticeCards } from "./NoticeCards";
import { useRecentNotices } from "../hooks/useRecentNotices";

function NoticeCardsSkeleton() {
  return (
    <div
      aria-label="Loading recent notices"
      className="grid gap-4 sm:grid-cols-2 xl:grid-cols-3"
    >
      {Array.from({ length: 3 }, (_, index) => (
        <div
          key={index}
          aria-hidden="true"
          className="h-40 animate-pulse rounded-xl border bg-muted/50"
        />
      ))}
    </div>
  );
}

export function RecentNoticesSection() {
  const { data: notices = [], isPending, isError, refetch, isFetching } =
    useRecentNotices();

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <BellRing aria-hidden="true" className="h-5 w-5 text-primary" />
          School notices
        </CardTitle>
        <CardDescription>
          Important updates and announcements from your school.
        </CardDescription>
      </CardHeader>
      <CardContent>
        {isPending ? <NoticeCardsSkeleton /> : null}

        {isError ? (
          <div
            role="alert"
            className="flex flex-col gap-3 rounded-lg border border-destructive/20 bg-destructive/5 p-4 sm:flex-row sm:items-center sm:justify-between"
          >
            <p className="text-sm text-destructive">
              Notices could not be loaded. Please try again.
            </p>
            <Button
              type="button"
              variant="outline"
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
          </div>
        ) : null}

        {!isPending && !isError && notices.length === 0 ? (
          <div className="rounded-lg border border-dashed p-8 text-center">
            <BellRing
              aria-hidden="true"
              className="mx-auto h-8 w-8 text-muted-foreground/60"
            />
            <p className="mt-3 font-medium">No recent notices</p>
            <p className="mt-1 text-sm text-muted-foreground">
              New school announcements will appear here.
            </p>
          </div>
        ) : null}

        {!isPending && !isError && notices.length > 0 ? (
          <NoticeCards notices={notices} />
        ) : null}
      </CardContent>
    </Card>
  );
}
