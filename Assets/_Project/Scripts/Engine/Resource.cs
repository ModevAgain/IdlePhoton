using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "IdleEngine/Resource", fileName = "Resource")]
public class Resource : ScriptableObject
{
    public string ResourceName;

    public Bint Value;
    
    private Bint[] _lastTrendBuffer = new Bint[100];
    private int _bufferIndex = 0;
    private Bint _lastValue;
    
    public void UpdateTrend()
    {
        if (_bufferIndex >= _lastTrendBuffer.Length)
            _bufferIndex = 0;
        
        _lastTrendBuffer[_bufferIndex] = Value - _lastValue;
        _lastValue = Value;

        _bufferIndex++;
    }

    // Trend per second
    public Bint GetTrend()
    {
        var sum = _lastTrendBuffer.Aggregate<Bint, Bint>(0, (current, t) => current + t);
        return (sum/_lastTrendBuffer.Length) * 1/IdleEngine.Root.TickTime;
    }
    
    public void Reset()
    {
        Value = 0;
    }
}