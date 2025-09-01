using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroServicio.Tarifas.Services;
using Microsoft.Extensions.Hosting;

namespace MicroServicio.Tarifas.Workers
{
    public class Worker : BackgroundService
    {
        private readonly IMongoService _mongoService;

        public Worker(IMongoService mongoService)
        {
            _mongoService = mongoService;
        }
        
         protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var users = await _mongoService.GetAllFares();
            Console.WriteLine($"Usuarios en DB: {users.Count}");

            await Task.Delay(1000, stoppingToken);
        }
    }
        
    }
}