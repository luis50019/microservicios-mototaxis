using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using ServiceReservation.Application.DTOs;
using ServiceReservation.Infrastructure.Messaging;
using ServiceReservation.Presentation.Hubs;

public class CodeVerificationListener:IHostedService
{
    private readonly RabbitMqService _rabbitService;
    private readonly UserConnectionManager _userConnectionManager;
    private readonly IHubContext<ReservationHub> _hubContext;

    public CodeVerificationListener(
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
        Console.WriteLine("====================== Resevation =============");

        _ = Task.Run(async () =>
        {
            await _rabbitService.ConsumeAsync("viaje_registrado_queue", async (channel, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine("Mensaje recibido de RabbitMQ: " + message);

                try
                {
                    var response = JsonSerializer.Deserialize<ResponseCode>(message);

                    if (response != null)
                    {
                        var connections = _userConnectionManager.GetConnections(response.IdClient);
                        var connectionDriver = _userConnectionManager.GetConnections(response.IdDriver);
                        //? enviamos la informacion al conductor
                        await _hubContext.Clients.Clients(connectionDriver).SendAsync("ReservationRegister", response.IdReservation);
                        //? enviamos la informacion al usuario
                        await _hubContext.Clients.Clients(connections).SendAsync("CodeGenerate", response);
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
    Console.WriteLine("CodeVerification iniciado.");
        Start();
        return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    throw new NotImplementedException();
  }
}
