using AutoMapper;
using SchoolERP.Application.Features.Designation.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the Designation feature.</summary>
public class DesignationProfile : Profile
{
    public DesignationProfile()
    {
        CreateMap<Designation, DesignationDto>();
        CreateMap<CreateDesignationDto, Designation>();
        CreateMap<UpdateDesignationDto, Designation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
