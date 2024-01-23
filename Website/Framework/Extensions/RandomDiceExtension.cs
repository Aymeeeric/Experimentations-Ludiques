namespace Aymeeeric.Website.Framework.Extensions;

public static class RandomDiceExtension
{
    public static int RollD3(this Random random)
    {
        var units = random.Next(1, 4); // Upper is exclusive...
        return units;
    }
    
    public static int RollD6(this Random random)
    {
        var units = random.Next(1, 7); // Upper is exclusive...
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
    
    public static int RollD666(this Random random, int max)
    {
        if (max < 1 || max > 666)
            throw new Exception("Un dé 666 doit être compris entre 1 et 666.");
            
        var roll = random.RollD666();
        while (roll > max)
        {
            roll = random.RollD666();
        }

        return roll;
    }
}