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
        SetExponent();
    }
    
    public void Remove(float value)
    {
        BaseValue -= value;
        SetExponent();
    }

    private void SetExponent()
    {
        Exponent = (int)Math.Floor(Math.Log10(BaseValue));
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
        if (resource.BaseValue > 999999)
            return $"{resource.BaseValue/Mathf.Pow(10, resource.Exponent):0.00}e{resource.Exponent}";
        return Mathf.FloorToInt(resource.BaseValue).ToString();
    }
}