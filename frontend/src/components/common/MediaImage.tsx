import { useState } from "react";
import { UserRound } from "lucide-react";
import { cn } from "@/lib/utils";
import { resolveMediaUrl } from "@/lib/media";

interface MediaImageProps {
  src?: string | null;
  alt: string;
  className?: string;
  fallbackClassName?: string;
}

export function MediaImage({
  src,
  alt,
  className,
  fallbackClassName,
}: MediaImageProps) {
  const resolved = resolveMediaUrl(src);
  const [failed, setFailed] = useState(false);

  if (!resolved || failed) {
    return (
      <span
        className={cn(
          "flex h-full w-full items-center justify-center text-muted-foreground",
          fallbackClassName
        )}
        aria-hidden="true"
      >
        <UserRound className="h-5 w-5" />
      </span>
    );
  }

  return (
    <img
      src={resolved}
      alt={alt}
      className={cn("h-full w-full object-cover", className)}
      onError={() => setFailed(true)}
    />
  );
}
