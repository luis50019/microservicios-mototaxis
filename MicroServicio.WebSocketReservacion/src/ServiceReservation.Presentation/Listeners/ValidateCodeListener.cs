using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ServiceReservation.Application.DTOs;
using ServiceReservation.Infrastructure.Messaging;
using ServiceReservation.Presentation.Hubs;

namespace ServiceReservation.Presentation.Listeners
{
    public class ValidateCodeListener : IHostedService
    {
        private readonly RabbitMqService _rabbitMQService;
        private readonly UserConnectionManager _userConnectionManager;
        private IHubContext<ReservationHub> _hubContext;

        public ValidateCodeListener(RabbitMqService service, UserConnectionManager connectionManager, IHubContext<ReservationHub> hubContext)
        {
            _rabbitMQService = service;
            _userConnectionManager = connectionManager;
            _hubContext = hubContext;
        }

        public void Start()
        {
            Console.WriteLine("============================ Validate Code Listener ===================");
            _ = Task.Run(async () =>
            {
                await _rabbitMQService.ConsumeAsync("code_validate", async (channel, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Console.WriteLine("Mensaje recibido de RabbitMQ: " + message);

                    try
                    {
                        var response = JsonSerializer.Deserialize<ResponseValidateCode>(message);
                        Console.WriteLine(response.idClient);
                        if (response != null)
                        {
                            List<string> connectionDriver = new();
                            if (response.isCorrect)
                            {
                                connectionDriver = _userConnectionManager.GetConnections(response.idDriver).ToList();
                            }
                            //? si el codigo es correcto el mensaje debe ser enviado a ambos
                            var connections = _userConnectionManager.GetConnections(response.idClient);

                            var allConnections = connections.Concat(connectionDriver).ToList();
                            await _hubContext.Clients.Clients(allConnections).SendAsync("CodeValidate", response);

                        }

                        await channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error procesando mensaje: " + ex.Message);
                        await channel.BasicNackAsync(ea.DeliveryTag, false, false);
                    }
                });
            });
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Validate Code Listener iniciado.");
            Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}