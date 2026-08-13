using AutoMapper;
using SchoolERP.Application.Features.User.DTOs;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Application.Mappings;

/// <summary>
/// AutoMapper profile for the User feature. PasswordHash is deliberately
/// excluded from every mapping — UserService sets it explicitly via
/// <c>IPasswordHasher</c> so plain-text passwords never flow onto the entity
/// through reflection-based mapping.
/// </summary>
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();

        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

        CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}
