using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceReservation.Presentation.Listeners
{
    public class ErrorValidationCodeListener : IHostedService
    {
        private readonly RabbitMqService _rabbitService;
        private readonly UserConnectionManager _userConnectionManager;
        private readonly IHubContext<ReservationHub> _hubContext;

        public ErrorValidationCodeListener(RabbitMqService rabbitService, UserConnectionManager userConnectionManager, IHubContext<ReservationHub> hubContext)
        {
            _rabbitService = rabbitService;
            _userConnectionManager = userConnectionManager;
            _hubContext = hubContext;
        }

        public void Start()
        {
            Console.WriteLine("=======Error Validate Code Listener Iniciado==================");
            _ = Task.Run(async () =>
            {
                await _rabbitService.ConsumeAsync("ErrorValidationCode", async (channel, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    Console.WriteLine("Mensaje recibido en ErrorValidationCode");
                    try
                    {
                        var response = JsonSerializer.Deserialize<ResponseErrorValidateCode>(message);
                        if (response != null)
                        {
                            var connectionsDriver = _userConnectionManager.GetConnections(response.IdDriver);

                            //!mandar las conexiones para conductor y cliente agregado
                            if (connectionsDriver != null && connectionsDriver.any())
                            {
                                foreach (var connectionId in connectionsDriver)
                                {
                                    await _hubContext.Clients.Client(connectionId)
                                        .SendAsync("ErrorValidationCode", response.MessageError);
                                }
                            }

                            await channel.BasicAckAsync(ea.DeliveryTag, false);
                        }

                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("error mensaje error codigo verificacion");
                        await channel.BasicNackAsync(ea.DeliveryTag, false, false);
                    }
                });
            });
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            System.Console.WriteLine("ErrorValidationCode iniciado");
            Start();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

    }
}