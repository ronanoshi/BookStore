namespace BookProcessor.UnitTests.Rules;

public class FilterRuleResultTests
{
    [Fact]
    public void Include_ReturnsShouldIncludeTrue()
    {
        // Act
        var result = FilterRuleResult.Include();

        // Assert
        result.IncldueInOutput.Should().BeTrue();
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
        result.IncldueInOutput.Should().BeFalse();
        result.ExclusionReason.Should().Be(reason);
    }
}
