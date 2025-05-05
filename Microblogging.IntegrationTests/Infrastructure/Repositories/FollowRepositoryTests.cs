using System.Threading.Tasks;
using Xunit;
using Moq;
using StackExchange.Redis;
using Microblogging.Domain.Entities;
using FluentAssertions;
using System;
using System.Linq;
using Microblogging.Infrastructure.Repositories;
using Microblogging.Domain.ValueObjects;
using System.Text.Json;

namespace Microblogging.IntegrationTests.Infrastructure.Repositories;

public class FollowRepositoryTests
{
    [Fact]
    public async Task AddAsync_Should_Add_UserId_To_Set()
    {
        var mockDb = new Mock<IDatabase>();
        var mockConnection = new Mock<IConnectionMultiplexer>();
        mockConnection.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDb.Object);

        var repo = new FollowRepository(mockConnection.Object);
        var follower = new UserId(Guid.NewGuid());
        var followed = new UserId(Guid.NewGuid());
        var follow = new Follow(follower, followed);

        await repo.AddAsync(follow);

        mockDb.Verify(db => db.SetAddAsync(
            $"follows:{follower}", followed.ToString(), CommandFlags.None), Times.Once);
    }

    [Fact]
    public async Task GetFollowedUserIdsAsync_Should_Return_All_Followed_Users()
    {
        var follower = new UserId(Guid.NewGuid());
        var expected = new[] { Guid.NewGuid(), Guid.NewGuid() };

        var redisValues = expected.Select(g => (RedisValue)g.ToString()).ToArray();

        var mockDb = new Mock<IDatabase>();
        mockDb.Setup(db => db.SetMembersAsync($"follows:{follower}", CommandFlags.None))
              .ReturnsAsync(redisValues);

        var mockConn = new Mock<IConnectionMultiplexer>();
        mockConn.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDb.Object);

        var repo = new FollowRepository(mockConn.Object);

        var result = await repo.GetFollowedUserIdsAsync(follower);

        result.Should().BeEquivalentTo(expected.Select(g => new UserId(g)));
    }


    [Fact]
    public async Task GetFollowableUserIdsAsync_Returns_Users_Not_Followed_And_Excludes_Self()
    {
        // Arrange
        var followerId = new UserId(Guid.NewGuid());
        var keyUsers = "users";
        var keyFollows = $"follows:{followerId.Value}";

        // Create 4 users including self
        var userGuids = new[]
        {
                followerId.Value,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };
        var allUsers = userGuids.Select(g => (RedisValue)g.ToString()).ToArray();

        // Follows one of them
        var followed = new[] { (RedisValue)userGuids[1].ToString() };
        var mockDb = new Mock<IDatabase>();

        mockDb.Setup(db => db.SetMembersAsync(keyUsers, CommandFlags.None))
               .ReturnsAsync(allUsers);
        mockDb.Setup(db => db.SetMembersAsync(keyFollows, CommandFlags.None))
               .ReturnsAsync(followed);

        var mockConn = new Mock<IConnectionMultiplexer>();
        mockConn.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDb.Object);
        var repo = new FollowRepository(mockConn.Object);

        // Act
        var result = await repo.GetFollowableUserIdsAsync(followerId);

        // Assert
        // Should exclude self and the one already followed
        var expected = userGuids.Skip(2).Select(g => new UserId(g));
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetFollowedUserIdsAsync_Returns_All_Followed_Users()
    {
        // Arrange
        var followerId = new UserId(Guid.NewGuid());
        var keyFollows = $"follows:{followerId.Value}";

        var followedGuids = new[] { Guid.NewGuid(), Guid.NewGuid() };
        var followed = followedGuids.Select(g => (RedisValue)g.ToString()).ToArray();
        var mockDb = new Mock<IDatabase>();

        mockDb.Setup(db => db.SetMembersAsync(keyFollows, CommandFlags.None))
               .ReturnsAsync(followed);


        var mockConn = new Mock<IConnectionMultiplexer>();
        mockConn.Setup(c => c.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDb.Object);
        var repo = new FollowRepository(mockConn.Object);

        // Act
        var result = await repo.GetFollowedUserIdsAsync(followerId);

        // Assert
        var expected = followedGuids.Select(g => new UserId(g));
        result.Should().BeEquivalentTo(expected);
    }
}