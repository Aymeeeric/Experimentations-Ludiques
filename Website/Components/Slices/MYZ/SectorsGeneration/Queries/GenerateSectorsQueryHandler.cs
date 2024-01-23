using System.Text.Json;
using MediatR;

namespace Aymeeeric.Website.Components.Slices.MYZ.SectorsGeneration.Queries;

public record GenerateSectorsQuery(GenerationMode GenerationMode, int NumberOfSectorsToGenerate, int ThreatLevel, string Seed = "Mutant: year Zero" ) : IRequest<List<Sector>>;

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
    private List<Atmosphere> _tableAtmospheres;
    private List<Threat> _tableHumanoidThreat;
    private List<Threat> _tableMonsterThreat;
    private List<Threat> _tablePhenomenonThreat;
    private List<Artifact> _tableArtifacts;
    private List<Trinklet> _tableTrinklets;
    
    public async Task<List<Sector>> Handle(GenerateSectorsQuery query, CancellationToken cancellationToken)
    {
        var seed = query.Seed;
        var numberOfSectorsToGenerate = query.NumberOfSectorsToGenerate;
        var threatLevel = query.ThreatLevel;
        var generationMode = query.GenerationMode; // Non implémentée.
        
        if(numberOfSectorsToGenerate < 1 ||numberOfSectorsToGenerate > 10)
            throw new Exception("Le nombre de secteurs à générer doit être compris entre 1 et 10.");
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
    
    private void SeedRandomisation(string seed)
    {
        var intSeed = seed.GetHashCode();
        _random = new Random(intSeed);
    }

    private void LoadAllTables()
    {
        _tableArtifacts = ConvertJsonFile<Artifact>("Data\\artifacts.json");
        _tableAtmospheres = ConvertJsonFile<Atmosphere>("Data\\atmospheres.json");
        _tableEnvironments = ConvertJsonFile<Environment>("Data\\environments.json");
        _tableIndustrialRuins = ConvertJsonFile<Ruin>("Data\\industrial_ruins.json");
        _tableNormalRuins = ConvertJsonFile<Ruin>("Data\\normal_ruins.json");
        
        _tableHumanoidThreat = ConvertJsonFile<Threat>("Data\\humanoid_threat.json");
        _tableMonsterThreat = ConvertJsonFile<Threat>("Data\\monster_threat.json");
        _tablePhenomenonThreat = ConvertJsonFile<Threat>("Data\\phenomenon_threat.json");
        
        _tableTrinklets = ConvertJsonFile<Trinklet>("Data\\trinklets.json");
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
    
    private string GetGangreneColor(int level)
    {
        switch (level)
        {
            case 1 :
                return "yellow";
            case 2 :
                return "orange";
            case 3 :
                return "red";
            
            default:
                throw new Exception("Le niveau de gangrène doit être compris entre 1 et 3.");
        }
    }


    private Sector GenerateSector(int threatLevel)
    {
        throw new NotImplementedException();
    }
}

public enum GenerationMode {
    Perlin,
    Random
}

public record Environment(int D66Min, int D66Max, string EnvironmentName, bool IsRuinInEnvironment, bool IsThreatInEnvironment, bool IsArtifactInEnvironment);
public record Ruin(int D66Min, int D66Max, string RuinName, string RuinDefinition);
public record Atmosphere(int D66Min, int D66Max, string AtmosphereName, string AtmosphereDefinition);
public record Threat();
public record Artifact(int D666Min, int D666Max, string ArtifactName, string ArtifactPage);
public record Trinklet(int D666Min, int D666Max, string TrinkletName);