using MediatR;
using Microblogging.Application.Abstractions.Repositories;
using Microblogging.Application.Follows.Queries;
using Microblogging.Domain.ValueObjects;

namespace Microblogging.Application.Follows.Handlers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserId>>
{
    private readonly IFollowRepository _followRepository;

    public GetUsersQueryHandler(IFollowRepository followRepository)
    {
        _followRepository = followRepository;
    }

    public async Task<IEnumerable<UserId>> Handle(GetUsersQuery query, CancellationToken cancellationToken)
    {
        var ret = await _followRepository.GetFollowableUserIdsAsync(query.UserId);

        return ret;
    }
}
