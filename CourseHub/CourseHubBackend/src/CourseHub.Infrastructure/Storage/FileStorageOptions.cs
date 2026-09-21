using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseHub.Infrastructure.Storage
{
    public class FileStorageOptions
    {
        public const string SectionName = "FileStorage";

        /// <summary>
        /// Absolute path where uploaded files are stored. In Docker this
        /// should point at a mounted volume (so files survive container
        /// restarts) — see docker-compose.yml.
        /// </summary>
        public string RootPath { get; set; } = "uploads";

        public long MaxFileSizeBytes { get; set; } = 10 * 1024 * 1024; // 10 MB

        public string[] AllowedExtensions { get; set; } = { ".pdf", ".txt" };
    }
}
