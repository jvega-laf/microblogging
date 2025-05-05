using Microblogging.Application.Tweets.Commands;
using Microblogging.Application.Abstractions.Repositories;
using Microblogging.Domain.Entities;
using Microblogging.Application.Common;
using MediatR;

namespace Microblogging.Application.Tweets.Handlers;

public class PostTweetCommandHandler : IRequestHandler<PostTweetCommand, Result<Tweet>>
{
    private readonly ITweetRepository _tweetRepository;

    public PostTweetCommandHandler(ITweetRepository tweetRepository)
    {
        _tweetRepository = tweetRepository;
    }

    public async Task<Result<Tweet>> Handle(PostTweetCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var tweet = new Tweet(request.AuthorId, request.Content);

            await _tweetRepository.AddAsync(tweet);

            return Result<Tweet>.SuccessResult(tweet);
        }
        catch (Exception ex)
        {
            return Result<Tweet>.FailureResult(ex.Message);
        }
    }
}
