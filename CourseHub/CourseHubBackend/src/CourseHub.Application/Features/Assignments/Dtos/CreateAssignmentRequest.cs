using CourseHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Application.Features.Assignments.Dtos
{
    public record CreateAssignmentRequest(
     Guid CourseId,
     Guid? CreatedByTeacherId,
     string Title,
     string? Description,
     int MaxMarks,
     DateTime DueDate);
}
