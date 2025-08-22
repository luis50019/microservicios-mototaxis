using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using ServiceLocation.Domain.Interfaces;
using ServiceLocation.Infrastructure.Repositories;

namespace ServiceLocation.Infrastructure.Data.Mongo;

public static class MongoDbContext
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration config)
    {
        var settings = config.GetSection("MongoDb").Get<MongoDbSettings>();
        var client = new MongoClient(settings!.ConnectionString);
        var database = client.GetDatabase(settings.Database);

        services.AddSingleton<IMongoClient>(client);
        services.AddSingleton(database);

        services.AddScoped<ILocationRepository, MongoLocationRepository>();

        return services;
    }
}
