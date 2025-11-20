using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using RabbitMQ.Client;
using ServiceReservation.Application.DTOs;
using ServiceReservation.Infrastructure.Configurations;

namespace ServiceReservation.Infrastructure.Messaging.Producers
{
    public class RabbitMqRideFarePublisher
    {
        private readonly RabbitMqService _rabbitMQ;
        private readonly string _exchangeName = "solicitud_viaje";

        public RabbitMqRideFarePublisher(RabbitMqService rabbitMq)
        {
            _rabbitMQ = rabbitMq;
        }

        public async Task PublicAsync(RequestDistanceTraveled data)
        {
            Console.WriteLine("llegue al publisher");
            var json = JsonSerializer.Serialize<RequestDistanceTraveled>(data);
            Console.WriteLine("informacion: " + json);
            await _rabbitMQ.PublicAsync(_exchangeName,json);
        }
        
    }
}