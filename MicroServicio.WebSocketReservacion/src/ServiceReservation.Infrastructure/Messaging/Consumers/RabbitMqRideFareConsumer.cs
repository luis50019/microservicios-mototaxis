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
        public async Task<ResponseConsumerRideFare> ConsumerRideAsync()
        {
            var tcs = new TaskCompletionSource<ResponseConsumerRideFare>();

            await _rabbitMQ.ConsumeAsync(_exchangeName, async (msg) =>
            {

                var message = JsonSerializer.Deserialize<ResponseConsumerRideFare>(msg);
                Console.WriteLine("-----------> tarifa" + msg);
                tcs.SetResult(message); // completamos la tarea
            });

            return await tcs.Task; // esperamos a que llegue el mensaje
        }

        public void handlerMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
