
using System;
using UnityEngine;

[System.Serializable]
public class Simulation : ITickable
{
    public string Name;
    public int Index;
    public SimulationState CurrentState;
    public Resource OutputResource;

    public float SimulationValue = 0;
    public float SimulationTarget = 0;
    public float SimulationCompletion = 0;
    private PhotonManager _photonManager;
    private PhotonGenerator _photonGenerator;
    private float _completionDegradationPerTick;
    private Action _onSimulationFinished;

    public Simulation(
        string name, 
        int index, 
        PhotonManager photonManager, 
        PhotonGenerator photonGenerator, 
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
        SimulationValue += outputValue - _completionDegradationPerTick;
        SimulationCompletion = (SimulationValue / SimulationTarget) * 100;
        if(SimulationCompletion >= 100)
            _onSimulationFinished?.Invoke();
    }

    public void Stop()
    {
        _photonManager.PhotonsFinished -= PhotonFinished;
        _photonGenerator.Stop();
        _onSimulationFinished = null;
    }
}

[Serializable]
public class SimulationState
{
    public int Count;
    public float RatePerSecond;
    public float Efficiency = 1;
}