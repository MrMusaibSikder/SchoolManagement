import { z } from "zod";

const bdPhone = /^(?:\+?88)?01[3-9]\d{8}$/;

export const employeeFormSchema = z.object({
  employeeCode: z
    .string()
    .trim()
    .min(1, "Employee code is required.")
    .max(50, "Employee code must be 50 characters or fewer."),
  fullName: z
    .string()
    .trim()
    .min(2, "Full name must be at least 2 characters.")
    .max(150, "Full name must be 150 characters or fewer."),
  phone: z
    .string()
    .trim()
    .min(1, "Phone number is required.")
    .regex(bdPhone, "Enter a valid Bangladeshi mobile number."),
  email: z
    .string()
    .trim()
    .email("Enter a valid email address.")
    .or(z.literal(""))
    .optional(),
  joiningDate: z.string().min(1, "Joining date is required."),
  isActive: z.boolean(),
  designationId: z
    .string()
    .min(1, "Select a designation."),
  userId: z.string().optional(),
});

export type EmployeeFormValues = z.infer<typeof employeeFormSchema>;
