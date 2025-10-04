using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MicroServicio.ValidarCodigoVerificacion.DTOs;
using MicroServicio.ValidarCodigoVerificacion.Services;

namespace MicroServicio.ValidarCodigoVerificacion.Messages.Publisher
{
    public class RabbitMQValidateCodePublisher : IDisposable
    {
        private readonly RabbitMQService _rabbitMQ;

        public RabbitMQValidateCodePublisher(RabbitMQService rabbitMQ)
        {
            _rabbitMQ = rabbitMQ;
        }

        public void Dispose()
        {
            _rabbitMQ.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task PublicValidateCode(ResponseValidateCode response)
        {
            await _rabbitMQ.PublisherAsync(response);
        }

    }
}