using AutoMapper;
using SchoolERP.Application.Features.FeeType.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the FeeType feature.</summary>
public class FeeTypeProfile : Profile
{
    public FeeTypeProfile()
    {
        // Entity ? DTO (needs .Include(x => x.FeeCategory) in repository)
        CreateMap<FeeType, FeeTypeDto>()
            .ForMember(dest => dest.FeeCategoryName,
                       opt => opt.MapFrom(src => src.FeeCategory.Name));

        CreateMap<FeeType, FeeTypeListDto>()
            .ForMember(dest => dest.FeeCategoryName,
                       opt => opt.MapFrom(src => src.FeeCategory.Name));

        // DTO ? Entity
        CreateMap<CreateFeeTypeDto, FeeType>();

        CreateMap<UpdateFeeTypeDto, FeeType>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}

