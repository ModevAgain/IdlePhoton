using System;
using System.Globalization;
using System.Numerics;
using NUnit.Framework;


[TestFixture]
public class BintTests
{
    
    [TestCase(1, "1e0")]
    [TestCase(10, "1e1")]
    [TestCase(100, "1e2")]
    [TestCase(1000, "1e3")]
    [TestCase(10000, "1e4")]
    [TestCase(100000, "1e5")]
    [TestCase(1e6, "1e6")]
    [TestCase(1e9, "1e9")]
    public void NewBint_WithValue_ReturnsExpected(double value, string result)
    {
        var bint = new Bint(value);
        Assert.That(bint.ToString(), Is.EqualTo(result));
    }
    
    [TestCase(0.1, 1, "1e0")]
    [TestCase(0.1, 2, "1e1")]
    [TestCase(1, 2, "1e2")]
    [TestCase(10, 2, "1e3")]
    [TestCase(0.01, 3, "1e1")]
    [TestCase(0.1, 4, "1e3")]
    [TestCase(0.0001, 6, "1e2")]
    [TestCase(1, 1500, "1e1500")]
    public void NewBint_WithValueAndExponent_ReturnsExpected(double value, int exponent, string result)
    {
        var bint = new Bint(value, exponent);
        Assert.That(bint.ToString(), Is.EqualTo(result));
    }
    
    [TestCase(1, 0, "1.00")]
    [TestCase(1, 1, "10.00")]
    [TestCase(10, 0, "10.00")]
    [TestCase(10, 1, "100.00")]
    [TestCase(1, 3, "1000.00")]
    [TestCase(5.5, 3, "5500.00")]
    [TestCase(11, 3, "11000.00")]
    [TestCase(9.99999, 5, "999999.00")]
    [TestCase(1, 6, "1.00e6")]
    [TestCase(.1, 300, "1.00e299")]
    public void Bint_ToScientific_ReturnsExpected(double value, int exponent, string result)
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
        var bint  = new Bint(value, exponent);
        Assert.That(bint.ToScientificString(), Is.EqualTo(result));
    }
    
    [Test]
    public void Bint_Equals_ReturnsExpected()
    {
        Assert.That(new Bint(1), Is.EqualTo(new Bint(1)));
        Assert.That(new Bint(1), Is.Not.EqualTo(new Bint(0)));
        Assert.That(new Bint(0), Is.Not.EqualTo(new Bint(1)));
        Assert.That(new Bint(5, 5), Is.EqualTo(new Bint(0.5, 6)));
        Assert.That(new Bint(0.5, 6), Is.EqualTo(new Bint(5, 5)));
        Assert.That(new Bint(5, 6), Is.Not.EqualTo(new Bint(5, 5)));
        Assert.That(new Bint(5, 5), Is.Not.EqualTo(new Bint(5, 6)));
    }
    
    [Test]
    public void Bint_Addition_ReturnsExpected()
    {
        Assert.That(new Bint(1) + new Bint(1), Is.EqualTo(new Bint(2)));
        Assert.That(new Bint(1) + new Bint(10), Is.EqualTo(new Bint(11)));
        Assert.That(new Bint(10) + new Bint(10), Is.EqualTo(new Bint(20)));
        Assert.That(new Bint(10) + new Bint(1000), Is.EqualTo(new Bint(1010)));
        Assert.That(new Bint(1) + new Bint(1000), Is.EqualTo(new Bint(1001)));
    }
    
    [Test]
    public void MathBint_Log_ReturnsExpected()
    {
        Assert.That(MathBint.Log(new Bint(1)).AsDouble() , Is.EqualTo(Math.Log(1)));
        Assert.That(MathBint.Log(new Bint(10)).AsDouble() , Is.EqualTo(Math.Log(10)));
        Assert.That(MathBint.Log(new Bint(100)).AsDouble() , Is.EqualTo(Math.Log(100)));
        Assert.That(MathBint.Log(new Bint(2.4)).AsDouble() , Is.EqualTo(Math.Log(2.4)));
        Assert.That(MathBint.Log(new Bint(10000.24)).AsDouble() , Is.EqualTo(Math.Log(10000.24)));
    }
    
    [TestCase(1,1)]
    [TestCase(1,10)]
    [TestCase(10,1)]
    [TestCase(1,100)]
    [TestCase(1,308)]
    [TestCase(100,2)]
    [TestCase(9,15000)]
    public void MathBint_Pow_ReturnsExpected(double value, int exponent)
    {
        var result = MathBint.Pow(new Bint(value), new Bint(exponent));
        var resultAsBigInteger = ToBigInteger(result);
        var expected = BigInteger.Pow(new BigInteger(value), exponent);
        Assert.That(resultAsBigInteger.ToString("e2"), Is.EqualTo(expected.ToString("e2")));
    }

    private static BigInteger ToBigInteger(Bint bint)
    {
        return new BigInteger(bint.Value * 1e9) * BigInteger.Pow(10, bint.Exponent) / new BigInteger(1e9);
    }
}
