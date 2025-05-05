using Microblogging.Domain.Entities;
using Microblogging.Domain.ValueObjects;

namespace Microblogging.Application.Abstractions.Repositories;

public interface ITweetRepository
{
    Task AddAsync(Tweet tweet);
    Task<IEnumerable<Tweet>> GetTimelineAsync(UserId userId, int skip = 0, int take = 50);
}
