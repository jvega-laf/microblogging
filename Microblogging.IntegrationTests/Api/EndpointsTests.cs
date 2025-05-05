using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microblogging.Application.Common;
using Microblogging.Application.Contracts.Requests;
using Microblogging.Application.Follows.Commands;
using Microblogging.Application.Tweets.Commands;
using Microblogging.Application.Tweets.Queries;
using Microblogging.Domain.Entities;
using Microblogging.Domain.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using Microblogging.Api.Endpoints;
using Microsoft.AspNetCore.TestHost;
using Microblogging.Api.Extensions;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using Microblogging.Infrastructure.Repositories;
using StackExchange.Redis;
using Microblogging.Application.Tweets.Handlers;
using Microblogging.Application.Abstractions.Repositories;
using Microblogging.Application.Follows.Queries;

namespace Microblogging.IntegrationTests.Api;

public class EndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public EndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostTweet_Should_Return_OK_And_Invoke_Mediator()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var content = "Hola mundo";
        var tweet = new Tweet(new UserId(userId), content);

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<PostTweetCommand>(), default))
            .ReturnsAsync(Result<Tweet>.SuccessResult(tweet));

        var client = CreateTestClient(services =>
        {
            services.AddSingleton(mediatorMock.Object);
        });

        client.DefaultRequestHeaders.Add("X-User-Id", userId.ToString());

        var request = new PostTweetRequest { Content = content };

        // Act
        var response = await client.PostAsJsonAsync("/tweets", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetProperty("message").GetString().Should().Be("Tweet creado.");

        mediatorMock.Verify(m =>
            m.Send(It.Is<PostTweetCommand>(c =>
                c.AuthorId.Value == userId && c.Content == content),
            default),
            Times.Once);
    }

    [Fact]
    public async Task GetTimeline_Should_Return_Tweets_From_Mediator()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tweets = new List<Tweet>
    {
        new Tweet(new UserId(userId), "Primero"),
        new Tweet(new UserId(userId), "Segundo")
    };

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetTimelineQuery>(), default))
            .ReturnsAsync(tweets);

        var client = CreateTestClient(services =>
        {
            services.AddSingleton(mediatorMock.Object);
        });

        client.DefaultRequestHeaders.Add("X-User-Id", userId.ToString());

        // Act
        var response = await client.GetAsync("/timeline");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetArrayLength().Should().Be(2);

        mediatorMock.Verify(m =>
            m.Send(It.Is<GetTimelineQuery>(q => q.UserId.Value == userId), default),
            Times.Once);
    }

    [Fact]
    public async Task FollowUser_Should_Return_OK_And_Invoke_Mediator()
    {
        // Arrange
        var followerId = Guid.NewGuid();
        var followedId = Guid.NewGuid();

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<FollowUserCommand>(), default))
            .ReturnsAsync(Result.SuccessResult());

        var client = CreateTestClient(services =>
        {
            services.AddSingleton(mediatorMock.Object);
        });

        client.DefaultRequestHeaders.Add("X-User-Id", followerId.ToString());

        var request = new FollowUserRequest { FollowedUserId = followedId };

        // Act
        var response = await client.PostAsJsonAsync("/follow", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Usuario seguido");

        mediatorMock.Verify(m =>
            m.Send(It.Is<FollowUserCommand>(c =>
                c.FollowerId.Value == followerId && c.FollowedId.Value == followedId),
            default),
            Times.Once);
    }

    [Fact]
    public async Task Timeline_Should_Handle_20000_Tweets_From_Followed_Users()
    {
        // Arrange
        var followerId = new UserId(Guid.NewGuid());
        int followedCount = 5;
        int tweetsPerUser = 20_000;

        var followedUsers = Enumerable.Range(0, followedCount)
            .Select(_ => new UserId(Guid.NewGuid()))
            .ToList();

        var tweets = followedUsers.SelectMany(u =>
            Enumerable.Range(0, tweetsPerUser).Select(i =>
                new Tweet(u, $"Tweet {i}"))).ToList();

        var followRepoMock = new Mock<IFollowRepository>();
        followRepoMock
            .Setup(r => r.GetFollowedUserIdsAsync(followerId))
            .ReturnsAsync(followedUsers);

        var tweetRepoMock = new Mock<ITweetRepository>();
        tweetRepoMock
            .Setup(r => r.GetTimelineAsync(followerId, 0, tweetsPerUser * followedCount))
            .ReturnsAsync(tweets.OrderByDescending(r => r.CreatedAt));

        var query = new GetTimelineQuery(followerId, 0, tweetsPerUser * followedCount);
        var handler = new GetTimelineQueryHandler(tweetRepoMock.Object);

        // Act
        var stopwatch = Stopwatch.StartNew();
        var result = await handler.Handle(query, CancellationToken.None);
        stopwatch.Stop();

        // Assert
        result.Should().HaveCount(tweets.Count);
        result.Should().BeInDescendingOrder(t => t.CreatedAt);

        Console.WriteLine($"Returned {result.Count()} tweets in {stopwatch.ElapsedMilliseconds}ms");
    }

    [Fact]
    public async Task GetFollowableUsers_Should_Return_Tweets_From_Mediator()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var users = new List<UserId>
        {
            new UserId(userId),
            new UserId(userId),
        };

        var mediatorMock = new Mock<IMediator>();
        mediatorMock
            .Setup(m => m.Send(It.IsAny<GetUsersQuery>(), default))
            .ReturnsAsync(users);

        var client = CreateTestClient(services =>
        {
            services.AddSingleton(mediatorMock.Object);
        });

        client.DefaultRequestHeaders.Add("X-User-Id", userId.ToString());

        // Act
        var response = await client.GetAsync("/followable_users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        json.GetArrayLength().Should().Be(2);

        mediatorMock.Verify(m =>
            m.Send(It.Is<GetUsersQuery>(q => q.UserId.Value == userId), default),
            Times.Once);
    }



    private static HttpClient CreateTestClient(Action<IServiceCollection> configureServices)
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddLogging();
                services.AddRouting();
                services.AddEndpointsApiExplorer();
                services.AddSingleton(TimeProvider.System);
                services.AddApplication();

                configureServices(services);
            })
            .Configure(app =>
            {
                app.Use(async (context, next) =>
                {
                    // Simular userId desde header
                    if (context.Request.Headers.TryGetValue("X-User-Id", out var userId))
                        context.Items["UserId"] = userId.ToString();

                    await next();
                });

                app.UseRouting();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapTweetEndpoints();
                    endpoints.MapFollowEndpoints();
                });
            });

        var server = new TestServer(builder);
        return server.CreateClient();
    }
    // private static HttpClient CreateTestClient(Action<IServiceCollection> configureServices)
    // {
    //     var builder = WebApplication.CreateSlimBuilder();

    //     builder.Services.AddLogging();
    //     builder.Services.AddEndpointsApiExplorer();
    //     builder.Services.AddSingleton(TimeProvider.System); // si usás TimeProvider
    //     builder.Services.AddApplication();

    //     configureServices(builder.Services);

    //     var app = builder.Build();

    //     app.MapTweetEndpoints();
    //     app.MapFollowEndpoints();

    //     app.Use(async (context, next) =>
    //     {
    //         // Mockeá el UserId desde el header X-User-Id
    //         if (context.Request.Headers.TryGetValue("X-User-Id", out var userId))
    //         {
    //             context.Items["UserId"] = userId.ToString();
    //         }
    //         await next();
    //     });

    //     // app.Run();
    //     var server = new TestServer(app.Services);
    //     return server.CreateClient();
    // }
}