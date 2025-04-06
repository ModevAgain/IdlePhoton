
using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class SimulationDisplay : MonoBehaviour
{
    public TMP_Text TMP_SimName;
    public TMP_Text TMP_SimValue;
    public TMP_Text TMP_PhotonCount;
    public TMP_Text TMP_PhotonRate;
    public TMP_Text TMP_PhotonEfficiency;
    
    public List<GeneratorUpgradeUI> GeneratorUpgradeDisplays;

    public TMP_Text TMP_BuyType;
    public BuyType CurrentBuyType;

    private void Awake()
    {
        CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
    }

    public void Init(Simulation sim)
    {
        TMP_SimName.text = $"Simulation - {sim.Name}";
        UpdateState(sim);
    }
    
    public void UpdateState(Simulation sim)
    {
        TMP_SimValue.text = sim.OutputResource.ToHumanReadableString();
        
        TMP_PhotonCount.text = sim.CurrentState.Count.ToString();
        TMP_PhotonRate.text = sim.CurrentState.RatePerSecond.ToString("0.0");
        TMP_PhotonEfficiency.text = sim.CurrentState.Efficiency.ToString("0.0");
    }

    public void SetTriggerBuyAmount()
    {
        CurrentBuyType++;
        if(!Enum.IsDefined(typeof(BuyType), CurrentBuyType))
            CurrentBuyType = 0;
        
        TMP_BuyType.text = CurrentBuyType.ToString().ToLower();
        
        foreach (var display in GeneratorUpgradeDisplays)
        {
            display.CurrentBuyType = CurrentBuyType;
        }
    }
}
