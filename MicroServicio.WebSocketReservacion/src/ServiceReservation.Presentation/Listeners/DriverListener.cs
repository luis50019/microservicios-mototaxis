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

        _ = Task.Run(async () =>
        {
            await _rabbitService.ConsumeAsync("driverFound", async (channel, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());

                try
                {
                    var response = JsonSerializer.Deserialize<ResponseDriverFound>(message);
                    Console.WriteLine("]Mensaje recibido:   "+message);
                    if (response != null)
                    {
                        if (string.IsNullOrEmpty(response.Data.id))
                        {
                            Console.WriteLine($"Conductor encontrado: {response.Data.id}");
                        }
                  

                        var connections = _userConnectionManager.GetConnections(string.IsNullOrEmpty(response.Data.id) ? response.Data.client : response.Data.id);
                        foreach (var connectionId in connections)
                        {
                            await _hubContext.Clients.Client(connectionId)
                                .SendAsync("ReceiveDriver", string.IsNullOrEmpty(response.Data.id) ?new 
                                {
                                    Message = "No hay conductores disponibles",
                                    Suggest =" Intenta nuevamente más tarde"
                                }:(object) response);
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
