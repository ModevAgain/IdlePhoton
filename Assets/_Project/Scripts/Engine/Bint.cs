using System;
using UnityEngine;

public struct Bint : IEquatable<Bint>, IComparable<Bint>
{
    public string ValueStr;

    public double Value;
    public int Exponent;

    
    public Bint(double value, int exponent = 0)
    {
        Value = value;
        Exponent = exponent;
        ValueStr = string.Empty;
        Normalize();

        ValueStr = $"{Value}e{Exponent}";
    }

    public Bint(float value, int exponent = 0)
    {
        Value = value;
        Exponent = exponent;
        ValueStr = string.Empty;
        Normalize();

        ValueStr = $"{Value}e{Exponent}";
    }

    private void Normalize()
    {
        if (Value < 1 && Exponent != 0)
        {
            while (Value < 1 && Exponent != 0)
            {
                Value *= 10;
                Exponent -= 1;
            }
        }
        else if (Value >= 10)
        {
            while (Value >= 10)
            {
                Value /= 10;
                Exponent += 1;
            }
        }
        else if (Value <= 0)
        {
            Value = 0;
            Exponent = 0;
        }
    }

    private void Align(int targetExp)
    {
        var diff = targetExp - Exponent;
        if (diff <= 0) return;
        Value /= Math.Pow(10, diff);
        Exponent = targetExp;
    }

    public static Bint operator +(Bint a, Bint b)
    {
        if (b.Exponent < a.Exponent)
            b.Align(a.Exponent);
        else
            a.Align(b.Exponent);
        return new Bint(a.Value + b.Value, a.Exponent);
    }

    public static Bint operator -(Bint a, Bint b)
    {
        if (b.Exponent < a.Exponent)
            b.Align(a.Exponent);
        else
            a.Align(b.Exponent);
        return new Bint(a.Value - b.Value, a.Exponent);
    }

    public static Bint operator *(Bint a, double factor)
    {
        return factor >= 0 ? new Bint(a.Value * factor, a.Exponent) : a;
    }
    
    public static Bint operator *(Bint a, Bint factor)
    {
        a.Align(factor.Exponent);
        a.Value *= factor.Value;
        a.Normalize();
        return a;
    }

    public static Bint operator /(Bint a, double divisor)
    {
        return divisor > 0 ? new Bint(a.Value / divisor, a.Exponent) : a;
    }
    
    public static Bint operator /(Bint a, Bint divisor)
    {
        return divisor.Value > 0 ? new Bint(a.Value / divisor.Value, a.Value > 0 ? a.Exponent - divisor.Exponent : 0) : a;
    }
    
    public static bool operator ==(Bint a, Bint b)
    {
        a.Align(b.Exponent);
        b.Align(a.Exponent);
        return Math.Abs(a.Value - b.Value) < 0.00001f;
    }

    public static bool operator !=(Bint a, Bint b) => !(a == b);

    public static bool operator >(Bint a, Bint b)
    {
        a.Align(b.Exponent);
        b.Align(a.Exponent);
        return a.Value > b.Value;
    }

    public static bool operator <(Bint a, Bint b)
    {
        a.Align(b.Exponent);
        b.Align(a.Exponent);
        return a.Value < b.Value;
    }

    public static bool operator >=(Bint a, Bint b) => (a > b) || (a == b);
    public static bool operator <=(Bint a, Bint b) => (a < b) || (a == b);

    public override string ToString() => $"{Value}E{Exponent}";

    public static implicit operator Bint(double value) => new (value);

    public override bool Equals(object obj) => obj is Bint other && this == other;
    public bool Equals(Bint other) => this == other;
    public override int GetHashCode() => HashCode.Combine(Value, Exponent);
    public int CompareTo(Bint other) => this > other ? 1 : this < other ? -1 : 0;
}

public static class MathBint
{
    public static Bint Log(Bint value) => new (value.Exponent);

    public static Bint Floor(Bint value)
    {
        return new Bint(Math.Floor(value.Value));
    }

    // Should this have a int range check?
    public static Bint Pow(Bint value, Bint exponent)
    {
        return new Bint(value.Value, (int)Math.Pow(value.Exponent > 0 ? value.Exponent : 1, Math.Pow(exponent.Value, exponent.Exponent)));
    }

    public static int AsInt(this Bint value)
    {
        return (int)Math.Floor(Math.Pow(value.Value, value.Exponent));
    }
    
    public static float AsFloat(this Bint value)
    {
        return (float)Math.Pow(value.Value, value.Exponent);
    }
    
    public static string ToScientificString(this Bint value, bool prependSign = false)
    {
        var sign = value > 0 ? "+" : value == 0 ? "" : "-";
        return value.Exponent < 6
            ? (value.Value * Mathf.Pow(10, value.Exponent)).ToString($"{(prependSign ? sign : string.Empty)}0.00")
            : $"{(prependSign ? sign : string.Empty)}{value.Value:0.00}e{value.Exponent}";
    }
}