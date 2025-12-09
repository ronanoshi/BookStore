namespace BookProcessor.UnitTests.Rules;

public class FilterRuleResultTests
{
    [Fact]
    public void Include_ReturnsShouldIncludeTrue()
    {
        // Act
        var result = FilterRuleResult.Include();

        // Assert
        result.ShouldInclude.Should().BeTrue();
        result.ExclusionReason.Should().BeNull();
    }

    [Fact]
    public void Exclude_ReturnsShouldIncludeFalse_WithReason()
    {
        // Arrange
        var reason = "Test exclusion reason";

        // Act
        var result = FilterRuleResult.Exclude(reason);

        // Assert
        result.ShouldInclude.Should().BeFalse();
        result.ExclusionReason.Should().Be(reason);
    }

    [Fact]
    public void Constructor_WithShouldIncludeTrue_SetsProperties()
    {
        // Act
        var result = new FilterRuleResult(true);

        // Assert
        result.ShouldInclude.Should().BeTrue();
        result.ExclusionReason.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithShouldIncludeFalseAndReason_SetsProperties()
    {
        // Arrange
        var reason = "Custom reason";

        // Act
        var result = new FilterRuleResult(false, reason);

        // Assert
        result.ShouldInclude.Should().BeFalse();
        result.ExclusionReason.Should().Be(reason);
    }
}
