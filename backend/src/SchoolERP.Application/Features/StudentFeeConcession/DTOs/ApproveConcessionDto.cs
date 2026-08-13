using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.StudentFeeConcession.DTOs
{
    /// <summary>
    /// Input model for approving a pending concession.
    /// </summary>
    public class ApproveConcessionDto
    {
        public int ConcessionId { get; set; }
    }
}
