using AutoMapper;
using SchoolERP.Application.Features.Exam.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the Exam feature.</summary>
public class ExamProfile : Profile
{
    public ExamProfile()
    {
        CreateMap<Exam, ExamDto>()
            .ForMember(dest => dest.ExamTypeName, opt => opt.MapFrom(src => src.ExamType != null ? src.ExamType.Name : null))
            .ForMember(dest => dest.AcademicYearName, opt => opt.MapFrom(src => src.AcademicYear != null ? src.AcademicYear.Name : null));

        CreateMap<CreateExamDto, Exam>();

        CreateMap<UpdateExamDto, Exam>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore());
    }
}
