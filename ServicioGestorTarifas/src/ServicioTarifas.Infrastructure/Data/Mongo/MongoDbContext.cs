using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using ServicioTarifas.Domain.Interfaces;
using ServicioTarifas.Infrastructure.Repositories;

namespace ServicioTarifas.Infrastructure.Data.Mongo
{
    public static class MongoDbContext
    {

        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration config)
        {
            var settings = config.GetSection("MongoDb").Get<MongoDbSettings>();
            var client = new MongoClient(settings!.ConnectionString);
            var database = client.GetDatabase(settings.Database);

            services.AddSingleton<IMongoClient>(client);
            services.AddSingleton(database);

            services.AddScoped<IFareRepository, MongoFareLocation>();

            return services;
        }

    }
}