using Microsoft.AspNetCore.Http;
using Microblogging.Api.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;
using FluentAssertions;
using Microblogging.Api.Responses;

namespace Microblogging.IntegrationTests.Api;

public class HttpRequestExtensionsTests
{
    [Fact]
    public void TryGetUserId_Should_Return_UserId_When_Header_Is_Valid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var context = new DefaultHttpContext();
        context.Request.Headers["X-User-Id"] = userId.ToString();

        // Act
        var result = context.Request.TryGetUserId(out var parsedUserId);

        // Assert
        Assert.Null(result); // null = success
        Assert.Equal(userId, parsedUserId.Value);
    }

    [Fact]
    public void TryGetUserId_Should_Return_Error_When_Header_Is_Missing()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = context.Request;

        // Act
        IResult? result = request.TryGetUserId(out var _);

        // Assert
        Assert.NotNull(result);

        result.Should().NotBeNull();
        var badRequest = result as BadRequest<ErrorMsg>;
        badRequest.Should().NotBeNull();

        var value = badRequest!.Value;
        var errors = value?.Errors ?? new string[0];


        errors.Should().NotBeNull();
        errors!.Should().Contain(e => e.Contains("Falta el header X-User-Id"));
    }

    [Fact]
    public void TryGetUserId_Should_Return_Error_When_Header_Is_Invalid_Guid()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.Headers["X-User-Id"] = "no-es-un-guid";

        // Act
        IResult? result = request.TryGetUserId(out var _);

        // Assert
        Assert.NotNull(result);

        result.Should().NotBeNull();
        var badRequest = result as BadRequest<ErrorMsg>;
        badRequest.Should().NotBeNull();

        var value = badRequest!.Value;
        var errors = value?.Errors ?? new string[0];

        errors.Should().NotBeNull();
        errors!.Should().Contain(e => e.Contains("X-User-Id no es válido"));
    }

    [Fact]
    public void TryGetUserId_Should_Return_Error_When_Header_Is_Empty_Guid()
    {
        // Arrange
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.Headers["X-User-Id"] = Guid.Empty.ToString();

        // Act
        var result = request.TryGetUserId(out var _);

        // Assert
        result.Should().NotBeNull();
        var badRequest = result as BadRequest<ErrorMsg>;
        badRequest.Should().NotBeNull();

        var value = badRequest!.Value;
        var errors = value?.Errors ?? new string[0];

        errors.Should().NotBeNull();
        errors!.Should().Contain("X-User-Id");
        errors.Should().Contain("User Id not valid");
    }


}
