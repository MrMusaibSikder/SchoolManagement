using AutoMapper;
using SchoolERP.Application.Features.Guardian.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the Guardian feature.</summary>
public class GuardianProfile : Profile
{
    public GuardianProfile()
    {
        CreateMap<Guardian, GuardianDto>();
        CreateMap<CreateGuardianDto, Guardian>();
        CreateMap<UpdateGuardianDto, Guardian>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
