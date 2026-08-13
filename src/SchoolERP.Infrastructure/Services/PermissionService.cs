using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Permission.DTOs;
using SchoolERP.Application.Features.Permission.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for Permission records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class PermissionService : IPermissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PermissionService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<PermissionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.PermissionRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<PermissionDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<PermissionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.PermissionRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<PermissionDto>(entity);
    }

    /// <inheritdoc />
    public async Task<PermissionDto> CreateAsync(CreatePermissionDto request, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<Permission>(request);

        await _unitOfWork.PermissionRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PermissionDto>(entity);
    }

    /// <inheritdoc />
    public async Task<PermissionDto> UpdateAsync(int id, UpdatePermissionDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.PermissionRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Permission), id);

        _mapper.Map(request, entity);

        _unitOfWork.PermissionRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PermissionDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.PermissionRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Permission), id);

        _unitOfWork.PermissionRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
