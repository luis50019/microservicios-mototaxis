using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservicio.Reservaciones.Config
{
    public class RabbitMQSettings
    {
        public string url { get; set; } = string.Empty;
        public String QueueName { get; set; } = string.Empty;
    }
}
