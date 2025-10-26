using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.ValidarCodigoVerificacion.Messages.Consumers;
using MicroServicio.ValidarCodigoVerificacion.Messages.Publisher;

namespace MicroServicio.ValidarCodigoVerificacion.Services
{
    public class ValidateCodeService : IDisposable
    {
        private readonly RabbitMQValidateCodeConsumer _rabbitConsumer;
        private readonly RabbitMQValidateCodePublisher _rabbitPublisher;
        public ValidateCodeService(RabbitMQValidateCodeConsumer consumer, RabbitMQValidateCodePublisher publisher, MongoService service)
        {
            _rabbitConsumer = consumer;
            _rabbitPublisher = publisher;
        }

        public async Task StartAsync()
        {
            Console.WriteLine(" ================== Escucahando mensajes ===============");
            _rabbitConsumer.StartConsumingAsync();
        }

        public void Dispose()
        {
            
            GC.SuppressFinalize(this);
        }
    }
}   