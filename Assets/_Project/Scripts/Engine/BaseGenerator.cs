using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseGenerator : TickObject
{
    public List<BaseGeneratorUpgrade> Upgrades;
    
    public Bint GenerationRatePerTick;
    
    private Bint _generationValue;
    private bool _initialized;
    private bool _active;

    public virtual void Init()
    {
        Upgrades.First(u => u.UpgradeType == BaseGeneratorUpgradeType.FREQUENCY).Init(f => GenerationRatePerTick = f);
        
        _initialized = true;
        _active = true;
    }

    public override void Tick(uint index, float tickBalanceValue, float deltaTime)
    {
        if (!_initialized || !_active)
            return;
        
        _generationValue += GenerationRatePerTick;
        if (_generationValue >= 1)
        {
            var generationCount = MathBint.Floor(_generationValue);
            Generate(generationCount);
            _generationValue -= generationCount;
        }
    }

    public void Stop()
    {
        _active = false;
    }

    protected abstract void Generate(Bint amount);
}

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

    private Action<Bint> _callback;
    
    public void Init(Action<Bint> callback)
    {
        UpgradeCount = 0;
        _callback = callback;
        CurrentValue = BaseValue + ValueCoefficient * UpgradeCount;
        _callback?.Invoke(CurrentValue);
    }

    public void AddUpgrade(Bint amount, Bint cost)
    {
        CostResource.Value -= cost;
        UpgradeCount += amount;
        CurrentValue = BaseValue + ValueCoefficient * UpgradeCount;
        _callback?.Invoke(CurrentValue);
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

public enum BaseGeneratorUpgradeType
{
    FREQUENCY,
    EFFICIENCY
}
