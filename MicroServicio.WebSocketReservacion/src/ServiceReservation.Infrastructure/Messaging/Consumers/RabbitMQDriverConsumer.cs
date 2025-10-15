using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceReservation.Application.DTOs;

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

        //*Medod que coonsume el mensaje de que el conductor fue encontrado
        public async Task<ResponseDriverFound?> ConsumerRideAsync(TimeSpan? timeout = null)
        {
            Console.WriteLine("Esperando al conductor asignado");
            var tcs = new TaskCompletionSource<ResponseDriverFound>();

            await _rabbitMQ.ConsumeAsync(_exchangeName, async (msg,ea) =>
            {
                Console.WriteLine("Se asignó un conductor");
                Console.WriteLine("Datos del conductor: " + msg);

                try
                {
                    var body = ea.Body.ToArray();
                    var message = JsonSerializer.Deserialize<ResponseDriverFound>(body, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (message != null)
                    {
                        tcs.TrySetResult(message); // completamos la tarea solo si no es null
                    }
                    else
                    {
                        Console.WriteLine("❌ Mensaje deserializado como null");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ Error deserializando mensaje: " + ex.Message);
                }
            });

            // Esperamos a que llegue un mensaje o al timeout
            if (timeout.HasValue)
            {
                var task = await Task.WhenAny(tcs.Task, Task.Delay(timeout.Value));
                if (task == tcs.Task)
                    return tcs.Task.Result;
                else
                {
                    Console.WriteLine("⚠️ Timeout esperando conductor");
                    return null;
                }
            }

            return await tcs.Task;
        }


        public async Task ConsumerAcceptTrip()
        {
            Console.WriteLine("esperando mensaje de viaje aceptado");
            await _rabbitMQ.ConsumeAsync(_exchangeDriverAccepted, async (msg,ea) =>
            {
                Console.WriteLine("==========================================");
                Console.WriteLine("viaje aceptado: ");
                Console.WriteLine("datos del viaje: " + msg);
                Console.WriteLine("==========================================");
            });
        }

        public async Task ConsumerRejectTrip()
        {
            Console.WriteLine("esperando mensake de viaje rechazado");
            await _rabbitMQ.ConsumeAsync(_exchageTripReject, async (msg,ea) =>
            {
                Console.WriteLine("El conductor ha rechazado el viaje");
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
