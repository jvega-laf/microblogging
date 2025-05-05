using FluentValidation;
using Microblogging.Application.Tweets.Commands;

namespace Microblogging.Validators;

public class PostTweetCommandValidator : AbstractValidator<PostTweetCommand>
{
    public PostTweetCommandValidator()
    {
        RuleFor(x => x.AuthorId)
            .NotNull()
            .NotEmpty()
            .WithMessage("El autor es requerido.");

        RuleFor(x => x)
            .Must(cmd => cmd.AuthorId.Value != Guid.Empty)
            .WithMessage("El autor es requerido.");


        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("El contenido no puede estar vacío.")
            .MaximumLength(280).WithMessage("El tweet no puede exceder los 280 caracteres.");
    }
}