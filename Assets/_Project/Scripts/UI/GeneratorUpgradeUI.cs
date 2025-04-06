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

    protected override void Start()
    {
        _upgrade = Generator.Upgrades.First(u => u.UpgradeType == UpgradeType)!;

        TMPUpgradeLabel.text = $"{_upgrade.Name} ( {_upgrade.UpgradeCount} )";
        
        var (amount, cost) = GetCostInfo();
        TMPUpgradeValues.text = $"{cost:0.0}\n+{amount}";
        
        BtnUpgrade.onClick.AddListener(OnClickUpgrade);
        
        base.Start();
    }

    public override void Tick(uint index, float tickBalanceValue, float deltaTime)
    {
        var (amount, cost) = GetCostInfo();
        BtnUpgrade.interactable = _upgrade.CostResource.Value > cost;
        BuyableOverlay.fillAmount = 1 - Mathf.Clamp01((float)_upgrade.CostResource.Value / cost);
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
