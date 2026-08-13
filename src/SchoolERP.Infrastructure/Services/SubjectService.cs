using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Subject.DTOs;
using SchoolERP.Application.Features.Subject.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for Subject records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class SubjectService : ISubjectService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SubjectService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SubjectDto>> GetAllAsync(
       CancellationToken cancellationToken = default)
    {
        var subjects =
            await _unitOfWork.SubjectRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SubjectDto>>(subjects);
    }

    /// <inheritdoc />
    public async Task<SubjectDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var subject =
            await _unitOfWork.SubjectRepository
                .GetByIdAsync(id, cancellationToken);
            return subject == null
            ? null
            : _mapper.Map<SubjectDto>(subject);
    }

    /// <inheritdoc />
    public async Task<SubjectDto> CreateAsync(
     CreateSubjectDto request,
     CancellationToken cancellationToken = default)
    {

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new Exception("Subject name is required.");
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new Exception("Subject code is required.");
        if (request.FullMarks <= 0)
            throw new Exception("Full marks must be greater than zero.");
        if (request.PassMarks <= 0)
            throw new Exception("Pass marks must be greater than zero.");
        if (request.PassMarks > request.FullMarks)
            throw new Exception(
                "Pass marks cannot be greater than full marks.");
        var codeExists =
            await _unitOfWork.SubjectRepository.AnyAsync(
                x => x.Code == request.Code,
                cancellationToken);
        if (codeExists)
            throw new Exception(
                "Subject code already exists.");
        var entity =
            _mapper.Map<Subject>(request);
        await _unitOfWork.SubjectRepository
            .AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SubjectDto>(entity);
    }

    /// <inheritdoc />
    public async Task<SubjectDto> UpdateAsync(
       int id,
       UpdateSubjectDto request,
       CancellationToken cancellationToken = default)
    {

        var entity =
            await _unitOfWork.SubjectRepository
            .GetByIdTrackedAsync(id, cancellationToken)
            ??
            throw new NotFoundException(
                nameof(Subject),
                id);

        var codeExists =
            await _unitOfWork.SubjectRepository.AnyAsync(
                x => x.Code == request.Code
                && x.Id != id,
                cancellationToken);

        if (codeExists)
        {
            throw new Exception(
                "Subject code already exists.");
        }
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new Exception("Subject name is required.");
        if (string.IsNullOrWhiteSpace(request.Code))
            throw new Exception("Subject code is required.");
        if (request.PassMarks > request.FullMarks)
            throw new Exception(
                "Pass marks cannot be greater than full marks.");

        _mapper.Map(request, entity);
        _unitOfWork.SubjectRepository
            .Update(entity);

        await _unitOfWork
            .SaveChangesAsync(cancellationToken);

        return _mapper.Map<SubjectDto>(entity);
    }


    /// <inheritdoc />
    public async Task DeleteAsync(
    int id,
    CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            throw new ArgumentException(
                "Invalid subject id.");
        }


        var entity =
            await _unitOfWork.SubjectRepository
            .GetByIdTrackedAsync(
                id,
                cancellationToken);



        if (entity is null)
        {
            throw new NotFoundException(
                nameof(Subject),
                id);
        }



        _unitOfWork.SubjectRepository
            .Delete(entity);



        await _unitOfWork
            .SaveChangesAsync(cancellationToken);
    }
}
