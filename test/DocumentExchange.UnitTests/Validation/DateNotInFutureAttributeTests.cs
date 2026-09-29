using DocumentExchange.Api.Validation;

namespace DocumentExchange.UnitTests.Validation;

public class DateNotInFutureAttributeTests
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.UtcNow);

    private readonly DateNotInFutureAttribute _attribute = new();

    [Fact]
    public void IsValid_Yesterday_ReturnsTrue()
    {
        Assert.True(_attribute.IsValid(Today.AddDays(-1)));
    }

    [Fact]
    public void IsValid_Today_ReturnsTrue()
    {
        Assert.True(_attribute.IsValid(Today));
    }

    [Fact]
    public void IsValid_Tomorrow_ReturnsFalse()
    {
        Assert.False(_attribute.IsValid(Today.AddDays(1)));
    }

    [Fact]
    public void IsValid_Null_ReturnsTrue()
    {
        Assert.True(_attribute.IsValid(null));
    }

    [Fact]
    public void IsValid_NotADateOnly_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => _attribute.IsValid(DateTime.UtcNow));
    }
}
