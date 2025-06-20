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
        CurrentValue = EngineMath.CalculateGeneratorValue(BaseValue, ValueCoefficient, UpgradeCount);
        _onUpgrade?.Invoke(CurrentValue);
    }

    public void AddUpgrade(Bint amount, Bint cost)
    {
        CostResource.Value -= cost;
        UpgradeCount += amount;
        CurrentValue = EngineMath.CalculateGeneratorValue(BaseValue, ValueCoefficient, UpgradeCount);
        _onUpgrade?.Invoke(CurrentValue);
    }

    public Bint GetNextCost()
    {
        return GetUpgradeCost(UpgradeCount);
    }
    
    public (Bint upgradeCount, Bint totalCost) GetMaxAffordableUpgrades()
    {
        return EngineMath.GetMaxAffordableUpgrades(BaseCost, CostCoefficient, UpgradeCount, CostResource.Value);
    }

    private Bint GetTotalUpgradeCost(Bint startIndex, Bint count)
    {
        return EngineMath.CalculateTotalCost(BaseCost, CostCoefficient, startIndex, count);
    }

    private Bint GetUpgradeCost(Bint index)
    {
        return EngineMath.CalculateCost(BaseCost, CostCoefficient, index);
    }
}