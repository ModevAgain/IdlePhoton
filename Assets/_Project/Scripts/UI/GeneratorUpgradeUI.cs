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
        TMPUpgradeLabel.text = $"{_upgrade.Name} ( {_upgrade.UpgradeCount} )";
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
        
        var (amount, cost) = GetCostInfo();
        BtnUpgrade.interactable = amount > 0 && _upgrade.CostResource.Value > cost;
        BuyableOverlay.fillAmount = amount > 0 ? 1 - Mathf.Clamp01((float)_upgrade.CostResource.Value / cost) : 1;
        TMPUpgradeValues.text = $"{cost:0.0}\n+{amount}";
    }

    private void OnClickUpgrade()
    {
        var (amount, _) = GetCostInfo();
        _upgrade.AddUpgrade(amount);
        TMPUpgradeLabel.text = $"{_upgrade.Name} ( {_upgrade.UpgradeCount} )";
    }

    private (int, float) GetCostInfo()
    {
        if (CurrentBuyType == BuyType.SINGLE)
            return (1, _upgrade.GetNextCost());
        
        return _upgrade.GetMaxAffordableUpgrades();
    }
}
