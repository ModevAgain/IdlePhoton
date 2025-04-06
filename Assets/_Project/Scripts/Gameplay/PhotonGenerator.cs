using System.Linq;
using UnityEngine;

public class PhotonGenerator : BaseGenerator
{
    public PhotonManager PhotonManager;

    public float Efficiency = 1;
    
    public override void Init()
    {
        Upgrades.First(u => u.UpgradeType == BaseGeneratorUpgradeType.EFFICIENCY).Init(f =>  Efficiency = f);
        
        base.Init();
    }

    protected override void Generate(int amount)
    {
        for (int i = 0; i < amount; i++)
            PhotonManager.AddPhoton();
    }
}
