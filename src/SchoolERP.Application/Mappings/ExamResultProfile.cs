using AutoMapper;
using SchoolERP.Application.Features.ExamResult.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the ExamResult feature.</summary>
public class ExamResultProfile : Profile
{
    public ExamResultProfile()
    {
        CreateMap<ExamResult, ExamResultDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : string.Empty))
            .ForMember(dest => dest.RollNo, opt => opt.MapFrom(src => src.Student != null ? src.Student.RollNo : string.Empty))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.Student != null && src.Student.SchoolClass != null ? src.Student.SchoolClass.Name : string.Empty))
            .ForMember(dest => dest.SectionName, opt => opt.MapFrom(src => src.Student != null && src.Student.Section != null ? src.Student.Section.Name : string.Empty))
            .ForMember(dest => dest.ExamName, opt => opt.MapFrom(src => src.Exam != null ? src.Exam.Name : string.Empty));
    }
}
