using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Application.Common.Interfaces
{
    /// <summary>
    /// Abstraction over "wherever uploaded files actually live" — local disk
    /// today (see LocalDiskFileStorageService), swappable for cloud storage
    /// (S3/Azure Blob) later without touching any calling code. Callers
    /// never construct file paths themselves; they always go through this.
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Saves a file and returns an opaque storage key (NOT a file path
        /// or URL) — callers persist this key in the database and pass it
        /// back to GetAsync/DeleteAsync later. The key deliberately reveals
        /// nothing about the underlying storage layout.
        /// </summary>
        Task<string> SaveAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default);

        Task<FileDownloadResult> GetAsync(string storageKey, CancellationToken cancellationToken = default);

        Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
    }

    public record FileDownloadResult(Stream Stream, string ContentType, string FileName);
}
