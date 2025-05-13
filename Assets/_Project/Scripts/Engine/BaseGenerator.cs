
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class BaseGenerator : TickObject
{
    public List<BaseGeneratorUpgrade> Upgrades;
    
    public float GenerationRatePerTick;
    
    private float _generationValue;
    private bool _initialized;

    public virtual void Init()
    {
        Upgrades.First(u => u.UpgradeType == BaseGeneratorUpgradeType.FREQUENCY).Init(f => GenerationRatePerTick = f);
        Upgrades.First(u => u.UpgradeType == BaseGeneratorUpgradeType.COST).Init(f =>
        {
            foreach (var upgrade in Upgrades.Where(u => u.UpgradeType != BaseGeneratorUpgradeType.COST))
            {
                upgrade.CostMultiplier = f;
            }
        });
        
        _initialized = true;
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void Tick(uint index, float tickBalanceValue, float deltaTime)
    {
        if (!_initialized)
            return;
        
        _generationValue += GenerationRatePerTick;
        if (_generationValue >= 1)
        {
            var generationCount = Mathf.FloorToInt(_generationValue);
            Generate(generationCount);
            _generationValue -= generationCount;
        }
    }

    protected abstract void Generate(int amount);
}

[System.Serializable]
public class BaseGeneratorUpgrade
{
    public BaseGeneratorUpgradeType UpgradeType;
    public string Name;
    
    [Header("Count & Value")]
    public int UpgradeCount;
    public int MaxCount;
    public Vector2 ValueRange;
    public float CurrentValue;

    [Header("Cost")] 
    public Resource CostResource;
    public Vector2 CostRange;
    public float CostMultiplier = 1;

    private Action<float> _callback;
    
    public void Init(Action<float> callback)
    {
        _callback = callback;
        CurrentValue = Mathf.Lerp(ValueRange.x, ValueRange.y, (float)UpgradeCount / MaxCount);
        _callback?.Invoke(CurrentValue);
    }

    public void AddUpgrade(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CostResource.Remove(Mathf.Lerp(CostRange.x, CostRange.y, (float)(UpgradeCount) / MaxCount) * CostMultiplier);
            
            UpgradeCount++;
        }
        CurrentValue = Mathf.Lerp(ValueRange.x, ValueRange.y, (float)UpgradeCount / MaxCount);
        _callback?.Invoke(CurrentValue);
    }

    public float GetNextCost()
    {
        return GetUpgradeCost(UpgradeCount);
    }
    
    public (int upgradeCount, float totalCost) GetMaxAffordableUpgrades()
    {
        var left = 0;
        var right = MaxCount - UpgradeCount;
        var bestCount = 0;
        var bestCost = 0f;

        while (left <= right)
        {
            var mid = (left + right) / 2;
            var totalCost = GetTotalUpgradeCost(UpgradeCount, mid);

            if (totalCost <= CostResource.BaseValue)
            {
                bestCount = mid;
                bestCost = totalCost;
                left = mid + 1; // Try to get more upgrades
            }
            else
            {
                right = mid - 1; // Too expensive, reduce
            }
        }

        return (bestCount, bestCost);
    }

    private float GetTotalUpgradeCost(int startIndex, int count)
    {
        float total = 0f;
        for (int i = 0; i < count; i++)
        {
            total += GetUpgradeCost(startIndex + i);
        }
        return total;
    }

    private float GetUpgradeCost(int index)
    {
        return Mathf.Lerp(CostRange.x, CostRange.y, (float)(index + 1) / MaxCount) * CostMultiplier;

        // If you want exponential instead, replace above with something like:
        // return Mathf.Pow(1.1f, index) * BaseCost;
    }
}

public enum BaseGeneratorUpgradeType
{
    FREQUENCY,
    EFFICIENCY,
    COST
}
