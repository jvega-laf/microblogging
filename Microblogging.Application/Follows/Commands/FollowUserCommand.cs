using MediatR;
using Microblogging.Application.Common;
using Microblogging.Domain.ValueObjects;

namespace Microblogging.Application.Follows.Commands;

public record FollowUserCommand(UserId FollowerId, UserId FollowedId) : IRequest<Result>;
