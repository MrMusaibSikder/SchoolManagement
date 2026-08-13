using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Interfaces.Services;
using SchoolERP.Application.Features.User.DTOs;
using SchoolERP.Application.Features.User.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for User records. Calls the repository (via the Unit of Work),
/// hashes credentials, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.UserRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<UserDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UserRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<UserDto>(entity);
    }

    /// <inheritdoc />
    public async Task<UserDto> CreateAsync(CreateUserDto request, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<User>(request);
        entity.PasswordHash = _passwordHasher.Hash(request.Password);

        await _unitOfWork.UserRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserDto>(entity);
    }

    /// <inheritdoc />
    public async Task<UserDto> UpdateAsync(int id, UpdateUserDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UserRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(User), id);

        _mapper.Map(request, entity);

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            entity.PasswordHash = _passwordHasher.Hash(request.Password);
        }

        _unitOfWork.UserRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<UserDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.UserRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(User), id);

        _unitOfWork.UserRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
