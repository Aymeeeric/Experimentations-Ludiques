using MediatR;

namespace Aymeeeric.Website.Components.Slices.MYZ.SectorsGeneration.Queries;

public record GenerateSectorsQuery(GenerationMode GenerationMode, int NumberOfSectorsToGenerate, int ThreatLevel, string Seed = "Mutant: year Zero" ) : IRequest<List<Sector>>;

public class GenerateSectorsQueryHandler: IRequestHandler<GenerateSectorsQuery, List<Sector>>
{
    private Random _random;

    private List<Environnement> _tableEnvironnements;
    private List<Ruine> _tableRuines;
    private List<Ambiance> _tableAmbiances;
    private List<Menace> _tableMenacesHumanoides;
    private List<Menace> _tableMenacesMonstres;
    private List<Menace> _tableMenacesPhenomenes;
    private List<Artefact> _tableArtefacts;
    private List<Babiole> _tableBabioles;
    
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
        await LoadAllTables();

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

    private async Task LoadAllTables()
    {
        _tableEnvironnements = ConvertJsonFile<Environnement>("environnements.json");
        _tableRuines = ConvertJsonFile<Ruine>("environnements.json");
        _tableAmbiances = ConvertJsonFile<Ambiance>("environnements.json");
        _tableMenacesHumanoides = ConvertJsonFile<Menace>("environnements.json");
        _tableMenacesMonstres = ConvertJsonFile<Menace>("environnements.json");
        _tableMenacesPhenomenes = ConvertJsonFile<Menace>("environnements.json");
        _tableArtefacts = ConvertJsonFile<Artefact>("environnements.json");
        _tableBabioles = ConvertJsonFile<Babiole>("environnements.json");
    }

    private List<T> ConvertJsonFile<T>(string environnementsJson)
    {
        throw new NotImplementedException();
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

public record Sector();
public record Environnement();
public record Ruine();
public record Ambiance();
public record Menace();
public record Artefact();
public record Babiole();
