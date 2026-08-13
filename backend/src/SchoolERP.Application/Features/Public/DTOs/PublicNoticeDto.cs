using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Features.Public.DTOs
{
    public class PublicNoticeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public DateTime PublishDate { get; set; }
        public string Priority { get; set; } = string.Empty;
    }
}
