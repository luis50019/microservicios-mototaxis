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
    public class ReservationHub : Hub
    {
        //!debemos de crear los servicios que vamos a utilizar
        private readonly RabbitMqRideFarePublisher _publisherRideFare;
        private readonly RabbitMQFindDriver _publisherFindDriver;
        private readonly RabbitMqRideFareConsumer _consumerRideFare;
        public ReservationHub(RabbitMqRideFarePublisher publisherRideFare, RabbitMqRideFareConsumer consumerRideFare, RabbitMQFindDriver finDriver)
        {
            _publisherRideFare = publisherRideFare;
            _publisherFindDriver = finDriver;
            _consumerRideFare = consumerRideFare;
        }

        public async Task SendDistanceTraveled(RequestDistanceTraveled data)
        {
            //?preparamos el mensaje a enviar
            Console.WriteLine("Recibiendo la distancia recorrida");
            //? publicamos en RabbitMq
            Console.WriteLine($"info recibida: {data.IdUser} = {data.distanceTraveled}");
            await _publisherRideFare.PublicAsync(data);
            //?notificar al usuario que su tarifa esta siendo calculada
            await _consumerRideFare.ConsumerRideAsync();
        }

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
        }

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
        }
        public async Task ReceiveDistance()
        {
            await _consumerRideFare.ConsumerRideAsync();
        }

        
    }
}