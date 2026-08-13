using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Employee.DTOs;
using SchoolERP.Application.Features.Employee.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for Employee records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;

    public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileService= fileService;

    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.EmployeeRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<EmployeeDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.EmployeeRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<EmployeeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto request, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<Employee>(request);
        if (request.EmployeePhotoFile is not null)
        {
            entity.EmployeePhoto = await _fileService.UploadAsync(
                request.EmployeePhotoFile.OpenReadStream(),
                request.EmployeePhotoFile.FileName,
                "employees",
                request.EmployeePhotoFile.ContentType,
                request.EmployeePhotoFile.Length,
                cancellationToken);
        }
        await _unitOfWork.EmployeeRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EmployeeDto>(entity);
    }

    /// <inheritdoc />
    public async Task<EmployeeDto> UpdateAsync(int id, UpdateEmployeeDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.EmployeeRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);

        _mapper.Map(request, entity);
        //new Add By Musaib Sikder
        entity.EmployeePhoto = await _fileService.ReplaceAsync(
       request.EmployeePhotoFile?.OpenReadStream(),
       entity.EmployeePhoto,
       request.EmployeePhotoFile?.FileName ?? string.Empty,
       "employees",
       request.EmployeePhotoFile?.ContentType ?? string.Empty,
       request.EmployeePhotoFile?.Length ?? 0,
       cancellationToken);

        _unitOfWork.EmployeeRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EmployeeDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.EmployeeRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Employee), id);
        await _fileService.DeleteAsync(entity.EmployeePhoto);

        _unitOfWork.EmployeeRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
