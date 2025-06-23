public static class EngineMath
{
    public static Bint CalculateValue(float baseValue, float valueCoefficient, Bint currentCount)
    {
        return baseValue + valueCoefficient * currentCount;
    }
    
    public static Bint CalculateCostForNextUpgrade(float baseCost, float costCoefficient, Bint currentCount)
    {
        return baseCost * MathBint.Pow(costCoefficient, currentCount);
    }
        //return -baseCost * ((MathBint.Pow(costCoefficient, currentCount) -  MathBint.Pow(costCoefficient, currentCount + targetCount)) / (1 - costCoefficient));
    public static Bint CalculateTotalCost(float baseCost, float costCoefficient, Bint currentCount, Bint targetCount)
    {
        var powA = MathBint.Pow(costCoefficient, currentCount);
        var powB = MathBint.Pow(costCoefficient, targetCount);
        var divisionUpper = powA * (powB - 1);
        var divisionLower = costCoefficient - 1;
        return baseCost * (divisionUpper / divisionLower);
    }
    
    public static (Bint upgradeCount, Bint totalCost) CalculateMaxAffordableUpgrades(float baseCost, float costCoefficient,
        Bint currentCount, Bint resourceValue)
    {
        var innerLogA = resourceValue * (costCoefficient - 1) / (baseCost * MathBint.Pow(costCoefficient, currentCount)) + 1;
        Bint innerLogB = costCoefficient;
        var log = MathBint.Log(innerLogA) / MathBint.Log(innerLogB);
        
        var max = MathBint.Floor(log);

        var cost = CalculateTotalCost(baseCost, costCoefficient, currentCount, max);

        return (max, cost);
    }
}
