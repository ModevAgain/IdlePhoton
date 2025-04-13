
using System;
using UnityEngine;

public class SimulationController : TickObject
{
    public SimulationDisplay SimulationDisplay;
    
    public int SimulationIndex;
    public float[] DegradationValues;
    public Simulation CurrentSimulation;
    
    public PhotonManager PhotonManager;
    public PhotonGenerator PhotonGenerator;
    public Resource PhotonSimResource;

    public bool IsRunningSimulation;

    public void StartInitial()
    {
        StartSimulation("Photon", PhotonManager, PhotonGenerator, PhotonSimResource);
    }
    
    public void StartSimulation(string simName, PhotonManager photonManager, PhotonGenerator photonGenerator, Resource resource)
    {
        var sim = new Simulation(
            simName, 
            SimulationIndex, 
            photonManager, 
            photonGenerator, 
            resource, 
            DegradationValues[SimulationIndex],
            OnSimulationComplete);
        CurrentSimulation = sim;
        
        SimulationDisplay.Init(CurrentSimulation);

        IsRunningSimulation = true;
    }

    public override void Tick(uint index, float tickBalanceValue, float deltaTime)
    {
        if(IsRunningSimulation)
            SimulationDisplay.UpdateState(CurrentSimulation);
    }

    public void OnSimulationComplete()
    {
        Debug.Log("Sim completed!");
    }
}
