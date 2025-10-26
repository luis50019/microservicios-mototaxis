using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using ServiceReservation.Application.DTOs;
using ServiceReservation.Infrastructure.Messaging;
using ServiceReservation.Presentation.Hubs;

public class DriverListener:IHostedService
{
    private readonly RabbitMqService _rabbitService;
    private readonly UserConnectionManager _userConnectionManager;
    private readonly IHubContext<ReservationHub> _hubContext;

    public DriverListener(
        RabbitMqService rabbitService,
        UserConnectionManager userConnectionManager,
        IHubContext<ReservationHub> hubContext)
    {
        _rabbitService = rabbitService;
        _userConnectionManager = userConnectionManager;
        _hubContext = hubContext;
    }

    public void Start()
    {
        Console.WriteLine("====================== Listener drivers =============");

        _ = Task.Run(async () =>
        {
            await _rabbitService.ConsumeAsync("driverFound", async (channel, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine("Mensaje recibido de RabbitMQ: " + message);

                try
                {
                    var response = JsonSerializer.Deserialize<ResponseDriverFound>(message);
                    Console.WriteLine(response.Data.infoPassager.urlPhoto);
                    if (response != null)
                    {
                        var connections = _userConnectionManager.GetConnections(response.Data.id);
                        foreach (var connectionId in connections)
                        {
                            await _hubContext.Clients.Client(connectionId)
                                .SendAsync("ReceiveDriver", response);
                        }
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
    Console.WriteLine("DriverListener iniciado.");
        Start();
        return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    Console.WriteLine("DriverListener detenido.");
        return Task.CompletedTask;
  }
}
