using TMPro;
using UnityEngine;

public class ResourceUI : TickObject
{
    public Resource Res;
    public TMP_Text TMPResource;

    public override void Tick(uint index, float tickBalanceValue, float deltaTime)
    {
        TMPResource.text = Res.ToHumanReadableString();
    }
}