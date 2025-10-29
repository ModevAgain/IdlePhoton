using UnityEngine;

public class GameplayModule : MonoBehaviour
{
    public ModuleData Data;
    public bool ModuleIsEnabled;

    public virtual void OnModuleEnable()
    {
        ModuleIsEnabled = true;
    }

    public virtual void OnModuleReset()
    {
        
    }
}