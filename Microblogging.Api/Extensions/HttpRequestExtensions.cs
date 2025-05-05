using Microblogging.Api.Responses;
using Microblogging.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Microblogging.Api.Extensions;

public static class HttpRequestExtensions
{
    public static IResult? TryGetUserId(this HttpRequest request, out UserId userId)
    {
        userId = default!;
        if (!request.Headers.TryGetValue("X-User-Id", out var userIdStr))
            return ErrorResults.MissingHeader("X-User-Id");

        if (!Guid.TryParse(userIdStr, out var guid))
            return ErrorResults.InvalidHeader("X-User-Id");

        if (guid == Guid.Empty)
            return ErrorResults.Custom("X-User-Id", "User Id not valid");

        userId = new UserId(guid);
        return null;
    }
}
