using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Transcript.Interfaces
{
    public interface ITranscriptPdfService
    {
        //Created by Musaib Sikder
        Task<byte[]> GenerateStudentTranscriptPdfAsync(
      int studentId,
      CancellationToken cancellationToken = default);

        Task<byte[]> GenerateAcademicYearTranscriptPdfAsync(
            int studentId,
            int academicYearId,
            CancellationToken cancellationToken = default);
    }
}
