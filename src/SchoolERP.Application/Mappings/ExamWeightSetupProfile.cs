using AutoMapper;
using SchoolERP.Application.Features.ExamWeightSetup.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the ExamWeightSetup feature (setups and their items).</summary>
public class ExamWeightSetupProfile : Profile
{
    public ExamWeightSetupProfile()
    {
        CreateMap<ExamWeightItem, ExamWeightItemDto>()
            .ForMember(dest => dest.ExamName, opt => opt.MapFrom(src => src.Exam != null ? src.Exam.Name : string.Empty));

        CreateMap<ExamWeightSetup, ExamWeightSetupDto>()
            .ForMember(dest => dest.AcademicYearName, opt => opt.MapFrom(src => src.AcademicYear != null ? src.AcademicYear.Name : string.Empty))
            .ForMember(dest => dest.TotalWeight, opt => opt.MapFrom(src => src.Items.Where(i => !i.IsDeleted).Sum(i => i.WeightPercentage)))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items.Where(i => !i.IsDeleted)));
    }
}
