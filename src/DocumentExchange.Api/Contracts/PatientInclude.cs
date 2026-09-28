namespace DocumentExchange.Api.Contracts;

/// <summary>
/// Extra information that can be requested with a patient, e.g. <c>?include=allergies&amp;include=medications</c>.
/// </summary>
public enum PatientInclude
{
    Allergies,
    Medications
}
