using AutoMapper;
using SchoolERP.Application.Features.SmsLog.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the SmsLog feature.</summary>
public class SmsLogProfile : Profile
{
    public SmsLogProfile()
    {
        CreateMap<SmsLog, SmsLogDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.FullName : null));
        CreateMap<CreateSmsLogDto, SmsLog>();
        CreateMap<UpdateSmsLogDto, SmsLog>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
