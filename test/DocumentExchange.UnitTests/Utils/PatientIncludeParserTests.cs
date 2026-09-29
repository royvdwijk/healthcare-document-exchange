using DocumentExchange.Api.Contracts;
using DocumentExchange.Api.Utils;

namespace DocumentExchange.UnitTests.Utils;

public class PatientIncludeParserTests
{
    [Theory]
    [InlineData("allergies", PatientInclude.Allergies)]
    [InlineData("ALLERGIES", PatientInclude.Allergies)]
    [InlineData("Medications", PatientInclude.Medications)]
    public void TryParse_Name_IgnoresCasing(string value, PatientInclude expected)
    {
        Assert.True(PatientIncludeParser.TryParseName(value, out var include));

        Assert.Equal(expected, include);
    }

    [Theory]
    [InlineData("hobbies")]
    [InlineData("")]
    [InlineData(" allergies")]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("5")]
    [InlineData("Allergies,Medications")]
    public void TryParse_NotAName_ReturnsFalse(string value)
    {
        Assert.False(PatientIncludeParser.TryParseName(value, out _));
    }

    [Fact]
    public void TryParse_NoValues_ReturnsEmpty()
    {
        Assert.True(PatientIncludeParser.TryParseNames([], out var includes, out _));

        Assert.Empty(includes);
    }

    [Fact]
    public void TryParse_MultipleValues_ReturnsEachIncludeOnce()
    {
        Assert.True(PatientIncludeParser.TryParseNames(["allergies", "Allergies", "medications"], out var includes, out _));

        Assert.Equal(2, includes.Count);
        Assert.Contains(PatientInclude.Allergies, includes);
        Assert.Contains(PatientInclude.Medications, includes);
    }

    [Fact]
    public void TryParse_InvalidAfterValid_ReturnsFirstInvalidValue()
    {
        Assert.False(PatientIncludeParser.TryParseNames(["allergies", "hobbies", "0"], out var includes, out var invalidValue));

        Assert.Null(includes);
        Assert.Equal("hobbies", invalidValue);
    }
}
