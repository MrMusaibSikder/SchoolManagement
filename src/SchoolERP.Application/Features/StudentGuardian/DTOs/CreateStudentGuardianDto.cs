using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.StudentGuardian.DTOs
{
    public class CreateStudentGuardianDto
    {
        public int GuardianId { get; set; }

        public string Relation { get; set; } = string.Empty;
    }
}
