export interface StudentListItem {
  id?: number;
  fullName?: string | null;
  admissionNumber?: string | null;
  admissionDate?: string | null;
  rollNo?: string | null;
}

export interface TeacherListItem {
  id?: number;
  employeeId?: number | null;
  qualification?: string | null;
  specialization?: string | null;
}

export interface EmployeeListItem {
  id?: number;
  employeeCode?: string | null;
  fullName?: string | null;
  phone?: string | null;
  email?: string | null;
  joiningDate?: string | null;
  isActive?: boolean;
}

export interface InvoiceListItem {
  id?: number;
  invoiceNumber?: string | null;
  studentId?: number;
  studentName?: string | null;
  invoiceDate?: string | null;
  dueDate?: string | null;
  amount?: number | null;
  amountPaid?: number | null;
  balanceDue?: number | null;
  status?: string | null;
}

export interface UpcomingExamItem {
  examId?: number;
  examName?: string | null;
  examTypeName?: string | null;
  nextExamDate?: string | null;
  daysRemaining?: number | null;
  totalSchedules?: number | null;
}

export interface NoticeListItem {
  id?: number;
  title?: string | null;
  description?: string | null;
  publishDate?: string | null;
  expiryDate?: string | null;
  priority?: string | null;
}

export interface AttendanceSummaryDto {
  presentCount?: number;
  absentCount?: number;
  attendanceRate?: number;
  totalStudents?: number;
}

export interface DashboardData {
  students: StudentListItem[];
  teachers: TeacherListItem[];
  employees: EmployeeListItem[];
  invoices: InvoiceListItem[];
  upcomingExams: UpcomingExamItem[];
  notices: NoticeListItem[];
  attendance: AttendanceSummaryDto | null;
}
