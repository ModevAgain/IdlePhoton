using System;
using UnityEngine;

[Serializable]
public class BaseGeneratorUpgrade
{
    public BaseGeneratorUpgradeType UpgradeType;
    public string Name;
    
    [Header("Count & Value")]
    public Bint UpgradeCount;
    public Bint CurrentValue;
    [Space]
    public float BaseValue;
    public float ValueCoefficient;

    [Header("Cost")] 
    public Resource CostResource;
    public float BaseCost;
    public float CostCoefficient = 1;

    private Action<Bint> _onUpgrade;
    private Func<float> _getResourceValue;
    
    public void Init(Action<Bint> onUpgradeCallback)
    {
        UpgradeCount = 0;
        _onUpgrade = onUpgradeCallback;
        CurrentValue = BaseValue + ValueCoefficient * UpgradeCount;
        _onUpgrade?.Invoke(CurrentValue);
    }

    public void AddUpgrade(Bint amount, Bint cost)
    {
        CostResource.Value -= cost;
        UpgradeCount += amount;
        CurrentValue = BaseValue + ValueCoefficient * UpgradeCount;
        _onUpgrade?.Invoke(CurrentValue);
    }

    public Bint GetNextCost()
    {
        return GetUpgradeCost(UpgradeCount);
    }
    
    public (Bint upgradeCount, Bint totalCost) GetMaxAffordableUpgrades()
    {
        var innerLogA = CostResource.Value * (CostCoefficient - 1) / (BaseCost * MathBint.Pow(CostCoefficient, UpgradeCount)) + 1;
        Bint innerLogB = CostCoefficient;
        var log = MathBint.Log(innerLogA) / MathBint.Log(innerLogB);
        
        var max = MathBint.Floor(log);

        var cost = GetTotalUpgradeCost(UpgradeCount, max);

        return (max, cost);
    }

    private Bint GetTotalUpgradeCost(Bint startIndex, Bint count)
    {
        return BaseCost * (MathBint.Pow(CostCoefficient, startIndex) * (MathBint.Pow(CostCoefficient, count) - 1)) / (CostCoefficient - 1);
    }

    private Bint GetUpgradeCost(Bint index)
    {
        return BaseCost * MathBint.Pow(CostCoefficient, index);
    }
}