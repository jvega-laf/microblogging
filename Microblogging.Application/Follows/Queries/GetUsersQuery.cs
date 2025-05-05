using MediatR;
using Microblogging.Domain.ValueObjects;

namespace Microblogging.Application.Follows.Queries;

public record GetUsersQuery(UserId UserId) : IRequest<IEnumerable<UserId>>;
