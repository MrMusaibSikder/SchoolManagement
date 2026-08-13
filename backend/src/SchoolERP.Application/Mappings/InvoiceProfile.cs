using AutoMapper;
using SchoolERP.Application.Features.Invoice.DTOs;
using SchoolERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Mappings
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            // Entity → DTO (needs .Include() for AcademicYear, Student, InvoiceItems.FeeType)
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.AcademicYearName,
                           opt => opt.MapFrom(src => src.AcademicYear.Name))
                .ForMember(dest => dest.StudentName,
                           opt => opt.MapFrom(src => src.Student.FullName))
                .ForMember(dest => dest.StudentAdmissionNumber,
                           opt => opt.MapFrom(src => src.Student.AdmissionNumber))

                .ForMember(dest => dest.Items,
                           opt => opt.MapFrom(src => src.InvoiceItems));

            CreateMap<Invoice, InvoiceListDto>()
                .ForMember(dest => dest.StudentName,
                           opt => opt.MapFrom(src => src.Student.FullName));

            CreateMap<InvoiceItem, InvoiceItemDto>()
                .ForMember(dest => dest.FeeTypeName,
                           opt => opt.MapFrom(src => src.FeeType.Name));

            // DTO → Entity (Create) — Computed fields ignored, set in service
            CreateMap<CreateInvoiceDto, Invoice>()
                .ForMember(dest => dest.InvoiceItems,
                           opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.InvoiceNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.TotalAmount, opt => opt.Ignore())
                .ForMember(dest => dest.AmountPaid, opt => opt.Ignore())
                .ForMember(dest => dest.BalanceDue, opt => opt.Ignore());

            CreateMap<CreateInvoiceItemDto, InvoiceItem>()
                .ForMember(dest => dest.NetAmount, opt => opt.Ignore());
        }
    }
}
