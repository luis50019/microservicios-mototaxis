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
    public class ErrorReservationListener : IHostedService
    {
        private readonly RabbitMqService _rabbitService;
        private readonly UserConnectionManager _userConnectionManager;
        private readonly IHubContext<ReservationHub> _hubContext;

        public ErrorReservationListener(RabbitMqService rabbitService, UserConnectionManager userConnectionManager, IHubContext<ReservationHub> hubContext)
        {
            _rabbitService = rabbitService;
            _userConnectionManager = userConnectionManager;
            _hubContext = hubContext;
        }

        public void Start()
        {
            _ = Task.Run(async () =>
            {
                await _rabbitService.ConsumeAsync("ErrorReservation", async (channel, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    try
                    {
                        var response = JsonSerializer.Deserialize<ResponseErrorReservation>(message);
                        if (response != null)
                        {
                            var connectionsClient = _userConnectionManager.GetConnections(response.IdClient);
                            var connectionsDriver = _userConnectionManager.GetConnections(response.IdDriver);

                            //!mandar las conexiones para conductor y cliente agregado
                            if (connectionsClient != null && connectionsClient.Any())
                            {
                                foreach (var connectionId in connectionsClient)
                                {
                                    await _hubContext.Clients.Client(connectionId)
                                        .SendAsync("ErrorReservation", response.MessageErrorClient);
                                }
                            }

                            if (connectionsDriver != null && connectionsDriver.Any())
                            {
                                foreach (var connectionId in connectionsDriver)
                                {
                                    await _hubContext.Clients.Client(connectionId)
                                        .SendAsync("ErrorReservation", response.MessageErrorDriver);
                                }
                            }
                            await channel.BasicAckAsync(ea.DeliveryTag, false);
                        }

                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("error mensaje error reservacion");
                        await channel.BasicNackAsync(ea.DeliveryTag, false, false);
                    }
                });
            });
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            System.Console.WriteLine("ErrorReservationListener iniciado");
            Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}