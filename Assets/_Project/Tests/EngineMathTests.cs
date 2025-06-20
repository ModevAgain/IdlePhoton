using NUnit.Framework;

[TestFixture]
public class EngineMathTests
{
    [Test]
    public void CalculateCost_ReturnsExpected()
    {
        var result = EngineMath.CalculateCostForNextUpgrade(1, 2, 0);
        Assert.That(result, Is.EqualTo(new Bint(1)));
        
        result = EngineMath.CalculateCostForNextUpgrade(1, 2, 1);
        Assert.That(result, Is.EqualTo(new Bint(2)));
    }
    
    [TestCase(1, 2, 0, 1, 1)]
    [TestCase(1, 2, 0, 2, 3)]
    [TestCase(1, 2, 1, 2, 6)]
    [TestCase(1, 2, 0, 10, 1023)]
    [TestCase(1, 2, 2, 5, 124)]
    [TestCase(3.7f, 1.07f, 0, 1, 3.7)]
    [TestCase(3.7f, 1.07f, 0, 2, 11.359)]
    [TestCase(3.7f, 1.07f, 5, 10, 213.16547)]
    public void CalculateTotalCost_ReturnsExpected(float baseCost, float coeff, double currentCount, double targetCount, double expected)
    {
        var result = EngineMath.CalculateTotalCost(baseCost, coeff, new Bint(currentCount), new Bint(targetCount));
        Assert.That(result, Is.EqualTo(new Bint(expected)));
    }
    
    [TestCase(1, 2, 2, 5, 124)]
    public void GetMaxAffordableUpgrades_ReturnsExpected(float baseCost, float coeff, double currentCount, double resourceValue, float expected)
    {
        var result = EngineMath.CalculateTotalCost(baseCost, coeff, new Bint(currentCount), new Bint(expected));
        Assert.That(result, Is.EqualTo(new Bint(expected)));
    }
}