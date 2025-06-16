using TMPro;
using UnityEngine;

public class ResourceUI : TickObject
{
    public Resource Res;
    public TMP_Text TMP_Name;
    public TMP_Text TMP_Value;
    public TMP_Text TMP_Trend;

    protected override void Start()
    {
        TMP_Name.text = Res.ResourceName;
        base.Start();
    }
    
    public override void Tick(uint index, float tickBalanceValue, float deltaTime)
    {
        Res.UpdateTrend();
        
        TMP_Value.text = Res.Value.ToScientificString();
        TMP_Trend.text = Res.GetTrend().ToScientificString(true);
    }
}