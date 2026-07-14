namespace EnergyMix.Api.Domain;

public static class EnergyConstants
{
    public static readonly IReadOnlySet<string> CleanEnergySources = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "biomass",
        "nuclear",
        "hydro",
        "wind",
        "solar"
    };

    public static bool IsCleanEnergy(string fuel) => CleanEnergySources.Contains(fuel);
}