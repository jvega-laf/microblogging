using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Microblogging.Application.Tweets.Commands;
using Microblogging.Application.Tweets.Handlers;
using Microblogging.Application.Abstractions.Repositories;
using Microblogging.Domain.Entities;
using Microblogging.Domain.ValueObjects;
using Microblogging.Application.Common;

namespace Microblogging.IntegrationTests.Application.Handlers;

public class PostTweetCommandHandlerTests
{
    private readonly Mock<ITweetRepository> _tweetRepositoryMock;
    private readonly PostTweetCommandHandler _handler;

    public PostTweetCommandHandlerTests()
    {
        _tweetRepositoryMock = new Mock<ITweetRepository>();
        _handler = new PostTweetCommandHandler(_tweetRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenTweetIsPosted()
    {
        var userId = Guid.NewGuid();
        // Arrange
        var command = new PostTweetCommand(new UserId(userId), "Hello world!");

        _tweetRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Tweet>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(command.Content, result.Value!.Content);
        Assert.Equal(command.AuthorId, result.Value!.AuthorId);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailureResult_WhenRepositoryThrows()
    {
        var userId = Guid.NewGuid();
        // Arrange
        var command = new PostTweetCommand(new UserId(userId), "This will fail");

        _tweetRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Tweet>()))
            .ThrowsAsync(new Exception("DB failure"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Value);
        Assert.Equal("DB failure", result.Error);
    }
}

