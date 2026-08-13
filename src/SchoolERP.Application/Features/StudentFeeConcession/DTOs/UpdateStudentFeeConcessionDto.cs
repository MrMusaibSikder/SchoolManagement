using SchoolERP.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.StudentFeeConcession.DTOs
{
    /// <summary>
    /// Input model for updating an existing StudentFeeConcession.
    /// </summary>
    public class UpdateStudentFeeConcessionDto
    {
        public int Id { get; set; }
        public ConcessionType Type { get; set; }
        public decimal? Value { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool IsActive { get; set; }
    }
}
