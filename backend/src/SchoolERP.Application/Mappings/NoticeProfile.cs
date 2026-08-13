using AutoMapper;
using SchoolERP.Application.Features.Notice.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the Notice feature.</summary>
public class NoticeProfile : Profile
{
    public NoticeProfile()
    {
        CreateMap<Notice, NoticeDto>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src =>
                src.IsPublished &&
                !src.IsArchived &&
                (!src.ExpiryDate.HasValue || src.ExpiryDate.Value.Date >= DateTime.Today)))
            .ForMember(dest => dest.IsUpcoming, opt => opt.MapFrom(src =>
                src.PublishDate.Date > DateTime.Today))
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src =>
                src.ExpiryDate.HasValue && src.ExpiryDate.Value.Date < DateTime.Today));

        CreateMap<CreateNoticeDto, Notice>()
            .ForMember(dest => dest.AttachmentPath, opt => opt.Ignore());

        CreateMap<UpdateNoticeDto, Notice>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.AttachmentPath, opt => opt.Ignore())
            .ForMember(dest => dest.IsPublished, opt => opt.Ignore())
            .ForMember(dest => dest.IsArchived, opt => opt.Ignore());
    }
}
