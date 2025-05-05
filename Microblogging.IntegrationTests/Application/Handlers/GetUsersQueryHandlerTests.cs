using Xunit;
using Moq;
using Microblogging.Application.Abstractions.Repositories;
using Microblogging.Domain.Entities;
using Microblogging.Domain.ValueObjects;
using Microblogging.Application.Follows.Handlers;
using Microblogging.Application.Follows.Queries;

namespace Microblogging.IntegrationTests.Application.Handlers;
public class GetUsersQueryHandlerTests
{
    private readonly Mock<IFollowRepository> _followRepositoryMock;
    private readonly GetUsersQueryHandler _handler;

    public GetUsersQueryHandlerTests()
    {
        _followRepositoryMock = new Mock<IFollowRepository>();
        _handler = new GetUsersQueryHandler(_followRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTweetsFromRepository()
    {
        var userIdGuid = Guid.NewGuid();
        // Arrange
        var userId = new UserId(userIdGuid);
        var userId1 = new UserId(Guid.NewGuid());
        var userId2 = new UserId(Guid.NewGuid());

        var followables = new List<UserId>
        {
            userId1,
            userId2
        };

        _followRepositoryMock
            .Setup(r => r.GetFollowableUserIdsAsync(userId))
            .ReturnsAsync(followables);

        var query = new GetUsersQuery(userId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Collection(result,
            t => Assert.Equal(userId1.Value, t.Value),
            t => Assert.Equal(userId2.Value, t.Value)
        );
    }
}
