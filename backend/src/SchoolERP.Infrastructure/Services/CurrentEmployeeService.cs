using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Services
{
    public class CurrentEmployeeService : ICurrentEmployeeService
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IUnitOfWork _unitOfWork;
        private EmployeeContext? _cache;

        public CurrentEmployeeService(ICurrentUserService currentUser, IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<EmployeeContext> GetAsync(CancellationToken ct = default)
        {
            if (_cache is not null) return _cache;

            var userId = _currentUser.UserId
                ?? throw new UnauthorizedException("User not authenticated.");

            var employee = await _unitOfWork.EmployeeRepository.GetByUserIdAsync(userId, ct)
                ?? throw new UnauthorizedException("Current user is not linked to an employee record.");

            _cache = new EmployeeContext(employee.Id, employee.FullName);
            return _cache;
        }

        public async Task<int> GetIdAsync(CancellationToken ct = default)
        {
            var emp = await GetAsync(ct);
            return emp.Id;
        }
    }
}
