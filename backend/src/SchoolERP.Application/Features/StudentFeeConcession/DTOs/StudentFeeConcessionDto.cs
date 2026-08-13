using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.StudentFeeConcession.DTOs
{
    /// <summary>
    /// Read model returned to clients for a StudentFeeConcession record.
    /// </summary>
    public class StudentFeeConcessionDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int FeeTypeId { get; set; }
        public string FeeTypeName { get; set; } = string.Empty;
        public int AcademicYearId { get; set; }
        public string AcademicYearName { get; set; } = string.Empty;
        public ConcessionType Type { get; set; }
        public decimal? Value { get; set; }
        public string Reason { get; set; } = string.Empty;
        public bool RequiresApproval { get; set; }
        public bool IsApproved { get; set; }
        public int? ApprovedByEmployeeId { get; set; }
        public string? ApprovedByEmployeeName { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
