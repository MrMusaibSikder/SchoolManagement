using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.School.DTOs;
using SchoolERP.Application.Features.School.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Md Musaib Sikder
/// Business logic for School records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class SchoolService : ISchoolService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;

    public SchoolService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileService = fileService;

    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SchoolDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.SchoolRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SchoolDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<SchoolDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SchoolRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<SchoolDto>(entity);
    }

    /// <inheritdoc />
    public async Task<SchoolDto> CreateAsync(CreateSchoolDto request, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<School>(request);
        if (request.LogoFile is not null)
        {
            entity.Logo = await _fileService.UploadAsync(
                request.LogoFile.OpenReadStream(),
                request.LogoFile.FileName,
                "schools",
                request.LogoFile.ContentType,
                request.LogoFile.Length,
                cancellationToken);
        }
        await _unitOfWork.SchoolRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SchoolDto>(entity);
    }

    /// <inheritdoc />
    public async Task<SchoolDto> UpdateAsync(int id, UpdateSchoolDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SchoolRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(School), id);

        _mapper.Map(request, entity);
        if (request.LogoFile is not null)
        {
            entity.Logo = await _fileService.ReplaceAsync(
                request.LogoFile.OpenReadStream(),
                entity.Logo,
                request.LogoFile.FileName,
                "schools",
                request.LogoFile.ContentType,
                request.LogoFile.Length,
                cancellationToken);
        }
        _unitOfWork.SchoolRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SchoolDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SchoolRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(School), id);
        await _fileService.DeleteAsync(entity.Logo);
        _unitOfWork.SchoolRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
