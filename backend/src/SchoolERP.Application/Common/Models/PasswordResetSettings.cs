using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Common.Models
{
    /// <summary>
    /// Configuration for the Forgot Password / Reset Password flow,
    /// bound from the "PasswordReset" section.
    /// </summary>
    public class PasswordResetSettings
    {
        /// <summary>
        /// How long a generated reset token remains valid.
        /// </summary>
        public int TokenExpiryMinutes { get; set; } = 30;
    }
}
