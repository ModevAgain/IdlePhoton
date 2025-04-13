
using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SimulationDisplay : MonoBehaviour
{
    public TMP_Text TMP_SimName;
    public TMP_Text TMP_SimValue;
    public TMP_Text TMP_PhotonCount;
    public TMP_Text TMP_PhotonRate;
    public TMP_Text TMP_PhotonEfficiency;

    public Image CompletionFill;
    public TMP_Text TMP_Completion;
    
    public List<GeneratorUpgradeUI> GeneratorUpgradeDisplays;

    public TMP_Text TMP_BuyType;
    public BuyType CurrentBuyType;

    private void Awake()
    {
        CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

        TMP_SimName.text = "Simulation - [EMTPY]";
        
        TMP_SimValue.text = "/";
        
        TMP_Completion.text = "/";
        CompletionFill.fillAmount = 0;
    }

    public void Init(Simulation sim)
    {
        TMP_SimName.text = $"Simulation - {sim.Name}";
        
        UpdateState(sim);

        foreach (var gen in GeneratorUpgradeDisplays)
        {
            gen.Init();
        }
    }

    public void UpdateState(Simulation sim)
    {
        TMP_SimValue.text = sim.OutputResource.ToHumanReadableString();

        TMP_PhotonCount.text = sim.CurrentState.Count.ToString();
        TMP_PhotonRate.text = sim.CurrentState.RatePerSecond.ToString("0.0");
        TMP_PhotonEfficiency.text = sim.CurrentState.Efficiency.ToString("0.0");
        
        TMP_Completion.text = $"{sim.SimulationCompletion:00.00} %";
        CompletionFill.fillAmount = sim.SimulationCompletion/100;
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
