using AutoMapper;
using SchoolERP.Application.Features.Result.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the Result (mark entry) feature.</summary>
public class ResultProfile : Profile
{
    public ResultProfile()
    {
        CreateMap<Result, ResultDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : null))
            .ForMember(dest => dest.RollNo, opt => opt.MapFrom(src => src.Student != null ? src.Student.RollNo : null))
            .ForMember(dest => dest.ExamName, opt => opt.MapFrom(src => src.ExamSchedule != null && src.ExamSchedule.Exam != null ? src.ExamSchedule.Exam.Name : null))
            .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.ExamSchedule != null && src.ExamSchedule.Subject != null ? src.ExamSchedule.Subject.Name : null))
            .ForMember(dest => dest.FullMarks, opt => opt.MapFrom(src => src.ExamSchedule != null ? src.ExamSchedule.FullMarks : 0))
            .ForMember(dest => dest.PassMarks, opt => opt.MapFrom(src => src.ExamSchedule != null ? src.ExamSchedule.PassMarks : 0))
            .ForMember(dest => dest.EnteredByTeacherName, opt => opt.MapFrom(src => src.EnteredByTeacher != null && src.EnteredByTeacher.Employee != null ? src.EnteredByTeacher.Employee.FullName : null));

        CreateMap<CreateResultDto, Result>()
            .ForMember(dest => dest.EnteredByTeacherId, opt => opt.MapFrom(src => src.TeacherId));

        CreateMap<UpdateResultDto, Result>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.EnteredByTeacherId, opt => opt.MapFrom(src => src.TeacherId));
    }
}
