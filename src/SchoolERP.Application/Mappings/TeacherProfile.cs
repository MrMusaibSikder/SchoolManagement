using AutoMapper;
using SchoolERP.Application.Features.Teacher.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the Teacher feature.</summary>
public class TeacherProfile : Profile
{
    public TeacherProfile()
    {
        CreateMap<Teacher, TeacherDto>();
        CreateMap<CreateTeacherDto, Teacher>();
        CreateMap<UpdateTeacherDto, Teacher>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
