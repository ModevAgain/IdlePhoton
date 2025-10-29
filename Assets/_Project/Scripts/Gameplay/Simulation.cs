
using System;
using UnityEngine;

[System.Serializable]
public class Simulation : ITickable
{
    public string Name;
    public int Index;
    public SimulationState CurrentState;
    public Resource OutputResource;

    public Bint SimulationValue = 0;
    public Bint SimulationTarget;
    public float SimulationCompletion = 0;
    public bool IsAccumulating;
    
    private PhotonManager _photonManager;
    private PhotonGenerator _photonGenerator;
    private float _completionDegradationPerTick;
    private Action _onSimulationFinished;

    public Simulation(
        string name, 
        int index, 
        PhotonManager photonManager, 
        PhotonGenerator photonGenerator, 
        GameplayModule[] modules,
        Resource resource, 
        float simulationTarget,
        float completionDegradationPerTick,
        Action onFinished)
    {
        Name = name;
        Index = index;
        CurrentState = new SimulationState();
        
        _photonManager = photonManager;
        _photonManager!.Render = true;
        _photonManager.PhotonsFinished += PhotonFinished;
        
        _photonGenerator = photonGenerator;
        _photonGenerator.Init();
        
        foreach (var module in modules)
        {
            module.OnModuleEnable();
        }
        
        resource.Reset();
        OutputResource = resource;
        
        SimulationTarget = simulationTarget;
        _completionDegradationPerTick = completionDegradationPerTick;
        _onSimulationFinished = onFinished;
        
        IdleEngine.Root.RegisterTickable(this);
    }

    public void Tick(uint index, float tickBalanceValue, float deltaTime)
    {
        _photonManager.Tick(deltaTime);

        CurrentState.Count = _photonManager.CurrentPhotonCount;
        CurrentState.RatePerSecond = _photonGenerator.GenerationRatePerTick;
        CurrentState.Efficiency = _photonGenerator.Efficiency;
    }

    private void PhotonFinished(int count)
    {
        var outputValue = count * CurrentState.Efficiency;
        OutputResource.Value += count * CurrentState.Efficiency;
        SimulationValue += MathBint.Max(0, outputValue - _completionDegradationPerTick);
        if (!IsAccumulating)
        {
            SimulationCompletion = SimulationValue.AsReverseFloat() / SimulationTarget.AsReverseFloat() * 100;
            if(SimulationCompletion >= 100)
                _onSimulationFinished?.Invoke();
        }
    }

    public void StopEvents()
    {
        _photonManager.PhotonsFinished -= PhotonFinished;
        _onSimulationFinished = null;
    }

    public void StopGeneration()
    {
        _photonGenerator.Stop();
    }
}

[Serializable]
public class SimulationState
{
    public Bint Count;
    public Bint RatePerSecond;
    public Bint Efficiency = 1;
}