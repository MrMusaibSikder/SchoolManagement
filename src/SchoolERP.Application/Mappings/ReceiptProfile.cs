using AutoMapper;
using SchoolERP.Application.Features.Receipt.DTOs;
using SchoolERP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolERP.Application.Mappings
{
    public class ReceiptProfile : Profile
    {
        public ReceiptProfile()
        {
            // Entity → DTO (needs .Include() for Payment, IssuedByUser)
            CreateMap<Receipt, ReceiptDto>()
                .ForMember(dest => dest.PaymentNumber,
                           opt => opt.MapFrom(src => src.Payment.PaymentNumber))
                .ForMember(dest => dest.IssuedByEmployeeName,
                           opt => opt.MapFrom(src => src.IssuedByEmployee.FullName));
        }
    }
}
