using System.Linq;
using UnityEngine;

public class SimulationController : TickObject
{
    public SimulationDisplay SimulationDisplay;
    
    public int SimulationIndex;
    public Simulation CurrentSimulation;
    public float SimulationTargetValue;
    
    public PhotonManager PhotonManager;
    public PhotonGenerator PhotonGenerator;
    [Space]
    public GameplayModule[] Modules;
    [Space]
    public Resource PhotonSimResource;
    public Resource SimulationCompleteResource;

    public bool IsRunningSimulation;

    public void StartInitial()
    {
        StartSimulation("Photon", PhotonManager, PhotonGenerator, PhotonSimResource);
    }
    
    public void StartSimulation(string simName, PhotonManager photonManager, PhotonGenerator photonGenerator, Resource resource)
    {
        SimulationIndex++;
        
        CurrentSimulation = new Simulation(
            simName,
            SimulationIndex,
            photonManager,
            photonGenerator,
            GetStartingModules(),
            resource,
            GetSimulationTarget(SimulationIndex),
            GetDegradationValue(SimulationIndex),
            OnSimulationFinished);
        
        SimLogger.Log($"Start Simulation [{simName}-{SimulationIndex}]");
        
        SimulationDisplay.InitSimulation(CurrentSimulation);

        IsRunningSimulation = true;
    }

    public override void Tick(uint index, float tickBalanceValue, float deltaTime)
    {
        if(IsRunningSimulation)
            SimulationDisplay.UpdateState(CurrentSimulation);
    }

    private void OnSimulationFinished()
    {
        var valueToAdd = 1;
        
        CurrentSimulation.StopEvents();
        SimulationDisplay.ShowCompleteSimBtn(valueToAdd, SimulationCompleteResource, () => ConfirmSimulationCompletion(valueToAdd));
        
        SimLogger.Log("Simulation data at 100%.");
    }

    private void ConfirmSimulationCompletion(float value)
    {
        IsRunningSimulation = false;
        
        CurrentSimulation.StopGeneration();

        foreach (var module in Modules.Where(m => m.ModuleIsEnabled && m.Data.ResetOnSimulationEnd))
        {
            module.OnModuleReset();
        }
        
        SimulationCompleteResource.Value += value;
        SimLogger.Log($"Simulation completed. Added [+{value}] {SimulationCompleteResource.ResourceName}.");
    }

    private float GetSimulationTarget(int index) => SimulationTargetValue;
    private float GetDegradationValue(int index) => 3;

    private GameplayModule[] GetStartingModules() =>
        Modules.Where(m => (m.Data.IsBaseModule || m.Data.Unlocked) && m.Data.EnableOnSimulationStart).ToArray();
}
