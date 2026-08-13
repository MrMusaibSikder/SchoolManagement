using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.EmployeeSalary.DTOs;
using SchoolERP.Application.Features.EmployeeSalary.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for EmployeeSalary records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class EmployeeSalaryService : IEmployeeSalaryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EmployeeSalaryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EmployeeSalaryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.EmployeeSalaryRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<EmployeeSalaryDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<EmployeeSalaryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.EmployeeSalaryRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<EmployeeSalaryDto>(entity);
    }

    /// <inheritdoc />
    public async Task<EmployeeSalaryDto> CreateAsync(CreateEmployeeSalaryDto request, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<EmployeeSalary>(request);

        await _unitOfWork.EmployeeSalaryRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EmployeeSalaryDto>(entity);
    }

    /// <inheritdoc />
    public async Task<EmployeeSalaryDto> UpdateAsync(int id, UpdateEmployeeSalaryDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.EmployeeSalaryRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(EmployeeSalary), id);

        _mapper.Map(request, entity);

        _unitOfWork.EmployeeSalaryRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EmployeeSalaryDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.EmployeeSalaryRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(EmployeeSalary), id);

        _unitOfWork.EmployeeSalaryRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
