using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GeneratorUpgradeUI : TickObject
{
    public BaseGeneratorUpgradeType UpgradeType;
    public BaseGenerator Generator;

    public BuyType CurrentBuyType;
    
    public TMP_Text TMPUpgradeLabel;
    public TMP_Text TMPUpgradeValues;
    
    public Button BtnUpgrade;

    public Image BuyableOverlay;
    
    private BaseGeneratorUpgrade _upgrade;
    private bool _active;

    protected override void Start()
    {
        _upgrade = Generator.Upgrades.First(u => u.UpgradeType == UpgradeType)!;
        
        BtnUpgrade.onClick.AddListener(OnClickUpgrade);
        
        Reset();
        base.Start();
    }

    public void Init()
    {
        TMPUpgradeLabel.text = $"{_upgrade.Name} ( {_upgrade.UpgradeCount.ToScientificString()} )";
        _active = true;
    }

    public void Reset()
    {
        _active = false;
        
        TMPUpgradeLabel.text = "/";
        TMPUpgradeValues.text = "/";

        BtnUpgrade.interactable = false;
        BuyableOverlay.fillAmount = 1;
    }

    public override void Tick(uint index, float tickBalanceValue, float deltaTime)
    {
        if (!_active)
            return;
        
        UpdateButtonDisplay();
    }

    private void UpdateButtonDisplay()
    {
        var (amount, cost) = GetCostInfo();
        BtnUpgrade.interactable = amount > 0 && _upgrade.CostResource.Value > cost;
        BuyableOverlay.fillAmount = amount > 0 ? 1 - Mathf.Clamp01((_upgrade.CostResource.Value / cost).AsFloat()) : 1;
        TMPUpgradeValues.text = $"{cost.ToScientificString()}\n+{amount.ToScientificString()}";
    }

    private void OnClickUpgrade()
    {
        var (amount, cost) = GetCostInfo();
        _upgrade.AddUpgrade(amount, cost);
        TMPUpgradeLabel.text = $"{_upgrade.Name} ( {_upgrade.UpgradeCount.ToScientificString()} )";
        UpdateButtonDisplay();
    }

    private (Bint, Bint) GetCostInfo()
    {
        var single =  ((Bint)1, _upgrade.GetNextCost());
        if (CurrentBuyType == BuyType.SINGLE)
            return single;
        var multiple = _upgrade.GetMaxAffordableUpgrades();
        return multiple.upgradeCount == 0 ? single : multiple;
    }
}
