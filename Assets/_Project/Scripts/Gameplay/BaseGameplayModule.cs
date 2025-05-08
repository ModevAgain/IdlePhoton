using UnityEngine;

public class BaseGameplayModule : MonoBehaviour
{
    public bool ModuleIsEnabled;

    public virtual void OnModuleEnable()
    {
        ModuleIsEnabled = true;
    }
}