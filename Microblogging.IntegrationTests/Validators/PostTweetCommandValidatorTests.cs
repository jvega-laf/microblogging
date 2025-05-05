using FluentValidation.TestHelper;
using Microblogging.Application.Tweets.Commands;
using Microblogging.Domain.ValueObjects;
using Microblogging.Validators;

namespace Microblogging.IntegrationTests.Validators;

public class PostTweetCommandValidatorTests
{
    private readonly PostTweetCommandValidator _validator;

    public PostTweetCommandValidatorTests()
    {
        _validator = new PostTweetCommandValidator();
    }

    [Fact]
    public void Should_Have_No_Validation_Errors_When_Valid()
    {
        // Arrange
        var command = new PostTweetCommand(
            new UserId(Guid.NewGuid()),
            "Este es un tweet válido."
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Have_Error_When_AuthorId_Is_Null()
    {
        // Arrange
        var command = new PostTweetCommand
        (
           new UserId(Guid.Empty),
           "Este no es un tweet válido."
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.AuthorId)
            .WithErrorMessage("El autor es requerido.");
    }

    [Fact]
    public void Should_Have_Error_When_Content_Is_Empty()
    {
        // Arrange
        var command = new PostTweetCommand(
            new UserId(Guid.NewGuid()),
            ""
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("El contenido no puede estar vacío.");
    }

    [Fact]
    public void Should_Have_Error_When_Content_Exceeds_280_Characters()
    {
        // Arrange
        var command = new PostTweetCommand(
            new UserId(Guid.NewGuid()),
            new string('a', 281)
        );

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Content)
            .WithErrorMessage("El tweet no puede exceder los 280 caracteres.");
    }
}