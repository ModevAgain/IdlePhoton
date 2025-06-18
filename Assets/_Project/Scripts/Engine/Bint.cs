using System;

public struct Bint : IEquatable<Bint>, IComparable<Bint>
{
    public double Value;
    public int Exponent;

    
    public Bint(double value, int exponent = 0)
    {
        Value = value;
        Exponent = exponent;
        Normalize();
    }

    public Bint(float value, int exponent = 0)
    {
        Value = value;
        Exponent = exponent;
        Normalize();
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
        return factor >= 0 ? new Bint(a.Value * factor, a.Exponent) : 0;
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

    public override string ToString() => $"{Value}e{Exponent}";

    public static implicit operator Bint(double value) => new (value);

    public override bool Equals(object obj) => obj is Bint other && this == other;
    public bool Equals(Bint other) => this == other;
    public override int GetHashCode() => HashCode.Combine(Value, Exponent);
    public int CompareTo(Bint other) => this > other ? 1 : this < other ? -1 : 0;
}

public static class MathBint
{
    public static Bint Log(Bint value)
    { 
       return Math.Log(value.Value) + Math.Log(Math.Pow(10, value.Exponent));
    }

    public static Bint Floor(Bint value)
    {
        return new Bint(Math.Floor(value.Value), value.Exponent);
    }
    
    public static Bint Pow(Bint value, Bint exponent)
    {
        // Step 1: log10(base)
        double logBase = Math.Log10(value.Value) + value.Exponent;

        // Step 2: Multiply by exponent
        double exponentValue = exponent.Value * Math.Pow(10, exponent.Exponent);
        double power = exponentValue * logBase;

        // Step 3: Convert result: 10^power = mantissa × 10^exponent
        int newExp = (int)Math.Floor(power);
        double newValue = Math.Pow(10, power - newExp);

        return new Bint(newValue, newExp);
    }

    public static int FloorToInt(this Bint value)
    {
        return (int)Math.Floor(value.Value * Math.Pow(10, value.Exponent));
    }
    
    public static float AsFloat(this Bint value)
    {
        return (float)(value.Value * Math.Pow(10, value.Exponent));
    }

    public static double AsDouble(this Bint value)
    {
        return value.Value * Math.Pow(10, value.Exponent);
    }
    
    public static string ToScientificString(this Bint value, bool prependSign = false)
    {
        var sign = value > 0 ? "+" : value == 0 ? "" : "-";
        return value.Exponent < 6
            ? (value.Value * Math.Pow(10, value.Exponent)).ToString($"{(prependSign ? sign : string.Empty)}0.00")
            : $"{(prependSign ? sign : string.Empty)}{value.Value:0.00}e{value.Exponent}";
    }
}