using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolERP.Api.Authorization;
using SchoolERP.Application.Features.AcademicYear.DTOs;
using SchoolERP.Application.Features.AcademicYear.Interfaces;
using SchoolERP.Domain.Constants;
using SchoolERP.Domain.Entities;
using SchoolERP.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class AcademicYearsController : ControllerBase
    {
        private readonly IAcademicYearService _academicYearService;

        public AcademicYearsController(IAcademicYearService academicYearService)
        {
            _academicYearService = academicYearService;
        }

        /// <summary>
        /// Get all academic years.
        /// </summary>
        [HttpGet]
        [PermissionAuthorize(PermissionNames.AcademicYearView)]
        [ProducesResponseType(typeof(IReadOnlyList<AcademicYearDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IReadOnlyList<AcademicYearDto>>> GetAll(
            CancellationToken cancellationToken = default)
        {
            var academicYears = await _academicYearService.GetAllAsync(cancellationToken);

            return Ok(academicYears);
        }

        /// <summary>
        /// Get academic year by Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [PermissionAuthorize(PermissionNames.AcademicYearView)]
        [ProducesResponseType(typeof(AcademicYearDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AcademicYearDto>> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var academicYear = await _academicYearService.GetByIdAsync(id, cancellationToken);

            if (academicYear is null)
                return NotFound();

            return Ok(academicYear);
        }

        /// <summary>
        /// Create academic year.
        /// </summary>
        [HttpPost]
        [PermissionAuthorize(PermissionNames.AcademicYearCreate)]
        [ProducesResponseType(typeof(AcademicYearDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AcademicYearDto>> Create(
            [FromBody] CreateAcademicYearDto request,
            CancellationToken cancellationToken = default)
        {
            var academicYear = await _academicYearService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = academicYear.Id },
                academicYear);
        }

        /// <summary>
        /// Update academic year.
        /// </summary>
        [HttpPut("{id:int}")]
        [PermissionAuthorize(PermissionNames.AcademicYearEdit)]
        [ProducesResponseType(typeof(AcademicYearDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AcademicYearDto>> Update(
            int id,
            [FromBody] UpdateAcademicYearDto request,
            CancellationToken cancellationToken = default)
        {
            if (id != request.Id)
                return BadRequest("Route Id and Academic Year Id must match.");

            var academicYear = await _academicYearService.UpdateAsync(id, request, cancellationToken);

            return Ok(academicYear);
        }

        /// <summary>
        /// Delete academic year.
        /// </summary>
        [HttpDelete("{id:int}")]
        [PermissionAuthorize(PermissionNames.AcademicYearDelete)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken = default)
        {
            await _academicYearService.DeleteAsync(id, cancellationToken);

            return NoContent();
        }

        ///// <summary>
        ///// Activate academic year.
        ///// </summary>
        //[HttpPatch("{id:int}/activate")]
        //[PermissionAuthorize(PermissionNames.AcademicYearActivate)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> Activate(
        //    int id,
        //    CancellationToken cancellationToken = default)
        //{
        //    await _academicYearService.ActivateAsync(id, cancellationToken);

        //    return NoContent();
        //}

        ///// <summary>
        ///// Close academic year.
        ///// </summary>
        //[HttpPatch("{id:int}/close")]
        //[PermissionAuthorize(PermissionNames.AcademicYearClose)]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> Close(
        //    int id,
        //    CancellationToken cancellationToken = default)
        //{
        //    await _academicYearService.CloseAsync(id, cancellationToken);

        //    return NoContent();
        //}
    }
}