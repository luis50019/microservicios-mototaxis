using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Infrastructure.Messaging.Consumers
{
    public class RabbitMQDriverConsumer
    {
        private readonly RabbitMqService _rabbitMQ;
        private readonly string _exchangeName = "driverFound";

        public RabbitMQDriverConsumer(RabbitMqService rabbitMq)
        {
            _rabbitMQ = rabbitMq;
        }

        public async Task ConsumerRideAsync()
        {
            Console.WriteLine("llegue al publisher");

            await _rabbitMQ.ConsumeAsync(_exchangeName,async(msg)=>
            {
                Console.WriteLine("mensaje recibido:" + msg);
            });
        }

        public void handlerMessage(string message) {
            Console.WriteLine(message);
        }
    }
}