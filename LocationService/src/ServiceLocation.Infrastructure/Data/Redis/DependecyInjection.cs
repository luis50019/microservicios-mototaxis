using StackExchange.Redis;
using ServiceLocation.Application.Interfaces;
using ServiceLocation.Application.Services;
using ServiceLocation.Domain.Interfaces;
using ServiceLocation.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace ServiceLocation.Infrastructure.Data.Redis
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                {
                    var options = new ConfigurationOptions
                    {
                        EndPoints = { { "redis-15279.c262.us-east-1-3.ec2.cloud.redislabs.com", 15279 } },
                        User = "default",
                        Password = "EkV483jGkb10kQVpaDFrunTggMxgyuBQ",

                        Ssl = false,
                        AbortOnConnectFail = false,
                        ConnectRetry = 5,
                        ConnectTimeout = 10000,
                        SyncTimeout = 10000
                    };

                    return ConnectionMultiplexer.Connect(options);
                });

            services.AddScoped<IUserRespository, RedisUserRepository>();
            return services;
        }
    }
}
