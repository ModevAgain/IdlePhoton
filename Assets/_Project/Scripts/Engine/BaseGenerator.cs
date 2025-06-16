using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseGenerator : TickObject
{
    public List<BaseGeneratorUpgrade> Upgrades;
    
    public float GenerationRatePerTick;
    
    private float _generationValue;
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
            var generationCount = Mathf.FloorToInt(_generationValue);
            Generate(generationCount);
            _generationValue -= generationCount;
        }
    }

    public void Stop()
    {
        _active = false;
    }

    protected abstract void Generate(int amount);
}

[Serializable]
public class BaseGeneratorUpgrade
{
    public BaseGeneratorUpgradeType UpgradeType;
    public string Name;
    
    [Header("Count & Value")]
    public int UpgradeCount;
    public float CurrentValue;
    [Space]
    public float BaseValue;
    public float ValueCoefficient;

    [Header("Cost")] 
    public Resource CostResource;
    public float BaseCost;
    public float CostCoefficient = 1;

    private Action<float> _callback;
    
    public void Init(Action<float> callback)
    {
        UpgradeCount = 0;
        _callback = callback;
        CurrentValue = BaseValue + ValueCoefficient * UpgradeCount;
        _callback?.Invoke(CurrentValue);
    }

    public void AddUpgrade(int amount, Bint cost)
    {
        CostResource.Value -= cost;
        UpgradeCount += amount;
        CurrentValue = CurrentValue = BaseValue + ValueCoefficient * UpgradeCount;
        _callback?.Invoke(CurrentValue);
    }

    public Bint GetNextCost()
    {
        return GetUpgradeCost(UpgradeCount);
    }
    
    public (int upgradeCount, Bint totalCost) GetMaxAffordableUpgrades()
    {
        var innerLog = (CostResource.Value * CostCoefficient - 1) / (BaseCost * Mathf.Pow(CostCoefficient, UpgradeCount)) + 1;
        var max = (int)Math.Floor(BintExtensions.Log(innerLog, CostCoefficient));

        var cost = GetTotalUpgradeCost(UpgradeCount, max);

        return (max, cost);
    }

    private Bint GetTotalUpgradeCost(int startIndex, int count)
    {
        return BaseCost * (Mathf.Pow(CostCoefficient, startIndex) * Mathf.Pow(CostCoefficient, count) - 1) / (CostCoefficient - 1);
    }

    private Bint GetUpgradeCost(int index)
    {
        return BaseCost * Mathf.Pow(CostCoefficient, index);
    }
}

public enum BaseGeneratorUpgradeType
{
    FREQUENCY,
    EFFICIENCY,
    COST
}
