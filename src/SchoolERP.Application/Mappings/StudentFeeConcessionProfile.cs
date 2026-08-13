using AutoMapper;
using SchoolERP.Application.Features.StudentFeeConcession.DTOs;
using SchoolERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Mappings
{
    public class StudentFeeConcessionProfile : Profile
    {
        public StudentFeeConcessionProfile()
        {
            // Entity → DTO (needs .Include() for Student, FeeType, AcademicYear, ApprovedByUser)
            CreateMap<StudentFeeConcession, StudentFeeConcessionDto>()
                .ForMember(dest => dest.StudentName,
                           opt => opt.MapFrom(src => src.Student.FullName))
                .ForMember(dest => dest.FeeTypeName,
                           opt => opt.MapFrom(src => src.FeeType.Name))
                .ForMember(dest => dest.AcademicYearName,
                           opt => opt.MapFrom(src => src.AcademicYear.Name))
                .ForMember(dest => dest.ApprovedByEmployeeName,
                           opt => opt.MapFrom(src => src.ApprovedByEmployee != null ? src.ApprovedByEmployee.FullName : null));

            CreateMap<StudentFeeConcession, StudentFeeConcessionListDto>()
                .ForMember(dest => dest.StudentName,
                           opt => opt.MapFrom(src => src.Student.FullName))
                .ForMember(dest => dest.FeeTypeName,
                           opt => opt.MapFrom(src => src.FeeType.Name))
                .ForMember(dest => dest.AcademicYearName,
                           opt => opt.MapFrom(src => src.AcademicYear.Name));

            // DTO → Entity (Create)
            CreateMap<CreateStudentFeeConcessionDto, StudentFeeConcession>()
                .ForMember(dest => dest.IsApproved, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore());

            // DTO → Entity (Update)
            CreateMap<UpdateStudentFeeConcessionDto, StudentFeeConcession>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
