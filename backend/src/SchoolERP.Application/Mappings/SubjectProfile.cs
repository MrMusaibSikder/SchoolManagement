using AutoMapper;
using SchoolERP.Application.Features.Subject.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the Subject feature.</summary>
public class SubjectProfile : Profile
{
    public SubjectProfile()
    {
        CreateMap<Subject, SubjectDto>();
        CreateMap<CreateSubjectDto, Subject>();
        CreateMap<UpdateSubjectDto, Subject>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
