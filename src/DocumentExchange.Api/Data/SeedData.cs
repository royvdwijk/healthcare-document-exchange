using DocumentExchange.Api.Models;

namespace DocumentExchange.Api.Data;

// NOTE: Fake test data, provided on startup when running development mode and explicitly setting the startup parameter to seed.
public static class SeedData
{
    public static IReadOnlyList<Patient> Patients { get; } =
    [
        new Patient(
            Bsn: "999990019",
            Name: "Jan Jansen",
            DateOfBirth: new DateOnly(1942, 3, 14),
            Allergies:
            [
                new Allergy("Penicillin", "Skin rash"),
                new Allergy("Peanuts", "Swelling of the throat"),
                new Allergy("Latex", "Itching")
            ])
    ];
}
