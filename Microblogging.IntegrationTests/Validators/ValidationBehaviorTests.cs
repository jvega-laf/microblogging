using FluentValidation;
using Moq;
using Microblogging.Application.Tweets.Commands;
using MediatR;
using FluentAssertions;
using Microblogging.Validators;
using Microblogging.Domain.ValueObjects;
using FluentValidation.Results;
using Microblogging.Application.Common;
using Microblogging.Domain.Entities;


namespace Microblogging.IntegrationTests.Validators;

public class ValidationBehaviorTests
{
    private readonly Mock<IValidator<PostTweetCommand>> _validatorMock;
    private readonly ValidationBehavior<PostTweetCommand, Result> _validationBehavior;

    public ValidationBehaviorTests()
    {
        // Mock del validador
        _validatorMock = new Mock<IValidator<PostTweetCommand>>();

        // Comportamiento que se está probando
        _validationBehavior = new ValidationBehavior<PostTweetCommand, Result>(new[] { _validatorMock.Object });
    }

    [Fact]
    public async Task Should_Call_Next_If_No_Validation_Errors()
    {
        // Arrange
        var command = new PostTweetCommand
        (
          new UserId(Guid.NewGuid()),
            "Este es un tweet válido."
        );

        // Simulamos que no hay errores de validación
        _validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<PostTweetCommand>>()))
            .Returns(new ValidationResult());

        var resultValue = Result<Tweet>.SuccessResult();
        // Act
        var result = await _validationBehavior.Handle(command, (command) => Task.FromResult(resultValue), CancellationToken.None);

        // Assert
        result.Should().Be(resultValue);
        _validatorMock.Verify(v => v.Validate(It.IsAny<ValidationContext<PostTweetCommand>>()), Times.Once);
    }

    [Fact]
    public async Task Should_Throw_ValidationException_When_Validation_Fails()
    {
        // Arrange
        var command = new PostTweetCommand
        (
          new UserId(Guid.Empty), // Esto causará un error de validación
            "Este no es un tweet válido."
        );

        var validationFailure = new ValidationFailure("AuthorId", "El autor es requerido.");
        _validatorMock.Setup(v => v.Validate(It.IsAny<ValidationContext<PostTweetCommand>>()))
                .Returns(new ValidationResult(new[] { validationFailure
        }));

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _validationBehavior.Handle(command, (command) => Task.FromResult(Result<Tweet>.SuccessResult()), CancellationToken.None));
    }
}