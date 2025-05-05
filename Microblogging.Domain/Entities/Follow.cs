using Microblogging.Domain.ValueObjects;

namespace Microblogging.Domain.Entities;

public class Follow
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public UserId FollowerId { get; private set; }
    public UserId FollowedId { get; private set; }

    public Follow(UserId followerId, UserId followedId)
    {
        FollowerId = followerId;
        FollowedId = followedId;
    }
}