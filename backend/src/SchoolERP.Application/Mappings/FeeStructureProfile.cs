using AutoMapper;
using SchoolERP.Application.Features.FeeStructure.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the FeeStructure feature.</summary>
public class FeeStructureProfile : Profile
{
    public FeeStructureProfile()
    {
        // Entity ? DTO (needs .Include() for AcademicYear, SchoolClass, Section, FeeStructureItems.FeeType)
        CreateMap<FeeStructure, FeeStructureDto>()
            .ForMember(dest => dest.AcademicYearName,
                       opt => opt.MapFrom(src => src.AcademicYear.Name))
            .ForMember(dest => dest.SchoolClassName,
                       opt => opt.MapFrom(src => src.SchoolClass.Name))
            .ForMember(dest => dest.SectionName,
                       opt => opt.MapFrom(src => src.Section != null ? src.Section.Name : null))
            .ForMember(dest => dest.Items,
                       opt => opt.MapFrom(src => src.FeeStructureItems));

        CreateMap<FeeStructure, FeeStructureListDto>()
            .ForMember(dest => dest.AcademicYearName,
                       opt => opt.MapFrom(src => src.AcademicYear.Name))
            .ForMember(dest => dest.SchoolClassName,
                       opt => opt.MapFrom(src => src.SchoolClass.Name))
            .ForMember(dest => dest.SectionName,
                       opt => opt.MapFrom(src => src.Section != null ? src.Section.Name : null))
            .ForMember(dest => dest.ItemCount,
                       opt => opt.MapFrom(src => src.FeeStructureItems.Count))
            .ForMember(dest => dest.TotalAmount,
                       opt => opt.MapFrom(src => src.FeeStructureItems.Sum(i => i.Amount)));

        CreateMap<FeeStructureItem, FeeStructureItemDto>()
            .ForMember(dest => dest.FeeTypeName,
                       opt => opt.MapFrom(src => src.FeeType.Name))
            .ForMember(dest => dest.FeeTypeCode,
                       opt => opt.MapFrom(src => src.FeeType.Code));

        // DTO ? Entity (Create)
        CreateMap<CreateFeeStructureDto, FeeStructure>()
            .ForMember(dest => dest.FeeStructureItems,
                       opt => opt.MapFrom(src => src.Items));

        CreateMap<CreateFeeStructureItemDto, FeeStructureItem>();

        // DTO ? Entity (Update) — Items handled manually in service
        CreateMap<UpdateFeeStructureDto, FeeStructure>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FeeStructureItems, opt => opt.Ignore());
    }
}
