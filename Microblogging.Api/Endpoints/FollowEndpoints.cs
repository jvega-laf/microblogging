using Microblogging.Application.Contracts.Requests;
using Microblogging.Api.Extensions;
using Microblogging.Application.Follows.Commands;
using Microblogging.Domain.ValueObjects;
using MediatR;
using Microblogging.Application.Follows.Queries;

namespace Microblogging.Api.Endpoints;

public static class FollowEndpoints
{
    public static IEndpointRouteBuilder MapFollowEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/follow", async (
            FollowUserRequest requestDto,
            IMediator mediator,
            HttpRequest request
            ) =>
        {

            // Obtener el userId desde el header
            var error = request.TryGetUserId(out var userId);
            if (error is not null) return error;

            // Crear el comando
            var command = new FollowUserCommand(userId!, new UserId(requestDto.FollowedUserId));

            //Ejecuta el command
            var ret = await mediator.Send(command);
            if (ret.Success)
                return Results.Ok("Usuario seguido.");
            else
                return Results.BadRequest(ret.Error ?? "Error general");

        });

        app.MapGet("/followable_users", async (
            IMediator mediator,
            HttpRequest request
            ) =>
        {

            // Obtener el userId desde el header
            var error = request.TryGetUserId(out var userId);
            if (error is not null) return error;

            // Crear el query
            var query = new GetUsersQuery(userId!);

            var lst = await mediator.Send(query) ?? new List<UserId>();

            var result = lst.Select(r => r.Value);

            return Results.Ok(result);
        });


        return app;
    }
}
