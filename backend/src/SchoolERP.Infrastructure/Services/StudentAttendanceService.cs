using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.StudentAttendance.DTOs;
using SchoolERP.Application.Features.StudentAttendance.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for StudentAttendance records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class StudentAttendanceService : IStudentAttendanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StudentAttendanceService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<StudentAttendanceDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.StudentAttendanceRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<StudentAttendanceDto>>(entities);
    }

    public async Task<StudentAttendanceDto?> GetByIdAsync( int id,CancellationToken cancellationToken = default)
    {
        var entity =await _unitOfWork.StudentAttendanceRepository.GetByIdAsync(id, cancellationToken);
        return entity == null? null : _mapper.Map<StudentAttendanceDto>(entity);
    }

    public async Task<StudentAttendanceDto> CreateAsync( CreateStudentAttendanceDto request,CancellationToken cancellationToken = default)
    {
        // Future Date Validation
        if (request.AttendanceDate.Date > DateTime.Today)
        {
            throw new InvalidOperationException("Future attendance is not allowed.");
        }

        // Student Validation
        var student = await _unitOfWork.StudentRepository.GetByIdAsync(request.StudentId, cancellationToken);

        if (student is null)
        {
            throw new NotFoundException( nameof(Student),request.StudentId);
        }

        // Duplicate Validation
        var exists = await _unitOfWork.StudentAttendanceRepository.AnyAsync(x => x.StudentId == request.StudentId &&
                 x.AttendanceDate.Date == request.AttendanceDate.Date,
            cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException( "Attendance already exists for this student.");
        }

        var entity = _mapper.Map<StudentAttendance>(request);

        entity.AttendanceDate = request.AttendanceDate.Date;

        await _unitOfWork.StudentAttendanceRepository.AddAsync(entity, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<StudentAttendanceDto>(entity);
    }

    /// <summary>
    /// This Method Is Used to Bulk operation By Musaib Sikder..
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task BulkAttendanceAsync(BulkStudentAttendanceDto request,CancellationToken cancellationToken = default)
    {
        // Request validation
        if (request.Attendance == null || !request.Attendance.Any())
        {
            throw new InvalidOperationException("Attendance list cannot be empty.");
        }

        if (request.AttendanceDate.Date > DateTime.Today)
        {
            throw new InvalidOperationException( "Future attendance is not allowed.");
        }


        // Duplicate student check
        var hasDuplicateStudent = request.Attendance.GroupBy(x => x.StudentId) .Any(g => g.Count() > 1);

        if (hasDuplicateStudent)
        {
            throw new InvalidOperationException("Duplicate student found in attendance list.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {

            // Load students of selected class and section
            var students =
                await _unitOfWork.StudentRepository .GetByClassAndSectionAsync(request.ClassId, request.SectionId, cancellationToken);

            if (!students.Any())
            {
                throw new NotFoundException("No students found for selected class and section.");
            }

            var studentDictionary =students.ToDictionary(x => x.Id);

            // Load existing attendance for selected date
            var existingAttendances =
                await _unitOfWork.StudentAttendanceRepository
                    .GetByStudentsAndDateAsync(
                        studentDictionary.Keys,
                        request.AttendanceDate,
                        cancellationToken);

            var attendanceDictionary =existingAttendances.ToDictionary(x => x.StudentId);

            foreach (var item in request.Attendance)
            {

                // Verify student belongs to class & section
                if (!studentDictionary.ContainsKey(item.StudentId))
                {
                    throw new NotFoundException(nameof(Student), item.StudentId);
                }

                // Update existing attendance
                if (attendanceDictionary.TryGetValue(item.StudentId,out var attendance))
                {
                    attendance.Status = item.Status;
                    attendance.Remarks = item.Remarks;

                    _unitOfWork.StudentAttendanceRepository .Update(attendance);

                }
                else
                {
                    // Create new attendance

                    var newAttendance = new StudentAttendance
                    {
                        StudentId = item.StudentId,
                        AttendanceDate = request.AttendanceDate.Date,
                        Status = item.Status,
                        Remarks = item.Remarks
                    };


                    await _unitOfWork
                        .StudentAttendanceRepository
                        .AddAsync(
                            newAttendance,
                            cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

        }
        catch
        {

            await _unitOfWork.RollbackTransactionAsync( cancellationToken);

            throw;
        }
    }

    public async Task<IReadOnlyList<StudentAttendanceDto>>GetByClassSectionDateAsync( int classId,int sectionId,
            DateTime attendanceDate, CancellationToken cancellationToken = default)
    {

        var entities =
            await _unitOfWork
            .StudentAttendanceRepository
            .GetByClassSectionDateAsync(
                classId,
                sectionId,
                attendanceDate,
                cancellationToken);

        return _mapper.Map<IReadOnlyList<StudentAttendanceDto>>(entities);
    }


    public async Task<IReadOnlyList<StudentAttendanceDto>>GetStudentHistoryAsync(int studentId,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken cancellationToken = default)
    {

        var entities = await _unitOfWork.StudentAttendanceRepository
            .GetStudentHistoryAsync(
                studentId,
                fromDate,
                toDate,
                cancellationToken);

        return _mapper
            .Map<IReadOnlyList<StudentAttendanceDto>>(entities);
    }

    public async Task<StudentAttendanceDto> UpdateAsync(
        int id,
        UpdateStudentAttendanceDto request,
        CancellationToken cancellationToken = default)
    {

        var entity =await _unitOfWork.StudentAttendanceRepository
            .GetByIdTrackedAsync(
                id,
                cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(StudentAttendance),id);
        }
        

        _mapper.Map(request, entity);

        _unitOfWork
            .StudentAttendanceRepository
            .Update(entity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<StudentAttendanceDto>(entity);
    }


    public async Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity =await _unitOfWork.StudentAttendanceRepository
            .GetByIdTrackedAsync(
                id,
                cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(
                nameof(StudentAttendance),
                id);
        }

        _unitOfWork
            .StudentAttendanceRepository
            .Delete(entity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

