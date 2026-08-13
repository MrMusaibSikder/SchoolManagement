using AutoMapper;
using SchoolERP.Application.Features.School.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the School feature.
/// Edited By Musaib Sikder
/// </summary>
public class SchoolProfile : Profile
{
    public SchoolProfile()
    {
        CreateMap<School, SchoolDto>();
        CreateMap<CreateSchoolDto, School>()
           .ForMember(dest => dest.Logo, opt => opt.Ignore());

        CreateMap<UpdateSchoolDto, School>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Logo, opt => opt.Ignore());
    }
}

