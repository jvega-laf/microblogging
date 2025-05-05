
using MediatR;
using Microblogging.Application.Abstractions.Repositories;
using Microblogging.Application.Tweets.Queries;
using Microblogging.Domain.Entities;
using Microblogging.Domain.ValueObjects;

namespace Microblogging.Application.Tweets.Handlers;

public class GetTimelineQueryHandler : IRequestHandler<GetTimelineQuery, IEnumerable<Tweet>>
{
    private readonly ITweetRepository _tweetRepository;

    public GetTimelineQueryHandler(ITweetRepository tweetRepository)
    {
        _tweetRepository = tweetRepository;
    }

    public async Task<IEnumerable<Tweet>> Handle(GetTimelineQuery query, CancellationToken cancellationToken)
    {
        var ret = await _tweetRepository.GetTimelineAsync(query.UserId, query.skip, query.maxValues);

        return ret.OrderByDescending(r => r.CreatedAt);
    }
}
