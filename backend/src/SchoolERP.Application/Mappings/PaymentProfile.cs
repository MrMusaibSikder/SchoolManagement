using AutoMapper;
using SchoolERP.Application.Features.Payment.DTOs;
using SchoolERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Mappings
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            // Entity → DTO (needs .Include() for Invoice, Student, Receipt, CollectedByUser)
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.InvoiceNumber,
                           opt => opt.MapFrom(src => src.Invoice.InvoiceNumber))
                .ForMember(dest => dest.StudentName,
                           opt => opt.MapFrom(src => src.Student.FullName))
                .ForMember(dest => dest.ReceiptId,
                           opt => opt.MapFrom(src => src.Receipt != null ? src.Receipt.Id : (int?)null))
                .ForMember(dest => dest.ReceiptNo,
                           opt => opt.MapFrom(src => src.Receipt != null ? src.Receipt.ReceiptNo : null))
                .ForMember(dest => dest.CollectedByEmployeeName,
                           opt => opt.MapFrom(src => src.CollectedByEmployee.FullName));

            CreateMap<Payment, PaymentListDto>()
                .ForMember(dest => dest.StudentName,
                           opt => opt.MapFrom(src => src.Student.FullName))
                .ForMember(dest => dest.ReceiptNo,
                           opt => opt.MapFrom(src => src.Receipt != null ? src.Receipt.ReceiptNo : null));

            // DTO → Entity (Create) — Service sets generated fields
            CreateMap<CreatePaymentDto, Payment>()
                .ForMember(dest => dest.PaymentNumber, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore())
                .ForMember(dest => dest.CollectedByEmployeeId, opt => opt.Ignore());
        }
    }
}
