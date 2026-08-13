using AutoMapper;
using SchoolERP.Application.Features.ExamType.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the ExamType feature.</summary>
public class ExamTypeProfile : Profile
{
    public ExamTypeProfile()
    {
        CreateMap<ExamType, ExamTypeDto>();
        CreateMap<CreateExamTypeDto, ExamType>();
        CreateMap<UpdateExamTypeDto, ExamType>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
