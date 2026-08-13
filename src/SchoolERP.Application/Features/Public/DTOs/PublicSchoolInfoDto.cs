using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Public.DTOs
{
    public class PublicSchoolInfoDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? LogoUrl { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }
}
