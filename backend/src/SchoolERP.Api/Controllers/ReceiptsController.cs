using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.Receipt.DTOs;
using SchoolERP.Application.Features.Receipt.Interfaces;
using SchoolERP.Domain.Constants;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptService;
        private readonly IReceiptPdfService _receiptPdfService;

        public ReceiptsController(IReceiptService receiptService, IReceiptPdfService receiptPdfService)
        {
            _receiptService = receiptService;
            _receiptPdfService = receiptPdfService;
        }

        /// <summary>Get receipt by Id.</summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.ReceiptView)]
        [ProducesResponseType(typeof(ReceiptDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReceiptDto>> GetById(int id, CancellationToken cancellationToken)
        {
            var receipt = await _receiptService.GetByIdAsync(id, cancellationToken);
            if (receipt is null)
                return NotFound();
            return Ok(receipt);
        }

        /// <summary>Get the receipt for a specific payment (for print/reprint).</summary>
        [HttpGet("payment/{paymentId:int}")]
        [PermissionAuthorize(PermissionNames.ReceiptView)]
        [ProducesResponseType(typeof(ReceiptDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReceiptDto>> GetByPayment(int paymentId, CancellationToken cancellationToken)
        {
            var receipt = await _receiptService.GetByPaymentIdAsync(paymentId, cancellationToken);
            if (receipt is null)
                return NotFound();
            return Ok(receipt);
        }

        /// <summary>
        /// Void a receipt directly (edge-case only — e.g. a reprint error).
        /// Prefer voiding via PaymentsController.Void, which keeps invoice balance in sync.
        /// </summary>
        [HttpPost("{id:int}/void")]
        [PermissionAuthorize(PermissionNames.ReceiptVoid)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Void(
            int id, [FromBody] VoidReceiptDto request, CancellationToken cancellationToken)
        {
            await _receiptService.VoidAsync(id, request, cancellationToken);
            return NoContent();
        }
        /// <summary>Generates and downloads the receipt as a PDF file.</summary>
        [HttpGet("{id:int}/pdf")]
        [PermissionAuthorize(PermissionNames.ReceiptView)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPdf(int id, CancellationToken cancellationToken)
        {
            var pdfBytes = await _receiptPdfService.GenerateReceiptPdfAsync(id, cancellationToken);
            return File(pdfBytes, "application/pdf", $"Receipt-{id}.pdf");
        }
    }
}
