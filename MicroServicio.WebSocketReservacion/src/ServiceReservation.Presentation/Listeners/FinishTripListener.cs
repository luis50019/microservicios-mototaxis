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
    public class FinishTripListener:IHostedService
    {
        private readonly RabbitMqService _rabbitMQService;
        private readonly UserConnectionManager _userConnectionManager;
        private IHubContext<ReservationHub> _hubContext;

        public FinishTripListener(
            RabbitMqService service,
            UserConnectionManager connectionManager, IHubContext<ReservationHub> hubContext)
        {
            _rabbitMQService = service;
            _userConnectionManager = connectionManager;
            _hubContext = hubContext;
        }
        public void Start()
        {
            _ = Task.Run(async () =>
            {
                await _rabbitMQService.ConsumeAsync("FinishReservation", async (channel, ea) =>
                {

                    try
                    {
                        var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                        var response = JsonSerializer.Deserialize<ResponseCompletedTripDTO>(message);
                        Console.WriteLine(response.IdClient);
                        if (response != null)
                        {
                            List<string> connectionCliente = new();
                            if (response != null)
                            {
                                connectionCliente = _userConnectionManager.GetConnections(response.IdClient).ToList();
                            }
                            //? si el codigo es correcto el mensaje debe ser enviado a ambos
                            var connections = _userConnectionManager.GetConnections(response.IdDriver);

                            var allConnections = connections.Concat(connectionCliente).ToList();
                            await _hubContext.Clients.Clients(allConnections).SendAsync("TripFinish", response);

                        }
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
      Console.WriteLine("viaje finalizado iniciado.");
        Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }
}