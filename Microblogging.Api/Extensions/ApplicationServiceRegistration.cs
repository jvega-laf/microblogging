using FluentValidation;
using MediatR;
using Microblogging.Application.Follows.Commands;
using Microblogging.Application.Follows.Handlers;
using Microblogging.Application.Tweets.Commands;
using Microblogging.Application.Tweets.Handlers;
using Microblogging.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Microblogging.Api.Extensions
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Aquí se registrarán los casos de uso y handlers

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblyContaining<PostTweetCommandHandler>());

            services.AddValidatorsFromAssemblyContaining<PostTweetCommandValidator>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
