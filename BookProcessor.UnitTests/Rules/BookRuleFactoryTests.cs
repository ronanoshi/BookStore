namespace BookProcessor.UnitTests.Rules;

public class BookRuleFactoryTests
{
    [Fact]
    public void CreateDefaultTransformRules_ReturnsRoundPriceUpRule()
    {
        // Act
        var rules = BookRuleFactory.CreateDefaultTransformRules().ToList();

        // Assert
        rules.Should().ContainSingle();
        rules[0].Should().BeOfType<RoundPriceUpRule>();
    }

    [Fact]
    public void CreateDefaultFilterRules_ReturnsExcludeSaturdayPublishedRuleAsFirst()
    {
        // Act
        var rules = BookRuleFactory.CreateDefaultFilterRules().ToList();

        // Assert
        rules.Should().HaveCountGreaterThanOrEqualTo(1);
        rules.First().Should().BeOfType<ExcludeSaturdayPublishedRule>();
    }

    [Fact]
    public void CreateDefaultFilterRules_ReturnsExcludeAuthorContainsRule()
    {
        // Act
        var rules = BookRuleFactory.CreateDefaultFilterRules().ToList();

        // Assert
        rules.OfType<ExcludeAuthorContainsRule>().Should().ContainSingle();
    }

    [Fact]
    public void CreateDefaultTransformRules_ReturnsNewInstancesEachCall()
    {
        // Act
        var rules1 = BookRuleFactory.CreateDefaultTransformRules().ToList();
        var rules2 = BookRuleFactory.CreateDefaultTransformRules().ToList();

        // Assert
        rules1[0].Should().NotBeSameAs(rules2[0]);
    }

    [Fact]
    public void CreateDefaultFilterRules_ReturnsNewInstancesEachCall()
    {
        // Act
        var rules1 = BookRuleFactory.CreateDefaultFilterRules().ToList();
        var rules2 = BookRuleFactory.CreateDefaultFilterRules().ToList();

        // Assert
        rules1[0].Should().NotBeSameAs(rules2[0]);
    }
}
