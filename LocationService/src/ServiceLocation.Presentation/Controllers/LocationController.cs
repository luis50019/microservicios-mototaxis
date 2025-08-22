using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceLocation.Application.Interfaces;

namespace ServiceLocation.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {

        private readonly ILocationService _locationService;
        public LocationController(ILocationService locationService)
        {
            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
        }

        [HttpPut]
        public IActionResult UpdateLocation()
        {
            return Ok("Location update successfully");
        }

        [HttpGet("/users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var user = await _locationService.GetAllUserAsync();
                return Ok(user);
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex.Message);
            }
        }
    }
}