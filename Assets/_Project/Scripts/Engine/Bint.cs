using System;

public struct Bint : IEquatable<Bint>, IComparable<Bint>
{
    private const int MaxMagnitude = 12;
    private const double TenCubed = 1e3;

    public double Value;
    public int Exponent;

    public Bint(double value, int exponent = 0)
    {
        Value = value;
        Exponent = exponent;
        Normalize();
    }

    private void Normalize()
    {
        if (Value < 1 && Exponent != 0)
        {
            Value *= TenCubed;
            Exponent -= 3;
        }
        else if (Value >= TenCubed)
        {
            while (Value >= TenCubed)
            {
                Value /= TenCubed;
                Exponent += 3;
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
        Value = (diff <= MaxMagnitude) ? Value / Math.Pow(10, diff) : 0;
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

    public static Bint operator /(Bint a, double divisor)
    {
        return divisor > 0 ? new Bint(a.Value / divisor, a.Exponent) : a;
    }
    
    public static Bint operator /(Bint a, Bint b)
    {
        return b.Value > 0 ? new Bint(a.Value / b.Value, a.Exponent - b.Exponent) : a;
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

public static class BintExtensions
{
    public static double Log(Bint value, float baseValue)
    {
        return Math.Log10(value.Value) + value.Exponent / Math.Log10(baseValue);
    }
    
    public static double Log(Bint value, double baseValue)
    {
        return Math.Log10(value.Value) + value.Exponent / Math.Log10(baseValue);
    }
    
    public static string ToScientificString(this Bint value, bool prependSign = false)
    {
        var sign = value > 0 ? "+" : value == 0 ? "" : "-";
        return value.Value > 999999 
            ? $"{(prependSign ? sign : "")}{value.Value:0.00}e{value.Exponent}" 
            : value.Value.ToString($"{(prependSign ? sign : "")}0.00");
    }
}