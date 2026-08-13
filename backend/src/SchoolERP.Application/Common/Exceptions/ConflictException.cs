using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Common.Exceptions
{
    /// <summary>
    /// Thrown when an operation conflicts with the current state of a resource —
    /// typically an optimistic concurrency conflict (RowVersion mismatch) where
    /// another user modified the same record between read and save.
    /// Maps to HTTP 409 Conflict in the API layer.
    /// </summary>
    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }

        public ConflictException(string message, Exception innerException) : base(message, innerException) { }
    }
}
