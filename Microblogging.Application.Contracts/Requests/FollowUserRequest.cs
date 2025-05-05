namespace Microblogging.Application.Contracts.Requests;


public class FollowUserRequest
{
    public Guid FollowedUserId { get; set; }
}
