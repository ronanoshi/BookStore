namespace BookProcessor.UnitTests.Rules;

public class BookRuleFactoryTests
{
    [Fact]
    public void CreateTransformRules_ReturnsRoundPriceUpRule()
    {
        // Act
        var rules = BookRuleFactory.CreateTransformRules().ToList();

        // Assert
        rules.Should().ContainSingle();
        rules[0].Should().BeOfType<RoundPriceUpRule>();
    }

    [Fact]
    public void CreateFilterRules_ReturnsExpectedRules()
    {
        // Act
        var rules = BookRuleFactory.CreateFilterRules().ToList();

        // Assert
        rules.Should().HaveCountGreaterThanOrEqualTo(2);
        rules.First().Should().BeOfType<ExcludeSaturdayPublishedRule>();
        rules.OfType<ExcludeAuthorContainsRule>().Should().ContainSingle();
    }

    [Fact]
    public void CreateRules_ReturnsNewInstancesEachCall()
    {
        // Act
        var transformRules1 = BookRuleFactory.CreateTransformRules().ToList();
        var transformRules2 = BookRuleFactory.CreateTransformRules().ToList();
        var filterRules1 = BookRuleFactory.CreateFilterRules().ToList();
        var filterRules2 = BookRuleFactory.CreateFilterRules().ToList();

        // Assert
        transformRules1[0].Should().NotBeSameAs(transformRules2[0]);
        filterRules1[0].Should().NotBeSameAs(filterRules2[0]);
    }
}
