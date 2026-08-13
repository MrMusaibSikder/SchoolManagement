using AutoMapper;
using SchoolERP.Application.Features.SmsTemplate.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the SmsTemplate feature.</summary>
public class SmsTemplateProfile : Profile
{
    public SmsTemplateProfile()
    {
        CreateMap<SmsTemplate, SmsTemplateDto>();
        CreateMap<CreateSmsTemplateDto, SmsTemplate>();
        CreateMap<UpdateSmsTemplateDto, SmsTemplate>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
