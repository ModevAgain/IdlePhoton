using UnityEngine;

[CreateAssetMenu(fileName = "New ModuleData", menuName = "Data/ModuleData")]
public class ModuleData : ScriptableObject
{
    [Header("Settings")]
    public bool IsBaseModule;
    public bool EnableOnSimulationStart;
    public bool ResetOnSimulationEnd;
    
    [Header("Gameplay Data")]
    public bool Unlocked;
}