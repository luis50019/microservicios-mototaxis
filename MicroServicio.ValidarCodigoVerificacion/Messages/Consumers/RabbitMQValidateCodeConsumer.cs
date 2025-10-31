using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MicroServicio.ValidarCodigoVerificacion.DTOs;
using MicroServicio.ValidarCodigoVerificacion.Services;

namespace MicroServicio.ValidarCodigoVerificacion.Messages.Consumers
{
    public class RabbitMQValidateCodeConsumer : IDisposable
    {
        private readonly RabbitMQService _rabbitMQ;
        private readonly MongoService _service;

        public RabbitMQValidateCodeConsumer(RabbitMQService rabbitMQ, MongoService service)
        {
            _rabbitMQ = rabbitMQ;
            _service = service;
        }

        public void Dispose()
        {
            _rabbitMQ.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task StartConsumingAsync()
        {
            await _rabbitMQ.ConsumeAsync(async (msg) =>
            {
                var request = JsonSerializer.Deserialize<RequestValidateCode>(msg);
                if (request != null)
                {

                    Console.WriteLine($"Mensaje recibido: "+msg);
                    var response = await _service.validateCode(request);
                    //* despues publicamos la respuesta
                    await _rabbitMQ.PublisherAsync(response);
                }
            });
        }

    }
}