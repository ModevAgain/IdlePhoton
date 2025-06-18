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
        Assert.That(bint.ValueStr, Is.EqualTo(result));
    }
    
    [TestCase(0.1, 1, "1e0")]
    [TestCase(0.1, 2, "1e1")]
    [TestCase(1, 2, "1e2")]
    [TestCase(10, 2, "1e3")]
    [TestCase(0.01, 3, "1e1")]
    [TestCase(0.1, 4, "1e3")]
    [TestCase(0.0001, 6, "1e2")]
    public void NewBint_WithValueAndExponent_ReturnsExpected(double value, int exponent, string result)
    {
        var bint = new Bint(value, exponent);
        Assert.That(bint.ValueStr, Is.EqualTo(result));
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
    
    
}
