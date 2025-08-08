using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AuthService.Infrastructure.Data.Mongo;

namespace AuthService.Web.Configurations
{
    public static class ServicesConfigs
    {
        public static IServiceCollection AddServiceConfigs(
            this IServiceCollection services,
            Microsoft.Extensions.Logging.ILogger logger,
            WebApplicationBuilder builder1
        )
        {
            services.addMongoDb(builder1.Configuration);

            return services;
        }
    }
}