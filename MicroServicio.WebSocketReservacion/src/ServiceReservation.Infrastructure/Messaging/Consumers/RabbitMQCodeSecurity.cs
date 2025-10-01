using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceReservation.Application.DTOs;

namespace ServiceReservation.Infrastructure.Messaging.Consumers
{
    public class RabbitMQCodeSecurity
    {
        private readonly RabbitMqService _rabbitMQ;
        private readonly string _exchangeName = "codigo_generado";

        public RabbitMQCodeSecurity(RabbitMqService rabbitMq)
        {
            _rabbitMQ = rabbitMq;
        }
        public async Task<ResponseCode?> consumerCodeSecurity()
        {
            var tcs = new TaskCompletionSource<ResponseCode>();
            Console.WriteLine("esperando mensaje de viaje aceptado");
            await _rabbitMQ.ConsumeAsync(_exchangeName, async (msg) =>
            {
                var response = JsonSerializer.Deserialize<ResponseCode>(msg);
                tcs.TrySetResult(response);
            });
            return await tcs.Task;
        }
    }
}