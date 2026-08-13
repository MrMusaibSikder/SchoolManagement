using SchoolERP.Application.Features.ClassSubject.Interfaces;
using SchoolERP.Application.Features.Exam.Interfaces;
using SchoolERP.Application.Features.ExamResult.Interfaces;
using SchoolERP.Application.Features.ProgressReport.DTOs;
using SchoolERP.Application.Features.ProgressReport.Interfaces;
using SchoolERP.Application.Features.Student.Interfaces;
using SchoolERP.Application.Features.Subject.Interfaces;
using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Services
{
    public class ProgressReportService: IProgressReportService
    {
        private readonly IExamService _examService;
        private readonly IExamResultService _examResultService;
        private readonly IStudentRepository _studentRepository;
        private readonly IClassSubjectService _classSubjectService;
        private readonly ISubjectService _subjectService;

        public ProgressReportService(
            IExamService examService,
            IExamResultService examResultService,
            IStudentRepository studentRepository,
            IClassSubjectService classSubjectService,
            ISubjectService subjectService)
        {
            _examService = examService;
            _examResultService = examResultService;
            _studentRepository = studentRepository;
            _classSubjectService = classSubjectService;
            _subjectService = subjectService;
        }

        public async Task<ProgressReportDto> GetStudentProgressReportAsync(
            int studentId, int academicYearId, CancellationToken cancellationToken = default)
        {
            var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken)
                ?? throw new KeyNotFoundException($"Student {studentId} not found.");

            // 1. This academic year-s exam গুলো বের করা (শুধু Published/Completed, Draft/Cancelled বাদ)
            var allExams = await _examService.GetAllAsync(cancellationToken);
            var yearExams = allExams
                .Where(e => e.AcademicYearId == academicYearId
                            && (e.Status == ExamStatus.Completed || e.Status == ExamStatus.Published))
                .OrderBy(e => e.Id)
                .ToList();

            var examColumns = yearExams
                .Select(e => new ProgressReportExamColumnDto
                {
                    ExamId = e.Id,
                    ExamName = e.Name
                })
                .ToList();

            // 2. ছাত্রের ক্লাসের subject list (IsOptional সহ) — এটাই row-এর ভিত্তি
            var classSubjects = (await _classSubjectService.GetAllAsync(cancellationToken))
                .Where(cs => cs.ClassId == student.ClassId)
                .ToList();

            var allSubjects = await _subjectService.GetAllAsync(cancellationToken);
            var subjectNameLookup = allSubjects.ToDictionary(s => s.Id, s => s.Name);

            // 3. প্রতিটা exam-এ ছাত্রের result টেনে আনা (না থাকলে null রাখা — absent হিসেবে বোঝাবে)
            var resultsByExam = new Dictionary<int, IReadOnlyList<SchoolERP.Application.Features.ExamResult.DTOs.ExamResultDetailDto>>();
            string? studentName = null, rollNo = null, className = null, sectionName = null;

            foreach (var exam in yearExams)
            {
                try
                {
                    var studentResult = await _examResultService.GetStudentResultAsync(studentId, exam.Id, cancellationToken);
                    resultsByExam[exam.Id] = studentResult.Subjects;

                    // হেডার তথ্য যেকোনো একটা পাওয়া result থেকে নিয়ে নেওয়া (accurate, already enriched)
                    studentName ??= studentResult.Summary.StudentName;
                    rollNo ??= studentResult.Summary.RollNo;
                    className ??= studentResult.Summary.ClassName;
                    sectionName ??= studentResult.Summary.SectionName;
                }
                catch (KeyNotFoundException)
                {
                    // এই exam-এ ছাত্রের কোনো result নেই — absent হিসেবে গণ্য হবে
                    resultsByExam[exam.Id] = Array.Empty<SchoolERP.Application.Features.ExamResult.DTOs.ExamResultDetailDto>();
                }
            }

            // 4. Subject × Exam ম্যাট্রিক্স বানানো
            var subjectRows = new List<ProgressReportSubjectRowDto>();

            foreach (var cs in classSubjects)
            {
                var marksPerExam = new List<decimal?>();
                var gradePerExam = new List<string?>();

                foreach (var exam in yearExams)
                {
                    var subjectResult = resultsByExam[exam.Id]
                        .FirstOrDefault(s => s.SubjectId == cs.SubjectId);

                    if (subjectResult == null)
                    {
                        marksPerExam.Add(null);
                        gradePerExam.Add("Absent");
                    }
                    else
                    {
                        marksPerExam.Add(subjectResult.MarksObtained);
                        gradePerExam.Add(subjectResult.Grade);
                    }
                }

                subjectRows.Add(new ProgressReportSubjectRowDto
                {
                    SubjectId = cs.SubjectId,
                    SubjectName = subjectNameLookup.TryGetValue(cs.SubjectId, out var name) ? name : "Unknown",
                    IsOptional = cs.IsOptional,
                    MarksPerExam = marksPerExam,
                    GradePerExam = gradePerExam
                });
            }

            return new ProgressReportDto
            {
                StudentId = student.Id,
                StudentName = studentName ?? student.FullName,
                RollNo = rollNo ?? student.RollNo,
                ClassName = className ?? string.Empty,
                SectionName = sectionName ?? string.Empty,
                AcademicYearId = academicYearId,
                AcademicYearName = yearExams.FirstOrDefault()?.AcademicYearName ?? string.Empty,
                Exams = examColumns,
                Subjects = subjectRows
            };
        }
    }
}