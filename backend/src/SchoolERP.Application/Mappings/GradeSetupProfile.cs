using AutoMapper;
using SchoolERP.Application.Features.GradeSetup.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the GradeSetup feature.</summary>
public class GradeSetupProfile : Profile
{
    public GradeSetupProfile()
    {
        CreateMap<GradeSetup, GradeSetupDto>()
            .ForMember(dest => dest.AcademicYearName, opt => opt.MapFrom(src => src.AcademicYear != null ? src.AcademicYear.Name : string.Empty));

        CreateMap<CreateGradeSetupDto, GradeSetup>();
        CreateMap<UpdateGradeSetupDto, GradeSetup>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
