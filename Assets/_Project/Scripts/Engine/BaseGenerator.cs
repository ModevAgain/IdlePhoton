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

public enum BaseGeneratorUpgradeType
{
    FREQUENCY,
    EFFICIENCY
}
