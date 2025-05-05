using System.Text.Json;
using Microblogging.Application.Abstractions.Repositories;
using Microblogging.Domain.Entities;
using Microblogging.Domain.ValueObjects;
using StackExchange.Redis;

namespace Microblogging.Infrastructure.Repositories;

public class TweetRepository : ITweetRepository
{
    private readonly IDatabase _db;
    private readonly IFollowRepository _followRepository;

    public TweetRepository(IConnectionMultiplexer connection, IFollowRepository followRepository)
    {
        _db = connection.GetDatabase();
        _followRepository = followRepository;
    }

    public async Task AddAsync(Tweet tweet)
    {

        // Asegurar que el autor esté registrado como usuario
        await _db.SetAddAsync("users", tweet.AuthorId.ToString());

        var tweetJson = JsonSerializer.Serialize(tweet);

        // Guardar en tweets del autor
        var authorKey = $"tweets:{tweet.AuthorId}";
        await _db.ListLeftPushAsync(authorKey, tweetJson);

        // Fan-out: guardar en timelines de cada seguidor
        var followers = await _followRepository.GetFollowedUserIdsAsync(tweet.AuthorId);
        foreach (var followerId in followers)
        {
            var timelineKey = $"timeline:{followerId}";
            await _db.ListLeftPushAsync(timelineKey, tweetJson);
        }

        // (opcional) TTL para limitar tweets antiguos
        // await _db.KeyExpireAsync(timelineKey, TimeSpan.FromDays(30));
    }

    public async Task<IEnumerable<Tweet>> GetTimelineAsync(UserId userId, int skip = 0, int take = 50)
    {
        var key = $"timeline:{userId}";
        var items = await _db.ListRangeAsync(key, skip, skip + take - 1);

        return items
            .Select(item => JsonSerializer.Deserialize<Tweet>(item!))
            .Where(tweet => tweet != null)
            .Cast<Tweet>();
    }

}

