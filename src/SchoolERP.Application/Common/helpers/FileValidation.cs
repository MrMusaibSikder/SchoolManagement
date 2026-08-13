using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Common.helpers
{
    public static class FileValidation
    {
        public static readonly string[] AllowedImageTypes =
    [
        "image/jpeg",
    "image/png"
    
    ];

        public static bool IsValidImage(string contentType)
        {
            return AllowedImageTypes.Contains(contentType.ToLower());
        }

        public static bool IsValidSize(long size, int maxMB = 5)
        {
            return size <= maxMB * 1024 * 1024;
        }
    }
}
