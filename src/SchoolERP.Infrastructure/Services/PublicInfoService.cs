using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Public.DTOs;
using SchoolERP.Application.Features.Public.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Infrastructure.Services
{
    public class PublicInfoService : IPublicInfoService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PublicInfoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PublicSchoolInfoDto> GetSchoolInfoAsync(CancellationToken cancellationToken = default)
        {
            var schools = await _unitOfWork.SchoolRepository.GetAllAsync(cancellationToken);
            var school = schools.FirstOrDefault()
                ?? throw new NotFoundException(nameof(SchoolERP.Domain.Entities.School), 0);

            return new PublicSchoolInfoDto
            {
                Name = school.Name,
                Address = school.Address,
                //  নিচের ৩টা প্রপার্টি নাম আমি guess করছি — আপনার আসল School entity-তে
                // এই নামেই আছে কিনা মিলিয়ে নিন, না মিললে compile error দেবে
                LogoUrl = school.Logo,
                Phone = school.Phone,
                Email = school.Email
            };
        }

        public async Task<PublicStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
        {
            //  GenericRepository.CountAsync() আগে থেকেই আছে (soft-delete filter সহ) —
            // এর জন্য কোনো নতুন repository method লাগছে না
            return new PublicStatsDto
            {
                TotalStudents = await _unitOfWork.StudentRepository.CountAsync(cancellationToken),
                TotalTeachers = await _unitOfWork.TeacherRepository.CountAsync(cancellationToken),
                TotalEmployees = await _unitOfWork.EmployeeRepository.CountAsync(cancellationToken)
            };
        }

        public async Task<IReadOnlyList<PublicNoticeDto>> GetPublicNoticesAsync(int take = 5, CancellationToken cancellationToken = default)
        {
            var notices = await _unitOfWork.NoticeRepository.GetPublicPublishedAsync(take, cancellationToken);

            return notices.Select(n => new PublicNoticeDto
            {
                Id = n.Id,
                Title = n.Title,
                Summary = n.Description.Length > 150 ? n.Description[..150] + "..." : n.Description,  // ✅ Content → Description
                PublishDate = n.PublishDate,
                Priority = n.Priority.ToString()
            }).ToList();
        }
    }
}

