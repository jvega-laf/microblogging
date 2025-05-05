using Microblogging.Domain.Entities;
using Microblogging.Domain.ValueObjects;

namespace Microblogging.Application.Abstractions.Repositories;

public interface IFollowRepository
{
    Task AddAsync(Follow follow);
    Task<IEnumerable<UserId>> GetFollowedUserIdsAsync(UserId followerId);
    Task<IEnumerable<UserId>> GetFollowableUserIdsAsync(UserId followerId);
}
