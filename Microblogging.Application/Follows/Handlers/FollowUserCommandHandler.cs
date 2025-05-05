using Microblogging.Application.Follows.Commands;
using Microblogging.Application.Abstractions.Repositories;
using Microblogging.Domain.Entities;
using MediatR;
using Microblogging.Application.Common;

namespace Microblogging.Application.Follows.Handlers;

public class FollowUserCommandHandler : IRequestHandler<FollowUserCommand, Result>
{
    private readonly IFollowRepository _followRepository;

    public FollowUserCommandHandler(IFollowRepository followRepository)
    {
        _followRepository = followRepository;
    }

    public async Task<Result> Handle(FollowUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var follow = new Follow(request.FollowerId, request.FollowedId);
            await _followRepository.AddAsync(follow);
            return Result.SuccessResult();
        }
        catch (System.Exception ex)
        {
            return Result.FailureResult(ex.Message);
        }
    }

}
