using UnityEngine;

public interface ITickable
{
    public void Tick(uint index, float tickBalanceValue, float deltaTime);
}
