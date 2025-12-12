using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceLocation.Application.DTOs;
using ServiceLocation.Application.Exceptions;
using ServiceLocation.Application.Interfaces;
using ServiceLocation.Domain.Entities;
using ServiceLocation.Domain.Interfaces;

namespace ServiceLocation.Application.Services
{
	public class LocationServices : ILocationService
	{

		private readonly ILocationRepository _locationRepository;

		public LocationServices(ILocationRepository locationRepository)
		{
			_locationRepository = locationRepository;
		}

		//? metodo para obtener la infomacion de la ubicacion de un usuario por sus id
		public async Task<ResponseLocation> GetUserByIdAsync(string id, string typeUser)
		{
			if (typeUser == "client")
			{
				var infoUser = await _locationRepository.GetUserByIdAsync(id);
				Console.WriteLine(infoUser);
				if (infoUser == null) throw new UpdateException(new { message = "No se logro obtener la informacion del cliente", detail = "No se encontro al cliente" });
				return new ResponseLocation
				{
					id = id.ToString(),
					Lat = infoUser.Lat,
					Lng = infoUser.Lng
				};
			}
			if (typeUser == "driver")
			{
				var infoDriver = await _locationRepository.GetDriverByIsAsync(id);
				if(infoDriver == null) throw new UpdateException(new { message = "No se logro obtener la informacion del conductor", detail = "No se encontro al conductor" });
				return new ResponseLocation
				{
					id = id,
					Lat = infoDriver.Lat,
					Lng = infoDriver.Lng
				};
			}
			throw new UpdateException(new { message = "No se logro obtener la informacion del usuario", detail = "No se encontro al usuario" });
		}


		//? metodo para actualizar la ubicacion de un usuario por sus id
		public async Task UpdateLocationAsync(string id, string type, Coordinates coordinates)
		{

			Console.WriteLine("Valor lat: "+coordinates.Lat);
			if (coordinates.Lat == null || coordinates.Lng == null)
			{
				coordinates = new Coordinates
				{
					Lat = 0,
					Lng = 0
				};
			}

			if (type == "client")
			{

				await _locationRepository.UpdateLocationAsyn(id, coordinates);
			}
			else if (type == "driver")
			{
				await _locationRepository.UpdateDriverLocationAsync(id, coordinates);
			}
		}
	}
}