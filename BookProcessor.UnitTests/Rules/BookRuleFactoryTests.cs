using BookProcessor.Rules;
using FluentAssertions;
using Xunit;

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
    public void CreateDefaultFilterRules_ReturnsExcludeAuthorContainsRulesFromConfiguration()
    {
        // Act
        var rules = BookRuleFactory.CreateDefaultFilterRules().ToList();

        // Assert
        // Should have Saturday rule + author exclusion rules from appsettings.json
        // Test appsettings.json contains "Peter" and "TestExcludedAuthor"
        var authorRules = rules.OfType<ExcludeAuthorContainsRule>().ToList();
        authorRules.Should().HaveCountGreaterThanOrEqualTo(1);
        authorRules.Should().Contain(r => r.RuleName.Contains("Peter"));
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
