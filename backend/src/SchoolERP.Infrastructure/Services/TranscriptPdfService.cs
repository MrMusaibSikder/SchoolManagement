using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.School.DTOs;
using SchoolERP.Application.Features.School.Interfaces;
using SchoolERP.Application.Features.Student.Interfaces;
using SchoolERP.Application.Features.Transcript.DTOs;
using SchoolERP.Application.Features.Transcript.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Services
{
    public class TranscriptPdfService:ITranscriptPdfService
    {

        private readonly ITranscriptService _transcriptService;
        private readonly ISchoolService _schoolService;
        private readonly IStudentRepository _studentRepository;
        private readonly IFileService _fileService;

        public TranscriptPdfService(
            ITranscriptService transcriptService,
            ISchoolService schoolService,
            IStudentRepository studentRepository,
           IFileService fileService)
        {
            _transcriptService = transcriptService;
            _schoolService = schoolService;
            _studentRepository = studentRepository;
            _fileService = fileService;
        }

        public async Task<byte[]> GenerateStudentTranscriptPdfAsync(
            int studentId,
            CancellationToken cancellationToken = default)
        {
            var dto = await _transcriptService.GetStudentTranscriptAsync(studentId, cancellationToken);
            return await BuildPdfAsync(dto, studentId, cancellationToken);
        }

        public async Task<byte[]> GenerateAcademicYearTranscriptPdfAsync(
            int studentId,
            int academicYearId,
            CancellationToken cancellationToken = default)
        {
            var dto = await _transcriptService.GetAcademicYearTranscriptAsync(studentId, academicYearId, cancellationToken);
            return await BuildPdfAsync(dto, studentId, cancellationToken);
        }

        // ---------- shared build logic ----------

        private async Task<byte[]> BuildPdfAsync(TranscriptDto dto, int studentId, CancellationToken cancellationToken)
        {
            var schools = await _schoolService.GetAllAsync(cancellationToken);
            var school = schools.FirstOrDefault();

            var student = await _studentRepository.GetByIdAsync(studentId, cancellationToken);

            var logoBytes = await _fileService.ReadAsync(
                school?.Logo,
               cancellationToken);

            var photoBytes = await _fileService.ReadAsync(
                student?.Photo,
                cancellationToken);

            QuestPDF.Settings.License = LicenseType.Community;

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Element(c => ComposeHeader(c, school, logoBytes));
                    page.Content().Element(c => ComposeContent(c, dto, photoBytes));
                    page.Footer().Element(ComposeFooter);
                });
            }).GeneratePdf();

            return pdfBytes;
        }

       

        // ---------- layout sections ----------

        private void ComposeHeader(QuestPDF.Infrastructure.IContainer container, SchoolDto? school, byte[]? logoBytes)
        {
            container.Row(row =>
            {
                if (logoBytes != null)
                {
                    row.ConstantItem(60).Height(60).Image(logoBytes).FitArea();
                }

                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(school?.Name ?? "School Name").FontSize(16).Bold();
                    if (!string.IsNullOrWhiteSpace(school?.Address))
                        col.Item().Text(school.Address).FontSize(9);

                    var contactLine = string.Join(
                        "  |  ",
                        new[] { school?.Phone, school?.Email, school?.EIIN != null ? $"EIIN: {school.EIIN}" : null }
                            .Where(x => !string.IsNullOrWhiteSpace(x)));

                    if (!string.IsNullOrWhiteSpace(contactLine))
                        col.Item().Text(contactLine).FontSize(8);

                    col.Item().PaddingTop(4).Text("ACADEMIC TRANSCRIPT").FontSize(12).Bold();
                });
            });
        }

        private void ComposeContent(QuestPDF.Infrastructure.IContainer container, TranscriptDto dto, byte[]? photoBytes)
        {
            container.PaddingTop(10).Column(column =>
            {
                column.Spacing(10);

                // Student info + photo
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(info =>
                    {
                        info.Item().Text($"Name: {dto.StudentName}").Bold();
                        info.Item().Text($"Roll: {dto.RollNo}    Class: {dto.ClassName}    Section: {dto.SectionName}");
                        info.Item().Text($"Generated: {dto.GeneratedAt:dd MMM yyyy}");
                    });

                    if (photoBytes != null)
                    {
                        row.ConstantItem(70).Height(80).Image(photoBytes).FitArea();
                    }
                });

                // Summary
                column.Item().Border(1).Padding(8).Row(row =>
                {
                    row.RelativeItem().Text($"CGPA: {dto.Summary.CGPA:0.00}").Bold();
                    row.RelativeItem().Text($"Highest Year GPA: {dto.Summary.HighestYearGPA:0.00}");
                    row.RelativeItem().Text($"Lowest Year GPA: {dto.Summary.LowestYearGPA:0.00}");
                    row.RelativeItem().Text($"Result: {(dto.Summary.OverallPassed ? "PASSED" : "NOT PASSED")}");
                });

                // Year summaries
                foreach (var year in dto.YearSummaries)
                {
                    column.Item().PaddingTop(6).Text(
                        $"{year.AcademicYearName} — GPA: {year.FinalGPA:0.00} ({year.FinalGrade})   " +
                        $"Position: Class {year.ClassPosition?.ToString() ?? "-"} / " +
                        $"Section {year.SectionPosition?.ToString() ?? "-"} / " +
                        $"Merit {year.MeritPosition?.ToString() ?? "-"}"
                    ).Bold();

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Subject").Bold();
                            header.Cell().Text("Marks").Bold();
                            header.Cell().Text("Grade").Bold();
                            header.Cell().Text("GPA").Bold();
                        });

                        foreach (var subject in year.Subjects)
                        {
                            table.Cell().Text(subject.SubjectName + (subject.IsOptional ? " (Opt)" : ""));
                            table.Cell().Text(subject.MarksObtained.ToString("0.##"));
                            table.Cell().Text(subject.Grade);
                            table.Cell().Text(subject.GPA.ToString("0.00"));
                        }
                    });

                    if (!string.IsNullOrWhiteSpace(year.TeacherRemarks))
                        column.Item().Text($"Class Teacher's Remarks: {year.TeacherRemarks}").FontSize(9);

                    if (!string.IsNullOrWhiteSpace(year.PrincipalRemarks))
                        column.Item().Text($"Principal's Remarks: {year.PrincipalRemarks}").FontSize(9);
                }

                // Attendance
                var att = dto.AttendanceSummary;
                column.Item().PaddingTop(6).Border(1).Padding(8).Row(row =>
                {
                    row.RelativeItem().Text($"Total Days: {att.TotalDays}");
                    row.RelativeItem().Text($"Present: {att.PresentDays}");
                    row.RelativeItem().Text($"Absent: {att.AbsentDays}");
                    row.RelativeItem().Text($"Attendance: {att.AttendancePercentage:0.0}%");
                });

                // Signature boxes
                column.Item().PaddingTop(30).Row(row =>
                {
                    row.RelativeItem().AlignCenter().Text("_____________________\nClass Teacher");
                    row.RelativeItem().AlignCenter().Text("_____________________\nPrincipal");
                });
            });
        }

        private void ComposeFooter(QuestPDF.Infrastructure.IContainer container)
        {
            container.AlignCenter().Text(text =>
            {
                text.CurrentPageNumber();
                text.Span(" / ");
                text.TotalPages();
            });
        }
    }
}
    