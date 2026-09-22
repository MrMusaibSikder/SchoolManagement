import { useEffect, useRef, useState } from "react";
import { ImagePlus, Upload, X } from "lucide-react";
import { cn } from "@/lib/utils";
import { resolveMediaUrl } from "@/lib/media";
import { MediaImage } from "./MediaImage";

const ALLOWED_TYPES = ["image/jpeg", "image/png"];
const MAX_BYTES = 5 * 1024 * 1024;

interface ImageUploaderProps {
  existingUrl?: string | null;
  file: File | null;
  onFileChange: (file: File | null) => void;
  error?: string;
  disabled?: boolean;
}

export function ImageUploader({
  existingUrl,
  file,
  onFileChange,
  error,
  disabled,
}: ImageUploaderProps) {
  const objectUrlRef = useRef<string | null>(null);
  const [preview, setPreview] = useState<string | null>(
    resolveMediaUrl(existingUrl)
  );
  const [localError, setLocalError] = useState<string | null>(null);

  useEffect(() => {
    if (file) return;
    setPreview(resolveMediaUrl(existingUrl));
  }, [existingUrl, file]);

  useEffect(() => {
    return () => {
      if (objectUrlRef.current) URL.revokeObjectURL(objectUrlRef.current);
    };
  }, []);

  function applyFile(next: File | null) {
    if (objectUrlRef.current) {
      URL.revokeObjectURL(objectUrlRef.current);
      objectUrlRef.current = null;
    }

    if (!next) {
      setLocalError(null);
      onFileChange(null);
      setPreview(resolveMediaUrl(existingUrl));
      return;
    }

    if (!ALLOWED_TYPES.includes(next.type)) {
      setLocalError("Only JPEG or PNG images are allowed.");
      return;
    }
    if (next.size > MAX_BYTES) {
      setLocalError("Image must be 5 MB or smaller.");
      return;
    }

    setLocalError(null);
    objectUrlRef.current = URL.createObjectURL(next);
    setPreview(objectUrlRef.current);
    onFileChange(next);
  }

  const message = error ?? localError;

  return (
    <div className="space-y-2">
      <div className="flex flex-col gap-4 rounded-lg border bg-muted/20 p-4 sm:flex-row sm:items-center">
        <div className="flex h-24 w-24 shrink-0 items-center justify-center overflow-hidden rounded-full border bg-background">
          {preview ? (
            <MediaImage src={preview} alt="Employee photo preview" />
          ) : (
            <ImagePlus className="h-8 w-8 text-muted-foreground" aria-hidden="true" />
          )}
        </div>

        <div className="flex flex-wrap items-center gap-2">
          <label
            className={cn(
              "inline-flex cursor-pointer items-center gap-2 rounded-md border border-dashed bg-background px-3 py-2 text-sm text-muted-foreground transition hover:bg-accent",
              disabled && "pointer-events-none opacity-60"
            )}
          >
            <Upload className="h-4 w-4" aria-hidden="true" />
            <span>{file ? file.name : preview ? "Replace photo" : "Upload photo"}</span>
            <input
              type="file"
              accept="image/jpeg,image/png"
              className="sr-only"
              disabled={disabled}
              onChange={(event) => applyFile(event.target.files?.[0] ?? null)}
            />
          </label>

          {file && !disabled ? (
            <button
              type="button"
              className="inline-flex items-center gap-1 rounded-md border px-3 py-2 text-sm text-muted-foreground hover:bg-accent"
              onClick={() => applyFile(null)}
            >
              <X className="h-4 w-4" aria-hidden="true" />
              Remove selected photo
            </button>
          ) : null}
        </div>
      </div>
      <p className="text-xs text-muted-foreground">JPEG or PNG, up to 5 MB.</p>
      {message ? <p className="text-sm text-destructive">{message}</p> : null}
    </div>
  );
}
