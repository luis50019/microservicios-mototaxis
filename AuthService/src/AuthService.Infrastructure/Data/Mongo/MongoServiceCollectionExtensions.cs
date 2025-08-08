using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace AuthService.Infrastructure.Data.Mongo
{
    public static class MongoServiceCollectionExtensions
    {

        public static IServiceCollection addMongoDb(this IServiceCollection services, IConfiguration config)
        {
            var settings = config.GetSection("MongoDb").Get<MongoDbSettings>();
            var client = new MongoClient(settings!.ConnectionString);
            var database = client.GetDatabase(settings.Database);

            services.AddSingleton<IMongoClient>(client);
            services.AddSingleton(database);

            services.AddScoped<IUserRepository, MongoUserRepository>();

            return services;
        }

    }
}