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
        return GetMockedSectors();
        
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

    private List<Sector> GetMockedSectors()
    {
        var title = new SectorTitle("Ruines croulantes", 3, 1, GetGangreneColor(1));
        var ruin = new SectorRuin(true, "normale", "Station de radio",
            "une forêt d’antennes rouillées et de disques de métal surmonte le toit de ce vieux bâtiment. Un panneau sur la façade représente une antenne ceinte de quelques anneaux.");
        var atmosphere = new SectorAtmosphere(true, "Message",
            "quelqu’un a écrit quelque chose sur un mur. Quel est le message et est-il récent ?");
        var threat = new SectorThreat(true, "Phénomène", "champ d'inertie", 184);
        var artifact = new SectorArtifact(true, "Flacon de parfum", 193);
        var trinket = new SectorTrinket(["Peigne", "Bible"]);

        var sector = new Sector(title, ruin, atmosphere, threat, artifact, trinket);
        return [sector];
    }

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

    private void SeedRandomisation(string seed)
    {
        var intSeed = seed.GetHashCode();
        _random = new Random(intSeed);
    }

    private async Task LoadAllTables()
    {
        _tableEnvironnements = ConvertJsonFile<Environnement>("environnements.json");
        _tableRuines = ConvertJsonFile<Ruine>("ruines.json");
        _tableAmbiances = ConvertJsonFile<Ambiance>("ambiances.json");
        _tableMenacesHumanoides = ConvertJsonFile<Menace>("menaceHumanoides.json");
        _tableMenacesMonstres = ConvertJsonFile<Menace>("menacesMonstres.json");
        _tableMenacesPhenomenes = ConvertJsonFile<Menace>("menacesPhenomenes.json");
        _tableArtefacts = ConvertJsonFile<Artefact>("artefacts.json");
        _tableBabioles = ConvertJsonFile<Babiole>("babioles.json");
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

public record Environnement();
public record Ruine();
public record Ambiance();
public record Menace();
public record Artefact();
public record Babiole();