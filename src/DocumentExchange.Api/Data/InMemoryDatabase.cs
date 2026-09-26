using System.Collections.Concurrent;
using DocumentExchange.Api.Models;

namespace DocumentExchange.Api.Data;

/// <summary>Holds all data in memory, shared by the in-memory repositories.</summary>
/// <remarks>Data is lost when the application stops.</remarks>
public sealed class InMemoryDatabase
{
    public ConcurrentDictionary<string, Patient> Patients { get; } = new();
    public ConcurrentDictionary<Guid, Referral> Referrals { get; } = new();
}
