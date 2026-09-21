using CourseHub.Application.Common.Interfaces;
using CourseHub.Domain.Exceptions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Infrastructure.Storage
{
    public class LocalDiskFileStorageService : IFileStorageService
    {
        private readonly string _rootPath;
        private readonly long _maxFileSizeBytes;
        private readonly string[] _allowedExtensions;

        public LocalDiskFileStorageService(IOptions<FileStorageOptions> options)
        {
            var storageOptions = options.Value;

            _rootPath = storageOptions.RootPath;
            _maxFileSizeBytes = storageOptions.MaxFileSizeBytes;
            _allowedExtensions = storageOptions.AllowedExtensions
                .Select(x => x.ToLowerInvariant())
                .ToArray();

            Directory.CreateDirectory(_rootPath);
        }

        public async Task<string> SaveAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            if (fileStream is null)
                throw new ValidationException("File is required.");

            if (!fileStream.CanRead)
                throw new ValidationException("The uploaded file cannot be read.");

            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (!_allowedExtensions.Contains(extension))
                throw new ValidationException(
                    $"File type '{extension}' is not allowed.");

            if (fileStream.Length > _maxFileSizeBytes)
                throw new ValidationException(
                    $"File size cannot exceed {_maxFileSizeBytes / (1024 * 1024)} MB.");

            var safeFileName = Path.GetFileName(fileName);

            var storageKey = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(_rootPath, storageKey);

            await using var destination = File.Create(fullPath);

            await fileStream.CopyToAsync(
                destination,
                cancellationToken);

            return storageKey;
        }

        public Task<FileDownloadResult> GetAsync(
            string storageKey,
            CancellationToken cancellationToken = default)
        {
            var fullPath = ResolveSafePath(storageKey);

            if (!File.Exists(fullPath))
                throw new NotFoundException("File", storageKey);

            var stream = File.OpenRead(fullPath);

            var contentType = storageKey.EndsWith(
                ".pdf",
                StringComparison.OrdinalIgnoreCase)
                ? "application/pdf"
                : "text/plain";

            return Task.FromResult(
                new FileDownloadResult(
                    stream,
                    contentType,
                    storageKey));
        }

        public Task DeleteAsync(
            string storageKey,
            CancellationToken cancellationToken = default)
        {
            var fullPath = ResolveSafePath(storageKey);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.CompletedTask;
        }

        private string ResolveSafePath(string storageKey)
        {
            var rootPath = Path.GetFullPath(_rootPath);

            var fullPath = Path.GetFullPath(
                Path.Combine(rootPath, storageKey));

            if (!fullPath.StartsWith(
                    rootPath + Path.DirectorySeparatorChar,
                    StringComparison.OrdinalIgnoreCase)
                && !string.Equals(
                    fullPath,
                    rootPath,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ValidationException("Invalid file reference.");
            }

            return fullPath;
        }
    }
}
