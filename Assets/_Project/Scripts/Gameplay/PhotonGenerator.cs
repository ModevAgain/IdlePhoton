using System.Linq;
using UnityEngine;

public class PhotonGenerator : BaseGenerator
{
    public PhotonManager PhotonManager;

    public Bint Efficiency = 1;
    
    public override void Init()
    {
        Upgrades.First(u => u.UpgradeType == BaseGeneratorUpgradeType.EFFICIENCY).Init(f =>  Efficiency = f);
        
        base.Init();
    }

    protected override void Generate(Bint amount)
    {
        PhotonManager.AddPhoton(amount.AsInt());
    }
}
