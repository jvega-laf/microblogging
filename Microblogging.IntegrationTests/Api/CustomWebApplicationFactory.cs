using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StackExchange.Redis;
using Microblogging.Domain.Entities;
using System.Text.Json;
using System.Collections.Generic;
using Microblogging.Application.Abstractions.Repositories;

namespace Microblogging.IntegrationTests.Api;


public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IFollowRepository> FollowRepoMock { get; } = new();
    public Mock<ITweetRepository> TweetRepoMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Eliminar registros reales
            var followDesc = services.Single(d => d.ServiceType == typeof(IFollowRepository));
            services.Remove(followDesc);
            var tweetDesc = services.Single(d => d.ServiceType == typeof(ITweetRepository));
            services.Remove(tweetDesc);

            // Reemplazarlos con mocks
            services.AddSingleton(FollowRepoMock.Object);
            services.AddSingleton(TweetRepoMock.Object);
        });
    }
}