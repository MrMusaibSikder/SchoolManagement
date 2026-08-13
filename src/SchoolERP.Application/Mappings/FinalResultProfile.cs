using AutoMapper;
using SchoolERP.Application.Features.FinalResult.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the FinalResult feature.</summary>
public class FinalResultProfile : Profile
{
    public FinalResultProfile()
    {
        CreateMap<FinalResultDetail, FinalResultDetailDto>()
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Name : string.Empty));

        CreateMap<FinalResult, FinalResultDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : string.Empty))
            .ForMember(dest => dest.RollNo, opt => opt.MapFrom(src => src.Student != null ? src.Student.RollNo : string.Empty))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Student != null && src.Student.SchoolClass != null ? src.Student.SchoolClass.Name : string.Empty))
            .ForMember(dest => dest.SectionName, opt => opt.MapFrom(src => src.Student != null && src.Student.Section != null ? src.Student.Section.Name : string.Empty))
            .ForMember(dest => dest.AcademicYearName, opt => opt.MapFrom(src => src.AcademicYear != null ? src.AcademicYear.Name : string.Empty))
            .ForMember(dest => dest.Details, opt => opt.MapFrom(src => src.Details.Where(d => !d.IsDeleted)));
    }
}
