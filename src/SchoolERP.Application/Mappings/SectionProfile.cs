using AutoMapper;
using SchoolERP.Application.Features.Section.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the Section feature.</summary>
public class SectionProfile : Profile
{
    public SectionProfile()
    {
        CreateMap<Section, SectionDto>();
        CreateMap<CreateSectionDto, Section>();
        CreateMap<UpdateSectionDto, Section>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
