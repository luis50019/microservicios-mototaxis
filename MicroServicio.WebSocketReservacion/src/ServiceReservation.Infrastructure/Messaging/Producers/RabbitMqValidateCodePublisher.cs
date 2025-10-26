using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceReservation.Application.DTOs;

namespace ServiceReservation.Infrastructure.Messaging.Producers
{
    public class RabbitMqValidateCodePublisher
    {
        private readonly RabbitMqService _rabbitMQ;
        private readonly string _exchangeName = "validate_code";

        public RabbitMqValidateCodePublisher(RabbitMqService rabbit)
        {
            _rabbitMQ = rabbit;
        }

        public async Task PublicValidateCodeAsync(RequestValidateCode data)
        {
            Console.WriteLine("--- enviando mensaje para valida código");
            var json = JsonSerializer.Serialize<RequestValidateCode>(data);
            Console.WriteLine("infor: " + json.ToString());
            await _rabbitMQ.PublicQueAsync(_exchangeName,json);
        }


    }
}