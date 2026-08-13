using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.UserRole.DTOs;
using SchoolERP.Application.Features.UserRole.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for the UserRole association. Calls the repository (via the Unit
/// of Work), applies business rules and maps entities to/from DTOs.
/// </summary>
public class UserRoleService : IUserRoleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserRoleService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<UserRoleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.UserRoleRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<UserRoleDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<UserRoleDto?> GetAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UserRoleRepository.GetAsync(userId, roleId, cancellationToken);
        return entity is null ? null : _mapper.Map<UserRoleDto>(entity);
    }

    /// <inheritdoc />
    public async Task<UserRoleDto> AssignAsync(UserRoleDto request, CancellationToken cancellationToken = default)
    {
        var exists = await _unitOfWork.UserRoleRepository.ExistsAsync(request.UserId, request.RoleId, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException($"UserRole association ({request.UserId}, {request.RoleId}) already exists.");
        }

        var entity = _mapper.Map<UserRole>(request);

        await _unitOfWork.UserRoleRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserRoleDto>(entity);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(int userId, int roleId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UserRoleRepository.GetAsync(userId, roleId, cancellationToken)
            ?? throw new NotFoundException("UserRole", $"{userId},{roleId}");

        _unitOfWork.UserRoleRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
