using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.SubjectTeacher.DTOs;
using SchoolERP.Application.Features.SubjectTeacher.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for the SubjectTeacher association. Calls the repository (via the Unit
/// of Work), applies business rules and maps entities to/from DTOs.
/// </summary>
public class SubjectTeacherService : ISubjectTeacherService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SubjectTeacherService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SubjectTeacherDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.SubjectTeacherRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SubjectTeacherDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<SubjectTeacherDto?> GetAsync(int subjectId, int teacherId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SubjectTeacherRepository.GetAsync(subjectId, teacherId, cancellationToken);
        return entity is null ? null : _mapper.Map<SubjectTeacherDto>(entity);
    }

    /// <inheritdoc />
    public async Task<SubjectTeacherDto> AssignAsync(SubjectTeacherDto request, CancellationToken cancellationToken = default)
    {
        var exists = await _unitOfWork.SubjectTeacherRepository.ExistsAsync(request.SubjectId, request.TeacherId, cancellationToken);
        if (exists)
        {
            throw new InvalidOperationException($"SubjectTeacher association ({request.SubjectId}, {request.TeacherId}) already exists.");
        }

        var entity = _mapper.Map<SubjectTeacher>(request);

        await _unitOfWork.SubjectTeacherRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SubjectTeacherDto>(entity);
    }

    /// <inheritdoc />
    public async Task RemoveAsync(int subjectId, int teacherId, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SubjectTeacherRepository.GetAsync(subjectId, teacherId, cancellationToken)
            ?? throw new NotFoundException("SubjectTeacher", $"{subjectId},{teacherId}");

        _unitOfWork.SubjectTeacherRepository.Remove(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
