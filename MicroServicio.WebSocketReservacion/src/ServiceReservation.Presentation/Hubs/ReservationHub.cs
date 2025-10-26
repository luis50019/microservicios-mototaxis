using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using RabbitMQ.Client.Events;
using ServiceReservation.Application.DTOs;
using ServiceReservation.Infrastructure.Messaging;
using ServiceReservation.Infrastructure.Messaging.Consumers;
using ServiceReservation.Infrastructure.Messaging.Producers;

namespace ServiceReservation.Presentation.Hubs
{

    //TODO: testear todo la conexion que realizamos, calcular la tarifa, aceptar al tarifa, aceptar el viaje, rechazar el viaje
    public class ReservationHub : Hub
    {
        //!Añadiendo la clase para que se pueda guardar los usuarios conectados
        private readonly UserConnectionManager _userConnectionManager;


        //!debemos de crear los servicios que vamos a utilizar
        private readonly RabbitMqRideFarePublisher _publisherRideFare;
        private readonly RabbitMQFindDriver _publisherFindDriver;
        private readonly RabbitMqRideFareConsumer _consumerRideFare;
        private readonly RabbitMQDriverConsumer _consumerDriverInfo;
        private readonly RabbitMQCodeSecurity _consumerCodeSecurity;
        private readonly RabbitMqValidateCodePublisher _validateCode;
        public ReservationHub(UserConnectionManager UserConnectionManager, RabbitMqRideFarePublisher publisherRideFare, RabbitMQDriverConsumer consumerDriverInfo, RabbitMqRideFareConsumer consumerRideFare, RabbitMQFindDriver finDriver, RabbitMQCodeSecurity consumerCodeSecurity, RabbitMqService service,RabbitMqValidateCodePublisher validateCode)
        {
            _publisherRideFare = publisherRideFare;
            _consumerCodeSecurity = consumerCodeSecurity ?? throw new ArgumentNullException(nameof(consumerCodeSecurity));
            _publisherFindDriver = finDriver;
            _consumerRideFare = consumerRideFare ?? throw new ArgumentNullException(nameof(consumerRideFare));
            _userConnectionManager = UserConnectionManager;
            _consumerDriverInfo = consumerDriverInfo ?? throw new ArgumentNullException(nameof(consumerDriverInfo));
            _validateCode = validateCode ?? throw new ArgumentNullException(nameof(validateCode));

        }
        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveConnectionId", Context.ConnectionId);
            await base.OnConnectedAsync();
        }
        public Task registerUSer(string idUser)
        {
            _userConnectionManager.AddConnection(idUser, Context.ConnectionId);
            Console.WriteLine("\n=============== usuarios registrado ====================");
            Console.WriteLine(idUser);
            Console.WriteLine("===================================\n");
            return Task.CompletedTask;
        }

        //? Metodo que recibe la distancia a recoorrer y calcula la tarifa
        public async Task SendDistanceTraveled(RequestDistanceTraveled data)
        {
            try
            {
                Console.WriteLine("\n==============Publicando distancia...===============");
                await _publisherRideFare.PublicAsync(data);
                Console.WriteLine("==================================\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en SendDistanceTraveled: " + ex.Message);
                throw;
            }
        }


        //* Metodo que se encarga de mandar el mensaje para que comiencen a buscar a un conductor
        public async Task FindDriverAsync(RequestFindDriver infoTraveled)
        {
            try
            {
                Console.WriteLine("Iniciando la busqueda de un conductor");
                if (infoTraveled == null)
                {
                    Console.WriteLine("infoTraveled llegó nulo");
                    return;
                }

                if (_publisherFindDriver == null)
                {
                    Console.WriteLine("_publisherFindDriver es nulo (no fue inyectado)");
                    return;
                }

                Console.WriteLine("\n===========================bucsando conductor-------------------");
                Console.WriteLine($"info recibida websocket: {JsonSerializer.Serialize<RequestFindDriver>(infoTraveled)}");
                await _publisherFindDriver.PublicAsync(infoTraveled);
                Console.WriteLine("=========================================================\n");
            }
            catch (Exception ex)
            {

                Console.WriteLine("Error en FindDriverAsync: " + ex.Message);
                throw;
            }
        }

        //?Metodo para que el conductor pueda aceptar el viaje que se le fue asignado
        public async Task AcceptedTrip(RequestAcceptTrip driverInfo)
        {
            try
            {
                Console.WriteLine(" ======================= Conductor aceptando el viaje =============");
                if (driverInfo == null)
                {
                    Console.WriteLine("infoTraveled llegó nulo");
                    return;
                }

                if (_publisherFindDriver == null)
                {
                    Console.WriteLine("publisherFindDriver es nulo (no fue inyectado)");
                    return;
                }

                if (driverInfo.infoDriver == null || driverInfo.infoDriver.data == null)
                {
                    Console.WriteLine("driverInfo.infoDriver.data llegó nulo");
                    return;
                }

                Console.WriteLine($"info recibida: {driverInfo.infoDriver}");
                await _publisherFindDriver.PublicAceptTripAsync(driverInfo);

                Console.WriteLine(" ================================================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en AcceptedTrip: " + ex.Message);
            }
            //
        }

        //?metodo para que el conductor rechace el viaje que se le asigno
        public async Task RejectTrip(RequestRejectTrip driverInfo)
        {
            if (driverInfo == null)
            {
                Console.WriteLine("infoTraveled llegó nulo");
                return;
            }

            if (_publisherFindDriver == null)
            {
                Console.WriteLine("_publisherFindDriver es nulo (no fue inyectado)");
                return;
            }

            Console.WriteLine($"info recibida: {driverInfo.idDriver}");
            await _publisherFindDriver.PublicRejectTripAsync(driverInfo);
            //?Metodo que espera para notificar que su viaje fue rechazado
            //TODO: separa esta logica para evitar el cuello de botello
            await _consumerDriverInfo.ConsumerRejectTrip();
        }

        //? Metodo para validar el codigo de verificacion
        public async Task ValidateCode(RequestValidateCode infoCode)
        {
            try
            {
                if (infoCode == null)
                {
                    throw new Exception("no hay informacion sobre el codigo");
                }

                await _validateCode.PublicValidateCodeAsync(infoCode);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //? Metodo que recibe la distancia que se le due enviada
        public async Task ReceiveDistance()
        {
            await _consumerRideFare.ConsumerRideAsync("d");
        }


    }
}
