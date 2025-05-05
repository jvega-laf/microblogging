using FluentValidation;
using Microblogging.Application.Follows.Commands;

namespace Microblogging.Validators;


public class FollowUserCommandValidator : AbstractValidator<FollowUserCommand>
{
    public FollowUserCommandValidator()
    {
        RuleFor(x => x.FollowerId)
            .NotNull()
            .NotEmpty()
            .WithMessage("El usuario que sigue es requerido.");

        RuleFor(x => x)
            .Must(cmd => cmd.FollowerId.Value != Guid.Empty)
            .WithMessage("El usuario que sigue es requerido.");

        RuleFor(x => x.FollowedId)
            .NotNull()
            .NotEmpty()
            .WithMessage("El usuario a seguir es requerido.");

        RuleFor(x => x)
            .Must(cmd => cmd.FollowedId.Value != Guid.Empty)
            .WithMessage("El usuario a seguir es requerido.");

        RuleFor(x => x)
            .Must(cmd => cmd.FollowerId != cmd.FollowedId)
            .WithMessage("No puedes seguirte a ti mismo.");
    }
}