namespace Aymeeeric.Website.Framework.Extensions;

public static class RandomDiceExtension
{
    public static int RollD6(this Random random)
    {
        var units = random.Next(1, 6);
        return units;
    }
    
    public static int RollD66(this Random random)
    {
        var units = random.RollD6();
        var tens = random.RollD6();

        return int.Parse($"{tens}{units}");
    }
    
    public static int RollD666(this Random random)
    {
        var units = random.RollD6();
        var tens = random.RollD6();
        var hundreds = random.RollD6();
        
        return int.Parse($"{hundreds}{tens}{units}");
    }
}