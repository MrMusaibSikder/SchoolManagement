using AutoMapper;
using SchoolERP.Application.Features.RolePermission.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the RolePermission join-entity feature.</summary>
public class RolePermissionProfile : Profile
{
    public RolePermissionProfile()
    {
        CreateMap<RolePermission, RolePermissionDto>().ReverseMap();
    }
}
