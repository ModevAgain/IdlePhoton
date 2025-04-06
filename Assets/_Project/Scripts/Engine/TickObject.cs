using System;
using UnityEngine;

public class TickObject : MonoBehaviour, ITickable
{
    protected virtual void Start()
    {
        if (IdleEngine.Root != null) 
            IdleEngine.Root.RegisterTickable(this);
        else Debug.LogError("No Root Engine found.");
    }
    
    public virtual void Tick(uint index, float tickBalanceValue, float deltaTime)
    {
        throw new NotImplementedException();
    }
}