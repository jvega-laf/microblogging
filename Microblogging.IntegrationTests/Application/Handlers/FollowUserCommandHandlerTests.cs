using System;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microblogging.Application.Follows.Commands;
using Microblogging.Application.Follows.Handlers;
using Microblogging.Application.Abstractions.Repositories;
using Microblogging.Domain.ValueObjects;
using Microblogging.Domain.Entities;
using Microblogging.Application.Common;

namespace Microblogging.IntegrationTests.Application.Handlers;

public class FollowUserCommandHandlerTests
{
    private readonly Mock<IFollowRepository> _followRepositoryMock;
    private readonly FollowUserCommandHandler _handler;

    public FollowUserCommandHandlerTests()
    {
        _followRepositoryMock = new Mock<IFollowRepository>();
        _handler = new FollowUserCommandHandler(_followRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldAddFollowAndReturnSuccess()
    {
        // Arrange
        var followerId = new UserId(Guid.NewGuid());
        var followedId = new UserId(Guid.NewGuid());
        var command = new FollowUserCommand(followerId, followedId);

        _followRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Follow>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        _followRepositoryMock.Verify(r => r.AddAsync(It.Is<Follow>(
            f => f.FollowerId == followerId && f.FollowedId == followedId
        )), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenExceptionThrown_ShouldReturnFailure()
    {
        // Arrange
        var followerId = new UserId(Guid.NewGuid());
        var followedId = new UserId(Guid.NewGuid());
        var command = new FollowUserCommand(followerId, followedId);

        _followRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Follow>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Database error", result.Error);
    }
}
