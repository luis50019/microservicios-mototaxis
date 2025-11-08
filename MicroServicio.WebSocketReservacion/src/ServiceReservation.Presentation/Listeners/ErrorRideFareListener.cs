using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Presentation.Listeners
{
    public class ErrorRideFareListener : IHostedService
    {
        private readonly RabbitMqService _rabbitService;
        private readonly UserConnectionManager _userConnectionManager;
        private readonly IHubContext<ReservationHub> _hubContext;

        public ErrorRideFareListener(RabbitMqService rabbitService,
        UserConnectionManager userConnectionManager,
        IHubContext<ReservationHub> hubContext)
        {
            _rabbitService = rabbitService;
            _userConnectionManager = userConnectionManager;
            _hubContext = hubContext;
        }

        public void Start()
        {
            Console.WriteLine("====================== Listener error ride fare iniciado =============");

            _ = Task.Run(async () =>
            {
                await _rabbitService.ConsumeAsync("ErrorRideFare", async (channel, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Console.WriteLine("Mensaje errorRideFare recibido de RabbitMQ: " + message);

                    try
                    {
                        var response = JsonSerializer.Deserialize<ResponseErrorRideFare>(message);

                        if (response != null)
                        {
                            var connectionsClient = _userConnectionManager.GetConnections(response.IdClient);
                            if (connectionsClient != null && connectionsClient.any())
                            {
                                foreach (var connectionId in connectionsClient)
                                {
                                    await _hubContext.Clients.Client(connectionId)
                                        .SendAsync("ErrorRideFare", response.MessageError);
                                }
                            }
                        }

                        await channel.BasicAckAsync(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error procesando mensaje error ride fare: " + ex.Message);
                        await channel.BasicNackAsync(ea.DeliveryTag, false, false);
                    }
                });
            });
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            System.Console.WriteLine("ErrorRideFareListener iniciado");
            Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}