import { z } from "zod";

export const smsTemplateSchema = z.object({
  name: z
    .string()
    .trim()
    .min(1, "Template name is required.")
    .max(100, "Template name must be 100 characters or fewer."),
  message: z
    .string()
    .trim()
    .min(1, "Template message is required.")
    .max(500, "Template message must be 500 characters or fewer."),
  isActive: z.boolean(),
});

export type SmsTemplateFormValues = z.infer<typeof smsTemplateSchema>;

export const smsPlaceholderSchema = z.object({
  studentName: z.string().optional(),
  guardianName: z.string().optional(),
  teacherName: z.string().optional(),
  employeeName: z.string().optional(),
  class: z.string().optional(),
  section: z.string().optional(),
  roll: z.string().optional(),
  attendanceStatus: z.string().optional(),
  date: z.string().optional(),
  time: z.string().optional(),
  schoolName: z.string().optional(),
});

export type SmsPlaceholderFormValues = z.infer<typeof smsPlaceholderSchema>;
