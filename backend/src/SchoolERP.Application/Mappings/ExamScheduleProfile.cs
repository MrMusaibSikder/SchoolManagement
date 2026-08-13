using AutoMapper;
using SchoolERP.Application.Features.ExamSchedule.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the ExamSchedule feature.</summary>
public class ExamScheduleProfile : Profile
{
    public ExamScheduleProfile()
    {
        CreateMap<ExamSchedule, ExamScheduleDto>()
            .ForMember(dest => dest.ExamName, opt => opt.MapFrom(src => src.Exam != null ? src.Exam.Name : null))
            .ForMember(dest => dest.ClassName, opt => opt.MapFrom(src => src.SchoolClass != null ? src.SchoolClass.Name : null))
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.Name : null));

        CreateMap<CreateExamScheduleDto, ExamSchedule>();
        CreateMap<UpdateExamScheduleDto, ExamSchedule>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
