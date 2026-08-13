using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SchoolERP.Application.Common.helpers;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SchoolERP.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly string _rootPath;

        public FileService(IOptions<FileStorageSettings> options)
        {
            _rootPath = options.Value.RootPath;
        }

       
    public async Task<string?> UploadAsync(
    Stream fileStream,
    string originalFileName,
    string folderName,
    string contentType,
    long fileSize,
    CancellationToken cancellationToken = default)
        {
            // Validation
            if (fileStream == null || fileStream.Length == 0)
                throw new Exception("File is empty");

            if (!FileValidation.IsValidImage(contentType))
                throw new Exception("Invalid file type");

            if (!FileValidation.IsValidSize(fileSize))
                throw new Exception("File size exceeds 5MB");

            // Folder
            var folderPath = Path.Combine(_rootPath, folderName);
            Directory.CreateDirectory(folderPath);

            // Generate Unique File Name
            var extension = Path.GetExtension(originalFileName);
            var fileName = $"{Guid.NewGuid()}{extension}";

            // Save
            var fullPath = Path.Combine(folderPath, fileName);

            using var file = new FileStream(fullPath, FileMode.Create);
            await fileStream.CopyToAsync(file, cancellationToken);

            return $"/uploads/{folderName}/{fileName}";
        }

        public Task<bool> DeleteAsync(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return Task.FromResult(true);

            var fullPath = Path.Combine(_rootPath, relativePath.Replace("uploads/", ""));

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            return Task.FromResult(true);
        }

        public async Task<string?> ReplaceAsync(
      Stream? newFileStream,
      string? oldPath,
      string originalFileName,
      string folderName,
      string contentType,
      long fileSize,
      CancellationToken cancellationToken = default)
        {
            if (newFileStream == null)
                return oldPath;

            var newPath = await UploadAsync(
                newFileStream,
                originalFileName,
                folderName,
                contentType,
                fileSize,
                cancellationToken);

            if (!string.IsNullOrWhiteSpace(oldPath))
                await DeleteAsync(oldPath);

            return newPath;
        }

        public async Task<byte[]?> ReadAsync(string? relativePath, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            var physicalPath = Path.Combine(_rootPath, relativePath.TrimStart('/', '\\').Replace("uploads/", "").Replace("uploads\\", ""));

            if (!File.Exists(physicalPath))
                return null;

            return await File.ReadAllBytesAsync(physicalPath, cancellationToken);
        }
    }
}
