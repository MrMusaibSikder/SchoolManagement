using AutoMapper;
using SchoolERP.Application.Features.AcademicYear.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the AcademicYear feature.</summary>
public class AcademicYearProfile : Profile
{
    public AcademicYearProfile()
    {
        CreateMap<AcademicYear, AcademicYearDto>();
        CreateMap<CreateAcademicYearDto, AcademicYear>();
        CreateMap<UpdateAcademicYearDto, AcademicYear>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
