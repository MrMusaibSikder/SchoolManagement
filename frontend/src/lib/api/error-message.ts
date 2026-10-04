import { isAxiosError } from "axios";

function readMessage(value: unknown): string | null {
  if (typeof value === "string") {
    const message = value.trim();
    return message.length > 0 ? message : null;
  }

  if (Array.isArray(value)) {
    for (const item of value) {
      const message = readMessage(item);
      if (message) return message;
    }
    return null;
  }

  if (typeof value !== "object" || value === null) return null;

  const data = value as Record<string, unknown>;
  for (const key of ["message", "detail"]) {
    const message = readMessage(data[key]);
    if (message) return message;
  }

  const errors = data.errors;
  if (Array.isArray(errors)) {
    for (const error of errors) {
      if (typeof error === "object" && error !== null) {
        const message = readMessage((error as Record<string, unknown>).message);
        if (message) return message;
      }
    }
  } else if (typeof errors === "object" && errors !== null) {
    for (const messages of Object.values(errors)) {
      const message = readMessage(messages);
      if (message) return message;
    }
  }

  return readMessage(data.title);
}

export function getApiErrorMessage(error: unknown, fallback: string): string {
  if (isAxiosError(error)) {
    const responseMessage = readMessage(error.response?.data);
    if (responseMessage) return responseMessage;

    if (!error.response) {
      return "Unable to connect to the server. Please check your connection and try again.";
    }
  }

  return fallback;
}
