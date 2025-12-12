using System.Data.Common;
using System.Text.Json;
using ServiceLocation.Domain.Entities;
using ServiceLocation.Domain.Interfaces;
using StackExchange.Redis;

namespace ServiceLocation.Infrastructure.Repositories
{
    public class RedisUserRepository : IUserRespository
    {
        private readonly IDatabase _db;

        public RedisUserRepository(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        //? formatos de almacenamiento de datos en redis
        private string UserKey(string id) => $"user:{id}"; //? Formato par la info de usuario
        private string LocationKey(string id) => $"user:{id}:location"; //? Formato para la información de la ubicacion

        public async Task<UserRedis?> SaveUserConnected(string id, string typeUser, Coordinates location, string connectionId)
        {
            try
            {
                var user = new UserRedis
                {
                    Id = id,
                    TypeUser = typeUser,
                    State = "Connected",
                    ConnectionString = connectionId
                };

                var jsonUser = JsonSerializer.Serialize(user);
                Console.WriteLine("id: ==========================" + id);

                // Guardar usuario con TTL
                await _db.StringSetAsync(UserKey(id), jsonUser, TimeSpan.FromHours(1));

                // Guardar ubicación con TTL

                await _db.StringSetAsync(LocationKey(id), JsonSerializer.Serialize(location), TimeSpan.FromHours(1));

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar el usuario conectado en Redis", ex);
            }
        }

        public async Task<UserRedis?> DisconnectUser(string id, string typeUser, Coordinates location)
        {
            try
            {
                var user = new UserRedis
                {
                    Id = id,
                    TypeUser = typeUser,
                    State = "Disconnected",
                    ConnectionString = ""
                };

                var jsonUser = JsonSerializer.Serialize(user);

                await _db.StringSetAsync(UserKey(id), jsonUser, TimeSpan.FromHours(1));

                await _db.StringSetAsync(LocationKey(id), JsonSerializer.Serialize(location));

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar el usuario desconectado en Redis", ex);
            }
        }

        public async Task<UserRedis> SetLocationUser(string Id, Coordinates location, string IdClient)
        {
            try
            {
                Console.WriteLine("hola: id ===: " + IdClient.ToString());
                await _db.StringSetAsync(LocationKey(Id), JsonSerializer.Serialize<Coordinates>(location)); //?Guardamos la ubicación del usuario
                //? Obtenemos el idDe connection del usuarios
                var userConnection = await _db.StringGetAsync($"user:{Id}:connectionId");
                string connectionClient = "";

                if (IdClient != "")
                {//!Aqui se podrian tener un error ya qte el id llega vacio
                    connectionClient = await _db.StringGetAsync($"user:{IdClient}:connectionId");
                }

                return new UserRedis
                {
                    Id = Id,
                    TypeUser = "",
                    State = "Connected",
                    ConnectionString = userConnection.ToString(),
                    ConnectionClient = connectionClient.ToString()
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la ubicación en Redis", ex);
            }
        }
    }
}
