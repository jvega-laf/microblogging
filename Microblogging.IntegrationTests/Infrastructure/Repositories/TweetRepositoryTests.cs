using Xunit;
using Moq;
using StackExchange.Redis;
using Microblogging.Domain.Entities;
using FluentAssertions;
using System;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microblogging.Domain.ValueObjects;
using Microblogging.Application.Abstractions.Repositories;
using Microblogging.Infrastructure.Repositories;

namespace Microblogging.IntegrationTests.Infrastructure.Repositories;

public class TweetRepositoryTests
{
    [Fact]
    public async Task AddAsync_Should_Add_Tweet_To_Author_And_Followers_Timeline()
    {
        var mockDb = new Mock<IDatabase>();
        var mockConn = new Mock<IConnectionMultiplexer>();
        mockConn.Setup(c => c.GetDatabase(It.IsAny<int>(), null)).Returns(mockDb.Object);

        var authorId = new UserId(Guid.NewGuid());
        var tweet = new Tweet(authorId, "Hello world");

        var followers = new List<UserId> { new(Guid.NewGuid()), new(Guid.NewGuid()) };

        var mockFollowRepo = new Mock<IFollowRepository>();
        mockFollowRepo.Setup(f => f.GetFollowedUserIdsAsync(authorId)).ReturnsAsync(followers);

        var repo = new TweetRepository(mockConn.Object, mockFollowRepo.Object);

        await repo.AddAsync(tweet);

        var json = JsonSerializer.Serialize(tweet);
        mockDb.Verify(db => db.ListLeftPushAsync($"tweets:{authorId}", json, When.Always, CommandFlags.None), Times.Once);

        foreach (var follower in followers)
        {
            mockDb.Verify(db => db.ListLeftPushAsync($"timeline:{follower}", json, When.Always, CommandFlags.None), Times.Once);
        }
    }

    [Fact]
    public async Task GetTimelineAsync_Should_Return_Deserialized_Tweets()
    {
        var userId = new UserId(Guid.NewGuid());
        var tweets = new[]
        {
            new Tweet(userId, "t1"),
            new Tweet(userId, "t2")
        };

        var redisValues = tweets.Select(t => (RedisValue)JsonSerializer.Serialize(t)).ToArray();

        var mockDb = new Mock<IDatabase>();
        mockDb.Setup(db => db.ListRangeAsync($"timeline:{userId}", 0, 49, CommandFlags.None))
              .ReturnsAsync(redisValues);

        var mockConn = new Mock<IConnectionMultiplexer>();
        mockConn.Setup(c => c.GetDatabase(It.IsAny<int>(), null)).Returns(mockDb.Object);

        var mockFollowRepo = new Mock<IFollowRepository>();

        var repo = new TweetRepository(mockConn.Object, mockFollowRepo.Object);

        var result = await repo.GetTimelineAsync(userId);

        result.Should().BeEquivalentTo(tweets, options => options
                .Excluding(tweet => tweet.Id) // Excluir la comparación del Id
                .Excluding(tweet => tweet.CreatedAt)); // Excluir la comparación de CreatedAt
    }
}