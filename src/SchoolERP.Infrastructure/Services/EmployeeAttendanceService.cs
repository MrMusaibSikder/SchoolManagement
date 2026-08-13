using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Features.EmployeeAttendance.DTOs;
using SchoolERP.Application.Features.EmployeeAttendance.Interfaces;
using SchoolERP.Domain.Entities;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for EmployeeAttendance records. Calls the repository (via the Unit of Work),
/// applies business rules, and maps entities to/from DTOs using AutoMapper.
/// </summary>
public class EmployeeAttendanceService : IEmployeeAttendanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EmployeeAttendanceService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EmployeeAttendanceDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.EmployeeAttendanceRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<EmployeeAttendanceDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<EmployeeAttendanceDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.EmployeeAttendanceRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<EmployeeAttendanceDto>(entity);
    }

    /// <inheritdoc />
    public async Task<EmployeeAttendanceDto> CreateAsync(CreateEmployeeAttendanceDto request, CancellationToken cancellationToken = default)
    {
        // Future date validation
        if (request.AttendanceDate.Date > DateTime.Today)
        {
            throw new InvalidOperationException("Future attendance is not allowed.");
        }

        // Employee validation
        var employee = await _unitOfWork.EmployeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);

        if (employee is null)
        {
            throw new NotFoundException(nameof(Employee), request.EmployeeId);
        }

        // Duplicate validation
        var exists = await _unitOfWork.EmployeeAttendanceRepository.AnyAsync(
            x => x.EmployeeId == request.EmployeeId &&
                 x.AttendanceDate.Date == request.AttendanceDate.Date,
            cancellationToken);

        if (exists)
        {
            throw new InvalidOperationException("Attendance already exists for this employee.");
        }

        var entity = _mapper.Map<EmployeeAttendance>(request);
        entity.AttendanceDate = request.AttendanceDate.Date;

        await _unitOfWork.EmployeeAttendanceRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EmployeeAttendanceDto>(entity);
    }

    /// <inheritdoc />
    public async Task BulkAttendanceAsync(BulkEmployeeAttendanceDto request, CancellationToken cancellationToken = default)
    {
        // Request validation
        if (request.Attendance == null || !request.Attendance.Any())
        {
            throw new InvalidOperationException("Attendance list cannot be empty.");
        }

        if (request.AttendanceDate.Date > DateTime.Today)
        {
            throw new InvalidOperationException("Future attendance is not allowed.");
        }

        // Duplicate employee check within the payload itself
        var hasDuplicateEmployee = request.Attendance.GroupBy(x => x.EmployeeId).Any(g => g.Count() > 1);

        if (hasDuplicateEmployee)
        {
            throw new InvalidOperationException("Duplicate employee found in attendance list.");
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            // Validate every referenced employee exists
            var employeeIds = request.Attendance.Select(x => x.EmployeeId).ToList();
            var allEmployees = await _unitOfWork.EmployeeRepository.GetAllAsync(cancellationToken);
            var employeeDictionary = allEmployees
                .Where(x => employeeIds.Contains(x.Id))
                .ToDictionary(x => x.Id);

            // Load existing attendance for selected date
            var existingAttendances = await _unitOfWork.EmployeeAttendanceRepository
                .GetByEmployeesAndDateAsync(employeeIds, request.AttendanceDate, cancellationToken);

            var attendanceDictionary = existingAttendances.ToDictionary(x => x.EmployeeId);

            foreach (var item in request.Attendance)
            {
                if (!employeeDictionary.ContainsKey(item.EmployeeId))
                {
                    throw new NotFoundException(nameof(Employee), item.EmployeeId);
                }

                if (attendanceDictionary.TryGetValue(item.EmployeeId, out var attendance))
                {
                    // Update existing attendance
                    attendance.Status = item.Status;
                    attendance.CheckIn = item.CheckIn;
                    attendance.CheckOut = item.CheckOut;

                    _unitOfWork.EmployeeAttendanceRepository.Update(attendance);
                }
                else
                {
                    // Create new attendance
                    var newAttendance = new EmployeeAttendance
                    {
                        EmployeeId = item.EmployeeId,
                        AttendanceDate = request.AttendanceDate.Date,
                        Status = item.Status,
                        CheckIn = item.CheckIn,
                        CheckOut = item.CheckOut
                    };

                    await _unitOfWork.EmployeeAttendanceRepository.AddAsync(newAttendance, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EmployeeAttendanceDto>> GetByDateAsync(DateTime attendanceDate, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.EmployeeAttendanceRepository.GetAttendanceByDateAsync(attendanceDate, cancellationToken);
        return _mapper.Map<IReadOnlyList<EmployeeAttendanceDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<EmployeeAttendanceDto>> GetEmployeeHistoryAsync(
        int employeeId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.EmployeeAttendanceRepository
            .GetEmployeeHistoryAsync(employeeId, fromDate, toDate, cancellationToken);

        return _mapper.Map<IReadOnlyList<EmployeeAttendanceDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<EmployeeAttendanceDto> UpdateAsync(int id, UpdateEmployeeAttendanceDto request, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.EmployeeAttendanceRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(EmployeeAttendance), id);

        _mapper.Map(request, entity);

        _unitOfWork.EmployeeAttendanceRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EmployeeAttendanceDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.EmployeeAttendanceRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(EmployeeAttendance), id);

        _unitOfWork.EmployeeAttendanceRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
