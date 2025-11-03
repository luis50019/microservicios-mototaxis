using MicroServicio.ValidarCodigoVerificacion.Errors;
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
        public async Task StartConsumingAsync()
        {
            await _rabbitMQ.ConsumeAsync(async (msg) =>
            {

                try
                {
                    if (msg == null)
                    {
                        throw new ErrorValidateCode(403,msg.idDriver,"El contenido esta vacio","El mensaje no contiene informacion");
                    }
                    var response = await _service.validateCode(msg);
                    //* despues publicamos la respuesta
                    await _rabbitMQ.PublisherAsync(response);
                }catch(ErrorMongo ex)
                {
                    await _rabbitMQ.PublisherErrorValidateCodeAsync(new ErrorValidateCode(500, msg.idDriver, ex.Message, ex.details), "ErrorValidationCode");
                }
                catch (ErrorValidateCode ex)
                {
                    await _rabbitMQ.PublisherErrorValidateCodeAsync(ex, "ErrorValidationCode");
                }catch(Exception ex){
                    await _rabbitMQ.PublisherErrorValidateCodeAsync(new ErrorValidateCode(503,msg.idDriver,"Error del servidor","Error el servidor no responde"),"ErrorValidationCode");
                }
            });
        }

        public void Dispose()
        {
            _rabbitMQ.Dispose();
            GC.SuppressFinalize(this);
        }


    }
}