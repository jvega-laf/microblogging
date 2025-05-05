using System.Linq;
using Microblogging.Application.Abstractions.Repositories;
using Microblogging.Domain.Entities;
using Microblogging.Domain.ValueObjects;
using StackExchange.Redis;

namespace Microblogging.Infrastructure.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly IDatabase _db;

    public FollowRepository(IConnectionMultiplexer connection)
    {
        _db = connection.GetDatabase();
    }

    public async Task AddAsync(Follow follow)
    {
        var key = $"follows:{follow.FollowerId}";
        await _db.SetAddAsync(key, follow.FollowedId.ToString());
    }

    public async Task<IEnumerable<UserId>> GetFollowableUserIdsAsync(UserId followerId)
    {
        // Todos los usuarios registrados
        var userIds = await _db.SetMembersAsync("users");

        // IDs que ya está siguiendo
        var following = await _db.SetMembersAsync($"follows:{followerId}");
        var followingSet = following.Select(f => f.ToString()).ToHashSet();


        // También consideramos que uno no puede seguirse a sí mismo
        followingSet.Add(followerId.Value.ToString());

        // Filtrar los que no está siguiendo (puede incluirse a sí mismo si no se sigue)
        return userIds
            .Select(u => new UserId(Guid.Parse(u!)))
            .Where(u => !followingSet.Contains(u.Value.ToString()));
    }


    public async Task<IEnumerable<UserId>> GetFollowedUserIdsAsync(UserId followerId)
    {
        var key = $"follows:{followerId}";
        var members = await _db.SetMembersAsync(key);
        return members.Select(x => new UserId(Guid.Parse(x!)));
    }
}
