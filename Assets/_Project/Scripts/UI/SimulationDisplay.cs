
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

    public GameObject ObjRunSimBtn;
    public GameObject ObjCompleteSimBtn;
    public Button BtnCompleteSim;
    public TMP_Text TMP_CompleteSimValue;
    
    public Image CompletionFill;
    public TMP_Text TMP_Completion;
    
    public List<GeneratorUpgradeUI> GeneratorUpgradeDisplays;

    public TMP_Text TMP_BuyType;
    public BuyType CurrentBuyType;

    private void Awake()
    {
        CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");

       ClearSimulation();
    }

    public void InitSimulation(Simulation sim)
    {
        TMP_SimName.text = $"Simulation - {sim.Name}";
        
        UpdateState(sim);

        foreach (var gen in GeneratorUpgradeDisplays)
        {
            gen.Init();
        }
    }

    private void ClearSimulation()
    {
        TMP_SimName.text = "Simulation - [EMTPY]";
        
        TMP_SimValue.text = "/";
        TMP_Completion.text = "/";

        TMP_PhotonCount.text = "/";
        TMP_PhotonRate.text = "/";
        TMP_PhotonEfficiency.text = "/";
        
        CompletionFill.fillAmount = 0;
        
        foreach (var gen in GeneratorUpgradeDisplays)
        {
            gen.Reset();
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

    public void ShowCompleteSimBtn(float value, Resource resource, Action onComplete)
    {
        CompletionFill.fillAmount = 1;
        TMP_Completion.text = $"{100:00.00} %";
        
        ObjCompleteSimBtn.SetActive(true);
        TMP_CompleteSimValue.text = $"+{value:0.0} {resource.ResourceName}";
        
        BtnCompleteSim.onClick.AddListener(() => OnClick_CompleteSimBtn(onComplete));
    }

    private void OnClick_CompleteSimBtn(Action onComplete)
    {
        ObjCompleteSimBtn.SetActive(false);
        BtnCompleteSim.onClick.RemoveAllListeners();
        onComplete?.Invoke();
        
        ClearSimulation();
        ObjRunSimBtn.SetActive(true);
    }
}
