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
import { AcademicSessionsPage } from "./features/academic/pages/AcademicSessionsPage";
import { TeacherAssignmentsPage } from "./features/academic/pages/TeacherAssignmentsPage";
import { GuardianDetailsPage } from "./features/guardian/pages/GuardianDetailsPage";
import { GuardianFormPage } from "./features/guardian/pages/GuardianFormPage";
import { GuardiansPage } from "./features/guardian/pages/GuardiansPage";
import { StudentDetailsPage } from "./features/student/pages/StudentDetailsPage";
import { StudentFormPage } from "./features/student/pages/StudentFormPage";
import { StudentsPage } from "./features/student/pages/StudentsPage";

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
          <Route path="/academic/sessions" element={<AcademicSessionsPage />} />
          <Route path="/academic/teacher-assignments" element={<TeacherAssignmentsPage />} />
          <Route path="/students" element={<StudentsPage />} />
          <Route path="/students/new" element={<StudentFormPage />} />
          <Route path="/students/:id" element={<StudentDetailsPage />} />
          <Route path="/students/:id/edit" element={<StudentFormPage />} />
          <Route path="/guardians" element={<GuardiansPage />} />
          <Route path="/guardians/new" element={<GuardianFormPage />} />
          <Route path="/guardians/:id" element={<GuardianDetailsPage />} />
          <Route path="/guardians/:id/edit" element={<GuardianFormPage />} />
          <Route path="/change-password" element={<ChangePasswordPage />} />
        </Route>
      </Routes>
    </AuthProvider>
  );
}

export default App;
