using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Guardian.DTOs;
using SchoolERP.Application.Features.Guardian.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for Guardian records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class GuardianService : IGuardianService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GuardianService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<GuardianDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.GuardianRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<GuardianDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<GuardianDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.GuardianRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<GuardianDto>(entity);
    }

    /// <inheritdoc />
    public async Task<GuardianDto> CreateAsync(CreateGuardianDto request, CancellationToken cancellationToken = default)
    { //For Phone Number add this validation By Musaib Sikder
        if (await _unitOfWork.GuardianRepository.PhoneExistsAsync(request.PhoneNumber, cancellationToken))
        {
            throw new Exception("Phone number already exists.");
        }
        var entity = _mapper.Map<Guardian>(request);
       

        await _unitOfWork.GuardianRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GuardianDto>(entity);
    }

    /// <inheritdoc />
    public async Task<GuardianDto> UpdateAsync(int id, UpdateGuardianDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.GuardianRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Guardian), id);
        if (entity.PhoneNumber != request.PhoneNumber)
        {
            if (await _unitOfWork.GuardianRepository.PhoneExistsAsync(request.PhoneNumber, cancellationToken))
            {
                throw new Exception("Phone number already exists.");
            }
        }
        _mapper.Map(request, entity);


        _unitOfWork.GuardianRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<GuardianDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.GuardianRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Guardian), id);

        _unitOfWork.GuardianRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    /// Search gurdian
    public async Task<IReadOnlyList<GuardianDto>> SearchAsync(
    string keyword,
    CancellationToken cancellationToken = default)
    {
        var guardians = await _unitOfWork.GuardianRepository
            .SearchAsync(keyword, cancellationToken);

        return _mapper.Map<IReadOnlyList<GuardianDto>>(guardians);
    }
}
