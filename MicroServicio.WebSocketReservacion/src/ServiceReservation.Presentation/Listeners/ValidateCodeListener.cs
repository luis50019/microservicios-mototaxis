using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using ServiceReservation.Application.DTOs;
using ServiceReservation.Infrastructure.Messaging;
using ServiceReservation.Presentation.Hubs;

public class ValidateCodeListener : IHostedService
{
    private readonly RabbitMqService _rabbitMQService;
    private readonly UserConnectionManager _userConnectionManager;
    private IHubContext<ReservationHub> _hubContext;

    public ValidateCodeListener(
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
            await _rabbitMQService.ConsumeAsync("codeValidate", async (channel, ea) =>
            {

                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var response = JsonSerializer.Deserialize<ResponseValidateCode>(message);
                    Console.WriteLine(response.idClient);
                    if (response != null)
                    {
                        List<string> connectionCliente = new();
                        if (response.isCorrect)
                        {
                            connectionCliente = _userConnectionManager.GetConnections(response.idClient).ToList();
                        }
                        //? si el codigo es correcto el mensaje debe ser enviado a ambos
                        var connections = _userConnectionManager.GetConnections(response.idDriver);

                        var allConnections = connections.Concat(connectionCliente).ToList();
                        await _hubContext.Clients.Clients(allConnections).SendAsync("Code", response);

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
        Console.WriteLine("Validate Code Listener iniciado.");
        Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
