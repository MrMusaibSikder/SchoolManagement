using AutoMapper;
using Azure.Core;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.Student.DTOs;
using SchoolERP.Application.Features.Student.Interfaces;
using SchoolERP.Domain.Entities;
using System.Threading;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
///  by Musaib Sikder
/// Business logic for Student records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;

    public StudentService(IUnitOfWork unitOfWork, IMapper mapper,IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _fileService = fileService;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StudentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.StudentRepository.GetAllWithGuardiansAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<StudentDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<StudentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.StudentRepository.GetByIdWithGuardiansAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<StudentDto>(entity);
    }

    /// <inheritdoc />
    public async Task<StudentDto> CreateAsync(
     CreateStudentDto request,
     CancellationToken cancellationToken = default)
    {
        // Check Class
        var schoolClass = await _unitOfWork.SchoolClassRepository
            .GetByIdAsync(request.ClassId, cancellationToken)
            ?? throw new NotFoundException(nameof(SchoolClass), request.ClassId);

        // Check Section
        var section = await _unitOfWork.SectionRepository
            .GetByIdAsync(request.SectionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Section), request.SectionId);

        // Section belongs to Class
        if (section.ClassId != schoolClass.Id)
            throw new Exception("Selected section does not belong to the selected class.");

        // Admission Number Unique
        var admissionExists = await _unitOfWork.StudentRepository.AnyAsync(
            x => x.AdmissionNumber == request.AdmissionNumber,
            cancellationToken);

        if (admissionExists)
            throw new Exception("Admission number already exists.");

        // Roll Unique inside same Class + Section
        var rollExists = await _unitOfWork.StudentRepository.AnyAsync(
            x => x.ClassId == request.ClassId
              && x.SectionId == request.SectionId
              && x.RollNo == request.RollNo,
            cancellationToken);

        if (rollExists)
            throw new Exception(
                $"Roll No '{request.RollNo}' already exists in this class and section.");
        //Map Students
        var entity = _mapper.Map<Student>(request);
        //Upload Photo logic 
        if (request.PhotoFile is not null)
        {
            entity.Photo = await _fileService.UploadAsync(
                request.PhotoFile.OpenReadStream(),
                request.PhotoFile.FileName,
                "students",
                request.PhotoFile.ContentType,
                request.PhotoFile.Length,
                cancellationToken);
        }
        // Add StudentGurdian by searching gurdian table
        //Modified by Musaib Sikder
        foreach (var guardian in request.Guardians)
        {
            var guardianExists = await _unitOfWork.GuardianRepository
                .ExistsAsync(
                    guardian.GuardianId,
                    cancellationToken);

            if (!guardianExists)
            {
                throw new NotFoundException(
                    nameof(Guardian),
                    guardian.GuardianId);
            }


            entity.StudentGuardians.Add(
                new StudentGuardian
                {
                    GuardianId = guardian.GuardianId,
                    Relation = guardian.Relation
                });
        }
        // Save Student + StudentGuardian
        await _unitOfWork.StudentRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var savedStudent = await _unitOfWork.StudentRepository
    .GetByIdWithGuardiansAsync(
        entity.Id,
        cancellationToken);


        return _mapper.Map<StudentDto>(savedStudent);
    }

    /// <inheritdoc />
    public async Task<StudentDto> UpdateAsync(
     int id,
     UpdateStudentDto request,
     CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.StudentRepository
            .GetByIdWithGuardiansTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Student), id);

        // Check Class
        var schoolClass = await _unitOfWork.SchoolClassRepository
            .GetByIdAsync(request.ClassId, cancellationToken)
            ?? throw new NotFoundException(nameof(SchoolClass), request.ClassId);

        // Check Section
        var section = await _unitOfWork.SectionRepository
            .GetByIdAsync(request.SectionId, cancellationToken)
            ?? throw new NotFoundException(nameof(Section), request.SectionId);

        // Section belongs to Class
        if (section.ClassId != schoolClass.Id)
            throw new Exception("Selected section does not belong to the selected class.");

        // Admission Number Unique
        var admissionExists = await _unitOfWork.StudentRepository.AnyAsync(
            x => x.AdmissionNumber == request.AdmissionNumber
              && x.Id != id,
            cancellationToken);

        if (admissionExists)
            throw new Exception("Admission number already exists.");

        // Roll Unique inside same Class + Section
        var rollExists = await _unitOfWork.StudentRepository.AnyAsync(
            x => x.ClassId == request.ClassId
              && x.SectionId == request.SectionId
              && x.RollNo == request.RollNo
              && x.Id != id,
            cancellationToken);

        if (rollExists)
            throw new Exception(
                $"Roll No '{request.RollNo}' already exists in this class and section.");

        _mapper.Map(request, entity);

        // Remove existing guardians
        foreach (var oldGuardian in entity.StudentGuardians.ToList())
        {
            _unitOfWork.StudentGuardianRepository.Remove(oldGuardian);
        }


        // Add new guardians
        foreach (var guardian in request.Guardians)
        {
            var guardianExists = await _unitOfWork.GuardianRepository
                .ExistsAsync(
                    guardian.GuardianId,
                    cancellationToken);

            if (!guardianExists)
            {
                throw new NotFoundException(
                    nameof(Guardian),
                    guardian.GuardianId);
            }


            entity.StudentGuardians.Add(
                new StudentGuardian
                {
                    StudentId = entity.Id,
                    GuardianId = guardian.GuardianId,
                    Relation = guardian.Relation
                });
        }

        if (request.PhotoFile is not null)
        {
            entity.Photo = await _fileService.ReplaceAsync(
                request.PhotoFile.OpenReadStream(),
                entity.Photo,
                request.PhotoFile.FileName,
                "students",
                request.PhotoFile.ContentType,
                request.PhotoFile.Length,
                cancellationToken);
        }

        _unitOfWork.StudentRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var savedStudent = await _unitOfWork.StudentRepository
   .GetByIdWithGuardiansAsync(
       entity.Id,
       cancellationToken);


        return _mapper.Map<StudentDto>(savedStudent);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.StudentRepository.GetByIdWithGuardiansAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Student), id);

        await _fileService.DeleteAsync(entity.Photo);

        _unitOfWork.StudentRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
