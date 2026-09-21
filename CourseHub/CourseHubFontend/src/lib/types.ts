// Mirrors CourseHubBackend's Application-layer DTOs. Field names/casing
// match the JSON the API actually returns (camelCase, via ASP.NET Core's
// default System.Text.Json settings).

export interface UserSummary {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  status: string;
  roles: string[];
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAtUtc: string;
  user: UserSummary;
}

export interface CurrentUser {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  status: string;
  roles: string[];
  permissions: string[];
  lastLoginAt: string | null;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface CourseResponse {
  id: string;
  name: string;
  code: string;
  description: string | null;
  thumbnailUrl: string | null;
  durationInMonths: number;
  isActive: boolean;
  isPublic: boolean;
  assignedTeacherId: string | null;
  createdAt: string;
  updatedAt: string | null;
}

export interface EligibleUserResponse {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
}

export interface TeacherResponse {
  id: string;
  userId: string;
  employeeId: string;
  firstName: string;
  lastName: string;
  profileImageUrl: string | null;
  phone: string | null;
  email: string | null;
  bio: string | null;
  isActive: boolean;
  isProfilePublic: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface StudentResponse {
  id: string;
  userId: string;
  studentId: string;
  firstName: string;
  lastName: string;
  profileImageUrl: string | null;
  dateOfBirth: string | null;
  phone: string | null;
  email: string | null;
  address: string | null;
  guardianName: string | null;
  guardianPhone: string | null;
  isActive: boolean;
  isProfilePublic: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface BatchResponse {
  id: string;
  courseId: string;
  name: string;
  code: string;
  startDate: string;
  endDate: string | null;
  capacity: number | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export type EnrollmentStatus = "Pending" | "Active" | "Completed" | "Cancelled";

export interface EnrollmentResponse {
  id: string;
  studentId: string;
  batchId: string;
  enrollmentDate: string;
  status: EnrollmentStatus;
  createdAt: string;
  updatedAt: string | null;
}

export interface UserResponse {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  status: string;
  roles: string[];
  lastLoginAt: string | null;
  createdAt: string;
}

export interface RoleResponse {
  id: string;
  name: string;
  isSystemRole: boolean;
}

export interface PermissionResponse {
  id: string;
  name: string;
  resource: string;
  action: string;
  description: string | null;
}

export interface RolePermissionsResponse {
  roleId: string;
  roleName: string;
  permissions: string[];
}

export interface InstitutionProfileResponse {
  name: string;
  slug: string;
  logoUrl: string | null;
  coverImageUrl: string | null;
  description: string | null;
  address: string | null;
  phone: string | null;
  email: string | null;
  website: string | null;
}

export interface InstitutionStatsResponse {
  totalTeachers: number;
  totalStudents: number;
  totalCourses: number;
  totalActiveBatches: number;
  totalEnrollments: number;
}

export interface PublicTeacherResponse {
  id: string;
  firstName: string;
  lastName: string;
  profileImageUrl: string | null;
  bio: string | null;
}

export interface PublicCourseResponse {
  id: string;
  name: string;
  code: string;
  description: string | null;
  thumbnailUrl: string | null;
  durationInMonths: number;
}

export type SubmissionType = "Text" | "File";
export type SubmissionStatus = "Submitted" | "Graded";

export interface AssignmentResponse {
  id: string;
  courseId: string;
  createdByTeacherId: string | null;
  title: string;
  description: string | null;
  maxMarks: number;
  dueDate: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string | null;
}

export interface SubmissionResponse {
  id: string;
  assignmentId: string;
  studentId: string;
  submissionType: SubmissionType;
  textContent: string | null;
  filePath: string | null;
  originalFileName: string | null;
  submittedAt: string;
  isLate: boolean;
  marks: number | null;
  feedback: string | null;
  gradedAt: string | null;
  gradedByTeacherId: string | null;
  status: SubmissionStatus;
}

export interface MyAssignmentResponse extends AssignmentResponse {
  mySubmission: SubmissionResponse | null;
}

export interface RosterEntryResponse {
  studentId: string;
  studentFullName: string;
  submission: SubmissionResponse | null;
}

export interface ProblemDetails {
  status?: number;
  title?: string;
  detail?: string;
  instance?: string;
  traceId?: string;
  errors?: Record<string, string[]>;
}
