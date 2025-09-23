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
        private readonly string _exchangeDriverAccepted = "DriverAccepted";
        private readonly string _exchageTripReject = "tripReject";

        public RabbitMQDriverConsumer(RabbitMqService rabbitMq)
        {
            _rabbitMQ = rabbitMq;
        }

        public async Task ConsumerRideAsync()
        {
            Console.WriteLine("Esperando al conductor asiganado");

            await _rabbitMQ.ConsumeAsync(_exchangeName,async(msg)=>
            {
                Console.WriteLine("Se asigno un conductor");
                Console.WriteLine("Datos del conductor: " + msg);
            });
        }

        public async Task ConsumerAcceptTrip()
        {
            Console.WriteLine("esperando mensaje de viaje aceptado");
            await _rabbitMQ.ConsumeAsync(_exchangeDriverAccepted, async (msg) =>
            {
                Console.WriteLine("viaje aceptado: ");
                Console.WriteLine("datos del viaje: " + msg);
            });
        }

        public async Task ConsumerRejectTrip()
        {
            Console.WriteLine("esperando mensake de viaje rechazado");
            await _rabbitMQ.ConsumeAsync(_exchageTripReject, async (msg) =>
            {
                Console.WriteLine("viaje rechazado: ");
                Console.WriteLine("datos del viaje: " + msg);
            });
        }

        public void handlerMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
