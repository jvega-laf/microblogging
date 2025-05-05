
using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microblogging.Application.Tweets.Handlers;
using Microblogging.Application.Tweets.Queries;
using Microblogging.Application.Abstractions.Repositories;
using Microblogging.Domain.Entities;
using Microblogging.Domain.ValueObjects;

namespace Microblogging.IntegrationTests.Application.Handlers;
public class GetTimelineQueryHandlerTests
{
    private readonly Mock<ITweetRepository> _tweetRepositoryMock;
    private readonly GetTimelineQueryHandler _handler;

    public GetTimelineQueryHandlerTests()
    {
        _tweetRepositoryMock = new Mock<ITweetRepository>();
        _handler = new GetTimelineQueryHandler(_tweetRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTweetsFromRepository()
    {
        var userIdGuid = Guid.NewGuid();
        // Arrange
        var userId = new UserId(userIdGuid);
        var tweets = new List<Tweet>
        {
            new Tweet(userId, "Tweet 1"),
            new Tweet(userId, "Tweet 2")
        };

        _tweetRepositoryMock
            .Setup(r => r.GetTimelineAsync(userId, 0, 50))
            .ReturnsAsync(tweets);

        var query = new GetTimelineQuery(userId, 0, 50);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Collection(result,
            t => Assert.Equal("Tweet 2", t.Content),
            t => Assert.Equal("Tweet 1", t.Content)
        );
    }
}
