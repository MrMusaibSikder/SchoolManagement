using AutoMapper;
using SchoolERP.Application.Features.SubjectTeacher.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the SubjectTeacher join-entity feature.</summary>
public class SubjectTeacherProfile : Profile
{
    public SubjectTeacherProfile()
    {
        CreateMap<SubjectTeacher, SubjectTeacherDto>().ReverseMap();
    }
}
