using FluentValidation.TestHelper;
using Microblogging.Application.Follows.Commands;
using Microblogging.Domain.ValueObjects;
using Microblogging.Validators;

namespace Microblogging.IntegrationTests.Validators;

public class FollowUserCommandValidatorTests
{
    private readonly FollowUserCommandValidator _validator;

    public FollowUserCommandValidatorTests()
    {
        _validator = new FollowUserCommandValidator();
    }

    [Fact]
    public void Validate_Should_Pass_When_FollowerId_And_FollowedId_Are_Distinct()
    {
        // Arrange
        var followerId = new UserId(Guid.NewGuid());
        var followedId = new UserId(Guid.NewGuid());
        var command = new FollowUserCommand(followerId, followedId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_Should_Fail_When_Same_UserIds()
    {
        // Arrange
        var userId = new UserId(Guid.NewGuid());
        var command = new FollowUserCommand(userId, userId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd)
              .WithErrorMessage("No puedes seguirte a ti mismo.");
    }

    [Fact]
    public void Validate_Should_Fail_When_FollowerId_Is_Default()
    {
        // Arrange
        var followedId = new UserId(Guid.NewGuid());
        var command = new FollowUserCommand(default!, followedId);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.FollowerId)
              .WithErrorMessage("El usuario que sigue es requerido.");
    }

    [Fact]
    public void Validate_Should_Fail_When_FollowedId_Is_Default()
    {
        // Arrange
        var followerId = new UserId(Guid.NewGuid());
        var command = new FollowUserCommand(followerId, default!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(cmd => cmd.FollowedId)
              .WithErrorMessage("El usuario a seguir es requerido.");
    }
}