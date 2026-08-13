# Auto-Extracted Full API Reference

**Machine-generated** from the actual backend source (`Controllers/*.cs`, `Application/Features/*/DTOs/*.cs`, `Domain/Enums/*.cs`) by parsing attributes, method signatures, and DTO property declarations with a script — not hand-typed. This means routes, HTTP verbs, and field names are extracted directly from source and should be reliable, but the parser is regex-based, not a real C# compiler, so:

- Complex/multi-line attributes or unusual formatting may have been missed — if an endpoint you need isn't listed here, check the controller file directly before assuming it doesn't exist.
- DTO property nullability/collection wrapping is best-effort.
- No behavioral notes (business rules, side effects, which fields are server-assigned) are included here — for **Fee Management** and **Public (Anonymous)** endpoints, use the hand-written `API_REFERENCE.md` instead, which has that detail. This file covers **every other module**.

Re-run the extraction (see `.ai/scripts/` if present, or ask Claude to re-parse the backend zip) any time the backend changes significantly, rather than hand-editing this file out of sync with the source.

## Modules In This File
- [AcademicYearsController](#academicyearscontroller)
- [AttendanceReportController](#attendancereportcontroller)
- [AuthController](#authcontroller)
- [ClassSubjectsController](#classsubjectscontroller)
- [CurrentUserController](#currentusercontroller)
- [DesignationsController](#designationscontroller)
- [EmployeeAttendanceController](#employeeattendancecontroller)
- [EmployeeSalaryController](#employeesalarycontroller)
- [EmployeesController](#employeescontroller)
- [ExamController](#examcontroller)
- [ExamResultController](#examresultcontroller)
- [ExamScheduleController](#examschedulecontroller)
- [ExamTypeController](#examtypecontroller)
- [ExamWeightSetupController](#examweightsetupcontroller)
- [FinalResultController](#finalresultcontroller)
- [GradeSetupController](#gradesetupcontroller)
- [GuardiansController](#guardianscontroller)
- [MarkEntryController](#markentrycontroller)
- [NoticeController](#noticecontroller)
- [PermissionController](#permissioncontroller)
- [ProgressReportController](#progressreportcontroller)
- [ResultAuditLogController](#resultauditlogcontroller)
- [RoleController](#rolecontroller)
- [SchoolClassesController](#schoolclassescontroller)
- [SchoolController](#schoolcontroller)
- [SectionsController](#sectionscontroller)
- [SmsLogController](#smslogcontroller)
- [SmsTemplateController](#smstemplatecontroller)
- [StudentAttendanceController](#studentattendancecontroller)
- [StudentsController](#studentscontroller)
- [SubjectTeachersController](#subjectteacherscontroller)
- [SubjectsController](#subjectscontroller)
- [TeachersController](#teacherscontroller)
- [TranscriptController](#transcriptcontroller)
- [UserController](#usercontroller)

---


## AcademicYearsController

### `GET /api/AcademicYears`  
*Permission: `AcademicYearView`*

**Response:**

**`AcademicYearDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `StartDate`: `DateTime`
  - `EndDate`: `DateTime`
  - `IsCurrent`: `bool`

### `GET /api/AcademicYears/{id:int}`  
*Permission: `AcademicYearView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`AcademicYearDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `StartDate`: `DateTime`
  - `EndDate`: `DateTime`
  - `IsCurrent`: `bool`

### `POST /api/AcademicYears`  
*Permission: `AcademicYearCreate`*

**Request body** (`request`):

**`CreateAcademicYearDto`**
  - `Name`: `string`
  - `StartDate`: `DateTime`
  - `EndDate`: `DateTime`
  - `IsCurrent`: `bool`

**Response:**

**`AcademicYearDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `StartDate`: `DateTime`
  - `EndDate`: `DateTime`
  - `IsCurrent`: `bool`

### `PUT /api/AcademicYears/{id:int}`  
*Permission: `AcademicYearEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateAcademicYearDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `StartDate`: `DateTime`
  - `EndDate`: `DateTime`
  - `IsCurrent`: `bool`

**Response:**

**`AcademicYearDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `StartDate`: `DateTime`
  - `EndDate`: `DateTime`
  - `IsCurrent`: `bool`

### `DELETE /api/AcademicYears/{id:int}`  
*Permission: `AcademicYearDelete`*

**Parameters:**
- `id`: `int` (route)


---


## AttendanceReportController

### `GET /api/AttendanceReport/dashboard`  
*Permission: `AttendanceReportView`*

### `GET /api/AttendanceReport/student/{studentId}`  
*Permission: `AttendanceReportView`*

**Parameters:**
- `studentId`: `int` (route)
- `fromDate`: `DateTime?` (?)
- `toDate`: `DateTime?` (?)

### `GET /api/AttendanceReport/student/{studentId}/monthly`  
*Permission: `AttendanceReportView`*

**Parameters:**
- `studentId`: `int` (route)
- `month`: `int` (route)
- `year`: `int` (route)

### `GET /api/AttendanceReport/class-summary`  
*Permission: `AttendanceReportView`*

**Parameters:**
- `classId`: `int` (route)
- `sectionId`: `int` (route)
- `attendanceDate`: `DateTime` (?)

### `GET /api/AttendanceReport/teacher-dashboard`  
*Permission: `AttendanceReportView`*

**Parameters:**
- `classId`: `int` (route)
- `sectionId`: `int` (route)
- `attendanceDate`: `DateTime` (?)

### `GET /api/AttendanceReport/admin-dashboard`  
*Permission: `AttendanceReportView`*

**Parameters:**
- `fromDate`: `DateTime` (?)
- `toDate`: `DateTime` (?)

### `GET /api/AttendanceReport/trend`  
*Permission: `AttendanceReportView`*

**Parameters:**
- `fromDate`: `DateTime` (?)
- `toDate`: `DateTime` (?)

### `GET /api/AttendanceReport/student/{studentId}/percentage`  
*Permission: `AttendanceReportView`*

**Parameters:**
- `studentId`: `int` (route)
- `fromDate`: `DateTime?` (?)
- `toDate`: `DateTime?` (?)

### `GET /api/AttendanceReport/class-percentage`  
*Permission: `AttendanceReportView`*

**Parameters:**
- `classId`: `int` (route)
- `sectionId`: `int` (route)
- `fromDate`: `DateTime?` (?)
- `toDate`: `DateTime?` (?)


---


## AuthController

### `POST /api/Auth/login`  
*⚠️ no [PermissionAuthorize]/[Authorize]/[AllowAnonymous] detected — verify manually*

**Request body** (`request`):

**`LoginRequestDto`**
  - `UsernameOrEmail`: `string`
  - `Password`: `string`

### `POST /api/Auth/register`  
*⚠️ no [PermissionAuthorize]/[Authorize]/[AllowAnonymous] detected — verify manually*

**Request body** (`request`):

**`RegisterRequestDto`**
  - `Username`: `string`
  - `Email`: `string`
  - `Password`: `string`
  - `RoleName`: `string`

### `POST /api/Auth/refresh-token`  
*⚠️ no [PermissionAuthorize]/[Authorize]/[AllowAnonymous] detected — verify manually*

**Request body** (`request`):

**`RefreshTokenRequestDto`**
  - `RefreshToken`: `string`

### `POST /api/Auth/logout`  
*Authenticated (any logged-in user, no specific permission)*

**Request body** (`request`):

**`LogoutRequestDto`**
  - `RefreshToken`: `string` | null

### `POST /api/Auth/change-password`  
*Authenticated (any logged-in user, no specific permission)*

**Request body** (`request`):

**`ChangePasswordDto`**
  - `CurrentPassword`: `string`
  - `NewPassword`: `string`
  - `ConfirmNewPassword`: `string`

### `POST /api/Auth/forgot-password`  
*⚠️ no [PermissionAuthorize]/[Authorize]/[AllowAnonymous] detected — verify manually*

**Request body** (`request`):

**`ForgotPasswordDto`**
  - `Email`: `string`

### `POST /api/Auth/reset-password`  
*⚠️ no [PermissionAuthorize]/[Authorize]/[AllowAnonymous] detected — verify manually*

**Request body** (`request`):

**`ResetPasswordDto`**
  - `Token`: `string`
  - `NewPassword`: `string`
  - `ConfirmNewPassword`: `string`


---


## ClassSubjectsController

### `GET /api/ClassSubjects`  
*Permission: `ClassSubjectView`*

**Response:**

**`ClassSubjectDto`**
  - `ClassId`: `int`
  - `SubjectId`: `int`
  - `IsOptional`: `bool`

### `GET /api/ClassSubjects/{classId:int}/{subjectId:int}`  
*Permission: `ClassSubjectView`*

**Parameters:**
- `classId`: `int` (route)
- `subjectId`: `int` (route)

**Response:**

**`ClassSubjectDto`**
  - `ClassId`: `int`
  - `SubjectId`: `int`
  - `IsOptional`: `bool`

### `POST /api/ClassSubjects`  
*Permission: `ClassSubjectAssign`*

**Request body** (`request`):

**`ClassSubjectDto`**
  - `ClassId`: `int`
  - `SubjectId`: `int`
  - `IsOptional`: `bool`

**Response:**

**`ClassSubjectDto`**
  - `ClassId`: `int`
  - `SubjectId`: `int`
  - `IsOptional`: `bool`

### `DELETE /api/ClassSubjects/{classId:int}/{subjectId:int}`  
*Permission: `ClassSubjectRemove`*

**Parameters:**
- `classId`: `int` (route)
- `subjectId`: `int` (route)

### `PATCH /api/ClassSubjects/{classId:int}/{subjectId:int}/optional`  
*Permission: `ClassSubjectAssign`*

**Parameters:**
- `classId`: `int` (route)
- `subjectId`: `int` (route)
- `isOptional`: `bool` (Query)

**Response:**

**`ClassSubjectDto`**
  - `ClassId`: `int`
  - `SubjectId`: `int`
  - `IsOptional`: `bool`


---


## CurrentUserController

### `GET /api/CurrentUser/profile`  
*⚠️ no [PermissionAuthorize]/[Authorize]/[AllowAnonymous] detected — verify manually*

**Response:**

**`CurrentUserDto`**
  - `UserId`: `int`
  - `Username`: `string`
  - `Email`: `string`
  - `IsActive`: `bool`
  - `Roles`: `string`[]
  - `Permissions`: `string`[]

### `GET /api/CurrentUser/roles`  
*⚠️ no [PermissionAuthorize]/[Authorize]/[AllowAnonymous] detected — verify manually*

**Response:**

**`RoleDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Description`: `string` | null

### `GET /api/CurrentUser/permissions`  
*⚠️ no [PermissionAuthorize]/[Authorize]/[AllowAnonymous] detected — verify manually*

**Response:**

**`PermissionDto`**
  - `Id`: `int`
  - `Name`: `string`


---


## DesignationsController

### `GET /api/Designations`  
*Permission: `DesignationView`*

**Response:**

**`DesignationDto`**
  - `Id`: `int`
  - `Name`: `string`

### `GET /api/Designations/{id:int}`  
*Permission: `DesignationView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`DesignationDto`**
  - `Id`: `int`
  - `Name`: `string`

### `POST /api/Designations`  
*Permission: `DesignationCreate`*

**Request body** (`request`):

**`CreateDesignationDto`**
  - `Name`: `string`

**Response:**

**`DesignationDto`**
  - `Id`: `int`
  - `Name`: `string`

### `PUT /api/Designations/{id:int}`  
*Permission: `DesignationEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateDesignationDto`**
  - `Id`: `int`
  - `Name`: `string`

**Response:**

**`DesignationDto`**
  - `Id`: `int`
  - `Name`: `string`

### `DELETE /api/Designations/{id:int}`  
*Permission: `DesignationDelete`*

**Parameters:**
- `id`: `int` (route)


---


## EmployeeAttendanceController

### `GET /api/EmployeeAttendance`  
*Permission: `EmployeeAttendanceView`*

**Response:**

**`EmployeeAttendanceDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `AttendanceDate`: `DateTime`
  - `CheckIn`: `DateTime` | null
  - `CheckOut`: `DateTime` | null
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)

### `GET /api/EmployeeAttendance/{id:int}`  
*Permission: `EmployeeAttendanceView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`EmployeeAttendanceDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `AttendanceDate`: `DateTime`
  - `CheckIn`: `DateTime` | null
  - `CheckOut`: `DateTime` | null
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)

### `POST /api/EmployeeAttendance`  
*Permission: `EmployeeAttendanceCreate`*

**Request body** (`request`):

**`CreateEmployeeAttendanceDto`**
  - `EmployeeId`: `int`
  - `AttendanceDate`: `DateTime`
  - `CheckIn`: `DateTime` | null
  - `CheckOut`: `DateTime` | null
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)

**Response:**

**`EmployeeAttendanceDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `AttendanceDate`: `DateTime`
  - `CheckIn`: `DateTime` | null
  - `CheckOut`: `DateTime` | null
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)

### `POST /api/EmployeeAttendance/bulk`  
*Permission: `EmployeeAttendanceCreate`*

**Request body** (`request`):

**`BulkEmployeeAttendanceDto`**
  - `AttendanceDate`: `DateTime`
  - `Attendance`: `EmployeeAttendanceItemDto`[]

### `GET /api/EmployeeAttendance/by-date`  
*Permission: `EmployeeAttendanceView`*

**Parameters:**
- `attendanceDate`: `DateTime` (Query)

**Response:**

**`EmployeeAttendanceDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `AttendanceDate`: `DateTime`
  - `CheckIn`: `DateTime` | null
  - `CheckOut`: `DateTime` | null
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)

### `GET /api/EmployeeAttendance/employee/{employeeId:int}/history`  
*Permission: `EmployeeAttendanceView`*

**Parameters:**
- `employeeId`: `int` (route)
- `fromDate`: `DateTime?` (Query)
- `toDate`: `DateTime?` (Query)

**Response:**

**`EmployeeAttendanceDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `AttendanceDate`: `DateTime`
  - `CheckIn`: `DateTime` | null
  - `CheckOut`: `DateTime` | null
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)

### `PUT /api/EmployeeAttendance/{id:int}`  
*Permission: `EmployeeAttendanceEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateEmployeeAttendanceDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `AttendanceDate`: `DateTime`
  - `CheckIn`: `DateTime` | null
  - `CheckOut`: `DateTime` | null
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)

**Response:**

**`EmployeeAttendanceDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `AttendanceDate`: `DateTime`
  - `CheckIn`: `DateTime` | null
  - `CheckOut`: `DateTime` | null
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)

### `DELETE /api/EmployeeAttendance/{id:int}`  
*Permission: `EmployeeAttendanceDelete`*

**Parameters:**
- `id`: `int` (route)


---


## EmployeeSalaryController

### `GET /api/EmployeeSalary`  
*Permission: `EmployeeSalaryView`*

**Response:**

**`EmployeeSalaryDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `BasicSalary`: `decimal`
  - `EffectiveFrom`: `DateTime`

### `GET /api/EmployeeSalary/{id:int}`  
*Permission: `EmployeeSalaryView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`EmployeeSalaryDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `BasicSalary`: `decimal`
  - `EffectiveFrom`: `DateTime`

### `POST /api/EmployeeSalary`  
*Permission: `EmployeeSalaryCreate`*

**Request body** (`request`):

**`CreateEmployeeSalaryDto`**
  - `EmployeeId`: `int`
  - `BasicSalary`: `decimal`
  - `EffectiveFrom`: `DateTime`

**Response:**

**`EmployeeSalaryDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `BasicSalary`: `decimal`
  - `EffectiveFrom`: `DateTime`

### `PUT /api/EmployeeSalary/{id:int}`  
*Permission: `EmployeeSalaryUpdate`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateEmployeeSalaryDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `BasicSalary`: `decimal`
  - `EffectiveFrom`: `DateTime`

**Response:**

**`EmployeeSalaryDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `BasicSalary`: `decimal`
  - `EffectiveFrom`: `DateTime`

### `DELETE /api/EmployeeSalary/{id:int}`  
*Permission: `EmployeeSalaryDelete`*

**Parameters:**
- `id`: `int` (route)


---


## EmployeesController

### `GET /api/Employees`  
*Permission: `EmployeeView`*

**Response:**

**`EmployeeDto`**
  - `Id`: `int`
  - `EmployeeCode`: `string`
  - `FullName`: `string`
  - `Phone`: `string`
  - `Email`: `string` | null
  - `JoiningDate`: `DateTime`
  - `IsActive`: `bool`
  - `EmployeePhoto`: `string` | null
  - `DesignationId`: `int`
  - `UserId`: `int` | null

### `GET /api/Employees/{id:int}`  
*Permission: `EmployeeView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`EmployeeDto`**
  - `Id`: `int`
  - `EmployeeCode`: `string`
  - `FullName`: `string`
  - `Phone`: `string`
  - `Email`: `string` | null
  - `JoiningDate`: `DateTime`
  - `IsActive`: `bool`
  - `EmployeePhoto`: `string` | null
  - `DesignationId`: `int`
  - `UserId`: `int` | null

### `POST /api/Employees`  
*Permission: `EmployeeCreate`*

**Form fields (multipart/form-data)** (`request`):

**`CreateEmployeeDto`**
  - `EmployeeCode`: `string`
  - `FullName`: `string`
  - `Phone`: `string`
  - `Email`: `string` | null
  - `JoiningDate`: `DateTime`
  - `IsActive`: `bool`
  - `DesignationId`: `int`
  - `EmployeePhotoFile`: `IFormFile` | null
  - `UserId`: `int` | null

**Response:**

**`EmployeeDto`**
  - `Id`: `int`
  - `EmployeeCode`: `string`
  - `FullName`: `string`
  - `Phone`: `string`
  - `Email`: `string` | null
  - `JoiningDate`: `DateTime`
  - `IsActive`: `bool`
  - `EmployeePhoto`: `string` | null
  - `DesignationId`: `int`
  - `UserId`: `int` | null

### `PUT /api/Employees/{id:int}`  
*Permission: `EmployeeEdit`*

**Parameters:**
- `id`: `int` (route)

**Form fields (multipart/form-data)** (`request`):

**`UpdateEmployeeDto`**
  - `Id`: `int`
  - `EmployeeCode`: `string`
  - `FullName`: `string`
  - `Phone`: `string`
  - `Email`: `string` | null
  - `JoiningDate`: `DateTime`
  - `IsActive`: `bool`
  - `EmployeePhotoFile`: `IFormFile` | null
  - `DesignationId`: `int`
  - `UserId`: `int` | null

**Response:**

**`EmployeeDto`**
  - `Id`: `int`
  - `EmployeeCode`: `string`
  - `FullName`: `string`
  - `Phone`: `string`
  - `Email`: `string` | null
  - `JoiningDate`: `DateTime`
  - `IsActive`: `bool`
  - `EmployeePhoto`: `string` | null
  - `DesignationId`: `int`
  - `UserId`: `int` | null

### `DELETE /api/Employees/{id:int}`  
*Permission: `EmployeeDelete`*

**Parameters:**
- `id`: `int` (route)


---


## ExamController

### `GET /api/Exam`  
*Permission: `ExamView`*

**Response:**

**`ExamDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `ExamTypeId`: `int`
  - `ExamTypeName`: `string` | null
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string` | null
  - `Status`: `ExamStatus` enum (Draft=1/Published=2/Completed=3/Cancelled=4)

### `GET /api/Exam/{id:int}`  
*Permission: `ExamView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`ExamDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `ExamTypeId`: `int`
  - `ExamTypeName`: `string` | null
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string` | null
  - `Status`: `ExamStatus` enum (Draft=1/Published=2/Completed=3/Cancelled=4)

### `GET /api/Exam/{id:int}/details`  
*Permission: `ExamView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`ExamDetailsDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `ExamTypeId`: `int`
  - `ExamTypeName`: `string`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `Status`: `ExamStatus` enum (Draft=1/Published=2/Completed=3/Cancelled=4)
  - `TotalSchedules`: `int`
  - `Schedules`: `ExamScheduleDto`[]

### `GET /api/Exam/{id:int}/statistics`  
*Permission: `ExamView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`ExamStatisticsDto`**
  - `ExamId`: `int`
  - `ExamName`: `string`
  - `Status`: `ExamStatus` enum (Draft=1/Published=2/Completed=3/Cancelled=4)
  - `TotalSchedules`: `int`
  - `TotalSubjects`: `int`
  - `TotalClasses`: `int`
  - `StartDate`: `DateTime` | null
  - `EndDate`: `DateTime` | null
  - `DurationInDays`: `int`

### `POST /api/Exam`  
*Permission: `ExamCreate`*

**Request body** (`request`):

**`CreateExamDto`**
  - `Name`: `string`
  - `ExamTypeId`: `int`
  - `AcademicYearId`: `int`

**Response:**

**`ExamDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `ExamTypeId`: `int`
  - `ExamTypeName`: `string` | null
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string` | null
  - `Status`: `ExamStatus` enum (Draft=1/Published=2/Completed=3/Cancelled=4)

### `PUT /api/Exam/{id:int}`  
*Permission: `ExamEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateExamDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `ExamTypeId`: `int`
  - `AcademicYearId`: `int`

**Response:**

**`ExamDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `ExamTypeId`: `int`
  - `ExamTypeName`: `string` | null
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string` | null
  - `Status`: `ExamStatus` enum (Draft=1/Published=2/Completed=3/Cancelled=4)

### `DELETE /api/Exam/{id:int}`  
*Permission: `ExamDelete`*

**Parameters:**
- `id`: `int` (route)

### `POST /api/Exam/{id:int}/publish`  
*Permission: `ExamPublish`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`ExamDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `ExamTypeId`: `int`
  - `ExamTypeName`: `string` | null
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string` | null
  - `Status`: `ExamStatus` enum (Draft=1/Published=2/Completed=3/Cancelled=4)

### `POST /api/Exam/{id:int}/complete`  
*Permission: `ExamComplete`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`ExamDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `ExamTypeId`: `int`
  - `ExamTypeName`: `string` | null
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string` | null
  - `Status`: `ExamStatus` enum (Draft=1/Published=2/Completed=3/Cancelled=4)

### `POST /api/Exam/{id:int}/cancel`  
*Permission: `ExamCancel`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`ExamDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `ExamTypeId`: `int`
  - `ExamTypeName`: `string` | null
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string` | null
  - `Status`: `ExamStatus` enum (Draft=1/Published=2/Completed=3/Cancelled=4)

### `POST /api/Exam/{id:int}/reopen`  
*Permission: `ExamPublish`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`ExamDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `ExamTypeId`: `int`
  - `ExamTypeName`: `string` | null
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string` | null
  - `Status`: `ExamStatus` enum (Draft=1/Published=2/Completed=3/Cancelled=4)

### `GET /api/Exam/dashboard`  
*Permission: `ExamView`*

**Response:**

**`ExamDashboardDto`**
  - `TotalExams`: `int`
  - `DraftExams`: `int`
  - `PublishedExams`: `int`
  - `CompletedExams`: `int`
  - `CancelledExams`: `int`
  - `UpcomingExamsCount`: `int`
  - `UpcomingExams`: `UpcomingExamDto`[]
  - `RecentExams`: `ExamSummaryDto`[]

### `GET /api/Exam/upcoming`  
*Permission: `ExamView`*

**Parameters:**
- `count`: `int` (Query)

**Response:**

**`UpcomingExamDto`**
  - `ExamId`: `int`
  - `ExamName`: `string`
  - `ExamTypeName`: `string`
  - `NextExamDate`: `DateTime`
  - `DaysRemaining`: `int`
  - `TotalSchedules`: `int`

### `GET /api/Exam/calendar`  
*Permission: `ExamView`*

**Parameters:**
- `fromDate`: `DateTime` (Query)
- `toDate`: `DateTime` (Query)
- `classId`: `int?` (Query)

**Response:**

**`ExamCalendarDto`**
  - `ScheduleId`: `int`
  - `ExamId`: `int`
  - `ExamName`: `string`
  - `SubjectId`: `int`
  - `SubjectName`: `string`
  - `ClassId`: `int`
  - `ClassName`: `string`
  - `ExamDate`: `DateTime`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

### `GET /api/Exam/{id:int}/routine`  
*Permission: `ExamView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`ExamRoutineDto`**
  - `ExamId`: `int`
  - `ExamName`: `string`
  - `ExamTypeName`: `string`
  - `AcademicYearName`: `string`
  - `Status`: `ExamStatus` enum (Draft=1/Published=2/Completed=3/Cancelled=4)
  - `Schedules`: `ExamScheduleDto`[]

### `GET /api/Exam/{id:int}/routine/class/{classId:int}`  
*Permission: `ExamView`*

**Parameters:**
- `id`: `int` (route)
- `classId`: `int` (route)

**Response:**

**`ClassRoutineDto`**
  - `ExamId`: `int`
  - `ExamName`: `string`
  - `ClassId`: `int`
  - `ClassName`: `string`
  - `Schedules`: `ExamScheduleDto`[]

### `GET /api/Exam/{id:int}/routine/student/{studentId:int}`  
*Permission: `ExamView`*

**Parameters:**
- `id`: `int` (route)
- `studentId`: `int` (route)

**Response:**

**`StudentRoutineDto`**
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `ClassId`: `int`
  - `ClassName`: `string`
  - `ExamId`: `int`
  - `ExamName`: `string`
  - `Schedules`: `ExamScheduleDto`[]

### `GET /api/Exam/{id:int}/routine/teacher/{teacherId:int}`  
*Permission: `ExamView`*

**Parameters:**
- `id`: `int` (route)
- `teacherId`: `int` (route)

**Response:**

**`TeacherRoutineDto`**
  - `TeacherId`: `int`
  - `TeacherName`: `string`
  - `ExamId`: `int` | null
  - `ExamName`: `string` | null
  - `Schedules`: `ExamScheduleDto`[]


---


## ExamResultController

### `POST /api/ExamResult/exam/{examId:int}/calculate`  
*Permission: `ResultCalculate`*

**Parameters:**
- `examId`: `int` (route)

**Response:**

**`ExamResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `RollNo`: `string`
  - `ClassName`: `string`
  - `SectionName`: `string`
  - `ExamId`: `int`
  - `ExamName`: `string`
  - `TotalMarks`: `decimal`
  - `TotalFullMarks`: `decimal`
  - `Percentage`: `decimal`
  - `GPA`: `decimal`
  - `Grade`: `string`
  - `IsPassed`: `bool`
  - `MeritPosition`: `int` | null
  - `ClassPosition`: `int` | null
  - `SectionPosition`: `int` | null
  - `IsPublished`: `bool`
  - `PublishedAt`: `DateTime` | null
  - `TeacherRemarks`: `string` | null
  - `GuardianRemarks`: `string` | null

### `POST /api/ExamResult/exam/{examId:int}/publish`  
*Permission: `ResultPublish`*

**Parameters:**
- `examId`: `int` (route)

**Response:**

**`ExamResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `RollNo`: `string`
  - `ClassName`: `string`
  - `SectionName`: `string`
  - `ExamId`: `int`
  - `ExamName`: `string`
  - `TotalMarks`: `decimal`
  - `TotalFullMarks`: `decimal`
  - `Percentage`: `decimal`
  - `GPA`: `decimal`
  - `Grade`: `string`
  - `IsPassed`: `bool`
  - `MeritPosition`: `int` | null
  - `ClassPosition`: `int` | null
  - `SectionPosition`: `int` | null
  - `IsPublished`: `bool`
  - `PublishedAt`: `DateTime` | null
  - `TeacherRemarks`: `string` | null
  - `GuardianRemarks`: `string` | null

### `POST /api/ExamResult/exam/{examId:int}/unpublish`  
*Permission: `ResultUnlock`*

**Parameters:**
- `examId`: `int` (route)

### `GET /api/ExamResult/exam/{examId:int}`  
*Permission: `ResultView`*

**Parameters:**
- `examId`: `int` (route)
- `classId`: `int?` (Query)

**Response:**

**`ExamResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `RollNo`: `string`
  - `ClassName`: `string`
  - `SectionName`: `string`
  - `ExamId`: `int`
  - `ExamName`: `string`
  - `TotalMarks`: `decimal`
  - `TotalFullMarks`: `decimal`
  - `Percentage`: `decimal`
  - `GPA`: `decimal`
  - `Grade`: `string`
  - `IsPassed`: `bool`
  - `MeritPosition`: `int` | null
  - `ClassPosition`: `int` | null
  - `SectionPosition`: `int` | null
  - `IsPublished`: `bool`
  - `PublishedAt`: `DateTime` | null
  - `TeacherRemarks`: `string` | null
  - `GuardianRemarks`: `string` | null

### `GET /api/ExamResult/student/{studentId:int}/exam/{examId:int}`  
*Permission: `ResultView`*

**Parameters:**
- `studentId`: `int` (route)
- `examId`: `int` (route)

**Response:**

**`StudentExamResultDto`**
  - `Summary`: `ExamResultDto`
  - `Subjects`: `ExamResultDetailDto`[]

### `GET /api/ExamResult/exam/{examId:int}/tabulation/class/{classId:int}`  
*Permission: `ResultView`*

**Parameters:**
- `examId`: `int` (route)
- `classId`: `int` (route)

**Response:**

**`TabulationSheetDto`**
  - `ExamId`: `int`
  - `ExamName`: `string`
  - `ClassId`: `int`
  - `ClassName`: `string`
  - `SubjectNames`: `string`[]
  - `Rows`: `TabulationRowDto`[]

### `GET /api/ExamResult/exam/{examId:int}/merit/class/{classId:int}`  
*Permission: `ResultView`*

**Parameters:**
- `examId`: `int` (route)
- `classId`: `int` (route)

### `GET /api/ExamResult/exam/{examId:int}/merit/section/{sectionId:int}`  
*Permission: `ResultView`*

**Parameters:**
- `examId`: `int` (route)
- `sectionId`: `int` (route)

### `GET /api/ExamResult/exam/{examId:int}/failed`  
*Permission: `ResultView`*

**Parameters:**
- `examId`: `int` (route)
- `classId`: `int?` (Query)

### `GET /api/ExamResult/exam/{examId:int}/top`  
*Permission: `ResultView`*

**Parameters:**
- `examId`: `int` (route)
- `classId`: `int?` (Query)
- `count`: `int` (Query)

### `GET /api/ExamResult/exam/{examId:int}/subject-statistics`  
*Permission: `ResultView`*

**Parameters:**
- `examId`: `int` (route)

**Response:**

**`SubjectStatisticsDto`**
  - `SubjectId`: `int`
  - `SubjectName`: `string`
  - `TotalStudents`: `int`
  - `HighestMarks`: `decimal`
  - `LowestMarks`: `decimal`
  - `AverageMarks`: `decimal`
  - `PassCount`: `int`
  - `FailCount`: `int`
  - `PassRate`: `decimal`

### `GET /api/ExamResult/exam/{examId:int}/grade-distribution`  
*Permission: `ResultView`*

**Parameters:**
- `examId`: `int` (route)

**Response:**

**`GradeDistributionItemDto`**
  - `Grade`: `string`
  - `Count`: `int`
  - `Percentage`: `decimal`

### `GET /api/ExamResult/exam/{examId:int}/dashboard`  
*Permission: `ResultView`*

**Parameters:**
- `examId`: `int` (route)

**Response:**

**`ExamResultDashboardDto`**
  - `ExamId`: `int`
  - `ExamName`: `string`
  - `TotalStudents`: `int`
  - `AppearedStudents`: `int`
  - `AbsentStudents`: `int`
  - `TotalScheduleCount`: `int`
  - `FullySubmittedScheduleCount`: `int`
  - `CompletionPercentage`: `decimal`
  - `IsResultPublished`: `bool`
  - `PublishedResultCount`: `int`
  - `PendingResultCount`: `int`
  - `SubjectStatistics`: `SubjectStatisticsDto`[]

### `PATCH /api/ExamResult/student/{studentId:int}/exam/{examId:int}/remarks`  
*Permission: `ResultPublish`*

**Parameters:**
- `studentId`: `int` (route)
- `examId`: `int` (route)
- `teacherRemarks`: `string?` (Query)
- `guardianRemarks`: `string?` (Query)

**Response:**

**`ExamResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `RollNo`: `string`
  - `ClassName`: `string`
  - `SectionName`: `string`
  - `ExamId`: `int`
  - `ExamName`: `string`
  - `TotalMarks`: `decimal`
  - `TotalFullMarks`: `decimal`
  - `Percentage`: `decimal`
  - `GPA`: `decimal`
  - `Grade`: `string`
  - `IsPassed`: `bool`
  - `MeritPosition`: `int` | null
  - `ClassPosition`: `int` | null
  - `SectionPosition`: `int` | null
  - `IsPublished`: `bool`
  - `PublishedAt`: `DateTime` | null
  - `TeacherRemarks`: `string` | null
  - `GuardianRemarks`: `string` | null


---


## ExamScheduleController

### `GET /api/ExamSchedule`  
*Permission: `ExamScheduleView`*

**Response:**

**`ExamScheduleDto`**
  - `Id`: `int`
  - `ExamId`: `int`
  - `ExamName`: `string` | null
  - `ClassId`: `int`
  - `ClassName`: `string` | null
  - `SubjectId`: `int`
  - `SubjectName`: `string` | null
  - `ExamDate`: `DateTime`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

### `GET /api/ExamSchedule/{id:int}`  
*Permission: `ExamScheduleView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`ExamScheduleDto`**
  - `Id`: `int`
  - `ExamId`: `int`
  - `ExamName`: `string` | null
  - `ClassId`: `int`
  - `ClassName`: `string` | null
  - `SubjectId`: `int`
  - `SubjectName`: `string` | null
  - `ExamDate`: `DateTime`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

### `GET /api/ExamSchedule/exam/{examId:int}`  
*Permission: `ExamScheduleView`*

**Parameters:**
- `examId`: `int` (route)

**Response:**

**`ExamScheduleDto`**
  - `Id`: `int`
  - `ExamId`: `int`
  - `ExamName`: `string` | null
  - `ClassId`: `int`
  - `ClassName`: `string` | null
  - `SubjectId`: `int`
  - `SubjectName`: `string` | null
  - `ExamDate`: `DateTime`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

### `GET /api/ExamSchedule/class/{classId:int}`  
*Permission: `ExamScheduleView`*

**Parameters:**
- `classId`: `int` (route)
- `examId`: `int?` (Query)

**Response:**

**`ExamScheduleDto`**
  - `Id`: `int`
  - `ExamId`: `int`
  - `ExamName`: `string` | null
  - `ClassId`: `int`
  - `ClassName`: `string` | null
  - `SubjectId`: `int`
  - `SubjectName`: `string` | null
  - `ExamDate`: `DateTime`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

### `GET /api/ExamSchedule/teacher/{teacherId:int}`  
*Permission: `ExamScheduleView`*

**Parameters:**
- `teacherId`: `int` (route)
- `examId`: `int?` (Query)

**Response:**

**`ExamScheduleDto`**
  - `Id`: `int`
  - `ExamId`: `int`
  - `ExamName`: `string` | null
  - `ClassId`: `int`
  - `ClassName`: `string` | null
  - `SubjectId`: `int`
  - `SubjectName`: `string` | null
  - `ExamDate`: `DateTime`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

### `POST /api/ExamSchedule`  
*Permission: `ExamScheduleCreate`*

**Request body** (`request`):

**`CreateExamScheduleDto`**
  - `ExamId`: `int`
  - `ClassId`: `int`
  - `SubjectId`: `int`
  - `ExamDate`: `DateTime`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

**Response:**

**`ExamScheduleDto`**
  - `Id`: `int`
  - `ExamId`: `int`
  - `ExamName`: `string` | null
  - `ClassId`: `int`
  - `ClassName`: `string` | null
  - `SubjectId`: `int`
  - `SubjectName`: `string` | null
  - `ExamDate`: `DateTime`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

### `PUT /api/ExamSchedule/{id:int}`  
*Permission: `ExamScheduleEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateExamScheduleDto`**
  - `Id`: `int`
  - `ExamId`: `int`
  - `ClassId`: `int`
  - `SubjectId`: `int`
  - `ExamDate`: `DateTime`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

**Response:**

**`ExamScheduleDto`**
  - `Id`: `int`
  - `ExamId`: `int`
  - `ExamName`: `string` | null
  - `ClassId`: `int`
  - `ClassName`: `string` | null
  - `SubjectId`: `int`
  - `SubjectName`: `string` | null
  - `ExamDate`: `DateTime`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

### `DELETE /api/ExamSchedule/{id:int}`  
*Permission: `ExamScheduleDelete`*

**Parameters:**
- `id`: `int` (route)


---


## ExamTypeController

### `GET /api/ExamType`  
*Permission: `ExamTypeView`*

**Response:**

**`ExamTypeDto`**
  - `Id`: `int`
  - `Name`: `string`

### `GET /api/ExamType/{id:int}`  
*Permission: `ExamTypeView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`ExamTypeDto`**
  - `Id`: `int`
  - `Name`: `string`

### `POST /api/ExamType`  
*Permission: `ExamTypeCreate`*

**Request body** (`request`):

**`CreateExamTypeDto`**
  - `Name`: `string`

**Response:**

**`ExamTypeDto`**
  - `Id`: `int`
  - `Name`: `string`

### `PUT /api/ExamType/{id:int}`  
*Permission: `ExamTypeEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateExamTypeDto`**
  - `Id`: `int`
  - `Name`: `string`

**Response:**

**`ExamTypeDto`**
  - `Id`: `int`
  - `Name`: `string`

### `DELETE /api/ExamType/{id:int}`  
*Permission: `ExamTypeDelete`*

**Parameters:**
- `id`: `int` (route)


---


## ExamWeightSetupController

### `GET /api/ExamWeightSetup`  
*Permission: `WeightSetupView`*

**Response:**

**`ExamWeightSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `Name`: `string`
  - `IsActive`: `bool`
  - `TotalWeight`: `decimal`
  - `Items`: `ExamWeightItemDto`[]

### `GET /api/ExamWeightSetup/{id:int}`  
*Permission: `WeightSetupView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`ExamWeightSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `Name`: `string`
  - `IsActive`: `bool`
  - `TotalWeight`: `decimal`
  - `Items`: `ExamWeightItemDto`[]

### `GET /api/ExamWeightSetup/academic-year/{academicYearId:int}`  
*Permission: `WeightSetupView`*

**Parameters:**
- `academicYearId`: `int` (route)

**Response:**

**`ExamWeightSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `Name`: `string`
  - `IsActive`: `bool`
  - `TotalWeight`: `decimal`
  - `Items`: `ExamWeightItemDto`[]

### `GET /api/ExamWeightSetup/academic-year/{academicYearId:int}/active`  
*Permission: `WeightSetupView`*

**Parameters:**
- `academicYearId`: `int` (route)

**Response:**

**`ExamWeightSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `Name`: `string`
  - `IsActive`: `bool`
  - `TotalWeight`: `decimal`
  - `Items`: `ExamWeightItemDto`[]

### `POST /api/ExamWeightSetup`  
*Permission: `WeightSetupManage`*

**Request body** (`request`):

**`CreateExamWeightSetupDto`**
  - `AcademicYearId`: `int`
  - `Name`: `string`
  - `Items`: `CreateExamWeightItemDto`[]

**Response:**

**`ExamWeightSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `Name`: `string`
  - `IsActive`: `bool`
  - `TotalWeight`: `decimal`
  - `Items`: `ExamWeightItemDto`[]

### `PUT /api/ExamWeightSetup/{id:int}`  
*Permission: `WeightSetupManage`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateExamWeightSetupDto`**
  - `Id`: `int`
  - `Name`: `string`

**Response:**

**`ExamWeightSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `Name`: `string`
  - `IsActive`: `bool`
  - `TotalWeight`: `decimal`
  - `Items`: `ExamWeightItemDto`[]

### `DELETE /api/ExamWeightSetup/{id:int}`  
*Permission: `WeightSetupManage`*

**Parameters:**
- `id`: `int` (route)

### `POST /api/ExamWeightSetup/{id:int}/activate`  
*Permission: `WeightSetupManage`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`ExamWeightSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `Name`: `string`
  - `IsActive`: `bool`
  - `TotalWeight`: `decimal`
  - `Items`: `ExamWeightItemDto`[]

### `POST /api/ExamWeightSetup/{id:int}/deactivate`  
*Permission: `WeightSetupManage`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`ExamWeightSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `Name`: `string`
  - `IsActive`: `bool`
  - `TotalWeight`: `decimal`
  - `Items`: `ExamWeightItemDto`[]

### `POST /api/ExamWeightSetup/items`  
*Permission: `WeightSetupManage`*

**Request body** (`request`):

**`AddExamWeightItemDto`**
  - `ExamWeightSetupId`: `int`
  - `ExamId`: `int`
  - `WeightPercentage`: `decimal`

**Response:**

**`ExamWeightSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `Name`: `string`
  - `IsActive`: `bool`
  - `TotalWeight`: `decimal`
  - `Items`: `ExamWeightItemDto`[]

### `PUT /api/ExamWeightSetup/items/{itemId:int}`  
*Permission: `WeightSetupManage`*

**Parameters:**
- `itemId`: `int` (route)

**Request body** (`request`):

**`UpdateExamWeightItemDto`**
  - `Id`: `int`
  - `WeightPercentage`: `decimal`

**Response:**

**`ExamWeightSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `Name`: `string`
  - `IsActive`: `bool`
  - `TotalWeight`: `decimal`
  - `Items`: `ExamWeightItemDto`[]

### `DELETE /api/ExamWeightSetup/items/{itemId:int}`  
*Permission: `WeightSetupManage`*

**Parameters:**
- `itemId`: `int` (route)

**Response:**

**`ExamWeightSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `Name`: `string`
  - `IsActive`: `bool`
  - `TotalWeight`: `decimal`
  - `Items`: `ExamWeightItemDto`[]


---


## FinalResultController

### `POST /api/FinalResult/academic-year/{academicYearId:int}/calculate`  
*Permission: `FinalResultCalculate`*

**Parameters:**
- `academicYearId`: `int` (route)

**Response:**

**`FinalResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `RollNo`: `string`
  - `ClassName`: `string`
  - `SectionName`: `string`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `ExamWeightSetupId`: `int`
  - `FinalMarks`: `decimal`
  - `FinalGPA`: `decimal`
  - `FinalGrade`: `string`
  - `IsPassed`: `bool`
  - `PromotionStatus`: `PromotionStatus` enum (Pending=1/Promoted=2/NotPromoted=3)
  - `MeritPosition`: `int` | null
  - `ClassPosition`: `int` | null
  - `SectionPosition`: `int` | null
  - `IsPublished`: `bool`
  - `PublishedAt`: `DateTime` | null
  - `TeacherRemarks`: `string` | null
  - `PrincipalRemarks`: `string` | null
  - `Details`: `FinalResultDetailDto`[]

### `POST /api/FinalResult/academic-year/{academicYearId:int}/publish`  
*Permission: `FinalResultPublish`*

**Parameters:**
- `academicYearId`: `int` (route)

**Response:**

**`FinalResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `RollNo`: `string`
  - `ClassName`: `string`
  - `SectionName`: `string`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `ExamWeightSetupId`: `int`
  - `FinalMarks`: `decimal`
  - `FinalGPA`: `decimal`
  - `FinalGrade`: `string`
  - `IsPassed`: `bool`
  - `PromotionStatus`: `PromotionStatus` enum (Pending=1/Promoted=2/NotPromoted=3)
  - `MeritPosition`: `int` | null
  - `ClassPosition`: `int` | null
  - `SectionPosition`: `int` | null
  - `IsPublished`: `bool`
  - `PublishedAt`: `DateTime` | null
  - `TeacherRemarks`: `string` | null
  - `PrincipalRemarks`: `string` | null
  - `Details`: `FinalResultDetailDto`[]

### `POST /api/FinalResult/academic-year/{academicYearId:int}/unpublish`  
*Permission: `FinalResultUnlock`*

**Parameters:**
- `academicYearId`: `int` (route)

### `GET /api/FinalResult/academic-year/{academicYearId:int}`  
*Permission: `FinalResultView`*

**Parameters:**
- `academicYearId`: `int` (route)
- `classId`: `int?` (Query)

**Response:**

**`FinalResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `RollNo`: `string`
  - `ClassName`: `string`
  - `SectionName`: `string`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `ExamWeightSetupId`: `int`
  - `FinalMarks`: `decimal`
  - `FinalGPA`: `decimal`
  - `FinalGrade`: `string`
  - `IsPassed`: `bool`
  - `PromotionStatus`: `PromotionStatus` enum (Pending=1/Promoted=2/NotPromoted=3)
  - `MeritPosition`: `int` | null
  - `ClassPosition`: `int` | null
  - `SectionPosition`: `int` | null
  - `IsPublished`: `bool`
  - `PublishedAt`: `DateTime` | null
  - `TeacherRemarks`: `string` | null
  - `PrincipalRemarks`: `string` | null
  - `Details`: `FinalResultDetailDto`[]

### `GET /api/FinalResult/student/{studentId:int}/academic-year/{academicYearId:int}`  
*Permission: `FinalResultView`*

**Parameters:**
- `studentId`: `int` (route)
- `academicYearId`: `int` (route)

**Response:**

**`FinalResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `RollNo`: `string`
  - `ClassName`: `string`
  - `SectionName`: `string`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `ExamWeightSetupId`: `int`
  - `FinalMarks`: `decimal`
  - `FinalGPA`: `decimal`
  - `FinalGrade`: `string`
  - `IsPassed`: `bool`
  - `PromotionStatus`: `PromotionStatus` enum (Pending=1/Promoted=2/NotPromoted=3)
  - `MeritPosition`: `int` | null
  - `ClassPosition`: `int` | null
  - `SectionPosition`: `int` | null
  - `IsPublished`: `bool`
  - `PublishedAt`: `DateTime` | null
  - `TeacherRemarks`: `string` | null
  - `PrincipalRemarks`: `string` | null
  - `Details`: `FinalResultDetailDto`[]

### `GET /api/FinalResult/academic-year/{academicYearId:int}/merit/class/{classId:int}`  
*Permission: `FinalResultView`*

**Parameters:**
- `academicYearId`: `int` (route)
- `classId`: `int` (route)

### `GET /api/FinalResult/academic-year/{academicYearId:int}/merit/section/{sectionId:int}`  
*Permission: `FinalResultView`*

**Parameters:**
- `academicYearId`: `int` (route)
- `sectionId`: `int` (route)

### `PATCH /api/FinalResult/student/{studentId:int}/academic-year/{academicYearId:int}/remarks`  
*Permission: `FinalResultPublish`*

**Parameters:**
- `studentId`: `int` (route)
- `academicYearId`: `int` (route)
- `teacherRemarks`: `string?` (Query)
- `principalRemarks`: `string?` (Query)

**Response:**

**`FinalResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `RollNo`: `string`
  - `ClassName`: `string`
  - `SectionName`: `string`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `ExamWeightSetupId`: `int`
  - `FinalMarks`: `decimal`
  - `FinalGPA`: `decimal`
  - `FinalGrade`: `string`
  - `IsPassed`: `bool`
  - `PromotionStatus`: `PromotionStatus` enum (Pending=1/Promoted=2/NotPromoted=3)
  - `MeritPosition`: `int` | null
  - `ClassPosition`: `int` | null
  - `SectionPosition`: `int` | null
  - `IsPublished`: `bool`
  - `PublishedAt`: `DateTime` | null
  - `TeacherRemarks`: `string` | null
  - `PrincipalRemarks`: `string` | null
  - `Details`: `FinalResultDetailDto`[]


---


## GradeSetupController

### `GET /api/GradeSetup`  
*Permission: `GradeSetupView`*

**Response:**

**`GradeSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `GradeName`: `string`
  - `GradePoint`: `decimal`
  - `MinMarks`: `decimal`
  - `MaxMarks`: `decimal`
  - `MinPercentage`: `decimal`
  - `MaxPercentage`: `decimal`
  - `IsFail`: `bool`
  - `DisplayOrder`: `int`
  - `IsActive`: `bool`

### `GET /api/GradeSetup/{id:int}`  
*Permission: `GradeSetupView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`GradeSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `GradeName`: `string`
  - `GradePoint`: `decimal`
  - `MinMarks`: `decimal`
  - `MaxMarks`: `decimal`
  - `MinPercentage`: `decimal`
  - `MaxPercentage`: `decimal`
  - `IsFail`: `bool`
  - `DisplayOrder`: `int`
  - `IsActive`: `bool`

### `GET /api/GradeSetup/academic-year/{academicYearId:int}`  
*Permission: `GradeSetupView`*

**Parameters:**
- `academicYearId`: `int` (route)

**Response:**

**`GradeSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `GradeName`: `string`
  - `GradePoint`: `decimal`
  - `MinMarks`: `decimal`
  - `MaxMarks`: `decimal`
  - `MinPercentage`: `decimal`
  - `MaxPercentage`: `decimal`
  - `IsFail`: `bool`
  - `DisplayOrder`: `int`
  - `IsActive`: `bool`

### `POST /api/GradeSetup`  
*Permission: `GradeSetupManage`*

**Request body** (`request`):

**`CreateGradeSetupDto`**
  - `AcademicYearId`: `int`
  - `GradeName`: `string`
  - `GradePoint`: `decimal`
  - `MinMarks`: `decimal`
  - `MaxMarks`: `decimal`
  - `MinPercentage`: `decimal`
  - `MaxPercentage`: `decimal`
  - `IsFail`: `bool`
  - `DisplayOrder`: `int`

**Response:**

**`GradeSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `GradeName`: `string`
  - `GradePoint`: `decimal`
  - `MinMarks`: `decimal`
  - `MaxMarks`: `decimal`
  - `MinPercentage`: `decimal`
  - `MaxPercentage`: `decimal`
  - `IsFail`: `bool`
  - `DisplayOrder`: `int`
  - `IsActive`: `bool`

### `PUT /api/GradeSetup/{id:int}`  
*Permission: `GradeSetupManage`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateGradeSetupDto`**
  - `Id`: `int`
  - `GradeName`: `string`
  - `GradePoint`: `decimal`
  - `MinMarks`: `decimal`
  - `MaxMarks`: `decimal`
  - `MinPercentage`: `decimal`
  - `MaxPercentage`: `decimal`
  - `IsFail`: `bool`
  - `DisplayOrder`: `int`

**Response:**

**`GradeSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `GradeName`: `string`
  - `GradePoint`: `decimal`
  - `MinMarks`: `decimal`
  - `MaxMarks`: `decimal`
  - `MinPercentage`: `decimal`
  - `MaxPercentage`: `decimal`
  - `IsFail`: `bool`
  - `DisplayOrder`: `int`
  - `IsActive`: `bool`

### `POST /api/GradeSetup/{id:int}/activate`  
*Permission: `GradeSetupManage`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`GradeSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `GradeName`: `string`
  - `GradePoint`: `decimal`
  - `MinMarks`: `decimal`
  - `MaxMarks`: `decimal`
  - `MinPercentage`: `decimal`
  - `MaxPercentage`: `decimal`
  - `IsFail`: `bool`
  - `DisplayOrder`: `int`
  - `IsActive`: `bool`

### `POST /api/GradeSetup/{id:int}/deactivate`  
*Permission: `GradeSetupManage`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`GradeSetupDto`**
  - `Id`: `int`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `GradeName`: `string`
  - `GradePoint`: `decimal`
  - `MinMarks`: `decimal`
  - `MaxMarks`: `decimal`
  - `MinPercentage`: `decimal`
  - `MaxPercentage`: `decimal`
  - `IsFail`: `bool`
  - `DisplayOrder`: `int`
  - `IsActive`: `bool`

### `DELETE /api/GradeSetup/{id:int}`  
*Permission: `GradeSetupManage`*

**Parameters:**
- `id`: `int` (route)


---


## GuardiansController

### `GET /api/Guardians`  
*Permission: `GuardianView`*

**Response:**

**`GuardianDto`**
  - `Id`: `int`
  - `FullName`: `string`
  - `PhoneNumber`: `string`
  - `Email`: `string` | null
  - `Address`: `string` | null
  - `Occupation`: `string` | null

### `GET /api/Guardians/{id:int}`  
*Permission: `GuardianView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`GuardianDto`**
  - `Id`: `int`
  - `FullName`: `string`
  - `PhoneNumber`: `string`
  - `Email`: `string` | null
  - `Address`: `string` | null
  - `Occupation`: `string` | null

### `GET /api/Guardians/search`  
*Permission: `GuardianView`*

**Parameters:**
- `keyword`: `string` (Query)

**Response:**

**`GuardianDto`**
  - `Id`: `int`
  - `FullName`: `string`
  - `PhoneNumber`: `string`
  - `Email`: `string` | null
  - `Address`: `string` | null
  - `Occupation`: `string` | null

### `POST /api/Guardians`  
*Permission: `GuardianCreate`*

**Request body** (`request`):

**`CreateGuardianDto`**
  - `FullName`: `string`
  - `PhoneNumber`: `string`
  - `Email`: `string` | null
  - `Address`: `string` | null
  - `Occupation`: `string` | null

**Response:**

**`GuardianDto`**
  - `Id`: `int`
  - `FullName`: `string`
  - `PhoneNumber`: `string`
  - `Email`: `string` | null
  - `Address`: `string` | null
  - `Occupation`: `string` | null

### `PUT /api/Guardians/{id:int}`  
*Permission: `GuardianEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateGuardianDto`**
  - `Id`: `int`
  - `FullName`: `string`
  - `PhoneNumber`: `string`
  - `Email`: `string` | null
  - `Address`: `string` | null
  - `Occupation`: `string` | null

**Response:**

**`GuardianDto`**
  - `Id`: `int`
  - `FullName`: `string`
  - `PhoneNumber`: `string`
  - `Email`: `string` | null
  - `Address`: `string` | null
  - `Occupation`: `string` | null

### `DELETE /api/Guardians/{id:int}`  
*Permission: `GuardianDelete`*

**Parameters:**
- `id`: `int` (route)


---


## MarkEntryController

### `GET /api/MarkEntry`  
*Permission: `MarksEntryView`*

**Response:**

**`ResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string` | null
  - `RollNo`: `string` | null
  - `ExamScheduleId`: `int`
  - `ExamName`: `string` | null
  - `SubjectName`: `string` | null
  - `FullMarks`: `int`
  - `PassMarks`: `int`
  - `MarksObtained`: `decimal`
  - `GraceMarks`: `decimal`
  - `Grade`: `string` | null
  - `GPA`: `decimal` | null
  - `IsPassed`: `bool`
  - `Percentage`: `decimal` | null
  - `AttendanceStatus`: `MarkAttendanceStatus` enum (Present=1/Absent=2/Medical=3/Withheld=4/Incomplete=5/Excused=6/Late=7/Cheating=8/Blocked=9)
  - `EntryStatus`: `MarkEntryStatus` enum (Draft=1/Submitted=2)
  - `Remarks`: `string` | null
  - `IsLocked`: `bool`
  - `LockedAt`: `DateTime` | null
  - `EnteredByTeacherId`: `int` | null
  - `EnteredByTeacherName`: `string` | null
  - `CreatedAt`: `DateTime`
  - `UpdatedAt`: `DateTime` | null

### `GET /api/MarkEntry/{id:int}`  
*Permission: `MarksEntryView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`ResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string` | null
  - `RollNo`: `string` | null
  - `ExamScheduleId`: `int`
  - `ExamName`: `string` | null
  - `SubjectName`: `string` | null
  - `FullMarks`: `int`
  - `PassMarks`: `int`
  - `MarksObtained`: `decimal`
  - `GraceMarks`: `decimal`
  - `Grade`: `string` | null
  - `GPA`: `decimal` | null
  - `IsPassed`: `bool`
  - `Percentage`: `decimal` | null
  - `AttendanceStatus`: `MarkAttendanceStatus` enum (Present=1/Absent=2/Medical=3/Withheld=4/Incomplete=5/Excused=6/Late=7/Cheating=8/Blocked=9)
  - `EntryStatus`: `MarkEntryStatus` enum (Draft=1/Submitted=2)
  - `Remarks`: `string` | null
  - `IsLocked`: `bool`
  - `LockedAt`: `DateTime` | null
  - `EnteredByTeacherId`: `int` | null
  - `EnteredByTeacherName`: `string` | null
  - `CreatedAt`: `DateTime`
  - `UpdatedAt`: `DateTime` | null

### `GET /api/MarkEntry/exam-schedule/{examScheduleId:int}`  
*Permission: `MarksEntryView`*

**Parameters:**
- `examScheduleId`: `int` (route)

**Response:**

**`ResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string` | null
  - `RollNo`: `string` | null
  - `ExamScheduleId`: `int`
  - `ExamName`: `string` | null
  - `SubjectName`: `string` | null
  - `FullMarks`: `int`
  - `PassMarks`: `int`
  - `MarksObtained`: `decimal`
  - `GraceMarks`: `decimal`
  - `Grade`: `string` | null
  - `GPA`: `decimal` | null
  - `IsPassed`: `bool`
  - `Percentage`: `decimal` | null
  - `AttendanceStatus`: `MarkAttendanceStatus` enum (Present=1/Absent=2/Medical=3/Withheld=4/Incomplete=5/Excused=6/Late=7/Cheating=8/Blocked=9)
  - `EntryStatus`: `MarkEntryStatus` enum (Draft=1/Submitted=2)
  - `Remarks`: `string` | null
  - `IsLocked`: `bool`
  - `LockedAt`: `DateTime` | null
  - `EnteredByTeacherId`: `int` | null
  - `EnteredByTeacherName`: `string` | null
  - `CreatedAt`: `DateTime`
  - `UpdatedAt`: `DateTime` | null

### `GET /api/MarkEntry/student/{studentId:int}/exam/{examId:int}`  
*Permission: `MarksEntryView`*

**Parameters:**
- `studentId`: `int` (route)
- `examId`: `int` (route)

**Response:**

**`ResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string` | null
  - `RollNo`: `string` | null
  - `ExamScheduleId`: `int`
  - `ExamName`: `string` | null
  - `SubjectName`: `string` | null
  - `FullMarks`: `int`
  - `PassMarks`: `int`
  - `MarksObtained`: `decimal`
  - `GraceMarks`: `decimal`
  - `Grade`: `string` | null
  - `GPA`: `decimal` | null
  - `IsPassed`: `bool`
  - `Percentage`: `decimal` | null
  - `AttendanceStatus`: `MarkAttendanceStatus` enum (Present=1/Absent=2/Medical=3/Withheld=4/Incomplete=5/Excused=6/Late=7/Cheating=8/Blocked=9)
  - `EntryStatus`: `MarkEntryStatus` enum (Draft=1/Submitted=2)
  - `Remarks`: `string` | null
  - `IsLocked`: `bool`
  - `LockedAt`: `DateTime` | null
  - `EnteredByTeacherId`: `int` | null
  - `EnteredByTeacherName`: `string` | null
  - `CreatedAt`: `DateTime`
  - `UpdatedAt`: `DateTime` | null

### `POST /api/MarkEntry`  
*Permission: `MarksEntryCreate`*

**Request body** (`request`):

**`CreateResultDto`**
  - `StudentId`: `int`
  - `ExamScheduleId`: `int`
  - `TeacherId`: `int`
  - `MarksObtained`: `decimal`
  - `GraceMarks`: `decimal`
  - `AttendanceStatus`: `MarkAttendanceStatus` enum (Present=1/Absent=2/Medical=3/Withheld=4/Incomplete=5/Excused=6/Late=7/Cheating=8/Blocked=9)
  - `Remarks`: `string` | null

**Response:**

**`ResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string` | null
  - `RollNo`: `string` | null
  - `ExamScheduleId`: `int`
  - `ExamName`: `string` | null
  - `SubjectName`: `string` | null
  - `FullMarks`: `int`
  - `PassMarks`: `int`
  - `MarksObtained`: `decimal`
  - `GraceMarks`: `decimal`
  - `Grade`: `string` | null
  - `GPA`: `decimal` | null
  - `IsPassed`: `bool`
  - `Percentage`: `decimal` | null
  - `AttendanceStatus`: `MarkAttendanceStatus` enum (Present=1/Absent=2/Medical=3/Withheld=4/Incomplete=5/Excused=6/Late=7/Cheating=8/Blocked=9)
  - `EntryStatus`: `MarkEntryStatus` enum (Draft=1/Submitted=2)
  - `Remarks`: `string` | null
  - `IsLocked`: `bool`
  - `LockedAt`: `DateTime` | null
  - `EnteredByTeacherId`: `int` | null
  - `EnteredByTeacherName`: `string` | null
  - `CreatedAt`: `DateTime`
  - `UpdatedAt`: `DateTime` | null

### `PUT /api/MarkEntry/{id:int}`  
*Permission: `MarksEntryEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateResultDto`**
  - `Id`: `int`
  - `TeacherId`: `int`
  - `MarksObtained`: `decimal`
  - `GraceMarks`: `decimal`
  - `AttendanceStatus`: `MarkAttendanceStatus` enum (Present=1/Absent=2/Medical=3/Withheld=4/Incomplete=5/Excused=6/Late=7/Cheating=8/Blocked=9)
  - `Remarks`: `string` | null

**Response:**

**`ResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string` | null
  - `RollNo`: `string` | null
  - `ExamScheduleId`: `int`
  - `ExamName`: `string` | null
  - `SubjectName`: `string` | null
  - `FullMarks`: `int`
  - `PassMarks`: `int`
  - `MarksObtained`: `decimal`
  - `GraceMarks`: `decimal`
  - `Grade`: `string` | null
  - `GPA`: `decimal` | null
  - `IsPassed`: `bool`
  - `Percentage`: `decimal` | null
  - `AttendanceStatus`: `MarkAttendanceStatus` enum (Present=1/Absent=2/Medical=3/Withheld=4/Incomplete=5/Excused=6/Late=7/Cheating=8/Blocked=9)
  - `EntryStatus`: `MarkEntryStatus` enum (Draft=1/Submitted=2)
  - `Remarks`: `string` | null
  - `IsLocked`: `bool`
  - `LockedAt`: `DateTime` | null
  - `EnteredByTeacherId`: `int` | null
  - `EnteredByTeacherName`: `string` | null
  - `CreatedAt`: `DateTime`
  - `UpdatedAt`: `DateTime` | null

### `POST /api/MarkEntry/bulk`  
*Permission: `MarksEntryCreate`*

**Request body** (`request`):

**`BulkMarkEntryDto`**
  - `ExamScheduleId`: `int`
  - `TeacherId`: `int`
  - `Entries`: `MarkEntryItemDto`[]

**Response:**

**`ResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string` | null
  - `RollNo`: `string` | null
  - `ExamScheduleId`: `int`
  - `ExamName`: `string` | null
  - `SubjectName`: `string` | null
  - `FullMarks`: `int`
  - `PassMarks`: `int`
  - `MarksObtained`: `decimal`
  - `GraceMarks`: `decimal`
  - `Grade`: `string` | null
  - `GPA`: `decimal` | null
  - `IsPassed`: `bool`
  - `Percentage`: `decimal` | null
  - `AttendanceStatus`: `MarkAttendanceStatus` enum (Present=1/Absent=2/Medical=3/Withheld=4/Incomplete=5/Excused=6/Late=7/Cheating=8/Blocked=9)
  - `EntryStatus`: `MarkEntryStatus` enum (Draft=1/Submitted=2)
  - `Remarks`: `string` | null
  - `IsLocked`: `bool`
  - `LockedAt`: `DateTime` | null
  - `EnteredByTeacherId`: `int` | null
  - `EnteredByTeacherName`: `string` | null
  - `CreatedAt`: `DateTime`
  - `UpdatedAt`: `DateTime` | null

### `POST /api/MarkEntry/exam-schedule/{examScheduleId:int}/submit`  
*Permission: `MarksEntryEdit`*

**Parameters:**
- `examScheduleId`: `int` (route)
- `teacherId`: `int` (Query)

**Response:**

**`ResultDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string` | null
  - `RollNo`: `string` | null
  - `ExamScheduleId`: `int`
  - `ExamName`: `string` | null
  - `SubjectName`: `string` | null
  - `FullMarks`: `int`
  - `PassMarks`: `int`
  - `MarksObtained`: `decimal`
  - `GraceMarks`: `decimal`
  - `Grade`: `string` | null
  - `GPA`: `decimal` | null
  - `IsPassed`: `bool`
  - `Percentage`: `decimal` | null
  - `AttendanceStatus`: `MarkAttendanceStatus` enum (Present=1/Absent=2/Medical=3/Withheld=4/Incomplete=5/Excused=6/Late=7/Cheating=8/Blocked=9)
  - `EntryStatus`: `MarkEntryStatus` enum (Draft=1/Submitted=2)
  - `Remarks`: `string` | null
  - `IsLocked`: `bool`
  - `LockedAt`: `DateTime` | null
  - `EnteredByTeacherId`: `int` | null
  - `EnteredByTeacherName`: `string` | null
  - `CreatedAt`: `DateTime`
  - `UpdatedAt`: `DateTime` | null

### `POST /api/MarkEntry/exam-schedule/{examScheduleId:int}/lock`  
*Permission: `MarksEntryPublish`*

**Parameters:**
- `examScheduleId`: `int` (route)

### `POST /api/MarkEntry/exam-schedule/{examScheduleId:int}/unlock`  
*Permission: `MarksEntryPublish`*

**Parameters:**
- `examScheduleId`: `int` (route)

### `DELETE /api/MarkEntry/{id:int}`  
*Permission: `MarksEntryDelete`*

**Parameters:**
- `id`: `int` (route)


---


## NoticeController

### `GET /api/Notice`  
*Permission: `NoticeView`*

**Query parameters** (`query`):

**`NoticeQueryDto`**
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5) | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3) | null
  - `IsPublished`: `bool` | null
  - `IsArchived`: `bool` | null
  - `FromDate`: `DateTime` | null
  - `ToDate`: `DateTime` | null

### `GET /api/Notice/{id:int}`  
*Permission: `NoticeView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`NoticeDto`**
  - `Id`: `int`
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `IsPublished`: `bool`
  - `IsArchived`: `bool`
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentPath`: `string` | null
  - `IsActive`: `bool`
  - `IsUpcoming`: `bool`
  - `IsExpired`: `bool`
  - `CreatedAt`: `DateTime`

### `POST /api/Notice`  
*Permission: `NoticeCreate`*

**Form fields (multipart/form-data)** (`request`):

**`CreateNoticeDto`**
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentFile`: `IFormFile` | null

**Response:**

**`NoticeDto`**
  - `Id`: `int`
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `IsPublished`: `bool`
  - `IsArchived`: `bool`
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentPath`: `string` | null
  - `IsActive`: `bool`
  - `IsUpcoming`: `bool`
  - `IsExpired`: `bool`
  - `CreatedAt`: `DateTime`

### `PUT /api/Notice/{id:int}`  
*Permission: `NoticeEdit`*

**Parameters:**
- `id`: `int` (route)

**Form fields (multipart/form-data)** (`request`):

**`UpdateNoticeDto`**
  - `Id`: `int`
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentFile`: `IFormFile` | null
  - `RemoveAttachment`: `bool`

**Response:**

**`NoticeDto`**
  - `Id`: `int`
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `IsPublished`: `bool`
  - `IsArchived`: `bool`
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentPath`: `string` | null
  - `IsActive`: `bool`
  - `IsUpcoming`: `bool`
  - `IsExpired`: `bool`
  - `CreatedAt`: `DateTime`

### `POST /api/Notice/{id:int}/publish`  
*Permission: `NoticePublish`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`NoticeDto`**
  - `Id`: `int`
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `IsPublished`: `bool`
  - `IsArchived`: `bool`
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentPath`: `string` | null
  - `IsActive`: `bool`
  - `IsUpcoming`: `bool`
  - `IsExpired`: `bool`
  - `CreatedAt`: `DateTime`

### `POST /api/Notice/{id:int}/unpublish`  
*Permission: `NoticePublish`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`NoticeDto`**
  - `Id`: `int`
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `IsPublished`: `bool`
  - `IsArchived`: `bool`
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentPath`: `string` | null
  - `IsActive`: `bool`
  - `IsUpcoming`: `bool`
  - `IsExpired`: `bool`
  - `CreatedAt`: `DateTime`

### `POST /api/Notice/{id:int}/archive`  
*Permission: `NoticePublish`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`NoticeDto`**
  - `Id`: `int`
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `IsPublished`: `bool`
  - `IsArchived`: `bool`
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentPath`: `string` | null
  - `IsActive`: `bool`
  - `IsUpcoming`: `bool`
  - `IsExpired`: `bool`
  - `CreatedAt`: `DateTime`

### `POST /api/Notice/{id:int}/restore`  
*Permission: `NoticePublish`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`NoticeDto`**
  - `Id`: `int`
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `IsPublished`: `bool`
  - `IsArchived`: `bool`
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentPath`: `string` | null
  - `IsActive`: `bool`
  - `IsUpcoming`: `bool`
  - `IsExpired`: `bool`
  - `CreatedAt`: `DateTime`

### `POST /api/Notice/{id:int}/attachment`  
*Permission: `NoticeEdit`*

**Parameters:**
- `id`: `int` (route)
- `attachmentFile`: `IFormFile` (?)

**Response:**

**`NoticeDto`**
  - `Id`: `int`
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `IsPublished`: `bool`
  - `IsArchived`: `bool`
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentPath`: `string` | null
  - `IsActive`: `bool`
  - `IsUpcoming`: `bool`
  - `IsExpired`: `bool`
  - `CreatedAt`: `DateTime`

### `DELETE /api/Notice/{id:int}/attachment`  
*Permission: `NoticeEdit`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`NoticeDto`**
  - `Id`: `int`
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `IsPublished`: `bool`
  - `IsArchived`: `bool`
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentPath`: `string` | null
  - `IsActive`: `bool`
  - `IsUpcoming`: `bool`
  - `IsExpired`: `bool`
  - `CreatedAt`: `DateTime`

### `GET /api/Notice/active`  
*Permission: `NoticeView`*

**Response:**

**`NoticeDto`**
  - `Id`: `int`
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `IsPublished`: `bool`
  - `IsArchived`: `bool`
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentPath`: `string` | null
  - `IsActive`: `bool`
  - `IsUpcoming`: `bool`
  - `IsExpired`: `bool`
  - `CreatedAt`: `DateTime`

### `GET /api/Notice/upcoming`  
*Permission: `NoticeView`*

**Response:**

**`NoticeDto`**
  - `Id`: `int`
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `IsPublished`: `bool`
  - `IsArchived`: `bool`
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentPath`: `string` | null
  - `IsActive`: `bool`
  - `IsUpcoming`: `bool`
  - `IsExpired`: `bool`
  - `CreatedAt`: `DateTime`

### `GET /api/Notice/expired`  
*Permission: `NoticeView`*

**Response:**

**`NoticeDto`**
  - `Id`: `int`
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `IsPublished`: `bool`
  - `IsArchived`: `bool`
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentPath`: `string` | null
  - `IsActive`: `bool`
  - `IsUpcoming`: `bool`
  - `IsExpired`: `bool`
  - `CreatedAt`: `DateTime`

### `GET /api/Notice/recent`  
*Permission: `NoticeView`*

**Parameters:**
- `count`: `int` (Query)

**Response:**

**`NoticeDto`**
  - `Id`: `int`
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `IsPublished`: `bool`
  - `IsArchived`: `bool`
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentPath`: `string` | null
  - `IsActive`: `bool`
  - `IsUpcoming`: `bool`
  - `IsExpired`: `bool`
  - `CreatedAt`: `DateTime`

### `GET /api/Notice/high-priority`  
*Permission: `NoticeView`*

**Response:**

**`NoticeDto`**
  - `Id`: `int`
  - `Title`: `string`
  - `Description`: `string`
  - `PublishDate`: `DateTime`
  - `ExpiryDate`: `DateTime` | null
  - `Priority`: `NoticePriority` enum (Low=1/Medium=2/High=3)
  - `Audience`: `NoticeAudience` enum (Students=1/Teachers=2/Employees=3/Guardians=4/Everyone=5)
  - `IsPublished`: `bool`
  - `IsArchived`: `bool`
  - `SendSms`: `bool`
  - `SendEmail`: `bool`
  - `AttachmentPath`: `string` | null
  - `IsActive`: `bool`
  - `IsUpcoming`: `bool`
  - `IsExpired`: `bool`
  - `CreatedAt`: `DateTime`

### `GET /api/Notice/dashboard`  
*Permission: `NoticeView`*

**Response:**

**`NoticeDashboardSummaryDto`**
  - `TotalNotices`: `int`
  - `DraftNotices`: `int`
  - `PublishedNotices`: `int`
  - `ArchivedNotices`: `int`
  - `ActiveNotices`: `int`
  - `UpcomingNotices`: `int`
  - `ExpiredNotices`: `int`
  - `HighPriorityNotices`: `int`

### `DELETE /api/Notice/{id:int}`  
*Permission: `NoticeDelete`*

**Parameters:**
- `id`: `int` (route)


---


## PermissionController

### `GET /api/Permission`  
*Permission: `PermissionView`*

**Response:**

**`PermissionDto`**
  - `Id`: `int`
  - `Name`: `string`

### `GET /api/Permission/{id:int}`  
*Permission: `PermissionView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`PermissionDto`**
  - `Id`: `int`
  - `Name`: `string`

### `POST /api/Permission`  
*Permission: `PermissionCreate`*

**Request body** (`request`):

**`CreatePermissionDto`**
  - `Name`: `string`

**Response:**

**`PermissionDto`**
  - `Id`: `int`
  - `Name`: `string`

### `PUT /api/Permission/{id:int}`  
*Permission: `PermissionEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdatePermissionDto`**
  - `Id`: `int`
  - `Name`: `string`

**Response:**

**`PermissionDto`**
  - `Id`: `int`
  - `Name`: `string`

### `DELETE /api/Permission/{id:int}`  
*Permission: `PermissionDelete`*

**Parameters:**
- `id`: `int` (route)


---


## ProgressReportController

### `GET /api/ProgressReport/student/{studentId:int}/academic-year/{academicYearId:int}`  
*Permission: `ProgressReportView`*

**Parameters:**
- `studentId`: `int` (route)
- `academicYearId`: `int` (route)

**Response:**

**`ProgressReportDto`**
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `RollNo`: `string`
  - `ClassName`: `string`
  - `SectionName`: `string`
  - `AcademicYearId`: `int`
  - `AcademicYearName`: `string`
  - `Exams`: `ProgressReportExamColumnDto`[]
  - `Subjects`: `ProgressReportSubjectRowDto`[]


---


## ResultAuditLogController

### `GET /api/ResultAuditLog/{entityType}/{entityId:int}`  
*Permission: `ResultAuditView`*

**Parameters:**
- `entityType`: `string` (route)
- `entityId`: `int` (route)

**Response:**

**`ResultAuditLogDto`**
  - `Id`: `int`
  - `EntityType`: `string`
  - `EntityId`: `int`
  - `Action`: `ResultAuditAction` enum (Calculated=1/Recalculated=2/Verified=3/Published=4/Unpublished=5/Locked=6/Unlocked=7/Archived=8/RolledBack=9/MarkUpdated=10)
  - `PerformedBy`: `int` | null
  - `PerformedByName`: `string` | null
  - `Notes`: `string` | null
  - `PerformedAt`: `DateTime`


---


## RoleController

### `GET /api/Role`  
*Permission: `RoleView`*

**Response:**

**`RoleDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Description`: `string` | null

### `GET /api/Role/{id:int}`  
*Permission: `RoleView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`RoleDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Description`: `string` | null

### `POST /api/Role`  
*Permission: `RoleCreate`*

**Request body** (`request`):

**`CreateRoleDto`**
  - `Name`: `string`
  - `Description`: `string` | null

**Response:**

**`RoleDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Description`: `string` | null

### `PUT /api/Role/{id:int}`  
*Permission: `RoleEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateRoleDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Description`: `string` | null

**Response:**

**`RoleDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Description`: `string` | null

### `DELETE /api/Role/{id:int}`  
*Permission: `RoleDelete`*

**Parameters:**
- `id`: `int` (route)

### `GET /api/Role/{id:int}/permissions`  
*Permission: `RoleView`*

**Parameters:**
- `id`: `int` (route)

### `POST /api/Role/assign-permissions`  
*Permission: `RoleAssignPermission`*

**Request body** (`request`):

**`AssignPermissionsToRoleDto`**
  - `RoleId`: `int`
  - `PermissionIds`: `int`[]

### `DELETE /api/Role/{roleId:int}/permissions/{permissionId:int}`  
*Permission: `RoleAssignPermission`*

**Parameters:**
- `roleId`: `int` (route)
- `permissionId`: `int` (route)


---


## SchoolClassesController

### `GET /api/SchoolClasses`  
*Permission: `SchoolClassView`*

**Response:**

**`SchoolClassDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `DisplayOrder`: `int`

### `GET /api/SchoolClasses/{id:int}`  
*Permission: `SchoolClassView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`SchoolClassDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `DisplayOrder`: `int`

### `POST /api/SchoolClasses`  
*Permission: `SchoolClassCreate`*

**Request body** (`request`):

**`CreateSchoolClassDto`**
  - `Name`: `string`
  - `DisplayOrder`: `int`

**Response:**

**`SchoolClassDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `DisplayOrder`: `int`

### `PUT /api/SchoolClasses/{id:int}`  
*Permission: `SchoolClassEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateSchoolClassDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `DisplayOrder`: `int`

**Response:**

**`SchoolClassDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `DisplayOrder`: `int`

### `DELETE /api/SchoolClasses/{id:int}`  
*Permission: `SchoolClassDelete`*

**Parameters:**
- `id`: `int` (route)


---


## SchoolController

### `GET /api/School`  
*Permission: `SchoolView`*

**Response:**

**`SchoolDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `EIIN`: `string` | null
  - `Address`: `string` | null
  - `Phone`: `string` | null
  - `Email`: `string` | null
  - `Logo`: `string` | null

### `GET /api/School/{id:int}`  
*Permission: `SchoolView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`SchoolDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `EIIN`: `string` | null
  - `Address`: `string` | null
  - `Phone`: `string` | null
  - `Email`: `string` | null
  - `Logo`: `string` | null

### `POST /api/School`  
*Permission: `SchoolCreate`*

**Form fields (multipart/form-data)** (`request`):

**`CreateSchoolDto`**
  - `Name`: `string`
  - `EIIN`: `string` | null
  - `Address`: `string` | null
  - `Phone`: `string` | null
  - `Email`: `string` | null
  - `LogoFile`: `IFormFile` | null

**Response:**

**`SchoolDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `EIIN`: `string` | null
  - `Address`: `string` | null
  - `Phone`: `string` | null
  - `Email`: `string` | null
  - `Logo`: `string` | null

### `PUT /api/School/{id:int}`  
*Permission: `SchoolUpdate`*

**Parameters:**
- `id`: `int` (route)

**Form fields (multipart/form-data)** (`request`):

**`UpdateSchoolDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `EIIN`: `string` | null
  - `Address`: `string` | null
  - `Phone`: `string` | null
  - `Email`: `string` | null
  - `LogoFile`: `IFormFile` | null

**Response:**

**`SchoolDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `EIIN`: `string` | null
  - `Address`: `string` | null
  - `Phone`: `string` | null
  - `Email`: `string` | null
  - `Logo`: `string` | null

### `DELETE /api/School/{id:int}`  
*Permission: `SchoolDelete`*

**Parameters:**
- `id`: `int` (route)


---


## SectionsController

### `GET /api/Sections`  
*Permission: `SectionView`*

**Response:**

**`SectionDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `ClassId`: `int`

### `GET /api/Sections/{id:int}`  
*Permission: `SectionView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`SectionDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `ClassId`: `int`

### `POST /api/Sections`  
*Permission: `SectionCreate`*

**Request body** (`request`):

**`CreateSectionDto`**
  - `Name`: `string`
  - `ClassId`: `int`

**Response:**

**`SectionDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `ClassId`: `int`

### `PUT /api/Sections/{id:int}`  
*Permission: `SectionEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateSectionDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `ClassId`: `int`

**Response:**

**`SectionDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `ClassId`: `int`

### `DELETE /api/Sections/{id:int}`  
*Permission: `SectionDelete`*

**Parameters:**
- `id`: `int` (route)


---


## SmsLogController

### `GET /api/SmsLog`  
*Permission: `SmsLogView`*

**Query parameters** (`query`):

**`SmsLogQueryDto`**
  - `Status`: `SmsStatus` enum (Pending=1/Sent=2/Delivered=3/Failed=4) | null
  - `RecipientNumber`: `string` | null
  - `StudentId`: `int` | null
  - `Provider`: `string` | null
  - `FromDate`: `DateTime` | null
  - `ToDate`: `DateTime` | null

### `GET /api/SmsLog/{id:int}`  
*Permission: `SmsLogView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`SmsLogDto`**
  - `Id`: `int`
  - `RecipientNumber`: `string`
  - `Message`: `string`
  - `Status`: `SmsStatus` enum (Pending=1/Sent=2/Delivered=3/Failed=4)
  - `ProviderResponse`: `string` | null
  - `SentAt`: `DateTime` | null
  - `Provider`: `string` | null
  - `StudentId`: `int` | null
  - `StudentName`: `string` | null
  - `CreatedAt`: `DateTime`

### `POST /api/SmsLog`  
*Permission: `SmsLogCreate`*

**Request body** (`request`):

**`CreateSmsLogDto`**
  - `RecipientNumber`: `string`
  - `Message`: `string`
  - `Status`: `SmsStatus` enum (Pending=1/Sent=2/Delivered=3/Failed=4)
  - `ProviderResponse`: `string` | null
  - `SentAt`: `DateTime` | null
  - `Provider`: `string` | null
  - `StudentId`: `int` | null

**Response:**

**`SmsLogDto`**
  - `Id`: `int`
  - `RecipientNumber`: `string`
  - `Message`: `string`
  - `Status`: `SmsStatus` enum (Pending=1/Sent=2/Delivered=3/Failed=4)
  - `ProviderResponse`: `string` | null
  - `SentAt`: `DateTime` | null
  - `Provider`: `string` | null
  - `StudentId`: `int` | null
  - `StudentName`: `string` | null
  - `CreatedAt`: `DateTime`

### `DELETE /api/SmsLog/{id:int}`  
*Permission: `SmsLogDelete`*

**Parameters:**
- `id`: `int` (route)

### `GET /api/SmsLog/dashboard`  
*Permission: `SmsLogView`*

**Response:**

**`SmsDashboardStatsDto`**
  - `TotalSms`: `int`
  - `TodaySms`: `int`
  - `WeeklySms`: `int`
  - `MonthlySms`: `int`
  - `SuccessCount`: `int`
  - `FailedCount`: `int`
  - `PendingCount`: `int`
  - `SuccessRate`: `double`

### `GET /api/SmsLog/today`  
*Permission: `SmsLogView`*

**Response:**

**`SmsLogDto`**
  - `Id`: `int`
  - `RecipientNumber`: `string`
  - `Message`: `string`
  - `Status`: `SmsStatus` enum (Pending=1/Sent=2/Delivered=3/Failed=4)
  - `ProviderResponse`: `string` | null
  - `SentAt`: `DateTime` | null
  - `Provider`: `string` | null
  - `StudentId`: `int` | null
  - `StudentName`: `string` | null
  - `CreatedAt`: `DateTime`

### `GET /api/SmsLog/weekly`  
*Permission: `SmsLogView`*

**Response:**

**`SmsLogDto`**
  - `Id`: `int`
  - `RecipientNumber`: `string`
  - `Message`: `string`
  - `Status`: `SmsStatus` enum (Pending=1/Sent=2/Delivered=3/Failed=4)
  - `ProviderResponse`: `string` | null
  - `SentAt`: `DateTime` | null
  - `Provider`: `string` | null
  - `StudentId`: `int` | null
  - `StudentName`: `string` | null
  - `CreatedAt`: `DateTime`

### `GET /api/SmsLog/failed`  
*Permission: `SmsLogView`*

**Parameters:**
- `fromDate`: `DateTime?` (Query)
- `toDate`: `DateTime?` (Query)

**Response:**

**`SmsLogDto`**
  - `Id`: `int`
  - `RecipientNumber`: `string`
  - `Message`: `string`
  - `Status`: `SmsStatus` enum (Pending=1/Sent=2/Delivered=3/Failed=4)
  - `ProviderResponse`: `string` | null
  - `SentAt`: `DateTime` | null
  - `Provider`: `string` | null
  - `StudentId`: `int` | null
  - `StudentName`: `string` | null
  - `CreatedAt`: `DateTime`

### `GET /api/SmsLog/success-rate`  
*Permission: `SmsLogView`*

**Parameters:**
- `fromDate`: `DateTime?` (Query)
- `toDate`: `DateTime?` (Query)

### `GET /api/SmsLog/daily-report`  
*Permission: `SmsLogView`*

**Parameters:**
- `fromDate`: `DateTime` (Query)
- `toDate`: `DateTime` (Query)

**Response:**

**`SmsDailyReportDto`**
  - `Date`: `DateTime`
  - `Total`: `int`
  - `Success`: `int`
  - `Failed`: `int`
  - `Pending`: `int`

### `GET /api/SmsLog/monthly-report`  
*Permission: `SmsLogView`*

**Parameters:**
- `year`: `int` (Query)

**Response:**

**`SmsMonthlyReportDto`**
  - `Year`: `int`
  - `Month`: `int`
  - `MonthName`: `string`
  - `Total`: `int`
  - `Success`: `int`
  - `Failed`: `int`
  - `Pending`: `int`

### `GET /api/SmsLog/top-recipients`  
*Permission: `SmsLogView`*

**Parameters:**
- `count`: `int` (Query)
- `fromDate`: `DateTime?` (Query)
- `toDate`: `DateTime?` (Query)

**Response:**

**`TopRecipientDto`**
  - `RecipientNumber`: `string`
  - `StudentId`: `int` | null
  - `StudentName`: `string` | null
  - `MessageCount`: `int`

### `GET /api/SmsLog/recent`  
*Permission: `SmsLogView`*

**Parameters:**
- `count`: `int` (Query)

**Response:**

**`SmsLogDto`**
  - `Id`: `int`
  - `RecipientNumber`: `string`
  - `Message`: `string`
  - `Status`: `SmsStatus` enum (Pending=1/Sent=2/Delivered=3/Failed=4)
  - `ProviderResponse`: `string` | null
  - `SentAt`: `DateTime` | null
  - `Provider`: `string` | null
  - `StudentId`: `int` | null
  - `StudentName`: `string` | null
  - `CreatedAt`: `DateTime`


---


## SmsTemplateController

### `GET /api/SmsTemplate`  
*Permission: `SmsTemplateView`*

**Query parameters** (`query`):

**`SmsTemplateQueryDto`**
  - `IsActive`: `bool` | null

### `GET /api/SmsTemplate/all`  
*Permission: `SmsTemplateView`*

**Response:**

**`SmsTemplateDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Message`: `string`
  - `IsActive`: `bool`
  - `CreatedAt`: `DateTime`
  - `UpdatedAt`: `DateTime` | null

### `GET /api/SmsTemplate/{id:int}`  
*Permission: `SmsTemplateView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`SmsTemplateDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Message`: `string`
  - `IsActive`: `bool`
  - `CreatedAt`: `DateTime`
  - `UpdatedAt`: `DateTime` | null

### `POST /api/SmsTemplate`  
*Permission: `SmsTemplateCreate`*

**Request body** (`request`):

**`CreateSmsTemplateDto`**
  - `Name`: `string`
  - `Message`: `string`
  - `IsActive`: `bool`

**Response:**

**`SmsTemplateDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Message`: `string`
  - `IsActive`: `bool`
  - `CreatedAt`: `DateTime`
  - `UpdatedAt`: `DateTime` | null

### `PUT /api/SmsTemplate/{id:int}`  
*Permission: `SmsTemplateEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateSmsTemplateDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Message`: `string`
  - `IsActive`: `bool`

**Response:**

**`SmsTemplateDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Message`: `string`
  - `IsActive`: `bool`
  - `CreatedAt`: `DateTime`
  - `UpdatedAt`: `DateTime` | null

### `PATCH /api/SmsTemplate/{id:int}/activate`  
*Permission: `SmsTemplateEdit`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`SmsTemplateDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Message`: `string`
  - `IsActive`: `bool`
  - `CreatedAt`: `DateTime`
  - `UpdatedAt`: `DateTime` | null

### `PATCH /api/SmsTemplate/{id:int}/deactivate`  
*Permission: `SmsTemplateEdit`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`SmsTemplateDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Message`: `string`
  - `IsActive`: `bool`
  - `CreatedAt`: `DateTime`
  - `UpdatedAt`: `DateTime` | null

### `POST /api/SmsTemplate/{id:int}/preview`  
*Permission: `SmsTemplateView`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`RenderSmsTemplateDto`**
  - `Data`: `PlaceholderDataDto`

**Response:**

**`RenderedSmsTemplateDto`**
  - `TemplateId`: `int`
  - `TemplateName`: `string`
  - `RawMessage`: `string`
  - `RenderedMessage`: `string`

### `DELETE /api/SmsTemplate/{id:int}`  
*Permission: `SmsTemplateDelete`*

**Parameters:**
- `id`: `int` (route)


---


## StudentAttendanceController

### `GET /api/StudentAttendance`  
*Permission: `StudentAttendanceView`*

**Response:**

**`StudentAttendanceDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `AttendanceDate`: `DateTime`
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)
  - `Remarks`: `string` | null

### `GET /api/StudentAttendance/{id:int}`  
*Permission: `StudentAttendanceView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`StudentAttendanceDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `AttendanceDate`: `DateTime`
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)
  - `Remarks`: `string` | null

### `POST /api/StudentAttendance`  
*Permission: `StudentAttendanceCreate`*

**Request body** (`request`):

**`CreateStudentAttendanceDto`**
  - `StudentId`: `int`
  - `AttendanceDate`: `DateTime`
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)
  - `Remarks`: `string` | null

**Response:**

**`StudentAttendanceDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `AttendanceDate`: `DateTime`
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)
  - `Remarks`: `string` | null

### `POST /api/StudentAttendance/bulk`  
*Permission: `StudentAttendanceCreate`*

**Request body** (`request`):

**`BulkStudentAttendanceDto`**
  - `ClassId`: `int`
  - `SectionId`: `int`
  - `AttendanceDate`: `DateTime`
  - `Attendance`: `StudentAttendanceItemDto`[]

### `GET /api/StudentAttendance/class-section`  
*Permission: `StudentAttendanceView`*

**Parameters:**
- `classId`: `int` (Query)
- `sectionId`: `int` (Query)
- `attendanceDate`: `DateTime` (Query)

**Response:**

**`StudentAttendanceDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `AttendanceDate`: `DateTime`
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)
  - `Remarks`: `string` | null

### `GET /api/StudentAttendance/student/{studentId:int}/history`  
*Permission: `StudentAttendanceView`*

**Parameters:**
- `studentId`: `int` (route)
- `fromDate`: `DateTime?` (Query)
- `toDate`: `DateTime?` (Query)

**Response:**

**`StudentAttendanceDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `AttendanceDate`: `DateTime`
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)
  - `Remarks`: `string` | null

### `PUT /api/StudentAttendance/{id:int}`  
*Permission: `StudentAttendanceEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateStudentAttendanceDto`**
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)
  - `Remarks`: `string` | null

**Response:**

**`StudentAttendanceDto`**
  - `Id`: `int`
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `AttendanceDate`: `DateTime`
  - `Status`: `AttendanceStatus` enum (Present=1/Absent=2/Late=3/Leave=4/HalfDay=5)
  - `Remarks`: `string` | null

### `DELETE /api/StudentAttendance/{id:int}`  
*Permission: `StudentAttendanceDelete`*

**Parameters:**
- `id`: `int` (route)


---


## StudentsController

### `GET /api/Students`  
*Permission: `StudentView`*

**Response:**

**`StudentDto`**
  - `Id`: `int`
  - `AdmissionNumber`: `string`
  - `FullName`: `string`
  - `DateOfBirth`: `DateTime`
  - `RollNo`: `string`
  - `AdmissionDate`: `DateTime`
  - `Gender`: `string`
  - `BloodGroup`: `string` | null
  - `Address`: `string` | null
  - `Photo`: `string` | null
  - `ClassId`: `int`
  - `SectionId`: `int`
  - `Guardians`: `StudentGuardianDto`[]

### `GET /api/Students/{id:int}`  
*Permission: `StudentView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`StudentDto`**
  - `Id`: `int`
  - `AdmissionNumber`: `string`
  - `FullName`: `string`
  - `DateOfBirth`: `DateTime`
  - `RollNo`: `string`
  - `AdmissionDate`: `DateTime`
  - `Gender`: `string`
  - `BloodGroup`: `string` | null
  - `Address`: `string` | null
  - `Photo`: `string` | null
  - `ClassId`: `int`
  - `SectionId`: `int`
  - `Guardians`: `StudentGuardianDto`[]

### `POST /api/Students`  
*Permission: `StudentCreate`*

**Form fields (multipart/form-data)** (`request`):

**`CreateStudentDto`**
  - `AdmissionNumber`: `string`
  - `FullName`: `string`
  - `DateOfBirth`: `DateTime`
  - `RollNo`: `string`
  - `AdmissionDate`: `DateTime`
  - `Gender`: `string`
  - `BloodGroup`: `string` | null
  - `Address`: `string` | null
  - `PhotoFile`: `IFormFile` | null
  - `ClassId`: `int`
  - `SectionId`: `int`
  - `Guardians`: `CreateStudentGuardianDto`[]

**Response:**

**`StudentDto`**
  - `Id`: `int`
  - `AdmissionNumber`: `string`
  - `FullName`: `string`
  - `DateOfBirth`: `DateTime`
  - `RollNo`: `string`
  - `AdmissionDate`: `DateTime`
  - `Gender`: `string`
  - `BloodGroup`: `string` | null
  - `Address`: `string` | null
  - `Photo`: `string` | null
  - `ClassId`: `int`
  - `SectionId`: `int`
  - `Guardians`: `StudentGuardianDto`[]

### `PUT /api/Students/{id:int}`  
*Permission: `StudentEdit`*

**Parameters:**
- `id`: `int` (route)

**Form fields (multipart/form-data)** (`request`):

**`UpdateStudentDto`**
  - `Id`: `int`
  - `AdmissionNumber`: `string`
  - `FullName`: `string`
  - `DateOfBirth`: `DateTime`
  - `RollNo`: `string`
  - `AdmissionDate`: `DateTime`
  - `Gender`: `string`
  - `BloodGroup`: `string` | null
  - `Address`: `string` | null
  - `PhotoFile`: `IFormFile` | null
  - `ClassId`: `int`
  - `SectionId`: `int`
  - `Guardians`: `CreateStudentGuardianDto`[]

**Response:**

**`StudentDto`**
  - `Id`: `int`
  - `AdmissionNumber`: `string`
  - `FullName`: `string`
  - `DateOfBirth`: `DateTime`
  - `RollNo`: `string`
  - `AdmissionDate`: `DateTime`
  - `Gender`: `string`
  - `BloodGroup`: `string` | null
  - `Address`: `string` | null
  - `Photo`: `string` | null
  - `ClassId`: `int`
  - `SectionId`: `int`
  - `Guardians`: `StudentGuardianDto`[]

### `DELETE /api/Students/{id:int}`  
*Permission: `StudentDelete`*

**Parameters:**
- `id`: `int` (route)


---


## SubjectTeachersController

### `GET /api/SubjectTeachers`  
*Permission: `SubjectTeacherView`*

**Response:**

**`SubjectTeacherDto`**
  - `SubjectId`: `int`
  - `TeacherId`: `int`

### `GET /api/SubjectTeachers/{subjectId:int}/{teacherId:int}`  
*Permission: `SubjectTeacherView`*

**Parameters:**
- `subjectId`: `int` (route)
- `teacherId`: `int` (route)

**Response:**

**`SubjectTeacherDto`**
  - `SubjectId`: `int`
  - `TeacherId`: `int`

### `POST /api/SubjectTeachers`  
*Permission: `SubjectTeacherAssign`*

**Request body** (`request`):

**`SubjectTeacherDto`**
  - `SubjectId`: `int`
  - `TeacherId`: `int`

**Response:**

**`SubjectTeacherDto`**
  - `SubjectId`: `int`
  - `TeacherId`: `int`

### `DELETE /api/SubjectTeachers/{subjectId:int}/{teacherId:int}`  
*Permission: `SubjectTeacherRemove`*

**Parameters:**
- `subjectId`: `int` (route)
- `teacherId`: `int` (route)


---


## SubjectsController

### `GET /api/Subjects`  
*Permission: `SubjectView`*

**Response:**

**`SubjectDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Code`: `string`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

### `GET /api/Subjects/{id:int}`  
*Permission: `SubjectView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`SubjectDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Code`: `string`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

### `POST /api/Subjects`  
*Permission: `SubjectCreate`*

**Request body** (`request`):

**`CreateSubjectDto`**
  - `Name`: `string`
  - `Code`: `string`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

**Response:**

**`SubjectDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Code`: `string`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

### `PUT /api/Subjects/{id:int}`  
*Permission: `SubjectEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateSubjectDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Code`: `string`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

**Response:**

**`SubjectDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Code`: `string`
  - `FullMarks`: `int`
  - `PassMarks`: `int`

### `DELETE /api/Subjects/{id:int}`  
*Permission: `SubjectDelete`*

**Parameters:**
- `id`: `int` (route)


---


## TeachersController

### `GET /api/Teachers`  
*Permission: `TeacherView`*

**Response:**

**`TeacherDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `Qualification`: `string` | null
  - `Specialization`: `string` | null

### `GET /api/Teachers/{id:int}`  
*Permission: `TeacherView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`TeacherDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `Qualification`: `string` | null
  - `Specialization`: `string` | null

### `POST /api/Teachers`  
*Permission: `TeacherCreate`*

**Request body** (`request`):

**`CreateTeacherDto`**
  - `EmployeeId`: `int`
  - `Qualification`: `string` | null
  - `Specialization`: `string` | null

**Response:**

**`TeacherDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `Qualification`: `string` | null
  - `Specialization`: `string` | null

### `PUT /api/Teachers/{id:int}`  
*Permission: `TeacherEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateTeacherDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `Qualification`: `string` | null
  - `Specialization`: `string` | null

**Response:**

**`TeacherDto`**
  - `Id`: `int`
  - `EmployeeId`: `int`
  - `Qualification`: `string` | null
  - `Specialization`: `string` | null

### `DELETE /api/Teachers/{id:int}`  
*Permission: `TeacherDelete`*

**Parameters:**
- `id`: `int` (route)


---


## TranscriptController

### `GET /api/Transcript/student/{studentId:int}`  
*Permission: `TranscriptView`*

**Parameters:**
- `studentId`: `int` (route)

**Response:**

**`TranscriptDto`**
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `RollNo`: `string`
  - `ClassName`: `string`
  - `SectionName`: `string`
  - `Summary`: `TranscriptSummaryDto`
  - `ExamHistory`: `TranscriptExamEntryDto`[]
  - `YearSummaries`: `TranscriptYearSummaryDto`[]
  - `GpaHistory`: `GpaHistoryPointDto`[]
  - `PositionHistory`: `PositionHistoryPointDto`[]
  - `AttendanceSummary`: `TranscriptAttendanceSummaryDto`
  - `GeneratedAt`: `DateTime`

### `GET /api/Transcript/student/{studentId:int}/academic-year/{academicYearId:int}`  
*Permission: `TranscriptView`*

**Parameters:**
- `studentId`: `int` (route)
- `academicYearId`: `int` (route)

**Response:**

**`TranscriptDto`**
  - `StudentId`: `int`
  - `StudentName`: `string`
  - `RollNo`: `string`
  - `ClassName`: `string`
  - `SectionName`: `string`
  - `Summary`: `TranscriptSummaryDto`
  - `ExamHistory`: `TranscriptExamEntryDto`[]
  - `YearSummaries`: `TranscriptYearSummaryDto`[]
  - `GpaHistory`: `GpaHistoryPointDto`[]
  - `PositionHistory`: `PositionHistoryPointDto`[]
  - `AttendanceSummary`: `TranscriptAttendanceSummaryDto`
  - `GeneratedAt`: `DateTime`

### `GET /api/Transcript/student/{studentId:int}/pdf`  
*Permission: `TranscriptView`*

**Parameters:**
- `studentId`: `int` (route)

### `GET /api/Transcript/student/{studentId:int}/academic-year/{academicYearId:int}/pdf`  
*Permission: `TranscriptView`*

**Parameters:**
- `studentId`: `int` (route)
- `academicYearId`: `int` (route)


---


## UserController

### `GET /api/User`  
*Permission: `UserView`*

**Response:**

**`UserDto`**
  - `Id`: `int`
  - `Username`: `string`
  - `Email`: `string`
  - `IsActive`: `bool`

### `GET /api/User/{id:int}`  
*Permission: `UserView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`UserDto`**
  - `Id`: `int`
  - `Username`: `string`
  - `Email`: `string`
  - `IsActive`: `bool`

### `POST /api/User`  
*Permission: `UserCreate`*

**Request body** (`request`):

**`CreateUserDto`**
  - `Username`: `string`
  - `Email`: `string`
  - `IsActive`: `bool`
  - `Password`: `string`

**Response:**

**`UserDto`**
  - `Id`: `int`
  - `Username`: `string`
  - `Email`: `string`
  - `IsActive`: `bool`

### `PUT /api/User/{id:int}`  
*Permission: `UserEdit`*

**Parameters:**
- `id`: `int` (route)

**Request body** (`request`):

**`UpdateUserDto`**
  - `Id`: `int`
  - `Username`: `string`
  - `Email`: `string`
  - `IsActive`: `bool`
  - `Password`: `string`

**Response:**

**`UserDto`**
  - `Id`: `int`
  - `Username`: `string`
  - `Email`: `string`
  - `IsActive`: `bool`

### `DELETE /api/User/{id:int}`  
*Permission: `UserDelete`*

**Parameters:**
- `id`: `int` (route)

### `GET /api/User/{id:int}/roles`  
*Permission: `UserRoleView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`RoleDto`**
  - `Id`: `int`
  - `Name`: `string`
  - `Description`: `string` | null

### `GET /api/User/{id:int}/permissions`  
*Permission: `UserRoleView`*

**Parameters:**
- `id`: `int` (route)

**Response:**

**`PermissionDto`**
  - `Id`: `int`
  - `Name`: `string`

### `POST /api/User/assign-role`  
*Permission: `UserRoleAssign`*

**Request body** (`request`):

**`AssignRoleToUserDto`**
  - `UserId`: `int`
  - `RoleId`: `int`

### `DELETE /api/User/{userId:int}/roles/{roleId:int}`  
*Permission: `UserRoleRemove`*

**Parameters:**
- `userId`: `int` (route)
- `roleId`: `int` (route)


---
