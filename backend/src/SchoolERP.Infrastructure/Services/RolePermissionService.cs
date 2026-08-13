using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.RolePermission.DTOs;
using SchoolERP.Application.Features.RolePermission.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for the RolePermission association. Calls the repository (via the Unit
/// of Work), applies business rules and maps entities to/from DTOs.
/// </summary>
public class RolePermissionService : IRolePermissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RolePermissionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<RolePermissionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.RolePermissionRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<RolePermissionDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<RolePermissionDto?> GetAsync(int roleId, int permissionId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.RolePermissionRepository.GetAsync(roleId, permissionId, cancellationToken);
        return entity is null ? null : _mapper.Map<RolePermissionDto>(entity);
    }

    /// <inheritdoc />
    public async Task<RolePermissionDto> AssignAsync(RolePermissionDto request, CancellationToken cancellationToken = default)
    {
        var exists = await _unitOfWork.RolePermissionRepository.ExistsAsync(request.RoleId, request.PermissionId, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException($"RolePermission association ({request.RoleId}, {request.PermissionId}) already exists.");
        }

        var entity = _mapper.Map<RolePermission>(request);

        await _unitOfWork.RolePermissionRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<RolePermissionDto>(entity);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(int roleId, int permissionId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.RolePermissionRepository.GetAsync(roleId, permissionId, cancellationToken)
            ?? throw new NotFoundException("RolePermission", $"{roleId},{permissionId}");

        _unitOfWork.RolePermissionRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
