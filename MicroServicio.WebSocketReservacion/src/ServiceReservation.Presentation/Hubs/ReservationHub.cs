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
        private readonly RabbitMqRideFareConsumer _consumerRideFare;
        public ReservationHub(RabbitMqRideFarePublisher publisherRideFare, RabbitMqRideFareConsumer consumerRideFare)
        {
            _publisherRideFare = publisherRideFare;
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

        public async Task ReceiveDistance()
        {
            await _consumerRideFare.ConsumerRideAsync();
        }

        
    }
}