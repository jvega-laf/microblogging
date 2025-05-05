using Microblogging.Application.Contracts.Requests;
using Microblogging.Api.Extensions;
using Microblogging.Application.Tweets.Commands;
using MediatR;
using Microblogging.Application.Tweets.Queries;
using Microblogging.Domain.Entities;

namespace Microblogging.Api.Endpoints;

public static class TweetEndpoints
{
    public static IEndpointRouteBuilder MapTweetEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/tweets", async (
                PostTweetRequest requestDto,
                HttpRequest request,
                IMediator mediator
            ) =>
            {

                // Obtener el userId desde el header
                var error = request.TryGetUserId(out var userId);
                if (error is not null) return error;

                var command = new PostTweetCommand(userId!, requestDto.Content);

                //Ejecuta el command
                var result = await mediator.Send(command);
                if (result.Success)
                    return Results.Ok(new { message = "Tweet creado.", tweet = result.Value });
                else
                    return Results.BadRequest(new { error = result.Failure });
            });


        app.MapGet("/timeline", async (
            IMediator mediator,
            HttpRequest request) =>
        {
            var error = request.TryGetUserId(out var userId);
            if (error is not null) return error;

            var query = new GetTimelineQuery(userId!);

            var tweets = await mediator.Send(query) ?? new List<Tweet>();

            var result = tweets.Select(t => new
            {
                AuthorId = t.AuthorId.Value,
                t.Content,
                CreatedAt = t.CreatedAt.ToString("s")
            });

            return Results.Ok(result);
        });

        return app;
    }
}
