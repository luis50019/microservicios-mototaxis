using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroServicio.Conductores.Config
{
    public class RabbitMQSettings
    {
        public string url { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
        public string QueueAcceptTrip { get; set; } = string.Empty;
        public string QueueRejectTrip { get; set; } = string.Empty;
        // Maximum times to re-attempt finding a driver when a rejection occurs
        public int MaxRetryAttempts { get; set; } = 3;
        // Milliseconds to wait before retrying
        public int RetryDelayMs { get; set; } = 1000;
    }
}