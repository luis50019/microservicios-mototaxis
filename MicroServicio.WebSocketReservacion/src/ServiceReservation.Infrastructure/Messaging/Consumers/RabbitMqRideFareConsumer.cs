using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceReservation.Application.DTOs;

namespace ServiceReservation.Infrastructure.Messaging.Consumers
{
    public class RabbitMqRideFareConsumer
    {
        private readonly RabbitMqService _rabbitMQ;
        private readonly string _exchangeName = "fare_response_queue";

        public RabbitMqRideFareConsumer(RabbitMqService rabbitMq)
        {
            _rabbitMQ = rabbitMq;
        }

        //*Medod que coonsume el mensaje de que la tarifa fue calculada y devuelve la informacion al usuario que lo solicito
        public async Task<ResponseConsumerRideFare> ConsumerRideAsync(string requestId)
        {
            var tcs = new TaskCompletionSource<ResponseConsumerRideFare>();

            await _rabbitMQ.ConsumeAsync("fare_response_queue", async (channel, ea) =>
            {
                Console.WriteLine("===========================recibiendo informacion===============");
                var body = ea.Body.ToArray();
                var message = JsonSerializer.Deserialize<ResponseConsumerRideFare>(body);
                tcs.TrySetResult(message);
                channel.BasicAckAsync(ea.DeliveryTag, false); // Confirmar el mensaje
                
            });

            return await tcs.Task; // esperamos a que llegue el mensaje
        }

        public void handlerMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
