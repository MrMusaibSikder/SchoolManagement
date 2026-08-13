using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Teacher.DTOs;
using SchoolERP.Application.Features.Teacher.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for Teacher records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class TeacherService : ITeacherService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TeacherService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TeacherDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.TeacherRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<TeacherDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<TeacherDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.TeacherRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<TeacherDto>(entity);
    }

    /// <inheritdoc />
    public async Task<TeacherDto> CreateAsync(CreateTeacherDto request, CancellationToken cancellationToken = default)
    {
        // Check Employee exists
        var employee = await _unitOfWork.EmployeeRepository
            .GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null)
        {
            throw new NotFoundException(
                nameof(Employee),
                request.EmployeeId);
        }
        // Check employee already assigned as teacher
        var exists = await _unitOfWork.TeacherRepository
            .ExistsByEmployeeIdAsync(
                request.EmployeeId,
                cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException(
                "This employee is already assigned as a teacher.");
        }

        var entity = _mapper.Map<Teacher>(request);

        await _unitOfWork.TeacherRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TeacherDto>(entity);
    }

    /// <inheritdoc />
    public async Task<TeacherDto> UpdateAsync(int id, UpdateTeacherDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.TeacherRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Teacher), id);
        // Employee change duplicate check
        if (entity.EmployeeId != request.EmployeeId)
        {
            var exists = await _unitOfWork.TeacherRepository
                .ExistsByEmployeeIdAsync(
                    request.EmployeeId,
                    cancellationToken);

            if (exists)
            {
                throw new InvalidOperationException(
                    "This employee is already assigned as a teacher.");
            }


            // New employee exists check
            var employee = await _unitOfWork.EmployeeRepository
                .GetByIdAsync(request.EmployeeId, cancellationToken);

            if (employee is null)
            {
                throw new NotFoundException(
                    nameof(Employee),
                    request.EmployeeId);
            }
        }

        _mapper.Map(request, entity);

        _unitOfWork.TeacherRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TeacherDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.TeacherRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Teacher), id);

        _unitOfWork.TeacherRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
