using AutoMapper;
using SchoolERP.Application.Features.ClassSubject.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the ClassSubject join-entity feature.</summary>
public class ClassSubjectProfile : Profile
{
    public ClassSubjectProfile()
    {
        CreateMap<ClassSubject, ClassSubjectDto>().ReverseMap();
    }
}
