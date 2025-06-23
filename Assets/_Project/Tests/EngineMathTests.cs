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
    [TestCase(3.7f, 1.07f, 0, 2, 7.659)]
    [TestCase(3.7f, 1.07f, 2, 1, 4.24)]
    [TestCase(3.7f, 1.07f, 5, 5, 29.84)]
    public void CalculateTotalCost_ReturnsExpected(float baseCost, float coeff, double currentCount, double targetCount, double expected)
    {
        var result = EngineMath.CalculateTotalCost(baseCost, coeff, new Bint(currentCount), new Bint(targetCount));
        Assert.That(result.ToScientificString(), Is.EqualTo(new Bint(expected).ToScientificString()));
    }
    
    [TestCase(3.7f, 1.07f, 0, 4, 1)]
    [TestCase(3.7f, 1.07f, 0, 52, 10)]
    [TestCase(3.7f, 1.07f, 2, 5, 1)]
    [TestCase(3.7f, 1.07f, 2, 10, 2)]
    [TestCase(3.7f, 1.07f, 5, 0, 0)]
    [TestCase(3.7f, 1.07f, 5, 1, 0)]
    public void GetMaxAffordableUpgrades_ReturnsExpected(float baseCost, float coeff, double currentCount, double resourceValue, float expected)
    {
        var result = EngineMath.CalculateMaxAffordableUpgrades(baseCost, coeff, new Bint(currentCount), new Bint(resourceValue));
        Assert.That(result.upgradeCount, Is.EqualTo(new Bint(expected)));
    }
}