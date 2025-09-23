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
        public ReservationHub(UserConnectionManager UserConnectionManager, RabbitMqRideFarePublisher publisherRideFare, RabbitMqRideFareConsumer consumerRideFare, RabbitMQFindDriver finDriver)
        {
            _publisherRideFare = publisherRideFare;
            _publisherFindDriver = finDriver;
            _consumerRideFare = consumerRideFare;
            _userConnectionManager = UserConnectionManager;
        }

        public Task registerUSer(string idUser)
        {
            _userConnectionManager.AddConnection(idUser, Context.ConnectionId);
            return Task.CompletedTask;
        }

        //? Metodo que recibe la distancia a recoorrer y calcula la tarifa
        public async Task SendDistanceTraveled(RequestDistanceTraveled data)
        {
            //?preparamos el mensaje a enviar
            Console.WriteLine("Recibiendo la distancia recorrida");
            //! publicamos en RabbitMq
            Console.WriteLine($"info recibida: {data.IdUser} = {data.distanceTraveled}");
            await _publisherRideFare.PublicAsync(data);
            //!notificar al usuario que su tarifa esta siendo calculada
            var response = await _consumerRideFare.ConsumerRideAsync();
            Console.WriteLine("---------- tarifa recibida -------------");
            Console.WriteLine(response.IdUser);
            Console.WriteLine("-----------------------");
            //? reenviar el mensaje de la tarifa al usurio que le corresponde
            var connections = _userConnectionManager.GetConnections(data.IdUser);
            foreach (var connectionId in connections)
            {
                await Clients.Client(connectionId).SendAsync("ReceiveDistance", response);
            }
        }

        //* Metodo que se encarga de mandar el mensaje para que comiencen a buscar a un conductor
        public async Task FindDriverAsync(RequestFindDriver infoTraveled)
        {
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

            Console.WriteLine($"✅ info recibida: {infoTraveled.priceTraveled} = {infoTraveled.idUser}");
            await _publisherFindDriver.PublicAsync(infoTraveled);
            //?Metodo que espera la informacion del conductor asignado
            await _consumerDriverInfo.ConsumerRideAsync();
        }

        //?Metodo para que el conductor pueda aceptar el viaje que se le fue asignado
        public async Task AcceptedTrip(RequestAcceptTrip infoDriver)
        {
            if (infoDriver == null)
            {
                Console.WriteLine("❌ infoTraveled llegó nulo");
                return;
            }

            if (_publisherFindDriver == null)
            {
                Console.WriteLine("❌ _publisherFindDriver es nulo (no fue inyectado)");
                return;
            }

            Console.WriteLine($"✅ info recibida: {infoDriver.idDriver}");
            await _publisherFindDriver.PublicAceptTripAsync(infoDriver);
            //?merodo que espera para notificar al usuario que su viaje fue asignado
            await _consumerDriverInfo.ConsumerAcceptTrip();
        }

        //?metodo para que el conductor rechace el viaje que se le asigno
        public async Task RejectTrip(RequestRejectTrip infoDriver)
        {
            if (infoDriver == null)
            {
                Console.WriteLine("❌ infoTraveled llegó nulo");
                return;
            }

            if (_publisherFindDriver == null)
            {
                Console.WriteLine("❌ _publisherFindDriver es nulo (no fue inyectado)");
                return;
            }

            Console.WriteLine($"✅ info recibida: {infoDriver.idDriver}");
            await _publisherFindDriver.PublicRejectTripAsync(infoDriver);
            //?Metodo que espera para notificar que su viaje fue rechazado
            await _consumerDriverInfo.ConsumerRejectTrip();
        }

        //public async Task CodeGenerate(){

        //}


        //? Metodo que recibe la distancia que se le due enviada
        public async Task ReceiveDistance()
        {
            await _consumerRideFare.ConsumerRideAsync();
        }


    }
}
