using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Tarifas.Config
{
    public class RabbitMQSettings
    {
        public string url { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
    }
}