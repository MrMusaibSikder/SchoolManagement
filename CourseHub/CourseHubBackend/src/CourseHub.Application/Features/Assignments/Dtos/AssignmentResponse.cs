using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Application.Features.Assignments.Dtos
{
    public record AssignmentResponse(
     Guid Id,
     Guid CourseId,
     Guid? CreatedByTeacherId,
     string Title,
     string? Description,
     int MaxMarks,
     DateTime DueDate,
     bool IsActive,
     DateTime CreatedAt,
     DateTime? UpdatedAt);
}
