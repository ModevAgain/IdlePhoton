using TMPro;
using UnityEngine;

public class SimLogger : GameplayModule
{
    public TMP_Text TMP_Text;

    public string CurrentLogText;
    
    private static SimLogger _instance;

    private void Awake()
    {
        _instance = this;
    }

    private void LogInternal(string log)
    {
        CurrentLogText += log;
        TMP_Text.text = CurrentLogText;
    }
    
    public static void Log(string log)
    {
        _instance.LogInternal("\n" + log);
    }

    public override void OnModuleEnable()
    {
        base.OnModuleEnable();
    }
}