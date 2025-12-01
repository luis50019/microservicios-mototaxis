using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using ServiceLocation.Application.Exceptions;
using ServiceLocation.Domain.Entities;
using ServiceLocation.Domain.Interfaces;
using ZstdSharp.Unsafe;

namespace ServiceLocation.Infrastructure.Repositories
{
	public class MongoLocationRepository : ILocationRepository
	{
		//?Coleccion que contiene la informacion de los usuarios
		private readonly IMongoCollection<User> _userCollection;
		//?Coleccion que contiene la informacion de los conductores
		private readonly IMongoCollection<Driver> _driverCollection;

		public MongoLocationRepository(IMongoDatabase database)
		{
			_userCollection = database.GetCollection<User>("users");
			_driverCollection = database.GetCollection<Driver>("drivers");
		}

		//?Metodo que devuelve la informacion de un usuario
		public async Task<Coordinates> GetUserByIdAsync(string id)
		{
			var user = await _userCollection.Find(user => user.Id == MongoDB.Bson.ObjectId.Parse(id)).FirstOrDefaultAsync();

			return user == null?null: new Coordinates
			{
				Lat = user.Location.Current.Coordinates.Lat,
				Lng = user.Location.Current.Coordinates.Lng
			};
		}

		//?Metodo que devuelve la informacuin de un conductor
		public async Task<Coordinates> GetDriverByIsAsync(string id)
		{
			var driver = await _driverCollection.Find(driver => driver.Id == MongoDB.Bson.ObjectId.Parse(id)).FirstOrDefaultAsync();
			return driver==null?null:new Coordinates
			{
				Lat = driver.Location.Current.Coordinates.Lat,
				Lng = driver.Location.Current.Coordinates.Lng
			};
		}

		//?Metodo que actualiza la ubicacion de un usuario
		public Task UpdateLocationAsyn(string id, Coordinates coordinates)
		{
			//!Buscamos y actualizamos la ubicacion del usuario
			var userFound = _userCollection.FindOneAndUpdate(
					user => user.Id == MongoDB.Bson.ObjectId.Parse(id),
					Builders<User>.Update.Set("Location.Current.Coordinates", coordinates),
					new FindOneAndUpdateOptions<User>
					{
						ReturnDocument = ReturnDocument.After
					}
			);

			//!Si no se encntra el usuario lanzamos una excepcion
			if (userFound == null)
			{
				throw new UpdateException(new { message ="No se logro actualizar la ubicacion",detail ="El cliente no se logro encontrar" });
			}

			//?Indicamos que la tarea se ha finalizado
			return Task.CompletedTask;

		}

		//?Metodo que actualiza la ubicacion de un conductor
		public Task UpdateDriverLocationAsync(string id, Coordinates coordinates)
		{
			//!Buscamos y actualizamos la ubicacion del usuario
			var driverFound = _driverCollection.FindOneAndUpdate(
					user => user.Id == MongoDB.Bson.ObjectId.Parse(id),
					Builders<Driver>.Update.Set(d => d.Location.Current.Coordinates, coordinates),
					new FindOneAndUpdateOptions<Driver>
					{
						ReturnDocument = ReturnDocument.After
					}
			);

			//!Si no se encntra el usuario lanzamos una excepcion
			if (driverFound == null)
			{
				throw new UpdateException(new { message ="No se logro actualizar la ubicacion",detail ="El conductor no se logro encontrar" });
			}

			//?Indicamos que la tarea se ha finalizado
			return Task.CompletedTask;

		}
	}
}