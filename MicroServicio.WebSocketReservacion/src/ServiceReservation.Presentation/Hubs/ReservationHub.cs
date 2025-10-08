using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ServiceReservation.Application.DTOs;
using ServiceReservation.Infrastructure.Messaging.Consumers;
using ServiceReservation.Infrastructure.Messaging.Producers;

namespace ServiceReservation.Presentation.Hubs
{

    //TODO: testear todo la conexion que realizamos, calcular la tarifa, aceptar al tarifa, aceptar el viaje, rechazar el viaje
    public class ReservationHub : Hub
    {
        //!Añadiendo la clase para que se pueda guardar los usuarios conectados
        private readonly UserConnectionManager _userConnectionManager;


        //!debemos de crear los servicios que vamos a utilizar
        private readonly RabbitMqRideFarePublisher _publisherRideFare;
        private readonly RabbitMQFindDriver _publisherFindDriver;
        private readonly RabbitMqRideFareConsumer _consumerRideFare;
        private readonly RabbitMQDriverConsumer _consumerDriverInfo;
        private readonly RabbitMQCodeSecurity _consumerCodeSecurity;
        public ReservationHub(UserConnectionManager UserConnectionManager, RabbitMqRideFarePublisher publisherRideFare, RabbitMQDriverConsumer consumerDriverInfo, RabbitMqRideFareConsumer consumerRideFare, RabbitMQFindDriver finDriver, RabbitMQCodeSecurity consumerCodeSecurity)
        {
            _publisherRideFare = publisherRideFare;
            _consumerCodeSecurity = consumerCodeSecurity ?? throw new ArgumentNullException(nameof(consumerCodeSecurity));
            _publisherFindDriver = finDriver;
            _consumerRideFare = consumerRideFare ?? throw new ArgumentNullException(nameof(consumerRideFare));
            _userConnectionManager = UserConnectionManager;
            _consumerDriverInfo = consumerDriverInfo ?? throw new ArgumentNullException(nameof(consumerDriverInfo));
        }

        public Task registerUSer(string idUser)
        {
            _userConnectionManager.AddConnection(idUser, Context.ConnectionId);
            return Task.CompletedTask;
        }

        //? Metodo que recibe la distancia a recoorrer y calcula la tarifa
        public async Task SendDistanceTraveled(RequestDistanceTraveled data)
        {
            try
            {
                //?preparamos el mensaje a enviar
                Console.WriteLine(" ------------------------------------------------------Recibiendo la distancia recorrida");
                //! publicamos en RabbitMq
                Console.WriteLine($"info recibida: {data.IdUser} = {data.distanceTraveled}");
                await _publisherRideFare.PublicAsync(data);
                //!notificar al usuario que su tarifa esta siendo calculada
                var response = await _consumerRideFare.ConsumerRideAsync();
                Console.WriteLine("---------- tarifa recibida -------------");
                Console.WriteLine(response.Fare.FareId);
                Console.WriteLine("----------------------");
                //? reenviar el mensaje de la tarifa al usurio que le corresponde
                var connections = _userConnectionManager.GetConnections(data.IdUser);
                foreach (var connectionId in connections)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveDistance", response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en SendDistanceTraveled: " + ex.Message);
                throw;
            }
        }

        //* Metodo que se encarga de mandar el mensaje para que comiencen a buscar a un conductor
        public async Task FindDriverAsync(RequestFindDriver infoTraveled)
        {
            try
            {
                Console.WriteLine("Iniciando la busqueda de un conductor");
                if (infoTraveled == null)
                {
                    Console.WriteLine("❌ infoTraveled llegó nulo");
                    return;
                }

                if (_publisherFindDriver == null)
                {
                    Console.WriteLine("❌ _publisherFindDriver es nulo (no fue inyectado)");
                    return;
                }

                Console.WriteLine("----------------------bucsando conductor-------------------");
                Console.WriteLine($"✅ info recibida websocket: {infoTraveled.priceTraveled} = {infoTraveled.fare.idUser}");
                await _publisherFindDriver.PublicAsync(infoTraveled);
                //?Metodo que espera la informacion del conductor asignado

                var responseDriver = await _consumerDriverInfo.ConsumerRideAsync();
                if (responseDriver == null)
                {
                    Console.WriteLine("❌ responseDriver llegó nulo");
                    return;
                }
                if (responseDriver.Data == null)
                {
                    Console.WriteLine("❌ responseDriver.data llegó nulo");
                    return;
                }
                //! falta logica para enviar la informacion del conductor al usuario que lo solicito
                var connections = _userConnectionManager.GetConnections("68d72860e0429a307d8bfc94");
                if (connections == null || !connections.Any())
                {
                    Console.WriteLine("⚠️ No se encontraron conexiones activas para el conductor asignado");
                    return;
                }

                Console.WriteLine("------- conductor asignado -------");
                Console.WriteLine(responseDriver.Data.id);
                foreach (var connectionId in connections)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveDriver", new
                    {
                        infoDriver = responseDriver,
                        fareinfo = infoTraveled.fare,
                        origin = infoTraveled.locationStart,
                        destination = infoTraveled.locationEnd
                    });
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error en FindDriverAsync: " + ex.Message);
                throw;
            }
        }

        //?Metodo para que el conductor pueda aceptar el viaje que se le fue asignado
        public async Task AcceptedTrip(RequestAcceptTrip driverInfo)
        {
            try
            {
                Console.WriteLine("------------------------------------------ Conductor aceptando el viaje");
                if (driverInfo == null)
                {
                    Console.WriteLine("❌ infoTraveled llegó nulo");
                    return;
                }

                if (_publisherFindDriver == null)
                {
                    Console.WriteLine("❌ _publisherFindDriver es nulo (no fue inyectado)");
                    return;
                }

                if (driverInfo.infoDriver == null || driverInfo.infoDriver.data == null)
                {
                    Console.WriteLine("❌ driverInfo.infoDriver.data llegó nulo");
                    return;
                }

                Console.WriteLine($"✅ info recibida: {driverInfo.infoDriver.data.id}");
                await _publisherFindDriver.PublicAceptTripAsync(driverInfo);
                //!Escuchamos el mensaje del codigo de verificacion con esto nos damos cuento si se registro la reservacion o no
                var code = await _consumerCodeSecurity.consumerCodeSecurity();
                await CodeGenerate(code);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en AcceptedTrip: " + ex.Message);
            }
            //
        }

        //?metodo para que el conductor rechace el viaje que se le asigno
        public async Task RejectTrip(RequestRejectTrip driverInfo)
        {
            if (driverInfo == null)
            {
                Console.WriteLine("❌ infoTraveled llegó nulo");
                return;
            }

            if (_publisherFindDriver == null)
            {
                Console.WriteLine("❌ _publisherFindDriver es nulo (no fue inyectado)");
                return;
            }

            Console.WriteLine($"✅ info recibida: {driverInfo.idDriver}");
            await _publisherFindDriver.PublicRejectTripAsync(driverInfo);
            //?Metodo que espera para notificar que su viaje fue rechazado
            await _consumerDriverInfo.ConsumerRejectTrip();
        }

        //TODO: añadir la logica para que reciva el mensaje con el codigo generado y lo envie al usuario que le corresponde
        public async Task CodeGenerate(ResponseCode code)
        {
            var connections = _userConnectionManager.GetConnections(code.IdClient);
                if (connections == null || !connections.Any())
                {
                    Console.WriteLine("⚠️ No se encontraron conexiones activas para el conductor asignado");
                    return;
                }

                Console.WriteLine("------- conductor asignado -------");
                Console.WriteLine(code.Code);
                foreach (var connectionId in connections)
                {
                    await Clients.Client(connectionId).SendAsync("CodeGenerate",code);
                }

        }


        //? Metodo que recibe la distancia que se le due enviada
        public async Task ReceiveDistance()
        {
            await _consumerRideFare.ConsumerRideAsync();
        }


    }
}
