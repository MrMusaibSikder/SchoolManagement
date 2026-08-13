using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Common.Models
{
    /// <summary>
    /// Configuration for outbound SMTP email delivery, 
    /// bound from the "Email" section.
    /// Add By Musaib
    /// </summary>
    public class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromAddress { get; set; } = string.Empty;
        public string FromName { get; set; } = "SchoolERP";
        public bool EnableSsl { get; set; } = true;
    }
}
