using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Common.Models;
using SchoolERP.Application.Features.Invoice.DTOs;
using SchoolERP.Application.Features.Invoice.Interfaces;
using SchoolERP.Domain.Constants;
using SchoolERP.Domain.Enums;
using SchoolERP.Infrastructure.Services;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IInvoiceGenerationService _invoiceGenerationService;
        private readonly ILateFineApplicationService _lateFineApplicationService;
        public InvoicesController(IInvoiceService invoiceService, IInvoiceGenerationService invoiceGenerationService, ILateFineApplicationService lateFineApplicationService)
        {
            _invoiceService = invoiceService;
            _invoiceGenerationService = invoiceGenerationService;
            _lateFineApplicationService = lateFineApplicationService;
        }

        /// <summary>Get paged invoice list, filtered by status / student / academic year.</summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.InvoiceView)]
        [ProducesResponseType(typeof(PagedResult<InvoiceListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<InvoiceListDto>>> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] InvoiceStatus? status = null,
            [FromQuery] int? studentId = null,
            [FromQuery] int? academicYearId = null,
            CancellationToken cancellationToken = default)
        {
            var (items, totalCount) = await _invoiceService.GetPagedAsync(
                pageNumber, pageSize, status, studentId, academicYearId, cancellationToken);

            return Ok(new PagedResult<InvoiceListDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        /// <summary>Get invoice by Id, with full item detail.</summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.InvoiceView)]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoiceDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceService.GetByIdAsync(id, cancellationToken);
            if (invoice is null)
                return NotFound();
            return Ok(invoice);
        }

        /// <summary>Get all invoices for a specific student.</summary>
        [HttpGet("student/{studentId:int}")]
        [PermissionAuthorize(PermissionNames.InvoiceView)]
        [ProducesResponseType(typeof(IReadOnlyList<InvoiceListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<InvoiceListDto>>> GetByStudent(
            int studentId, CancellationToken cancellationToken)
        {
            return Ok(await _invoiceService.GetByStudentIdAsync(studentId, cancellationToken));
        }

        /// <summary>Create a manual invoice (auto-generated invoices are created by the billing job, not this endpoint).</summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.InvoiceCreate)]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InvoiceDto>> Create(
            [FromBody] CreateInvoiceDto request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
        }

        /// <summary>
        /// Cancel an issued invoice (state-transition action, not a generic update —
        /// blocked if any payment has already been recorded against it).
        /// </summary>
        [HttpPost("{id:int}/cancel")]
        [PermissionAuthorize(PermissionNames.InvoiceCancel)]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<InvoiceDto>> Cancel(
            int id, [FromBody] CancelInvoiceDto request, CancellationToken cancellationToken)
        {
            return Ok(await _invoiceService.CancelAsync(id, request, cancellationToken));
        }
        /// <summary>
        /// Bulk-generates monthly invoices for all students covered by active fee structures
        /// for the given academic year/month (optionally restricted to one class). Idempotent —
        /// re-running for the same period simply skips students who already have an invoice.
        /// </summary>
        [HttpPost("generate-monthly")]
        [PermissionAuthorize(PermissionNames.InvoiceCreate)]
        [ProducesResponseType(typeof(InvoiceGenerationResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InvoiceGenerationResultDto>> GenerateMonthly(
            [FromBody] GenerateMonthlyInvoicesDto request, CancellationToken cancellationToken)
        {
            return Ok(await _invoiceGenerationService.GenerateMonthlyInvoicesAsync(request, cancellationToken));
        }
        /// <summary>Recalculates and applies late fines on all overdue invoices. Idempotent — safe to run daily.</summary>
        [HttpPost("apply-late-fines")]
        [PermissionAuthorize(PermissionNames.InvoiceCreate)]
        [ProducesResponseType(typeof(LateFineApplicationResultDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<LateFineApplicationResultDto>> ApplyLateFines(
            [FromQuery] DateTime? asOfDate, CancellationToken cancellationToken)
        {
            return Ok(await _lateFineApplicationService.ApplyLateFinesAsync(asOfDate, cancellationToken));
        }
    }
}
