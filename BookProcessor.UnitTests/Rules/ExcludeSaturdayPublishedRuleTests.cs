namespace BookProcessor.UnitTests.Rules;

public class ExcludeSaturdayPublishedRuleTests
{
    private readonly ExcludeSaturdayPublishedRule _sut = new();

    [Fact]
    public void RuleName_ReturnsExpectedName()
    {
        // Assert
        _sut.RuleName.Should().Be("ExcludeSaturdayPublished");
    }

    [Fact]
    public void Evaluate_WithNullBook_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _sut.Evaluate(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Evaluate_WithSaturdayPublishDate_ReturnsExclude()
    {
        // Arrange - 2023-05-13 is a Saturday
        var saturday = new DateOnly(2023, 5, 13);
        var book = BookTestData.CreateSampleBook(
            id: "bk101",
            title: "Saturday Book",
            publishDate: saturday);

        // Act
        var result = _sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeFalse();
        result.ExclusionReason.Should().Contain("Saturday");
        result.ExclusionReason.Should().Contain("bk101");
        result.ExclusionReason.Should().Contain("Saturday Book");
        result.ExclusionReason.Should().Contain("2023-05-13");
    }

    [Theory]
    [InlineData(2023, 5, 14)] // Sunday
    [InlineData(2023, 5, 15)] // Monday
    [InlineData(2023, 5, 16)] // Tuesday
    [InlineData(2023, 5, 17)] // Wednesday
    [InlineData(2023, 5, 18)] // Thursday
    [InlineData(2023, 5, 19)] // Friday
    public void Evaluate_WithNonSaturdayPublishDate_ReturnsInclude(int year, int month, int day)
    {
        // Arrange
        var date = new DateOnly(year, month, day);
        var book = BookTestData.CreateSampleBook(publishDate: date);

        // Act
        var result = _sut.Evaluate(book);

        // Assert
        result.ShouldInclude.Should().BeTrue();
        result.ExclusionReason.Should().BeNull();
    }

    [Fact]
    public void Evaluate_WithMultipleSaturdays_AllExcluded()
    {
        // Arrange - Various Saturdays
        var saturdays = new[]
        {
            new DateOnly(2023, 1, 7),
            new DateOnly(2023, 6, 24),
            new DateOnly(2023, 12, 30),
            new DateOnly(2024, 2, 17)
        };

        foreach (var saturday in saturdays)
        {
            var book = BookTestData.CreateSampleBook(publishDate: saturday);

            // Act
            var result = _sut.Evaluate(book);

            // Assert
            result.ShouldInclude.Should().BeFalse($"Date {saturday} is a Saturday and should be excluded");
        }
    }
}
