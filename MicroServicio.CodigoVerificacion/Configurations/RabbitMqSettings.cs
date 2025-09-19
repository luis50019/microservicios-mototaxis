using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Tarifas.Config
{
    //! configuraciones de rabbit, tiene dos propiedades
    public class RabbitMQSettings
{
    public string Url { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string QueueNameConsume { get; set; } = string.Empty;
    public string QueueNamePublish { get; set; } = string.Empty;
}

}