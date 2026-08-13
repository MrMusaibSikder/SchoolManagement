using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Common.Interfaces
{
    public interface IFileService
    {

        Task<string?> UploadAsync(
        Stream fileStream,
        string fileName,
        string folderName,
        string contentType,
        long fileSize,
        CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(string? relativePath);

        Task<string?> ReplaceAsync(
            Stream? newFileStream,
            string? oldPath,
            string fileName,
            string folderName,
            string contentType,
            long fileSize,
            CancellationToken cancellationToken = default);
        //This make for Pdf making it read the file and make bytes

        Task<byte[]?> ReadAsync(
    string? relativePath,
    CancellationToken cancellationToken = default);
    }
}
