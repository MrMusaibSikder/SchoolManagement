import { z } from "zod";

export const createNoticeSchema = z
  .object({
    title: z
      .string()
      .trim()
      .min(1, "Notice title is required.")
      .max(200, "Notice title must be 200 characters or fewer."),
    description: z
      .string()
      .trim()
      .min(1, "Notice details are required.")
      .max(2000, "Notice details must be 2,000 characters or fewer."),
    publishDate: z
      .string()
      .min(1, "Publish date is required.")
      .refine((value) => !Number.isNaN(new Date(value).getTime()), "Enter a valid publish date."),
    expiryDate: z
      .string()
      .refine(
        (value) => !value || !Number.isNaN(new Date(value).getTime()),
        "Enter a valid expiry date."
      ),
    priority: z.enum(["1", "2", "3"]),
    audience: z.enum(["1", "2", "3", "4", "5"]),
  })
  .refine(
    (values) =>
      !values.expiryDate ||
      new Date(values.expiryDate).getTime() >=
        new Date(values.publishDate).getTime(),
    {
      message: "Expiry date cannot be earlier than the publish date.",
      path: ["expiryDate"],
    }
  );

export type CreateNoticeFormValues = z.infer<typeof createNoticeSchema>;
