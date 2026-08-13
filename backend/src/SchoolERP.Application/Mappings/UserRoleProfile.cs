using AutoMapper;
using SchoolERP.Application.Features.UserRole.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>AutoMapper profile for the UserRole join-entity feature.</summary>
public class UserRoleProfile : Profile
{
    public UserRoleProfile()
    {
        CreateMap<UserRole, UserRoleDto>().ReverseMap();
    }
}
