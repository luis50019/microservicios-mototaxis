using System;
using System.Text.Json;
using System.Threading.Tasks;
using MicroServicio.Conductores.Data;
using MicroServicio.Reservaciones.Config;
using MicroServicio.Reservaciones.Data;
using MicroServicio.Reservaciones.DTOs;
using MicroServicio.Reservaciones.models;
using MicroServicio.Reservaciones.utils;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace MicroServicio.Reservaciones.Services
{
    public class ReservationService : IDisposable
    {
        private readonly IMongoCollection<Reservation> _reservationsCollection;
        private readonly IMongoCollection<Driver> _driver;
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _queueName;

        public ReservationService(
            IOptions<MongoDBSettings> mongoSettings,
            IOptions<RabbitMQSettings> rabbitSettings)
        {
            //TODO: separar esta logica para obtener las bases de datos observa como se realiza en los demas servicio y guiate con eso

            var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoSettings.Value.Database);
            _reservationsCollection = mongoDatabase.GetCollection<Reservation>("reservationsPrivate");
            _driver = mongoDatabase.GetCollection<Driver>("drivers");

            //* Esta logica si se queda aqui la de rabbitMQ
            //?O si puedes separar la logica para que queda mas limpio el constructor y la logica de conexion a rabbit quede en otro metodo
            _queueName = rabbitSettings.Value.QueueName;

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(rabbitSettings.Value.url),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                RequestedConnectionTimeout = TimeSpan.FromSeconds(30)
            };
            _connection = Task.Run(async () => await factory.CreateConnectionAsync()).Result;
            _channel = Task.Run(async () => await _connection.CreateChannelAsync()).Result;

            Task.Run(async () =>
            {
                await _channel.QueueDeclareAsync(
                    queue: _queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
                await _channel.QueueDeclareAsync(
                    queue: "fare_response_queue",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
            }).Wait();

            _channel.BasicQosAsync(0, 1, false).GetAwaiter().GetResult();
        }

        //!Registra la reserva y publica mensaje en cola
        public async Task RegisterReservationAsync(RequestReservations reservation)
        {
            Console.WriteLine("Iniciando registro de reservación...");
            Console.WriteLine(JsonSerializer.Serialize(reservation));

            //?Cres que haga falta validar una distancia maxima entre origen y destino en este servicio?
            //? o eso se deberia de validar en el servicio que calcula la tarifa?
            const double maxDistanceMeters = 500000; // 500 km max

            //*Acutalizamos el estado del conductor a "Ocupado" y aumentamos su tasa de aceptación
            var filter = Builders<Driver>.Filter.Eq(d => d.Id, ObjectId.Parse(reservation.infoDriver.data.id));
            var update = Builders<Driver>.Update
                .Set(d => d.StateDriver, "Ocupado")
                .Inc(d => d.Performance.AcceptanceRate, 1)
                .CurrentDate(d => d.UpdatedAt);

            var result = await _driver.UpdateOneAsync(filter, update);

            //! generamos el codigo de verificacion
            string code = VerificationCode.GenerarCodigo();

            //*Creamos la reservacion
            //TODO: separar esta logica a un metodo aparte
            //? la idea es que al metodo solo le pases la variaable reservation y este metodo te regresa el objeto de Reservation ya creado
            var newReservation = new Reservation
            {
                Driver = ObjectId.Parse(reservation.infoDriver.data.id),
                Rate = ObjectId.Parse(reservation.infoDriver.data.rideFare.fareinfo.FareId),
                Route = new Route
                {
                    Start = new Coordinate
                    {
                        Lat = reservation.infoDriver.data.locationStart.Lat.Value,
                        Lng = reservation.infoDriver.data.locationStart.Lng.Value,
                    },
                    Destination = new Coordinate
                    {
                        Lat = reservation.infoDriver.data.locationEnd.Lat.Value,
                        Lng = reservation.infoDriver.data.locationEnd.Lng.Value,
                    },
                    Distance = reservation.infoDriver.data.rideFare.fareinfo.DistanceMax
                },
                NumberPassage = 1,//TODO: falta enviar como una opcion desde que se crea la reservacion
                Passage = ObjectId.Parse(reservation.infoDriver.data.rideFare.idUser),
                State = new State
                {
                    General = "En curso",
                    Details = new StateDetails
                    {
                        Detail = "Conductor en camino",
                        SpacenNumber = 0,
                    }
                },
                Security = new Security
                {
                    CodeVerification = code,
                    IsVerified = false
                },
                Comments = new Comments
                {
                    Rating = new Rating //* Despues se creara un servicio para modificar estas estadisticas 
                    //* por el momento se dejan con valores base
                    {
                        Overall = 2,
                        Categories = new RatingCategories
                        {
                            Punctuality = 2,
                            Driving = 2,
                            Vehicle = 2
                        }
                    }
                },
                Pay = new Pay
                {
                    Methodo = "efectivo",
                    State = "Pendiente"
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            //* insertamos la nueva tarifa
            await _reservationsCollection.InsertOneAsync(newReservation);

            Console.WriteLine("Reservación registrada con éxito.");
            Console.WriteLine(JsonSerializer.Serialize(newReservation));
            Console.WriteLine("======================================================");
            Console.WriteLine("\nPublicando mensaje en la cola...");

            var driver = await _driver.Find(d => d.Id == ObjectId.Parse(reservation.infoDriver.data.id))
                          .FirstOrDefaultAsync();

            if (driver == null)
            {
                Console.WriteLine("No se encontró información del conductor con ese ID.");
                throw new Exception("Conductor no encontrado en la base de datos.");
            }



            //! crear respuesta para devolver el mensaje
            var messageResponse = new ResponseReservation
            {
                IdReservation = newReservation.Id.ToString(),
                IdClient = newReservation.Passage.ToString(),
                IdDriver = newReservation.Driver.ToString(),
                CodeVerification = code,
                InfoDriver = new InfoDriver
                {
                    idDriver = newReservation.Driver.ToString(),
                    LicensePlate = driver.Unit.LicensePlate,
                    name = driver.BasicInfo.Name,
                    numberUnit = driver.Unit.Number,
                    Phone = driver.BasicInfo.Phone.Number,
                    PhotoDriver = driver.BasicInfo.ProfilePicture,
                },

            };

            string messageJson = JsonSerializer.Serialize(messageResponse);
            var body = System.Text.Encoding.UTF8.GetBytes(messageJson);

            await _channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: "viaje_registrado_queue",
                mandatory: true,
                basicProperties: new BasicProperties { Persistent = true },
                body: body);
        }

        public void Dispose()
        {
            _channel?.CloseAsync();
            _connection.CloseAsync();
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}