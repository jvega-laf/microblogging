using MediatR;
using Microblogging.Domain.Entities;
using Microblogging.Domain.ValueObjects;

namespace Microblogging.Application.Tweets.Queries;

public record GetTimelineQuery(UserId UserId, int skip = 0, int maxValues = 50) : IRequest<IEnumerable<Tweet>>;
