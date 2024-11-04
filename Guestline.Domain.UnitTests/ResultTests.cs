using FluentAssertions;
using Guestline.Infrastructure.Persistence.Contracts;

namespace Guestline.Domain.UnitTests;

public class ResultTests
{
    [TestCase]
    public void Value_WhenSuccess_ReturnsCorrectValue()
    {
        var result = Result<string>.Success("some value");
        
        result.Value.Should().Be("some value");
    }

    [TestCase]
    public void IsSuccess_WhenFailedWithException_ReturnFalse()
    {
        var exception = new Exception();
        var failedResult = Result<string>.Failure(exception);
        
        failedResult.IsSuccess.Should().BeFalse();
        failedResult.Exception.Should().Be(exception);
        failedResult.Error.Should().BeNull();
    }
    
    [TestCase]
    public void IsSuccess_WhenFailedWithError_ReturnFalse()
    {
        var failedResult = Result<string>.Failure("error");
        
        failedResult.IsSuccess.Should().BeFalse();
        failedResult.Error.Should().Be("error");
        failedResult.Exception.Should().BeNull();
    }
    
    [TestCase]
    public void Value_WhenErrorResult_ThrowsException()
    {
        var failedResult = Result<string>.Failure("Failed");
        
        failedResult.Invoking(fr => fr.Value).Should().Throw<Exception>();
    }

    [TestCase]
    public void Value_WhenSuccess_AndImplicitCasting_ReturnsCorrectValue()
    {
        var result = ImplicitResult("some value");
        
        result.Value.Should().Be("some value");
    }

    private static Result<string> ImplicitResult(string result) => result;
}