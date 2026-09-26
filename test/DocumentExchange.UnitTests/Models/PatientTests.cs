using DocumentExchange.Api.Models;

namespace DocumentExchange.UnitTests.Models;

public class PatientTests
{
    private const string Bsn = "999990019";
    private static readonly DateOnly DateOfBirth = new(1942, 3, 14);

    [Fact]
    public void Merge_SameBsn_UsesNameAndDateOfBirthOfOther()
    {
        var correctedDateOfBirth = DateOfBirth.AddDays(1);
        var known = CreatePatient(name: "Jan Jansen", dateOfBirth: DateOfBirth);
        var other = CreatePatient(name: "J. Jansen", dateOfBirth: correctedDateOfBirth);

        var merged = known.Merge(other);

        Assert.Equal("J. Jansen", merged.Name);
        Assert.Equal(correctedDateOfBirth, merged.DateOfBirth);
    }

    [Fact]
    public void Merge_NewAllergy_IsAdded()
    {
        var known = CreatePatient(allergies: [new Allergy("Latex", "Itching")]);
        var other = CreatePatient(allergies: [new Allergy("Peanuts", "Swelling of the throat")]);

        var merged = known.Merge(other);

        Assert.Equal(2, merged.Allergies.Count);
        Assert.Contains(new Allergy("Latex", "Itching"), merged.Allergies);
        Assert.Contains(new Allergy("Peanuts", "Swelling of the throat"), merged.Allergies);
    }

    [Fact]
    public void Merge_AllergyNotSentAlong_IsKept()
    {
        var known = CreatePatient(allergies: [new Allergy("Latex", "Itching")]);
        var other = CreatePatient(allergies: []);

        var merged = known.Merge(other);

        Assert.Equal([new Allergy("Latex", "Itching")], merged.Allergies);
    }

    [Theory]
    [InlineData("Penicillin")]
    [InlineData("penicillin")]
    [InlineData("PENICILLIN")]
    public void Merge_KnownAllergy_TakesReactionOfOtherWithoutDuplicating(string substance)
    {
        var known = CreatePatient(allergies: [new Allergy("Penicillin", "Skin rash")]);
        var other = CreatePatient(allergies: [new Allergy(substance, "Severe rash")]);

        var merged = known.Merge(other);

        var allergy = Assert.Single(merged.Allergies);
        Assert.Equal("Severe rash", allergy.Reaction);
    }

    [Fact]
    public void Merge_DifferentBsn_Throws()
    {
        var known = CreatePatient(bsn: Bsn);
        var other = CreatePatient(bsn: "111222333");

        var exception = Assert.Throws<ArgumentException>(() => known.Merge(other));

        Assert.Equal("other", exception.ParamName);
    }

    [Fact]
    public void Merge_Null_Throws()
    {
        var known = CreatePatient();

        var exception = Assert.Throws<ArgumentException>(() => known.Merge(null!));

        Assert.Equal("other", exception.ParamName);
    }

    [Fact]
    public void Merge_DoesNotChangeEitherPatient()
    {
        var known = CreatePatient(name: "Jan Jansen", allergies: [new Allergy("Latex", "Itching")]);
        var other = CreatePatient(name: "J. Jansen", allergies: [new Allergy("Peanuts", "Swelling of the throat")]);

        known.Merge(other);

        Assert.Equal("Jan Jansen", known.Name);
        Assert.Equal([new Allergy("Latex", "Itching")], known.Allergies);
        Assert.Equal("J. Jansen", other.Name);
        Assert.Equal([new Allergy("Peanuts", "Swelling of the throat")], other.Allergies);
    }

    private static Patient CreatePatient(
        string bsn = Bsn,
        string name = "Jan Jansen",
        DateOnly? dateOfBirth = null,
        IReadOnlyList<Allergy>? allergies = null) =>
        new(bsn, name, dateOfBirth ?? DateOfBirth, allergies ?? []);
}
