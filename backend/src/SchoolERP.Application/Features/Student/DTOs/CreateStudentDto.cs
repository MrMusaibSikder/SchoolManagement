using Microsoft.AspNetCore.Http;
using SchoolERP.Application.Features.StudentGuardian.DTOs;

namespace SchoolERP.Application.Features.Student.DTOs;

/// <summary>Input model for creating a new Student record.</summary>
public class CreateStudentDto
{
    public string AdmissionNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string RollNo { get; set; } = string.Empty;
    public DateTime AdmissionDate { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? BloodGroup { get; set; }
    public string? Address { get; set; }
    public IFormFile? PhotoFile { get; set; }
    public int ClassId { get; set; }
    public int SectionId { get; set; }
    public List<CreateStudentGuardianDto> Guardians { get; set; }
       = new();
}
