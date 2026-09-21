import { Route, Routes, Navigate } from "react-router-dom";
import { AuthProvider, useAuth } from "./lib/auth";
import { ProtectedRoute } from "./components/ProtectedRoute";
import { DashboardLayout } from "./components/DashboardLayout";
import { Spinner } from "./components/ui";

import LoginPage from "./pages/LoginPage";
import RegisterPage from "./pages/RegisterPage";
import LandingPage from "./pages/LandingPage";
import EnrollmentsPage from "./pages/EnrollmentsPage";

import AdminDashboard from "./pages/admin/AdminDashboard";
import CoursesPage from "./pages/admin/CoursesPage";
import TeachersPage from "./pages/admin/TeachersPage";
import StudentsPage from "./pages/admin/StudentsPage";
import BatchesPage from "./pages/admin/BatchesPage";
import RolesPage from "./pages/admin/RolesPage";
import UsersPage from "./pages/admin/UsersPage";
import AssignmentsPage from "./pages/admin/AssignmentsPage";

import TeacherDashboard from "./pages/teacher/TeacherDashboard";

import StudentDashboard from "./pages/student/StudentDashboard";
import StudentCoursesPage from "./pages/student/StudentCoursesPage";
import StudentTeachersPage from "./pages/student/StudentTeachersPage";
import StudentAssignmentsPage from "./pages/student/StudentAssignmentsPage";
import StudentEnrollmentsPage from "./pages/student/StudentEnrollmentsPage";

function RootRedirect() {
  const { currentUser, isLoading, homeRoute } = useAuth();
  if (isLoading) return <Spinner />;
  return <Navigate to={currentUser ? homeRoute() : "/login"} replace />;
}

export default function App() {
  return (
    <AuthProvider>
      <Routes>
        <Route path="/" element={<LandingPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        {/* Admin & SuperAdmin */}
        <Route element={<ProtectedRoute allowRoles={["SuperAdmin", "Admin"]} />}>
          <Route element={<DashboardLayout role="admin" />}>
            <Route path="/admin" element={<AdminDashboard />} />
            <Route path="/admin/courses" element={<CoursesPage />} />
            <Route path="/admin/teachers" element={<TeachersPage />} />
            <Route path="/admin/students" element={<StudentsPage />} />
            <Route path="/admin/batches" element={<BatchesPage />} />
            <Route path="/admin/enrollments" element={<EnrollmentsPage />} />
            <Route path="/admin/assignments" element={<AssignmentsPage />} />
            <Route path="/admin/users" element={<UsersPage />} />
            <Route path="/admin/roles" element={<RolesPage />} />
          </Route>
        </Route>

        {/* Teacher — reuses the same Courses/Batches/Enrollments/Assignments
            pages as Admin; every button on those pages already hides itself
            based on the current user's permissions, so a Teacher naturally
            only sees "view"/"grade" actions there. */}
        <Route element={<ProtectedRoute allowRoles={["Teacher"]} />}>
          <Route element={<DashboardLayout role="teacher" />}>
            <Route path="/teacher" element={<TeacherDashboard />} />
            <Route path="/teacher/courses" element={<CoursesPage />} />
            <Route path="/teacher/batches" element={<BatchesPage />} />
            <Route path="/teacher/enrollments" element={<EnrollmentsPage />} />
            <Route path="/teacher/assignments" element={<AssignmentsPage />} />
          </Route>
        </Route>

        {/* Student — read-only public catalog pages, plus self-service
            assignment submission via /api/me/assignments. */}
        <Route element={<ProtectedRoute allowRoles={["Student"]} />}>
          <Route element={<DashboardLayout role="student" />}>
            <Route path="/student" element={<StudentDashboard />} />
            <Route path="/student/courses" element={<StudentCoursesPage />} />
            <Route path="/student/teachers" element={<StudentTeachersPage />} />
            <Route path="/student/assignments" element={<StudentAssignmentsPage />} />
            <Route path="/student/enrollments" element={<StudentEnrollmentsPage />} />
          </Route>
        </Route>

        <Route path="*" element={<RootRedirect />} />
      </Routes>
    </AuthProvider>
  );
}
