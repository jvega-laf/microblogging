using Microblogging.Application.Abstractions.Repositories;
using Microblogging.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Microblogging.Api.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var cnnRedis = configuration.GetConnectionString("Redis");

            if (string.IsNullOrEmpty(cnnRedis)) throw new ArgumentException("Configuración no válida", "ConnectionStrings:Redis");

            Console.WriteLine($"cnnRedis: {cnnRedis}");

            var options = ConfigurationOptions.Parse(cnnRedis);
            options.AbortOnConnectFail = false;

            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(options));

            // Aquí se registrarán los repositorios, base de datos, cache, etc.

            services.AddScoped<IFollowRepository, FollowRepository>();
            services.AddScoped<ITweetRepository, TweetRepository>();

            return services;
        }
    }
}
