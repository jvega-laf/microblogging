using MediatR;
using Microblogging.Application.Common;
using Microblogging.Domain.Entities;
using Microblogging.Domain.ValueObjects;

namespace Microblogging.Application.Tweets.Commands;

public record PostTweetCommand(UserId AuthorId, string Content) : IRequest<Result<Tweet>>;
