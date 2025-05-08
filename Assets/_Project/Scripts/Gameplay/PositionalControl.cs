using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PositionalControl : BaseGameplayModule
{
    public float Steps;
    public Bounds TargetBounds;
    public Button Btn_UP, Btn_DOWN, Btn_LEFT, Btn_RIGHT;
    public Transform TargetTransform;
    public Transform SimCamTransform;
    public TMP_Text TargetText;
    
    public Action<Vector2> OnPositionChanged;

    private void Start()
    {
        Btn_UP.onClick.AddListener(() => OnTargetMove(new Vector2(0, 1)));
        Btn_DOWN.onClick.AddListener(() => OnTargetMove(new Vector2(0, -1)));
        Btn_LEFT.onClick.AddListener(() => OnTargetMove(new Vector2(-1, 0)));
        Btn_RIGHT.onClick.AddListener(() => OnTargetMove(new Vector2(1, 0)));
        
        TargetBounds.center = TargetTransform.position;
    }

    public void OnClick_CenterTarget()
    {
        OnTargetMove(Vector2.zero);
    }
    
    private void OnTargetMove(Vector2 direction)
    {
        var newPos = TargetTransform.position + (Vector3)direction * Steps;
        if (TargetBounds.Contains(newPos))
        {
            TargetTransform.position = newPos;
            TargetTransform.LookAt(SimCamTransform);
            TargetText.text = $"Target<br>Position<br>({TargetTransform.position.x:0}, {TargetTransform.position.y:0})";
            
            OnPositionChanged?.Invoke(newPos);
        }
    }

    public override void OnModuleEnable()
    {
        base.OnModuleEnable();
    }
}
