import { useState } from "react";
import { GraduationCap } from "lucide-react";
import { cn } from "@/lib/utils";
import { resolveMediaUrl } from "@/lib/media";

interface SchoolLogoProps {
  logo?: string | null;
  alt: string;
  className?: string;
}

export function SchoolLogo({ logo, alt, className }: SchoolLogoProps) {
  const src = resolveMediaUrl(logo);
  const [failedSrc, setFailedSrc] = useState<string | null>(null);

  return (
    <span
      className={cn(
        "flex h-9 w-9 shrink-0 items-center justify-center overflow-hidden rounded-lg bg-primary text-primary-foreground",
        className
      )}
    >
      {src && failedSrc !== src ? (
        <img
          src={src}
          alt={alt}
          className="h-full w-full bg-background object-contain p-1"
          onError={() => setFailedSrc(src)}
        />
      ) : (
        <GraduationCap aria-hidden="true" className="h-5 w-5" />
      )}
    </span>
  );
}
