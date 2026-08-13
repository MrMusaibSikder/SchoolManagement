using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.StudentGuardian.DTOs;
using SchoolERP.Application.Features.StudentGuardian.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for the StudentGuardian association. Calls the repository (via the Unit
/// of Work), applies business rules and maps entities to/from DTOs.
/// </summary>
public class StudentGuardianService : IStudentGuardianService
{

    /// <summary>
    /// No Use of this service no controller call it
    /// 
    /// </summary>
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StudentGuardianService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StudentGuardianDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.StudentGuardianRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<StudentGuardianDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<StudentGuardianDto?> GetAsync(int studentId, int guardianId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.StudentGuardianRepository.GetAsync(studentId, guardianId, cancellationToken);
        return entity is null ? null : _mapper.Map<StudentGuardianDto>(entity);
    }

    /// <inheritdoc />
    

    /// <inheritdoc />
    public async Task RemoveAsync(int studentId, int guardianId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.StudentGuardianRepository.GetAsync(studentId, guardianId, cancellationToken)
            ?? throw new NotFoundException("StudentGuardian", $"{studentId},{guardianId}");

        _unitOfWork.StudentGuardianRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
