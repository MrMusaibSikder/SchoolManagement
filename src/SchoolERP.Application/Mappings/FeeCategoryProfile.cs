using AutoMapper;
using SchoolERP.Application.Features.FeeCategory.DTOs;
using SchoolERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Mappings
{
    public class FeeCategoryProfile : Profile
    {
        public FeeCategoryProfile()
        {
            // Entity → DTO
            CreateMap<FeeCategory, FeeCategoryDto>();

            // DTO → Entity
            CreateMap<CreateFeeCategoryDto, FeeCategory>();

            CreateMap<UpdateFeeCategoryDto, FeeCategory>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
