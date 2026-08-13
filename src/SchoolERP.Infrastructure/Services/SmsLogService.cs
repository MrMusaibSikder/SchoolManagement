using AutoMapper;
using SchoolERP.Application.Common.Exceptions;
using SchoolERP.Application.Common.Interfaces;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.SmsLog.DTOs;
using SchoolERP.Application.Features.SmsLog.Interfaces;
using SchoolERP.Domain.Entities;
using SchoolERP.Domain.Enums;

namespace SchoolERP.Infrastructure.Services;

/// <summary>
/// Business logic for SmsLog records. Calls the repository (via the Unit of
/// Work), computes dashboard/report aggregates, and maps entities to/from
/// DTOs using AutoMapper. Logs are treated as immutable once created.
/// </summary>
public class SmsLogService : ISmsLogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SmsLogService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SmsLogDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.SmsLogRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<SmsLogDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<PagedResult<SmsLogDto>> GetPagedAsync(SmsLogQueryDto query, CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _unitOfWork.SmsLogRepository.GetPagedAsync(
            query.SearchTerm,
            query.Status,
            query.RecipientNumber,
            query.StudentId,
            query.Provider,
            query.FromDate,
            query.ToDate,
            query.PageNumber,
            query.PageSize,
            query.SortBy,
            query.SortDescending,
            cancellationToken);

        return new PagedResult<SmsLogDto>
        {
            Items = _mapper.Map<IReadOnlyList<SmsLogDto>>(items),
            TotalCount = totalCount,
            PageNumber = query.PageNumber,
            PageSize = query.PageSize
        };
    }

    /// <inheritdoc />
    public async Task<SmsLogDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SmsLogRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<SmsLogDto>(entity);
    }

    /// <inheritdoc />
    public async Task<SmsLogDto> CreateAsync(CreateSmsLogDto request, CancellationToken cancellationToken = default)
    {
        var entity = _mapper.Map<SmsLog>(request);

        await _unitOfWork.SmsLogRepository.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SmsLogDto>(entity);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.SmsLogRepository.GetByIdTrackedAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(SmsLog), id);

        _unitOfWork.SmsLogRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SmsDashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var weekStart = today.AddDays(-6);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var total = await _unitOfWork.SmsLogRepository.CountAsync(null, null, null, cancellationToken);
        var todayCount = await _unitOfWork.SmsLogRepository.CountAsync(null, today, today, cancellationToken);
        var weeklyCount = await _unitOfWork.SmsLogRepository.CountAsync(null, weekStart, today, cancellationToken);
        var monthlyCount = await _unitOfWork.SmsLogRepository.CountAsync(null, monthStart, today, cancellationToken);

        var sentCount = await _unitOfWork.SmsLogRepository.CountAsync(SmsStatus.Sent, null, null, cancellationToken);
        var deliveredCount = await _unitOfWork.SmsLogRepository.CountAsync(SmsStatus.Delivered, null, null, cancellationToken);
        var failedCount = await _unitOfWork.SmsLogRepository.CountAsync(SmsStatus.Failed, null, null, cancellationToken);
        var pendingCount = await _unitOfWork.SmsLogRepository.CountAsync(SmsStatus.Pending, null, null, cancellationToken);

        var successCount = sentCount + deliveredCount;
        var attempted = successCount + failedCount;

        return new SmsDashboardStatsDto
        {
            TotalSms = total,
            TodaySms = todayCount,
            WeeklySms = weeklyCount,
            MonthlySms = monthlyCount,
            SuccessCount = successCount,
            FailedCount = failedCount,
            PendingCount = pendingCount,
            SuccessRate = attempted == 0 ? 0 : Math.Round(successCount * 100.0 / attempted, 2)
        };
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SmsLogDto>> GetTodayAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var entities = await _unitOfWork.SmsLogRepository.GetBetweenDatesAsync(today, today, cancellationToken);
        return _mapper.Map<IReadOnlyList<SmsLogDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SmsLogDto>> GetWeeklyAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.Today;
        var entities = await _unitOfWork.SmsLogRepository.GetBetweenDatesAsync(today.AddDays(-6), today, cancellationToken);
        return _mapper.Map<IReadOnlyList<SmsLogDto>>(entities);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SmsLogDto>> GetFailedAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var query = new SmsLogQueryDto
        {
            Status = SmsStatus.Failed,
            FromDate = fromDate,
            ToDate = toDate,
            PageNumber = 1,
            PageSize = 100
        };

        var (items, _) = await _unitOfWork.SmsLogRepository.GetPagedAsync(
            null, query.Status, null, null, null, query.FromDate, query.ToDate,
            query.PageNumber, query.PageSize, "createdat", true, cancellationToken);

        return _mapper.Map<IReadOnlyList<SmsLogDto>>(items);
    }

    /// <inheritdoc />
    public async Task<double> GetSuccessRateAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var sentCount = await _unitOfWork.SmsLogRepository.CountAsync(SmsStatus.Sent, fromDate, toDate, cancellationToken);
        var deliveredCount = await _unitOfWork.SmsLogRepository.CountAsync(SmsStatus.Delivered, fromDate, toDate, cancellationToken);
        var failedCount = await _unitOfWork.SmsLogRepository.CountAsync(SmsStatus.Failed, fromDate, toDate, cancellationToken);

        var successCount = sentCount + deliveredCount;
        var attempted = successCount + failedCount;

        return attempted == 0 ? 0 : Math.Round(successCount * 100.0 / attempted, 2);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SmsDailyReportDto>> GetDailyReportAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        if (toDate.Date < fromDate.Date)
            throw new BadRequestException("'toDate' cannot be earlier than 'fromDate'.");

        var logs = await _unitOfWork.SmsLogRepository.GetBetweenDatesAsync(fromDate, toDate, cancellationToken);

        var report = new List<SmsDailyReportDto>();

        for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
        {
            var dayLogs = logs.Where(x => x.CreatedAt.Date == date).ToList();

            report.Add(new SmsDailyReportDto
            {
                Date = date,
                Total = dayLogs.Count,
                Success = dayLogs.Count(x => x.Status is SmsStatus.Sent or SmsStatus.Delivered),
                Failed = dayLogs.Count(x => x.Status == SmsStatus.Failed),
                Pending = dayLogs.Count(x => x.Status == SmsStatus.Pending)
            });
        }

        return report;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SmsMonthlyReportDto>> GetMonthlyReportAsync(int year, CancellationToken cancellationToken = default)
    {
        var yearStart = new DateTime(year, 1, 1);
        var yearEnd = new DateTime(year, 12, 31);

        var logs = await _unitOfWork.SmsLogRepository.GetBetweenDatesAsync(yearStart, yearEnd, cancellationToken);

        var report = new List<SmsMonthlyReportDto>();

        for (var month = 1; month <= 12; month++)
        {
            var monthLogs = logs.Where(x => x.CreatedAt.Month == month).ToList();

            report.Add(new SmsMonthlyReportDto
            {
                Year = year,
                Month = month,
                MonthName = new DateTime(year, month, 1).ToString("MMMM"),
                Total = monthLogs.Count,
                Success = monthLogs.Count(x => x.Status is SmsStatus.Sent or SmsStatus.Delivered),
                Failed = monthLogs.Count(x => x.Status == SmsStatus.Failed),
                Pending = monthLogs.Count(x => x.Status == SmsStatus.Pending)
            });
        }

        return report;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<TopRecipientDto>> GetTopRecipientsAsync(int count, DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        var results = await _unitOfWork.SmsLogRepository.GetTopRecipientsAsync(count, fromDate, toDate, cancellationToken);

        return results
            .Select(x => new TopRecipientDto
            {
                RecipientNumber = x.RecipientNumber,
                StudentId = x.StudentId,
                StudentName = x.StudentName,
                MessageCount = x.MessageCount
            })
            .ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<SmsLogDto>> GetRecentActivityAsync(int count, CancellationToken cancellationToken = default)
    {
        var entities = await _unitOfWork.SmsLogRepository.GetRecentAsync(count, cancellationToken);
        return _mapper.Map<IReadOnlyList<SmsLogDto>>(entities);
    }
}
