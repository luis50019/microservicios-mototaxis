using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Core.DTOs;
using AuthService.Core.Entities;
using AuthService.Core.Interfaces;
using AuthService.UseCases.Exceptions;

using MongoDB.Driver;
using Mono.TextTemplating;

namespace AuthService.Infrastructure.Data.Mongo
{
	public class MongoUserRepository : IUserRepository
	{

		private readonly IMongoCollection<User> _collection;
		private readonly IMongoCollection<Driver> _collectionDriver;

		public MongoUserRepository(IMongoDatabase database)
		{
			_collection = database.GetCollection<User>("users");
			_collectionDriver = database.GetCollection<Driver>("drivers");
		}
		//* metodo para añadir un nuevo usuario
		public async Task AddAsync(User newClient)
		{
			await _collection.InsertOneAsync(newClient);
		}

		//* Metodo para añadir un nuevo conductor
		public async Task AddAsyncDriver(Driver driver)
		{
			await _collectionDriver.InsertOneAsync(driver);
		}

		//* Metodo para buscar un usuario por id
		public async Task<User?> GetUserById(string id)
		{
			var objectId = new MongoDB.Bson.ObjectId(id);
			return await _collection.Find(u => u.Id == objectId).FirstOrDefaultAsync();
		}

		//*Busca el telefono que le indiquemos
		//*Lo busca en las colecciones de usuario y driver
		public async Task<bool> ExistPhoneAsync(string phone)
		{
			var userExistsTask = _collection
					.Find(u => u.BasicInfo.Phone.Number == phone)
					.AnyAsync();

			var driverExistsTask = _collectionDriver
					.Find(d => d.BasicInfo.Phone.Number == phone)
					.AnyAsync();

			await Task.WhenAll(userExistsTask, driverExistsTask);

			return userExistsTask.Result || driverExistsTask.Result;
		}

		//*Metodo par obtener un usuario mediante su numero de telefono y validar su contraseña
		public async Task<LoginResult> GetUser(Phone phone, string password)
		{
			try
			{
				var userTask = _collection.Find(u =>
						u.BasicInfo.Phone.Number == phone.Number
				).FirstOrDefaultAsync();

				var driverTask = _collectionDriver.Find(u =>
						u.BasicInfo.Phone.Number == phone.Number
				).FirstOrDefaultAsync();

				await Task.WhenAll(userTask, driverTask);

				var user = await userTask;
				var driver = await driverTask;

				if (user != null)
				{

					if (!string.IsNullOrEmpty(user.BasicInfo?.Password) &&
							BCrypt.Net.BCrypt.Verify(password, user.BasicInfo.Password))
					{
						return new LoginResult
						{
							Type = "client",
							Id = user.Id.ToString(),
							State = true
						};
					}
				}

				if (driver != null)
				{

					if (!string.IsNullOrEmpty(driver.BasicInfo?.Password) &&
							BCrypt.Net.BCrypt.Verify(password, driver.BasicInfo.Password))
					{
						return new LoginResult
						{
							Type = "driver",
							Id = driver.Id.ToString(),
							State = true
						};
					}
				}

				return new LoginResult { Error = "Credenciales inválidas", State = false };
			}
			catch (Exception ex)
			{
				Console.WriteLine("Excepción en GetUser: " + ex.Message);
				return new LoginResult { Error = "Error interno en el servidor", State = false };
			}
		}


	}
}