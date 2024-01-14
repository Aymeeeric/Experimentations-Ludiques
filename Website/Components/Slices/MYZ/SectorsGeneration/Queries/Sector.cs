namespace Aymeeeric.Website.Components.Slices.MYZ.SectorsGeneration.Queries;

public record Sector(SectorTitle Title, SectorRuin Ruin, SectorAtmosphere Atmosphere, SectorThreat Threat, SectorArtifact Artifact, SectorTrinket Trinket);

public record SectorTitle(string Environment, int ThreatLevel, int GangreneLevel, string GangreneColor);
public record SectorRuin(bool IsRuinInSector, string RuinType, string RuinName, string RuinDefinition);
public record SectorAtmosphere(bool IsAtmosphereInSector, string AtmosphereName, string AtmosphereDefinition);
public record SectorThreat(bool IsThreatInSector, string ThreatType, string ThreatName, int ThreatPage);
public record SectorArtifact(bool IsArtifactInSector, string ArtifactName, int ArtifactPage);
public record SectorTrinket(List<string> TrinketsNames);