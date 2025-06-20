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
    public static Bint CalculateTotalCost(float baseCost, float costCoefficient, Bint currentCount, Bint targetCount)
    {
        return -baseCost * ((MathBint.Pow(costCoefficient, currentCount) -  MathBint.Pow(costCoefficient, currentCount + targetCount)) / (1 - costCoefficient));
    }
    
    public static (Bint upgradeCount, Bint totalCost) GetMaxAffordableUpgrades(float baseCost, float costCoefficient,
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
