public class SimulationController : TickObject
{
    public SimulationDisplay SimulationDisplay;
    
    public int SimulationIndex;
    public Simulation CurrentSimulation;
    
    public PhotonManager PhotonManager;
    public PhotonGenerator PhotonGenerator;
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
            resource, 
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
        IsRunningSimulation = false;
        CurrentSimulation.Stop();
        var valueToAdd = 1;
        
        SimulationDisplay.ShowCompleteSimBtn(valueToAdd, SimulationCompleteResource, () => ConfirmSimCompletion(valueToAdd));
        
        SimLogger.Log("Simulation data at 100%.");
    }

    private void ConfirmSimCompletion(float value)
    {
        SimulationCompleteResource.Add(value);
        SimLogger.Log($"Simulation completed. Added [+{value}] {SimulationCompleteResource.ResourceName}.");
    }

    private float GetDegradationValue(int index) => 3;
}
