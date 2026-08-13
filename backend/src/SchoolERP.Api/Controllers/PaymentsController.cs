using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.Payment.DTOs;
using SchoolERP.Application.Features.Payment.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>Get payment by Id, with receipt reference.</summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.PaymentView)]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var payment = await _paymentService.GetByIdAsync(id, cancellationToken);
            if (payment is null)
                return NotFound();
            return Ok(payment);
        }

        /// <summary>Get all completed payments for a specific invoice (payment history).</summary>
        [HttpGet("invoice/{invoiceId:int}")]
        [PermissionAuthorize(PermissionNames.PaymentView)]
        [ProducesResponseType(typeof(IReadOnlyList<PaymentListDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<PaymentListDto>>> GetByInvoice(
            int invoiceId, CancellationToken cancellationToken)
        {
            return Ok(await _paymentService.GetByInvoiceIdAsync(invoiceId, cancellationToken));
        }

        /// <summary>
        /// Collect a payment against an invoice. Creates Payment + Receipt and updates
        /// Invoice.AmountPaid/BalanceDue/Status atomically in a single DB transaction.
        /// A 409 means another payment against the same invoice landed first — reload and retry.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.PaymentCollect)]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PaymentDto>> Create(
            [FromBody] CreatePaymentDto request, CancellationToken cancellationToken)
        {
            var payment = await _paymentService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
        }

        /// <summary>
        /// Void a payment (reverses the invoice balance and voids the linked receipt).
        /// Use this instead of deleting — financial records are never hard-deleted.
        /// </summary>
        [HttpPost("{id:int}/void")]
        [PermissionAuthorize(PermissionNames.PaymentVoid)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Void(
            int id, [FromBody] VoidPaymentDto request, CancellationToken cancellationToken)
        {
            await _paymentService.VoidAsync(id, request, cancellationToken);
            return NoContent();
        }
    }
}
