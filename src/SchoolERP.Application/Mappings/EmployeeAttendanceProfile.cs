using AutoMapper;
using SchoolERP.Application.Features.EmployeeAttendance.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the EmployeeAttendance feature.</summary>
public class EmployeeAttendanceProfile : Profile
{
    public EmployeeAttendanceProfile()
    {
        CreateMap<EmployeeAttendance, EmployeeAttendanceDto>();
        CreateMap<CreateEmployeeAttendanceDto, EmployeeAttendance>();
        CreateMap<UpdateEmployeeAttendanceDto, EmployeeAttendance>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
