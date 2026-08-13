using SchoolERP.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Common.Interfaces.Services
{
    public interface ICurrentEmployeeService
    {
        /// <summary>
        /// Returns the current employee (Id + Name). Cached per request.
        /// </summary>
        Task<EmployeeContext> GetAsync(CancellationToken ct = default);

        /// <summary>
        /// Just the Id — shorthand when you don't need Name.
        /// </summary>
        Task<int> GetIdAsync(CancellationToken ct = default);
    }
}
