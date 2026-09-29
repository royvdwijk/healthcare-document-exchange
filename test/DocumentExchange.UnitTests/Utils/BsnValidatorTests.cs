using DocumentExchange.Api.Utils;

namespace DocumentExchange.UnitTests.Utils;

public class BsnValidatorTests
{
    [Theory]
    [InlineData("999990019")]
    [InlineData("123456782")]
    [InlineData("111222333")]
    public void IsValid_PassesElevenCheck_ReturnsTrue(string bsn)
    {
        Assert.True(BsnValidator.IsValid(bsn));
    }

    [Theory]
    [InlineData("123456780")]
    [InlineData("123456789")]
    [InlineData("999990010")]
    public void IsValid_FailsElevenCheck_ReturnsFalse(string bsn)
    {
        Assert.False(BsnValidator.IsValid(bsn));
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345678")]
    [InlineData("1234567890")]
    [InlineData("12345678a")]
    [InlineData(" 99999001")]
    public void IsValid_NotNineDigits_ReturnsFalse(string bsn)
    {
        Assert.False(BsnValidator.IsValid(bsn));
    }

    [Theory]
    [InlineData("９９９９９００１９")]
    [InlineData("٩٩٩٩٩٠٠١٩")]
    [InlineData("𝟗𝟗𝟗𝟗𝟗𝟎𝟎𝟏𝟗")]
    [InlineData("99999００19")]
    [InlineData("⁹99990019")]
    public void IsValid_NonAsciiDigits_ReturnsFalse(string bsn)
    {
        Assert.False(BsnValidator.IsValid(bsn));
    }
}
