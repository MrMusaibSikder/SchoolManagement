using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Common.Exceptions
{
    /// <summary>
    /// Thrown when the current user lacks authentication context
    /// (e.g., no EmployeeId resolved from claims).
    /// Typically translated to an HTTP 401 by the API's exception middleware.
    /// </summary>
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message)
        {
        }
    }
}
