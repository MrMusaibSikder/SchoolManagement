using AutoMapper;
using SchoolERP.Application.Features.LateFineRule.DTOs;
using SchoolERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Mappings
{
    public class LateFineRuleProfile : Profile
    {
        public LateFineRuleProfile()
        {
            // Entity → DTO (needs .Include() for AcademicYear, FeeType)
            CreateMap<LateFineRule, LateFineRuleDto>()
                .ForMember(dest => dest.AcademicYearName,
                           opt => opt.MapFrom(src => src.AcademicYear.Name))
                .ForMember(dest => dest.FeeTypeName,
                           opt => opt.MapFrom(src => src.FeeType != null ? src.FeeType.Name : null));

            // DTO → Entity
            CreateMap<CreateLateFineRuleDto, LateFineRule>();

            CreateMap<UpdateLateFineRuleDto, LateFineRule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
