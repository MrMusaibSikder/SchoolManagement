using AutoMapper;
using SchoolERP.Application.Features.EmployeeSalary.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the EmployeeSalary feature.</summary>
public class EmployeeSalaryProfile : Profile
{
    public EmployeeSalaryProfile()
    {
        CreateMap<EmployeeSalary, EmployeeSalaryDto>();
        CreateMap<CreateEmployeeSalaryDto, EmployeeSalary>();
        CreateMap<UpdateEmployeeSalaryDto, EmployeeSalary>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
