using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceReservation.Application.DTOs;

namespace ServiceReservation.Infrastructure.Messaging.Producers
{
    public class RabbitMQFindDriver
    {
        private readonly RabbitMqService _rabbitMQ;
        private readonly string _exchangeName = "calculated_rate";

        public RabbitMQFindDriver(RabbitMqService rabbitMq)
        {
            _rabbitMQ = rabbitMq;
        }

        public async Task PublicAsync(RequestFindDriver data)
        {
            Console.WriteLine("llegue al publisher");
            var json = JsonSerializer.Serialize<RequestFindDriver>(data);
            await _rabbitMQ.PublicAsync(_exchangeName,json);
        }
    }
}