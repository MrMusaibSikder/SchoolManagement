using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Application.Features.Assignments.Dtos
{
    /// <summary>
    /// Exactly one of TextContent or File must be provided — enforced in
    /// MeService.SubmitAssignmentAsync, not here (this DTO just carries
    /// whichever the multipart form actually sent).
    /// </summary>
    public record SubmitTextAssignmentRequest(
      string TextContent);
}
