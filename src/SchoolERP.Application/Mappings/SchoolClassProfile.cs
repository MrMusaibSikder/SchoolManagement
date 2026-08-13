using AutoMapper;
using SchoolERP.Application.Features.SchoolClass.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the SchoolClass feature.</summary>
public class SchoolClassProfile : Profile
{
    public SchoolClassProfile()
    {
        CreateMap<SchoolClass, SchoolClassDto>();
        CreateMap<CreateSchoolClassDto, SchoolClass>();
        CreateMap<UpdateSchoolClassDto, SchoolClass>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
