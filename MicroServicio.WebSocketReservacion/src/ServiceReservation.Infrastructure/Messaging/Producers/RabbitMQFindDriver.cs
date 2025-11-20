using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ServiceReservation.Application.DTOs;

namespace ServiceReservation.Infrastructure.Messaging.Producers
{
    public class RabbitMQFindDriver
    {
        private readonly RabbitMqService _rabbitMQ;
        private readonly string _exchangeName = "calculated_rate";
        private readonly string _queueFinishTravled ="FinishTrip";
        private readonly string _queueAccepTraveled = "accept_trip";
        private readonly string _queueRejectTraveled = "rejectTrip";

        public RabbitMQFindDriver(RabbitMqService rabbitMq)
        {
            _rabbitMQ = rabbitMq;
        }

        public async Task PublicAsync(RequestFindDriver data)
        {
            Console.WriteLine("llegue al publisher");
            var json = JsonSerializer.Serialize<RequestFindDriver>(data);
            await _rabbitMQ.PublicAsync(_exchangeName, json);
        }

        public async Task PublicAceptTripAsync(RequestAcceptTrip data)
        {
            //TODO: modificar la logica de aceptar viaje
            //?Ahora el mensaje solo lo recibiara el servicio de reservations
            Console.WriteLine("==================== Viaje aceptado ============================");
            var json = JsonSerializer.Serialize<RequestAcceptTrip>(data);
            Console.WriteLine(json);
            await _rabbitMQ.PulblicExchangeAsync(_queueAccepTraveled, json);
            Console.WriteLine("================================================");
        }

        public async Task PublicFinshTrip(RequestFinishTrip data){
            Console.WriteLine("Viaje finalizado...");
            var json = JsonSerializer.Serialize<RequestFinishTrip>(data);
            await _rabbitMQ.PublicAsync(_queueFinishTravled, json);
        }
        
        public async Task PublicRejectTripAsync(RequestRejectTrip data)
        {
            Console.WriteLine("Cancelando el viaje...");
            var json = JsonSerializer.Serialize<RequestRejectTrip>(data);
            await _rabbitMQ.PublicAsync(_queueRejectTraveled, json);          
        }

    }
}