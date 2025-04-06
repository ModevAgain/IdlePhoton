using System;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "IdleEngine/Resource", fileName = "Resource")]
public class Resource : ScriptableObject
{
    public string ResourceName;
    
    [FormerlySerializedAs("Value")] 
    public float BaseValue;
    public int Exponent;

    public double Value => BaseValue * Math.Pow(10, Exponent);
    
    public void Add(float value)
    {
        BaseValue += value;
    }
    
    public void Remove(float value)
    {
        BaseValue -= value;
    }
    
    public void Reset()
    {
        BaseValue = 0;
        Exponent = 0;
    }
}

public static class StringExtensions
{
    public static string ToHumanReadableString(this Resource resource)
    {
        if (resource.Exponent > 1)
            return resource.BaseValue + "e" + resource.Exponent;
        return resource.BaseValue.ToString();
    }
}