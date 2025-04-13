
using System;
using UnityEngine;

[System.Serializable]
public class Simulation : ITickable
{
    public string Name;
    public int Index;
    public SimulationState CurrentState;
    public Resource OutputResource;

    public float SimulationCompletion = 0;
    private PhotonManager _photonManager;
    private PhotonGenerator _photonGenerator;
    private float _completionDegradationPerTick;
    private Action _onSimulationComplete;

    public Simulation(
        string name, 
        int index, 
        PhotonManager photonManager, 
        PhotonGenerator photonGenerator, 
        Resource resource, 
        float completionDegradationPerTick,
        Action onComplete)
    {
        Name = name;
        Index = index;
        CurrentState = new SimulationState();
        
        _photonManager = photonManager;
        _photonManager!.Render = true;
        _photonManager.PhotonsFinished += PhotonFinished;
        
        _photonGenerator = photonGenerator;
        _photonGenerator.Init();
        
        OutputResource = resource;
        
        _completionDegradationPerTick = completionDegradationPerTick;
        _onSimulationComplete = onComplete;
        
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
        OutputResource.Add(count * CurrentState.Efficiency);
        SimulationCompletion += outputValue - _completionDegradationPerTick;
        SimulationCompletion = Mathf.Clamp(SimulationCompletion, 0, 100);
        if(SimulationCompletion >= 100)
            _onSimulationComplete?.Invoke();
    }
}

[System.Serializable]
public class SimulationState
{
    public int Count = 0;
    public float RatePerSecond = 0;
    public float Efficiency = 1;
}