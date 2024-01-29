using System.Text.Json;
using Aymeeeric.Website.Framework.Extensions;
using MediatR;

namespace Aymeeeric.Website.Components.Slices.MYZ.SectorsGeneration.Queries;

public record GenerateSectorsQuery(GenerationMode GenerationMode, int NumberOfSectorsToGenerate, int ThreatLevel, string Seed) : IRequest<List<Sector>>;

public class GenerateSectorsQueryHandler: IRequestHandler<GenerateSectorsQuery, List<Sector>>
{
    private readonly IWebHostEnvironment _env;
    
    public GenerateSectorsQueryHandler(IWebHostEnvironment env)
    {
        _env = env;
    }
    
    private Random _random;

    private List<Environment> _tableEnvironments;
    private List<Ruin> _tableNormalRuins;
    private List<Ruin> _tableIndustrialRuins;
    private List<GangreneLevel> _tableGangreneLevel;
    private List<Atmosphere> _tableAtmospheres;
    private List<Threat> _tableHumanoidThreat;
    private List<Threat> _tableMonsterThreat;
    private List<Threat> _tablePhenomenonThreat;
    private List<Artifact> _tableArtifacts;
    private List<Trinket> _tableTrinkets;
    
    public async Task<List<Sector>> Handle(GenerateSectorsQuery query, CancellationToken cancellationToken)
    {
        var seed = query.Seed;
        var numberOfSectorsToGenerate = query.NumberOfSectorsToGenerate;
        var threatLevel = query.ThreatLevel;
        var generationMode = query.GenerationMode; // Non implémentée.
        
        if(numberOfSectorsToGenerate < 1 ||numberOfSectorsToGenerate > 12)
            throw new Exception("Le nombre de secteurs à générer doit être compris entre 1 et 12.");
        if(threatLevel < 0 ||threatLevel > 12)
            throw new Exception("Le niveau de menace doit être compris entre 1 et 12.");

        SeedRandomisation(seed);
        LoadAllTables();

        List<Sector> results = [];
        for (var i = 0; i < numberOfSectorsToGenerate; i++)
        {
            var sector = GenerateSector(threatLevel);
            results.Add(sector);
        }

        return results;
    }
    
    private Sector MapToModel(int threatLevel, Environment environment, GangreneLevel gangreneLevel, Ruin? ruin, Threat? threat, Artifact? artifact, Atmosphere? atmosphere, List<Trinket> trinkets)
    {
        var sectorTitle = new SectorTitle(
            environment.EnvironmentName,
            threatLevel,
            gangreneLevel.Level,
            GetGangreneColor(gangreneLevel.Level)
        );
        
        var sectorRuin = new SectorRuin(
            ruin is not null,
            ruin is not null ? ruin!.RuinType : string.Empty,
            ruin is not null ? ruin!.RuinName : string.Empty,
            ruin is not null ? ruin!.RuinDefinition : string.Empty
        );

        var sectorAtmosphere = new SectorAtmosphere(
            atmosphere is not null,
            atmosphere is not null ? atmosphere.AtmosphereName : string.Empty,
            atmosphere is not null ? atmosphere.AtmosphereDefinition : string.Empty
        );
        
        var sectorThreat = new SectorThreat(
            threat is not null,
            threat is not null ? threat.ThreatType : string.Empty,
            threat is not null ? threat.ThreatName : string.Empty,
            threat?.ThreatPage
        );

        var sectorArtifact = new SectorArtifact(
            artifact is not null,
            artifact is not null ? artifact.ArtifactName : string.Empty,
            artifact?.ArtifactPage
        );

        var sectorTrinket = new SectorTrinket(
            trinkets.Select( t => t.TrinketName).ToList()
        );

        return new Sector(
            sectorTitle,
            sectorRuin,
            sectorAtmosphere,
            sectorThreat,
            sectorArtifact,
            sectorTrinket
        );
    }
    
    #region Génération 
    
    private Sector GenerateSector(int threatLevel)
    {
        // 1. L’environnement
        var  environment = RollEnvironment(); // TODO - Cas particulier des Colonie ?
        
        // 2. Ruines
        Ruin? ruin = null;
        if (environment.IsRuinInEnvironment)
            ruin = RollRuin();
        
        // 3. Niveau de Gangrène
        var gangreneLevel = RollGangreneLevel();
        
        // 4. Niveau de Menace
        // param.
        
        // 5. Menaces dans la Zone (et présence d'artefact)
        Threat? threat = null;
        var isArtifactAuthorized = false;
        if(environment.IsThreatInEnvironment)
            (threat, isArtifactAuthorized) = RollThreatAndCheckIfArtifactIsAuthorized(threatLevel);

        // 6. Artefact
        Artifact? artifact = null;
        if (environment.IsArtifactInEnvironment && isArtifactAuthorized)
            artifact = RollArtifact();
        
        // Bonus. Éléments d’ambiance
        var atmosphere = RollAtmosphere();
        
        // Bonus. Babioles
        var trinkets = RollTrinkets();

        return MapToModel(threatLevel, environment, gangreneLevel, ruin, threat, artifact, atmosphere, trinkets);
    }

    private Environment RollEnvironment()
    {
        var roll = _random.RollD66();
        return _tableEnvironments.Single(env =>
            env.D66Min <= roll &&
            env.D66Max >= roll);
    }    
    
    private Ruin RollRuin()
    {
        var roll = _random.RollD66();
        var rollForRuinChoice = _random.RollD6();
        
        if(rollForRuinChoice <= 4) // Arbitraire, mais deux tiers de ruines 'normales'
            return _tableNormalRuins.Single(env =>
                env.D66Min <= roll &&
                env.D66Max >= roll);
        else // Un tiers de ruines industrielles 
            return _tableIndustrialRuins.Single(env =>
                env.D66Min <= roll &&
                env.D66Max >= roll);
    }
    
    private GangreneLevel RollGangreneLevel()
    {
        var roll = _random.RollD66();
        return _tableGangreneLevel.Single(env =>
            env.D66Min <= roll &&
            env.D66Max >= roll);
    }
    
    private (Threat? threat, bool artifactAuthorized) RollThreatAndCheckIfArtifactIsAuthorized(int threatLevel)
    {
        var isThreat = false;
        var isArtifactAuthorized = false;
        for (var i = 0; i < threatLevel; i++)
        {
            var rollD6 = _random.RollD6();
            if (rollD6 == 1)
                isThreat = true;
            if(rollD6 == 6)
                isArtifactAuthorized = true;
        }

        Threat? threat = null;
        if (isThreat)
        {
            var roll = _random.RollD66();
            var rollForThreatType = _random.RollD6();

            switch (rollForThreatType)
            {
                case >= 1 and <= 2:
                    threat = _tableHumanoidThreat.Single(thr =>
                        thr.D66Min <= roll &&
                        thr.D66Max >= roll);
                    break;
                    
                case <= 5:
                    threat = _tableMonsterThreat.Single(thr =>
                        thr.D66Min <= roll &&
                        thr.D66Max >= roll);
                    break;
                    
                case <= 6:
                    threat = _tablePhenomenonThreat.Single(thr =>
                        thr.D66Min <= roll &&
                        thr.D66Max >= roll);
                    break;
                
                default:
                    throw new Exception("Le dé pour le type de menace doit être compris entre 1 et 6.");
            }
        }

        return (threat, isArtifactAuthorized);
    }
    
    private Artifact RollArtifact()
    {
        var roll = _random.RollD666(642);
        return _tableArtifacts.Single(env =>
            env.D666Min <= roll &&
            env.D666Max >= roll);
    }
    
    private Atmosphere? RollAtmosphere()
    {
        var roll = _random.RollD66();
        var rollForPresence = _random.RollD6();
        
        if(rollForPresence <= 3) // Arbitraire, mais la moitié avec ambiance
            return _tableAtmospheres.Single(env =>
                env.D66Min <= roll &&
                env.D66Max >= roll);

        return null;
    }
    
    private List<Trinket> RollTrinkets()
    {
        var trinkets = new List<Trinket>();
        for (var i = 0; i <= 2; i++)
        {
            var roll = _random.RollD666();
            trinkets.Add(
            _tableTrinkets.Single(env =>
                    env.D666Min <= roll &&
                    env.D666Max >= roll)
            );
        }

        return trinkets;
    }
    
    private string GetGangreneColor(int level)
    {
        switch (level)
        {
            case 0 :
                return "yellow";
            case 1 :
                return "orange";
            case 2 :
                return "red";
            
            default:
                throw new Exception("Le niveau de gangrène doit être compris entre 0 et 2.");
        }
    }
    
    #endregion
    
    #region Outils
    
    private void SeedRandomisation(string seed)
    {
        var intSeed = seed.GetHashCode();
        _random = new Random(intSeed);
    }

    private void LoadAllTables()
    {
        _tableArtifacts = ConvertJsonFile<Artifact>("Data/artifacts.json");
        _tableAtmospheres = ConvertJsonFile<Atmosphere>("Data/atmospheres.json");
        _tableEnvironments = ConvertJsonFile<Environment>("Data/environments.json");
        _tableGangreneLevel = ConvertJsonFile<GangreneLevel>("Data/gangrene_level.json");
        _tableIndustrialRuins = ConvertJsonFile<Ruin>("Data/ruins_industrial.json");
        _tableNormalRuins = ConvertJsonFile<Ruin>("Data/ruins_normal.json");
        _tableHumanoidThreat = ConvertJsonFile<Threat>("Data/threats_humanoid.json");
        _tableMonsterThreat = ConvertJsonFile<Threat>("Data/threats_monster.json");
        _tablePhenomenonThreat = ConvertJsonFile<Threat>("Data/threats_phenomenon.json");
        _tableTrinkets = ConvertJsonFile<Trinket>("Data/trinkets.json");
    }

    private List<T> ConvertJsonFile<T>(string jsonFile)
    {
        var fullJsonPath = Path.Combine(_env.WebRootPath, jsonFile);
        var jsonContent = File.ReadAllText(fullJsonPath);
        
        return JsonSerializer.Deserialize<List<T>>(jsonContent, _jsonSerializerOptions)!;
    }
    
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    
    #endregion
}

public enum GenerationMode {
    Perlin,
    Random
}

public record Environment(int D66Min, int D66Max, string EnvironmentName, bool IsRuinInEnvironment, bool IsThreatInEnvironment, bool IsArtifactInEnvironment, bool isColony);
public record Ruin(int D66Min, int D66Max, string RuinType, string RuinName, string RuinDefinition);
public record GangreneLevel(int D66Min, int D66Max, int Level, string LevelTitle, string LevelDescription);
public record Atmosphere(int D66Min, int D66Max, string AtmosphereName, string AtmosphereDefinition);
public record Threat(int D66Min, int D66Max, string ThreatType, string ThreatName, int ThreatPage);
public record Artifact(int D666Min, int D666Max, string ArtifactName, int ArtifactPage);
public record Trinket(int D666Min, int D666Max, string TrinketName);