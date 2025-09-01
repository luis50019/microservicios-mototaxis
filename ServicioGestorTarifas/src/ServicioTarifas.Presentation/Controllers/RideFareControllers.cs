using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServicioTarifas.Application.DTOs;
using ServicioTarifas.Application.Exceptions;
using ServicioTarifas.Application.Interfaces;
using ServicioTarifas.Application.Services;

namespace ServicioTarifas.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RideFareControllers : ControllerBase
    {
        private readonly IRideFaresService _rideFareService;

        public RideFareControllers(IRideFaresService rideFareService)
        {
            _rideFareService = rideFareService;
        }

        // POST: api/ridefare
        [HttpPost]
        public async Task<IActionResult> AddRideFare([FromBody] RequestNewRideFare newFare)
        {
            try
            {
                var result = await _rideFareService.AddRideFare(newFare);
                return Ok(new ResponseRideFare
                {
                    Id = result.Id,
                    DistanceMax = result.DistanceMax,
                    DistanceMin = result.DistanceMin,
                    Price = result.Price,
                    IsActive = result.IsActive,
                    CreatedAt = result.CreatedAt
                });

            }
            catch (ExceptionRideFare ex)
            {
                return BadRequest(new { error = ex.Message, details = ex.Data });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = "Error inesperado", details = ex.Message });
            }
        }

        // GET: api/ridefare/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRideFare(string id)
        {
            try
            {
                var result = await _rideFareService.GetRideFare(id);
                return Ok(result);
            }
            catch (ExceptionRideFare ex)
            {
                return NotFound(new { error = ex.Message, details = ex.Data });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = "Error inesperado", details = ex.Message });
            }
        }

        // PATCH: api/ridefare/{id}/distance
        [HttpPatch("{id}/distance")]
        public async Task<IActionResult> UpdateDistance(string id, [FromBody] RequestUpdateDistance update)
        {
            try
            {
                var result = await _rideFareService.UpdateDistanceAsync(id, update.DistanceMin, update.DistanceMax);
                return Ok(result);
            }
            catch (ExceptionRideFare ex)
            {
                return BadRequest(new { error = ex.Message, details = ex.Data });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = "Error inesperado", details = ex.Message });
            }
        }

        // PATCH: api/ridefare/{id}/price
        [HttpPatch("{id}/price")]
        public async Task<IActionResult> UpdatePrice(string id, [FromBody] RequestUpdatePrice update)
        {
            try
            {
                var result = await _rideFareService.UpdatePriceAsync(id, update.NewPrice);
                return Ok(result);
            }
            catch (ExceptionRideFare ex)
            {
                return BadRequest(new { error = ex.Message, details = ex.Data });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = "Error inesperado", details = ex.Message });
            }
        }
    }

}