using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServiceLocation.Application.DTOs;
using ServiceLocation.Application.Exceptions;
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
			//?Inicializamos el servicio de ubicación
			_locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
		}

		//?Endpoint para actualizar la ubicacion de un usuario
		//?Se recibe un objeto con el id del usuario, el tipo de usuario y las nuevas coordenadas
		[HttpPut("/location/update")]
		public async Task<IActionResult> UpdateLocation(
				[FromBody] RequestUpdateLocation newLocation
		)
		{
			try
			{
				//?crear las coordenadas
				await _locationService.UpdateLocationAsync(
						newLocation.Id,
						newLocation.TypeUser,
						newLocation.newLocation
				);

				return Ok();
			}
			catch (UpdateException ex)
			{
				return StatusCode(300, ex.error);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new
				{
					message = "Error interno",
					detail = ex.Message
				});
			}
		}

		//?Endpoint para obtener la ubicacion de un usuario
		//?Por la url se recibe el is y el tipo de usuario que solo pueden ser client o driver
		[HttpGet("/location/{id}/{TypeUser}")]
		public async Task<IActionResult> GetLocation(string id, string TypeUser)
		{
			try
			{
				var response = await _locationService.GetUserByIdAsync(id, TypeUser);
				return Ok(response);
			}
			catch (UpdateException ex)
			{
				Console.WriteLine(ex);
				return StatusCode(300, ex.error);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(500, new
				{
					message = "Error interno",
					detail = ex.Message
				});
			}
		}

	}
}