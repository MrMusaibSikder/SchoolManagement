import { Routes, Route } from "react-router-dom";
import { LandingPage } from "./landing/pages/LandingPage";
import { AuthProvider } from "./features/auth/context/AuthProvider";
import { ProtectedRoute } from "./features/auth/components/ProtectedRoute";
import { LoginPage } from "./features/auth/pages/LoginPage";
import { RegisterPage } from "./features/auth/pages/RegisterPage";
import { ForgotPasswordPage } from "./features/auth/pages/ForgotPasswordPage";
import { ResetPasswordPage } from "./features/auth/pages/ResetPasswordPage";
import { ChangePasswordPage } from "./features/auth/pages/ChangePasswordPage";
import { AppShell } from "./layouts/AppShell";
import { DashboardPage } from "./features/dashboard/pages/DashboardPage";
import { AcademicManagementPage } from "./features/academic/pages/AcademicManagementPage";
import { AcademicYearsPage } from "./features/academic/pages/AcademicYearsPage";
import { SchoolClassesPage } from "./features/academic/pages/SchoolClassesPage";
import { SectionsPage } from "./features/academic/pages/SectionsPage";
import { SubjectsPage } from "./features/academic/pages/SubjectsPage";
import { TeachersPage } from "./features/academic/pages/TeachersPage";
import { TeacherAssignmentsPage } from "./features/academic/pages/TeacherAssignmentsPage";
import { GuardianDetailsPage } from "./features/guardian/pages/GuardianDetailsPage";
import { GuardianFormPage } from "./features/guardian/pages/GuardianFormPage";
import { GuardiansPage } from "./features/guardian/pages/GuardiansPage";
import { StudentDetailsPage } from "./features/student/pages/StudentDetailsPage";
import { StudentFormPage } from "./features/student/pages/StudentFormPage";
import { StudentsPage } from "./features/student/pages/StudentsPage";
import { EmployeesPage } from "./features/employee/pages/EmployeesPage";
import { EmployeeFormPage } from "./features/employee/pages/EmployeeFormPage";
import { EmployeeDetailsPage } from "./features/employee/pages/EmployeeDetailsPage";
import { AttendanceHubPage } from "./features/attendance/pages/AttendanceHubPage";
import { StudentAttendancePage } from "./features/attendance/pages/StudentAttendancePage";
import { EmployeeAttendancePage } from "./features/attendance/pages/EmployeeAttendancePage";
import { AttendanceReportsPage } from "./features/attendance/pages/AttendanceReportsPage";
import { ExamHubPage } from "./features/exam/pages/ExamHubPage";
import { ExamTypesPage } from "./features/exam/pages/ExamTypesPage";
import { GradeSetupPage } from "./features/exam/pages/GradeSetupPage";
import { ExamWeightSetupPage } from "./features/exam/pages/ExamWeightSetupPage";
import { ExamsPage } from "./features/exam/pages/ExamsPage";
import { ExamDetailsPage } from "./features/exam/pages/ExamDetailsPage";
import { ExamCalendarPage } from "./features/exam/pages/ExamCalendarPage";
import { MarksEntryPage } from "./features/result/pages/MarksEntryPage";
import { ExamResultsPage } from "./features/result/pages/ExamResultsPage";
import { FinalResultsPage } from "./features/result/pages/FinalResultsPage";

/**
 * Application routes.
 *
 * Public (anonymous):
 *   `/`                 → Landing Page (calls /api/public/*)
 *   `/login`            → Sign in
 *   `/register`         → Create an account
 *   `/forgot-password`  → Request a password reset email
 *   `/reset-password`   → Complete a reset using the emailed token
 *
 * Protected (authenticated, rendered inside the AppShell):
 *   `/dashboard`        → Home dashboard after login
 *   `/change-password`  → Update the current user's password
 *
 * The AuthProvider wraps the router so the auth context (session restore,
 * login, logout) is available to any route that needs it.
 */
function App() {
  return (
    <AuthProvider>
      <Routes>
        <Route path="/" element={<LandingPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/forgot-password" element={<ForgotPasswordPage />} />
        <Route path="/reset-password" element={<ResetPasswordPage />} />

        {/* Authenticated application shell */}
        <Route
          element={
            <ProtectedRoute>
              <AppShell />
            </ProtectedRoute>
          }
        >
          <Route path="/dashboard" element={<DashboardPage />} />
          <Route path="/academic" element={<AcademicManagementPage />} />
          <Route path="/academic/years" element={<AcademicYearsPage />} />
          <Route path="/academic/classes" element={<SchoolClassesPage />} />
          <Route path="/academic/sections" element={<SectionsPage />} />
          <Route path="/academic/subjects" element={<SubjectsPage />} />
          <Route path="/academic/teachers" element={<TeachersPage />} />
          <Route path="/academic/teacher-assignments" element={<TeacherAssignmentsPage />} />
          <Route path="/students" element={<StudentsPage />} />
          <Route path="/students/new" element={<StudentFormPage />} />
          <Route path="/students/:id" element={<StudentDetailsPage />} />
          <Route path="/students/:id/edit" element={<StudentFormPage />} />
          <Route path="/employees" element={<EmployeesPage />} />
          <Route path="/employees/new" element={<EmployeeFormPage />} />
          <Route path="/employees/:id" element={<EmployeeDetailsPage />} />
          <Route path="/employees/:id/edit" element={<EmployeeFormPage />} />
          <Route path="/guardians" element={<GuardiansPage />} />
          <Route path="/guardians/new" element={<GuardianFormPage />} />
          <Route path="/guardians/:id" element={<GuardianDetailsPage />} />
          <Route path="/guardians/:id/edit" element={<GuardianFormPage />} />
          <Route path="/attendance" element={<AttendanceHubPage />} />
          <Route path="/attendance/students" element={<StudentAttendancePage />} />
          <Route path="/attendance/employees" element={<EmployeeAttendancePage />} />
          <Route path="/attendance/reports" element={<AttendanceReportsPage />} />
          <Route path="/exams" element={<ExamHubPage />} />
          <Route path="/exams/list" element={<ExamsPage />} />
          <Route path="/exams/types" element={<ExamTypesPage />} />
          <Route path="/exams/grades" element={<GradeSetupPage />} />
          <Route path="/exams/weights" element={<ExamWeightSetupPage />} />
          <Route path="/exams/calendar" element={<ExamCalendarPage />} />
          <Route path="/exams/:id" element={<ExamDetailsPage />} />
          <Route path="/results/marks" element={<MarksEntryPage />} />
          <Route path="/results/exams" element={<ExamResultsPage />} />
          <Route path="/results/exams/:id" element={<ExamResultsPage />} />
          <Route path="/results/final" element={<FinalResultsPage />} />
          <Route path="/change-password" element={<ChangePasswordPage />} />
        </Route>
      </Routes>
    </AuthProvider>
  );
}

export default App;
