using System;
using System.Globalization;
using System.Numerics;
using BigFloatLibrary;
using NUnit.Framework;


[TestFixture]
public class BintTests
{
    [SetUp]
    public void Setup()
    {
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
    }
    
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
        var bint  = new Bint(value, exponent);
        Assert.That(bint.ToScientificString(), Is.EqualTo(result));
    }
    
    [TestCase(1, 1, true)]
    [TestCase(1, 0, false)]
    [TestCase(0, 1, false)]
    public void Bint_Equals_ReturnsExpected(double a, double b, bool expected)
    {
        var bintA = new Bint(a);
        var bintB = new Bint(b);
        Assert.That(bintA.Equals(bintB), Is.EqualTo(expected));
    }
    
    [TestCase(5, 5, 0.5, 6, true)]
    [TestCase(0.5, 6, 5, 6, false)]
    [TestCase(5, 6, 5, 5, false)]
    [TestCase(5, 5, 5, 6, false)]
    public void Bint_WithExponent_Equals_ReturnsExpected(double a, int aE, double b, int bE, bool expected)
    {
        var bintA = new Bint(a, aE);
        var bintB = new Bint(b, bE);
        Assert.That(bintA.Equals(bintB), Is.EqualTo(expected));
    }
    
    [TestCase(1, 1, 2)]
    [TestCase(1 , 10, 11)]
    [TestCase(10, 10, 20)]
    [TestCase(10, 1000, 1010)]
    [TestCase(1, 1000, 1001)]
    [TestCase(-1, 1000, 999)]
    [TestCase(1, -1000, -999)]
    public void Bint_Addition_ReturnsExpected(double a, double b, double expected)
    {
        var bintA =  new Bint(a);
        var bintB = new Bint(b);
        var sum = bintA + bintB;
        Assert.That(sum, Is.EqualTo(new Bint(expected)));
    }
    
    [Test]
    public void Bint_Multiplication_ReturnsExpected()
    {
        Assert.That(new Bint(1) * new Bint(1), Is.EqualTo(new Bint(1)));
        Assert.That(new Bint(1) * new Bint(10), Is.EqualTo(new Bint(10)));
        Assert.That(new Bint(10) * new Bint(10), Is.EqualTo(new Bint(100)));
        Assert.That(new Bint(10) * new Bint(1000), Is.EqualTo(new Bint(10000)));
        Assert.That(new Bint(1) * new Bint(1000), Is.EqualTo(new Bint(1000)));
        
        Assert.That(new Bint(-1) * new Bint(1000), Is.EqualTo(new Bint(-1000)));
        Assert.That(new Bint(1) * new Bint(-1000), Is.EqualTo(new Bint(-1000)));
        Assert.That(new Bint(-1) * new Bint(-1000), Is.EqualTo(new Bint(1000)));
    }
    
 
    
    [TestCase(1, 1, 1)]
    [TestCase(1, 10, 0.1)]
    [TestCase(10, 10, 1)]
    [TestCase(10, 1, 10)]
    [TestCase(-1, 100, -0.01)]
    [TestCase(1, 100, 0.01)]
    [TestCase(-1, 100, -0.01)]
    public void Bint_Division_ReturnsExpected(double a, double b, double expected)
    {
        Assert.That(new Bint(a) / new Bint(b), Is.EqualTo(new Bint(expected)));
    }
    
    
    [TestCase(1)]
    [TestCase(10)]
    [TestCase(100)]
    [TestCase(2.4)]
    [TestCase(10000.24)]
    public void MathBint_Log_ReturnsExpected(double value)
    {
        var resultAsLog2 = MathBint.Log(value) / MathBint.Log(2);
        var expected = BigFloat.Log2(new BigFloat(value));
        Assert.That(resultAsLog2.AsDouble() , Is.EqualTo(expected).Within(Bint.PRECISION_FACTOR));
    }
    
    [TestCase(1,1)]
    [TestCase(1,10)]
    [TestCase(10,1)]
    [TestCase(1,100)]
    [TestCase(1,308)]
    [TestCase(100,2)]
    [TestCase(9,15000)]
    [TestCase(3.7,17)]
    public void MathBint_Pow_ReturnsExpected(double value, int exponent)
    {
        var result = MathBint.Pow(new Bint(value), new Bint(exponent));
        var expectedAsBigFloatString = BigFloat.Pow(new BigFloat(value), exponent).ToString();
        var expectedAsBint = expectedAsBigFloatString.Contains("e+")
            ? new Bint(float.Parse(expectedAsBigFloatString.Split("e+")[0]), int.Parse(expectedAsBigFloatString.Split("e+")[1]))
            : new Bint(float.Parse(expectedAsBigFloatString));
        
        Assert.That(result, Is.EqualTo(expectedAsBint));
    }

    private static BigInteger ToBigInteger(Bint bint)
    {
        return new BigInteger(bint.Value * 1e9) * BigInteger.Pow(10, bint.Exponent) / new BigInteger(1e9);
    }
}
