
using System;

[System.Serializable]
public class Simulation : ITickable
{
    public string Name;
    public int Index;
    public SimulationState CurrentState;
    public Resource OutputResource;
    
    
    private PhotonManager _photonManager;
    private PhotonGenerator _photonGenerator;

    public Simulation(string name, int index, PhotonManager photonManager, PhotonGenerator photonGenerator, Resource resource)
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
        OutputResource.Add(count * CurrentState.Efficiency);
    }
}

[System.Serializable]
public class SimulationState
{
    public int Count = 0;
    public float RatePerSecond = 0;
    public float Efficiency = 1;
}